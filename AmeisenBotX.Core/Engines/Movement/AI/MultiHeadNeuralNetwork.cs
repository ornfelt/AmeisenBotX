#nullable enable
using System;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace AmeisenBotX.Core.Engines.Movement.AI
{
    /// <summary>
    /// Multi-task MLP with shared backbone and two output heads.
    /// Architecture:
    /// - Shared Backbone: Dense(64,ReLU) → BN → Dropout → Dense(64,ReLU) → BN → Dropout → Dense(32,ReLU)
    /// - Strategy Head: Dense(32,ReLU) → Dense(5,Softmax)
    /// - WinProb Head: Dense(16,ReLU) → Dense(1,Sigmoid)
    /// </summary>
    public class MultiHeadNeuralNetwork
    {
        // ========== Configuration ==========
        public double LearningRate { get; set; } = 0.001;
        public double DropoutRate { get; set; } = 0.1;
        public double StrategyLossWeight { get; set; } = 1.0;
        public double WinProbLossWeight { get; set; } = 0.5;
        public bool IsTraining { get; set; } = true;


        // ========== Layer Sizes ==========
        private const int InputSize = 20;
        private const int Backbone1Size = 64;
        private const int Backbone2Size = 64;
        private const int Backbone3Size = 32;
        private const int StrategyHead1Size = 32;
        private const int StrategyOutputSize = 6;
        private const int WinProbHead1Size = 16;
        private const int WinProbOutputSize = 1;

        // ========== Weights & Biases ==========
        // Backbone
        private double[,] w1, w2, w3;
        private double[] b1, b2, b3;
        // BatchNorm parameters (gamma, beta, running mean/var)
        private double[] bn1_gamma, bn1_beta, bn1_mean, bn1_var;
        private double[] bn2_gamma, bn2_beta, bn2_mean, bn2_var;
        // Strategy head
        private double[,] wS1, wS2;
        private double[] bS1, bS2;
        // WinProb head
        private double[,] wW1, wW2;
        private double[] bW1, bW2;

        // ========== Activations (for backprop) ==========
        private double[] a0, a1, a1_bn, a1_drop, a2, a2_bn, a2_drop, a3;
        private double[] aS1, aS2; // Strategy head
        private double[] aW1, aW2; // WinProb head
        private bool[] dropMask1, dropMask2;

        // ========== Momentum buffers (Adam optimizer) ==========
        private double[,] m_w1, v_w1, m_w2, v_w2, m_w3, v_w3;
        private double[,] m_wS1, v_wS1, m_wS2, v_wS2;
        private double[,] m_wW1, v_wW1, m_wW2, v_wW2;
        private double[] m_b1, v_b1, m_b2, v_b2, m_b3, v_b3;
        private double[] m_bS1, v_bS1, m_bS2, v_bS2;
        private double[] m_bW1, v_bW1, m_bW2, v_bW2;
        private long t = 0; // Adam timestep
        private const double Beta1 = 0.9;
        private const double Beta2 = 0.999;
        private const double Epsilon = 1e-8;

        private readonly Lock _lock = new();
        private readonly Random rand = new();

        // ========== Exposed for Visualization ==========
        public double[] InputLayer => a0;
        public double[] Backbone1 => a1;
        public double[] Backbone2 => a2;
        public double[] Backbone3 => a3;
        public double[] StrategyHead => aS1;
        public double[] StrategyOutput => aS2;
        public double[] WinProbHead => aW1;
        public double WinProbOutput => aW2?[0] ?? 0.5;
        public double[] BackboneOutput => a3;

        public MultiHeadNeuralNetwork()
        {
            InitializeWeights();
            InitializeAdam();
        }

        private void InitializeWeights()
        {
            // Xavier/He initialization
            w1 = InitWeight(InputSize, Backbone1Size);
            b1 = new double[Backbone1Size];
            w2 = InitWeight(Backbone1Size, Backbone2Size);
            b2 = new double[Backbone2Size];
            w3 = InitWeight(Backbone2Size, Backbone3Size);
            b3 = new double[Backbone3Size];

            // BatchNorm (init gamma=1, beta=0, mean=0, var=1)
            bn1_gamma = Fill(Backbone1Size, 1.0);
            bn1_beta = new double[Backbone1Size];
            bn1_mean = new double[Backbone1Size];
            bn1_var = Fill(Backbone1Size, 1.0);
            bn2_gamma = Fill(Backbone2Size, 1.0);
            bn2_beta = new double[Backbone2Size];
            bn2_mean = new double[Backbone2Size];
            bn2_var = Fill(Backbone2Size, 1.0);

            // Strategy head
            wS1 = InitWeight(Backbone3Size, StrategyHead1Size);
            bS1 = new double[StrategyHead1Size];
            wS2 = InitWeight(StrategyHead1Size, StrategyOutputSize);
            bS2 = new double[StrategyOutputSize];

            // WinProb head
            wW1 = InitWeight(Backbone3Size, WinProbHead1Size);
            bW1 = new double[WinProbHead1Size];
            wW2 = InitWeight(WinProbHead1Size, WinProbOutputSize);
            bW2 = new double[WinProbOutputSize];

            // Activation buffers
            a0 = new double[InputSize];
            a1 = new double[Backbone1Size];
            a1_bn = new double[Backbone1Size];
            a1_drop = new double[Backbone1Size];
            a2 = new double[Backbone2Size];
            a2_bn = new double[Backbone2Size];
            a2_drop = new double[Backbone2Size];
            a3 = new double[Backbone3Size];
            aS1 = new double[StrategyHead1Size];
            aS2 = new double[StrategyOutputSize];
            aW1 = new double[WinProbHead1Size];
            aW2 = new double[WinProbOutputSize];
            dropMask1 = new bool[Backbone1Size];
            dropMask2 = new bool[Backbone2Size];


        }

        private void InitializeAdam()
        {
            m_w1 = new double[InputSize, Backbone1Size]; v_w1 = new double[InputSize, Backbone1Size];
            m_w2 = new double[Backbone1Size, Backbone2Size]; v_w2 = new double[Backbone1Size, Backbone2Size];
            m_w3 = new double[Backbone2Size, Backbone3Size]; v_w3 = new double[Backbone2Size, Backbone3Size];
            m_wS1 = new double[Backbone3Size, StrategyHead1Size]; v_wS1 = new double[Backbone3Size, StrategyHead1Size];
            m_wS2 = new double[StrategyHead1Size, StrategyOutputSize]; v_wS2 = new double[StrategyHead1Size, StrategyOutputSize];
            m_wW1 = new double[Backbone3Size, WinProbHead1Size]; v_wW1 = new double[Backbone3Size, WinProbHead1Size];
            m_wW2 = new double[WinProbHead1Size, WinProbOutputSize]; v_wW2 = new double[WinProbHead1Size, WinProbOutputSize];
            m_b1 = new double[Backbone1Size]; v_b1 = new double[Backbone1Size];
            m_b2 = new double[Backbone2Size]; v_b2 = new double[Backbone2Size];
            m_b3 = new double[Backbone3Size]; v_b3 = new double[Backbone3Size];
            m_bS1 = new double[StrategyHead1Size]; v_bS1 = new double[StrategyHead1Size];
            m_bS2 = new double[StrategyOutputSize]; v_bS2 = new double[StrategyOutputSize];
            m_bW1 = new double[WinProbHead1Size]; v_bW1 = new double[WinProbHead1Size];
            m_bW2 = new double[WinProbOutputSize]; v_bW2 = new double[WinProbOutputSize];
        }

        // ========== Forward Pass ==========
        public (double[] Strategy, double WinProb) Forward(double[] inputs)
        {
            lock (_lock)
            {
                // Input
                Array.Copy(inputs, a0, Math.Min(inputs.Length, InputSize));

                // Backbone Layer 1: Dense → BN → Dropout → ReLU
                Dense(a0, w1, b1, a1);
                ReLU(a1);
                BatchNorm(a1, bn1_gamma, bn1_beta, bn1_mean, bn1_var, a1_bn);
                Dropout(a1_bn, dropMask1, a1_drop);

                // Backbone Layer 2: Dense → BN → Dropout → ReLU
                Dense(a1_drop, w2, b2, a2);
                ReLU(a2);
                BatchNorm(a2, bn2_gamma, bn2_beta, bn2_mean, bn2_var, a2_bn);
                Dropout(a2_bn, dropMask2, a2_drop);

                // Backbone Layer 3: Dense → ReLU
                Dense(a2_drop, w3, b3, a3);
                ReLU(a3);

                // Strategy Head: Dense → ReLU → Dense → Softmax
                Dense(a3, wS1, bS1, aS1);
                ReLU(aS1);
                Dense(aS1, wS2, bS2, aS2);
                Softmax(aS2);

                // WinProb Head: Dense → ReLU → Dense → Sigmoid
                Dense(a3, wW1, bW1, aW1);
                ReLU(aW1);
                Dense(aW1, wW2, bW2, aW2);
                Sigmoid(aW2);

                return (aS2, aW2[0]);
            }
        }

        // ========== Training ==========
        public void Train(double[] inputs, double[] strategyTargets, double winProbTarget)
        {
            lock (_lock)
            {
                IsTraining = true;
                t++;

                // Forward pass (before training)
                Forward(inputs);



                // Compute gradients via backprop
                // Strategy loss: Categorical Cross-Entropy
                double[] dS2 = new double[StrategyOutputSize];
                for (int i = 0; i < StrategyOutputSize; i++)
                    dS2[i] = aS2[i] - strategyTargets[i]; // Softmax + CCE gradient

                // WinProb loss: MSE
                double[] dW2 =
                [
                    (aW2[0] - winProbTarget) * aW2[0] * (1 - aW2[0]), // MSE + Sigmoid gradient
                ];

                // Backprop through heads
                double[] dS1 = BackpropDense(aS1, wS2, dS2, wS2, bS2, m_wS2, v_wS2, m_bS2, v_bS2);
                ReLUBackward(aS1, dS1);

                double[] dW1 = BackpropDense(aW1, wW2, dW2, wW2, bW2, m_wW2, v_wW2, m_bW2, v_bW2);
                ReLUBackward(aW1, dW1);

                // Combine gradients at backbone output
                double[] d3 = new double[Backbone3Size];
                double[] dS1_to_a3 = BackpropDenseNoUpdate(a3, wS1, dS1);
                double[] dW1_to_a3 = BackpropDenseNoUpdate(a3, wW1, dW1);
                for (int i = 0; i < Backbone3Size; i++)
                    d3[i] = (dS1_to_a3[i] * StrategyLossWeight) + (dW1_to_a3[i] * WinProbLossWeight);

                // Update strategy head layer 1
                UpdateWeightsAdam(a3, wS1, bS1, dS1, m_wS1, v_wS1, m_bS1, v_bS1);
                // Update winprob head layer 1
                UpdateWeightsAdam(a3, wW1, bW1, dW1, m_wW1, v_wW1, m_bW1, v_bW1);

                // Backprop through backbone
                ReLUBackward(a3, d3);
                double[] d2 = BackpropDense(a2_drop, w3, d3, w3, b3, m_w3, v_w3, m_b3, v_b3);
                // Skip dropout/BN backward for simplicity (approximate)
                ReLUBackward(a2, d2);

                double[] d1 = BackpropDense(a1_drop, w2, d2, w2, b2, m_w2, v_w2, m_b2, v_b2);
                ReLUBackward(a1, d1);

                BackpropDense(a0, w1, d1, w1, b1, m_w1, v_w1, m_b1, v_b1);

                IsTraining = false;
            }
        }

        // ========== Layer Operations ==========
        private void Dense(double[] input, double[,] w, double[] b, double[] output)
        {
            int inSize = input.Length;
            int outSize = output.Length;
            for (int j = 0; j < outSize; j++)
            {
                double sum = b[j];
                for (int i = 0; i < inSize; i++)
                    sum += input[i] * w[i, j];
                output[j] = sum;
            }
        }

        private void ReLU(double[] x)
        {
            for (int i = 0; i < x.Length; i++)
                x[i] = x[i] > 0 ? x[i] : 0;
        }

        private void ReLUBackward(double[] activated, double[] gradient)
        {
            for (int i = 0; i < activated.Length; i++)
                if (activated[i] <= 0) gradient[i] = 0;
        }

        private void Softmax(double[] x)
        {
            double max = x[0];
            for (int i = 1; i < x.Length; i++) if (x[i] > max) max = x[i];
            double sum = 0;
            for (int i = 0; i < x.Length; i++) { x[i] = Math.Exp(x[i] - max); sum += x[i]; }
            for (int i = 0; i < x.Length; i++) x[i] /= sum;
        }

        private void Sigmoid(double[] x)
        {
            for (int i = 0; i < x.Length; i++)
                x[i] = 1.0 / (1.0 + Math.Exp(-Math.Clamp(x[i], -45, 45)));
        }

        private void BatchNorm(double[] input, double[] gamma, double[] beta, double[] mean, double[] var, double[] output)
        {
            // Simplified: use running stats (no per-batch update for online learning)
            for (int i = 0; i < input.Length; i++)
            {
                double normalized = (input[i] - mean[i]) / Math.Sqrt(var[i] + 1e-5);
                output[i] = gamma[i] * normalized + beta[i];
            }
        }

        private void Dropout(double[] input, bool[] mask, double[] output)
        {
            double scale = 1.0 / (1.0 - DropoutRate);
            for (int i = 0; i < input.Length; i++)
            {
                if (IsTraining)
                {
                    mask[i] = rand.NextDouble() >= DropoutRate;
                    output[i] = mask[i] ? input[i] * scale : 0;
                }
                else
                {
                    output[i] = input[i];
                }
            }
        }

        private double[] BackpropDense(double[] input, double[,] w, double[] dOut, double[,] wRef, double[] bRef,
            double[,] mw, double[,] vw, double[] mb, double[] vb)
        {
            int inSize = input.Length;
            int outSize = dOut.Length;
            double[] dIn = new double[inSize];

            // Gradient w.r.t. input
            for (int i = 0; i < inSize; i++)
                for (int j = 0; j < outSize; j++)
                    dIn[i] += dOut[j] * w[i, j];

            // Update weights with Adam
            UpdateWeightsAdam(input, wRef, bRef, dOut, mw, vw, mb, vb);

            return dIn;
        }

        private double[] BackpropDenseNoUpdate(double[] input, double[,] w, double[] dOut)
        {
            int inSize = input.Length;
            int outSize = dOut.Length;
            double[] dIn = new double[inSize];
            for (int i = 0; i < inSize; i++)
                for (int j = 0; j < outSize; j++)
                    dIn[i] += dOut[j] * w[i, j];
            return dIn;
        }

        private void UpdateWeightsAdam(double[] input, double[,] w, double[] b, double[] dOut,
            double[,] mw, double[,] vw, double[] mb, double[] vb)
        {
            int inSize = input.Length;
            int outSize = dOut.Length;

            for (int i = 0; i < inSize; i++)
            {
                for (int j = 0; j < outSize; j++)
                {
                    double g = dOut[j] * input[i];
                    mw[i, j] = Beta1 * mw[i, j] + (1 - Beta1) * g;
                    vw[i, j] = Beta2 * vw[i, j] + (1 - Beta2) * g * g;
                    double mHat = mw[i, j] / (1 - Math.Pow(Beta1, t));
                    double vHat = vw[i, j] / (1 - Math.Pow(Beta2, t));
                    w[i, j] -= LearningRate * mHat / (Math.Sqrt(vHat) + Epsilon);
                }
            }

            for (int j = 0; j < outSize; j++)
            {
                double g = dOut[j];
                mb[j] = Beta1 * mb[j] + (1 - Beta1) * g;
                vb[j] = Beta2 * vb[j] + (1 - Beta2) * g * g;
                double mHat = mb[j] / (1 - Math.Pow(Beta1, t));
                double vHat = vb[j] / (1 - Math.Pow(Beta2, t));
                b[j] -= LearningRate * mHat / (Math.Sqrt(vHat) + Epsilon);
            }
        }

        // ========== Utilities ==========
        private double[,] InitWeight(int inSize, int outSize)
        {
            double[,] w = new double[inSize, outSize];
            double scale = Math.Sqrt(2.0 / inSize); // He init
            for (int i = 0; i < inSize; i++)
                for (int j = 0; j < outSize; j++)
                    w[i, j] = (rand.NextDouble() * 2 - 1) * scale;
            return w;
        }

        private double[] Fill(int size, double value)
        {
            double[] arr = new double[size];
            for (int i = 0; i < size; i++) arr[i] = value;
            return arr;
        }

        // ========== Serialization ==========
        // Use jagged arrays because System.Text.Json doesn't support 2D arrays
        public class NetworkData
        {
            public double[][]? W1 { get; set; }
            public double[]? B1 { get; set; }
            public double[][]? W2 { get; set; }
            public double[]? B2 { get; set; }
            public double[][]? W3 { get; set; }
            public double[]? B3 { get; set; }
            public double[][]? WS1 { get; set; }
            public double[]? BS1 { get; set; }
            public double[][]? WS2 { get; set; }
            public double[]? BS2 { get; set; }
            public double[][]? WW1 { get; set; }
            public double[]? BW1 { get; set; }
            public double[][]? WW2 { get; set; }
            public double[]? BW2 { get; set; }
            public double[]? BN1_Gamma { get; set; }
            public double[]? BN1_Beta { get; set; }
            public double[]? BN2_Gamma { get; set; }
            public double[]? BN2_Beta { get; set; }
        }

        private static double[][] ToJagged(double[,] arr)
        {
            int rows = arr.GetLength(0);
            int cols = arr.GetLength(1);
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                    result[i][j] = arr[i, j];
            }
            return result;
        }

        private static double[,] To2D(double[][] arr)
        {
            if (arr == null || arr.Length == 0) return new double[0, 0];
            int rows = arr.Length;
            int cols = arr[0].Length;
            double[,] result = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = arr[i][j];
            return result;
        }

        public void Save(string path)
        {
            lock (_lock)
            {
                var data = new NetworkData
                {
                    W1 = ToJagged(w1),
                    B1 = b1,
                    W2 = ToJagged(w2),
                    B2 = b2,
                    W3 = ToJagged(w3),
                    B3 = b3,
                    WS1 = ToJagged(wS1),
                    BS1 = bS1,
                    WS2 = ToJagged(wS2),
                    BS2 = bS2,
                    WW1 = ToJagged(wW1),
                    BW1 = bW1,
                    WW2 = ToJagged(wW2),
                    BW2 = bW2,
                    BN1_Gamma = bn1_gamma,
                    BN1_Beta = bn1_beta,
                    BN2_Gamma = bn2_gamma,
                    BN2_Beta = bn2_beta
                };
                File.WriteAllText(path, JsonSerializer.Serialize(data));
            }
        }

        public static MultiHeadNeuralNetwork? Load(string path)
        {
            if (!File.Exists(path)) return null;
            try
            {
                var data = JsonSerializer.Deserialize<NetworkData>(File.ReadAllText(path));
                if (data == null) return null;

                var net = new MultiHeadNeuralNetwork();
                if (data.W1 != null) net.w1 = To2D(data.W1);
                if (data.B1 != null) net.b1 = data.B1;
                if (data.W2 != null) net.w2 = To2D(data.W2);
                if (data.B2 != null) net.b2 = data.B2;
                if (data.W3 != null) net.w3 = To2D(data.W3);
                if (data.B3 != null) net.b3 = data.B3;
                if (data.WS1 != null) net.wS1 = To2D(data.WS1);
                if (data.BS1 != null) net.bS1 = data.BS1;
                if (data.WS2 != null) net.wS2 = To2D(data.WS2);
                if (data.BS2 != null) net.bS2 = data.BS2;
                if (data.WW1 != null) net.wW1 = To2D(data.WW1);
                if (data.BW1 != null) net.bW1 = data.BW1;
                if (data.WW2 != null) net.wW2 = To2D(data.WW2);
                if (data.BW2 != null) net.bW2 = data.BW2;
                if (data.BN1_Gamma != null) net.bn1_gamma = data.BN1_Gamma;
                if (data.BN1_Beta != null) net.bn1_beta = data.BN1_Beta;
                if (data.BN2_Gamma != null) net.bn2_gamma = data.BN2_Gamma;
                if (data.BN2_Beta != null) net.bn2_beta = data.BN2_Beta;
                return net;
            }
            catch
            {
                return null;
            }
        }
    }
}
