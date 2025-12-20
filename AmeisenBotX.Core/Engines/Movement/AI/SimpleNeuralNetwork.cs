using System;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace AmeisenBotX.Core.Engines.Movement.AI
{
    /// <summary>
    /// Activation function types supported by the neural network.
    /// </summary>
    public enum ActivationType
    {
        Sigmoid,
        LeakyReLU,
        Tanh
    }

    /// <summary>
    /// A production-grade Feed-Forward Neural Network with Backpropagation.
    /// Features: Configurable activation, L2 regularization, learning rate decay, momentum.
    /// </summary>
    public class SimpleNeuralNetwork
    {
        // ========== Public Properties ==========
        public int LayerCount => layerSizes.Length;
        public double[][] Neurons => neurons;       // Expose activations for visualization
        public double[][][] Weights => weights;     // Expose weights for visualization
        public int InputSize => layerSizes?.Length > 0 ? layerSizes[0] : 0;
        public int OutputSize => layerSizes?.Length > 0 ? layerSizes[^1] : 0;

        // ========== Configuration ==========
        public ActivationType HiddenActivation { get; set; } = ActivationType.LeakyReLU;
        public ActivationType OutputActivation { get; set; } = ActivationType.Sigmoid;
        public double LearningRate { get; set; } = 0.01;
        public double Momentum { get; set; } = 0.9;
        public double L2Regularization { get; set; } = 0.0001;  // Weight decay
        public double LearningRateDecay { get; set; } = 0.9999; // Per-step decay

        // ========== Internal State ==========
        private int[] layerSizes;
        private double[][] neurons;           // [Layer][Neuron]
        private double[][] biases;            // [Layer][Neuron]
        private double[][][] weights;         // [Layer][Neuron][PrevNeuron]
        private double[][][] previousWeightUpdates; // For momentum
        private double[][] previousBiasUpdates;     // For momentum
        private double[][] deltaBuffers;            // Reused for training
        private readonly Lock _lock = new();        // Thread safety
        private long trainingSteps = 0;             // For tracking decay

        // ========== Constructor ==========
        public SimpleNeuralNetwork(params int[] layerSizes)
        {
            Init(layerSizes);
        }

        private void Init(int[] sizes)
        {
            layerSizes = sizes;
            int layerCount = sizes.Length;

            neurons = new double[layerCount][];
            biases = new double[layerCount][];
            weights = new double[layerCount][][];
            previousBiasUpdates = new double[layerCount][];
            previousWeightUpdates = new double[layerCount][][];
            deltaBuffers = new double[layerCount][];

            var rand = new Random();

            for (int i = 0; i < layerCount; i++)
            {
                int neuronCount = sizes[i];
                neurons[i] = new double[neuronCount];
                biases[i] = new double[neuronCount];
                previousBiasUpdates[i] = new double[neuronCount];
                deltaBuffers[i] = new double[neuronCount];

                if (i > 0)
                {
                    // He initialization for ReLU variants, Xavier for Sigmoid/Tanh
                    double scale = HiddenActivation == ActivationType.LeakyReLU
                        ? Math.Sqrt(2.0 / sizes[i - 1])  // He
                        : Math.Sqrt(1.0 / sizes[i - 1]); // Xavier

                    for (int j = 0; j < neuronCount; j++)
                    {
                        biases[i][j] = 0.0; // Zero-init biases (modern practice)
                    }

                    int prevCount = sizes[i - 1];
                    weights[i] = new double[neuronCount][];
                    previousWeightUpdates[i] = new double[neuronCount][];

                    for (int j = 0; j < neuronCount; j++)
                    {
                        weights[i][j] = new double[prevCount];
                        previousWeightUpdates[i][j] = new double[prevCount];
                        for (int k = 0; k < prevCount; k++)
                        {
                            weights[i][j][k] = (rand.NextDouble() * 2.0 - 1.0) * scale;
                        }
                    }
                }
            }
        }

        // ========== Forward Pass ==========
        public double[] FeedForward(double[] inputs)
        {
            lock (_lock)
            {
                // Validate inputs
                for (int i = 0; i < inputs.Length && i < neurons[0].Length; i++)
                {
                    double val = inputs[i];
                    neurons[0][i] = double.IsNaN(val) || double.IsInfinity(val) ? 0.0 : val;
                }

                // Propagate through hidden layers
                for (int layer = 1; layer < layerSizes.Length; layer++)
                {
                    bool isOutputLayer = layer == layerSizes.Length - 1;
                    var activation = isOutputLayer ? OutputActivation : HiddenActivation;

                    for (int j = 0; j < layerSizes[layer]; j++)
                    {
                        double sum = biases[layer][j];
                        for (int k = 0; k < layerSizes[layer - 1]; k++)
                        {
                            sum += weights[layer][j][k] * neurons[layer - 1][k];
                        }
                        neurons[layer][j] = Activate(sum, activation);
                    }
                }

                return neurons[^1];
            }
        }

        // ========== Training ==========
        public void Train(double[] inputs, double[] targets)
        {
            lock (_lock)
            {
                // Apply learning rate decay
                double effectiveLR = LearningRate * Math.Pow(LearningRateDecay, trainingSteps);
                trainingSteps++;

                // Forward pass (reuse neurons)
                FeedForward(inputs);

                int outputLayer = layerSizes.Length - 1;

                // Calculate output layer deltas
                for (int i = 0; i < layerSizes[outputLayer]; i++)
                {
                    double output = neurons[outputLayer][i];
                    double error = targets[i] - output;
                    deltaBuffers[outputLayer][i] = error * ActivationDerivative(output, OutputActivation);
                }

                // Calculate hidden layer deltas (backprop)
                for (int layer = outputLayer - 1; layer > 0; layer--)
                {
                    for (int j = 0; j < layerSizes[layer]; j++)
                    {
                        double error = 0;
                        for (int k = 0; k < layerSizes[layer + 1]; k++)
                        {
                            error += deltaBuffers[layer + 1][k] * weights[layer + 1][k][j];
                        }
                        double output = neurons[layer][j];
                        deltaBuffers[layer][j] = error * ActivationDerivative(output, HiddenActivation);
                    }
                }

                // Update weights and biases (SGD with momentum + L2 regularization)
                for (int layer = 1; layer < layerSizes.Length; layer++)
                {
                    for (int j = 0; j < layerSizes[layer]; j++)
                    {
                        // Bias update
                        double biasGradient = deltaBuffers[layer][j];
                        double biasDelta = (effectiveLR * biasGradient) + (Momentum * previousBiasUpdates[layer][j]);
                        biases[layer][j] += biasDelta;
                        previousBiasUpdates[layer][j] = biasDelta;

                        // Weight update with L2 regularization
                        for (int k = 0; k < layerSizes[layer - 1]; k++)
                        {
                            double gradient = deltaBuffers[layer][j] * neurons[layer - 1][k];
                            double regularization = L2Regularization * weights[layer][j][k];
                            double weightDelta = (effectiveLR * (gradient - regularization)) + (Momentum * previousWeightUpdates[layer][j][k]);

                            weights[layer][j][k] += weightDelta;
                            previousWeightUpdates[layer][j][k] = weightDelta;
                        }
                    }
                }
            }
        }

        // ========== Activation Functions ==========
        private double Activate(double x, ActivationType type)
        {
            return type switch
            {
                ActivationType.Sigmoid => Sigmoid(x),
                ActivationType.LeakyReLU => LeakyReLU(x),
                ActivationType.Tanh => Math.Tanh(x),
                _ => Sigmoid(x)
            };
        }

        private double ActivationDerivative(double activated, ActivationType type)
        {
            return type switch
            {
                ActivationType.Sigmoid => activated * (1 - activated),
                ActivationType.LeakyReLU => activated > 0 ? 1.0 : 0.01,
                ActivationType.Tanh => 1 - (activated * activated),
                _ => activated * (1 - activated)
            };
        }

        private double Sigmoid(double x)
        {
            if (x < -45.0) return 0.0;
            if (x > 45.0) return 1.0;
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        private double LeakyReLU(double x) => x > 0 ? x : 0.01 * x;

        // ========== Serialization ==========
        public class NetworkData
        {
            public int[] LayerSizes { get; set; }
            public double[][] Biases { get; set; }
            public double[][][] Weights { get; set; }
            public int HiddenActivation { get; set; }
            public int OutputActivation { get; set; }
            public double LearningRate { get; set; }
            public double L2Regularization { get; set; }
        }

        public void Save(string path)
        {
            lock (_lock)
            {
                var data = new NetworkData
                {
                    LayerSizes = layerSizes,
                    Biases = biases,
                    Weights = weights,
                    HiddenActivation = (int)HiddenActivation,
                    OutputActivation = (int)OutputActivation,
                    LearningRate = LearningRate,
                    L2Regularization = L2Regularization
                };
                File.WriteAllText(path, JsonSerializer.Serialize(data));
            }
        }

        public static SimpleNeuralNetwork Load(string path)
        {
            if (!File.Exists(path)) return null;
            try
            {
                var data = JsonSerializer.Deserialize<NetworkData>(File.ReadAllText(path));
                var net = new SimpleNeuralNetwork(data.LayerSizes);
                net.biases = data.Biases;
                net.weights = data.Weights;
                net.HiddenActivation = (ActivationType)data.HiddenActivation;
                net.OutputActivation = (ActivationType)data.OutputActivation;
                net.LearningRate = data.LearningRate > 0 ? data.LearningRate : 0.01;
                net.L2Regularization = data.L2Regularization;
                net.InitBuffers();
                return net;
            }
            catch (Exception ex)
            {
                AmeisenBotX.Logging.AmeisenLogger.I.Log("SimpleNeuralNetwork", $"Failed to load: {ex.Message}", AmeisenBotX.Logging.Enums.LogLevel.Error);
                return null;
            }
        }

        private void InitBuffers()
        {
            int layerCount = layerSizes.Length;
            previousBiasUpdates = new double[layerCount][];
            previousWeightUpdates = new double[layerCount][][];
            deltaBuffers = new double[layerCount][];

            for (int i = 0; i < layerCount; i++)
            {
                previousBiasUpdates[i] = new double[layerSizes[i]];
                deltaBuffers[i] = new double[layerSizes[i]];
                if (i > 0)
                {
                    previousWeightUpdates[i] = new double[layerSizes[i]][];
                    for (int j = 0; j < layerSizes[i]; j++)
                    {
                        previousWeightUpdates[i][j] = new double[layerSizes[i - 1]];
                    }
                }
            }
        }

        // ========== Utility ==========
        public void ResetMomentum()
        {
            lock (_lock)
            {
                InitBuffers();
                trainingSteps = 0;
            }
        }
    }
}
