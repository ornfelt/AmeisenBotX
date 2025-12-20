#nullable enable
using AmeisenBotX.Core;
using AmeisenBotX.Core.Engines.Movement.AI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AmeisenBotX
{
    public partial class AiDebugWindow : Window
    {
        private readonly AmeisenBotInterfaces bot;
        private DispatcherTimer timer;
        private bool isSubscribed = false;

        public AiDebugWindow(AmeisenBotInterfaces bot)
        {
            InitializeComponent();
            this.bot = bot;

            // Watchdog for late AI initialization
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += ConnectionTick;
            timer.Start();

            // Try initial connect
            ConnectionTick(this, EventArgs.Empty);
        }

        private void ConnectionTick(object? sender, EventArgs e)
        {
            var analyzer = bot.CombatAi?.Analyzer as AmeisenBotX.Core.Engines.Movement.AI.CombatStateAnalyzer;

            if (analyzer != null && !isSubscribed)
            {
                analyzer.OnAnalysisUpdated += OnAnalysisResult;
                isSubscribed = true;
            }

            if (analyzer == null && isSubscribed)
            {
                isSubscribed = false;
            }

            // Update status based on state
            if (bot.CombatAi == null)
            {
                StatusText.Text = "AI Not Initialized (CombatAi is null)";
                return;
            }

            if (analyzer == null)
            {
                StatusText.Text = "Analyzer is null";
                return;
            }

            // Check if we have a target
            bool hasTarget = bot.Target != null && !bot.Target.IsDead;

            if (!hasTarget)
            {
                // No Target - Show clear state
                StrategyText.Text = "No Target";
                WinProbText.Text = "---";
                WinProbText.Foreground = System.Windows.Media.Brushes.Gray;
                StatusText.Text = "Waiting for target...";

                // Clear brain canvas and show no target message
                BrainCanvas.Children.Clear();
                TextBlock noTarget = new TextBlock
                {
                    Text = "🎯 No Target",
                    Foreground = Brushes.Gray,
                    FontSize = 16
                };
                Canvas.SetLeft(noTarget, BrainCanvas.ActualWidth / 2 - 40);
                Canvas.SetTop(noTarget, BrainCanvas.ActualHeight / 2 - 10);
                BrainCanvas.Children.Add(noTarget);

                StrategyText.Text = "---";
                WinProbText.Text = "---";
                WinProbText.Foreground = Brushes.Gray;
                return;
            }

            // Force periodic update even outside of combat (for visualization)
            try
            {
                analyzer.Tick();
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Tick Error: {ex.Message}";
                return;
            }

            // Directly draw brain as fallback
            var brain = analyzer.Learner?.Brain;
            if (brain != null)
            {
                BrainCanvas.Children.Clear();
                DrawBrain(brain);

                // Update status with target info
                string targetName = $"#{bot.Target?.EntryId ?? 0}";
                int targetLevel = bot.Target?.Level ?? 0;
                StrategyText.Text = $"{analyzer.CurrentStrategy} vs L{targetLevel} {targetName}";
                WinProbText.Text = $"{analyzer.CurrentWinProbability:P0}";
                StatusText.Text = analyzer.CurrentAnalysisReason ?? "Active";

                // Color code probability
                float prob = analyzer.CurrentWinProbability;
                if (prob > 0.7f) WinProbText.Foreground = System.Windows.Media.Brushes.LightGreen;
                else if (prob > 0.4f) WinProbText.Foreground = System.Windows.Media.Brushes.Yellow;
                else WinProbText.Foreground = System.Windows.Media.Brushes.Red;
            }
            else
            {
                StatusText.Text = "Brain is null";
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            timer.Stop();

            if (isSubscribed && bot.CombatAi != null && bot.CombatAi.Analyzer != null)
            {
                var analyzer = bot.CombatAi.Analyzer as AmeisenBotX.Core.Engines.Movement.AI.CombatStateAnalyzer;
                if (analyzer != null)
                {
                    analyzer.OnAnalysisUpdated -= OnAnalysisResult;
                }
            }
        }

        private void OnAnalysisResult(float prob, string reason, AmeisenBotX.Core.Engines.Movement.AI.AiCombatStrategy strategy)
        {
            Dispatcher.Invoke(() =>
            {
                // Update Labels
                StrategyText.Text = strategy.ToString();
                WinProbText.Text = $"{prob:P0}";
                StatusText.Text = reason;

                // Color code probability
                if (prob > 0.7f) WinProbText.Foreground = Brushes.LightGreen;
                else if (prob > 0.4f) WinProbText.Foreground = Brushes.Yellow;
                else WinProbText.Foreground = Brushes.Red;

                // Redraw Brain
                var analyzer = bot.CombatAi.Analyzer as AmeisenBotX.Core.Engines.Movement.AI.CombatStateAnalyzer;
                var brain = analyzer?.Learner?.Brain;
                if (brain != null)
                {
                    BrainCanvas.Children.Clear();
                    DrawBrain(brain);
                }
            });
        }

        private void DrawBrain(MultiHeadNeuralNetwork brain)
        {
            double canvasWidth = BrainCanvas.ActualWidth;
            double canvasHeight = BrainCanvas.ActualHeight;
            if (canvasWidth == 0 || canvasHeight == 0) return;

            // Get layer activations and deltas
            double[] a0 = brain.InputLayer ?? new double[20];
            double[] a1 = brain.Backbone1 ?? new double[64];
            double[] a2 = brain.Backbone2 ?? new double[64];
            double[] a3 = brain.Backbone3 ?? new double[32];
            double[] aS1 = brain.StrategyHead ?? new double[32]; // FIX: Added missing variable
            double[] aS2 = brain.StrategyOutput ?? new double[6];
            double[] aW1 = brain.WinProbHead ?? new double[16];   // FIX: Added missing variable
            double winProb = brain.WinProbOutput;

            // Column X positions - spread across width
            double labelWidth = 165; // Space for input labels (increased)
            double[] colX = {
                labelWidth + 20,                    // Inputs (after labels)
                canvasWidth * 0.25,                 // H1
                canvasWidth * 0.40,                 // H2
                canvasWidth * 0.55,                 // H3
                canvasWidth * 0.70,                 // Split Heads
                canvasWidth * 0.85                  // Outputs
            };

            // === LAYER 0: INPUTS (20 neurons) ===
            var p0 = DrawInputLayer(a0, colX[0], canvasHeight, labelWidth);

            // === HIDDEN LAYERS (ALL nodes) ===
            var p1 = DrawFullLayer(a1, colX[1], canvasHeight, "Hidden 1", 64);
            var p2 = DrawFullLayer(a2, colX[2], canvasHeight, "Hidden 2", 64);
            var p3 = DrawFullLayer(a3, colX[3], canvasHeight, "Hidden 3", 32);

            // === SPLIT HEADS (New Layer) ===
            // We stack them in the 4th column
            // StratHead (32) gets top 70%, WinHead (16) gets bottom 30%
            double headSplitY = canvasHeight * 0.70;
            var pStratHead = DrawFullLayer(aS1, colX[4], headSplitY, "Strat Head", 32, 0);
            var pWinHead = DrawFullLayer(aW1, colX[4], canvasHeight - headSplitY, "Win Head", 16, headSplitY);

            // === OUTPUTS (7 nodes) ===
            var pOut = DrawOutputLayer(aS2, winProb, colX[5], canvasHeight);

            // Draw connections using REAL node positions
            // We use a density factor to avoid drawing 4000+ lines in WPF (performance)
            // But aligned perfectly to the nodes.
            DrawConnectionLines(p0, p1, 0.4);
            DrawConnectionLines(p1, p2, 0.2);
            DrawConnectionLines(p2, p3, 0.3);

            // Back3 feeds BOTH heads (Full connection)
            DrawConnectionLines(p3, pStratHead, 0.5);
            DrawConnectionLines(p3, pWinHead, 0.5);

            // Heads feed their specific outputs
            // pOut has 7 points. First 6 are Strategy, Last 1 is Win.
            if (pOut.Count >= 7)
            {
                var pOutStrat = pOut.GetRange(0, 6);
                var pOutWin = pOut.GetRange(6, 1);

                DrawConnectionLines(pStratHead, pOutStrat, 1.0); // Full connect
                DrawConnectionLines(pWinHead, pOutWin, 1.0); // Full connect
            }
        }

        private void DrawConnectionLines(List<Point> fromPoints, List<Point> toPoints, double density)
        {
            if (fromPoints == null || toPoints == null) return;

            var random = new Random(1234); // Seed for stability

            foreach (var p1 in fromPoints)
            {
                foreach (var p2 in toPoints)
                {
                    // Randomly skip lines based on density, unless density is 1.0
                    if (density < 1.0 && random.NextDouble() > density) continue;

                    Line line = new Line
                    {
                        X1 = p1.X,
                        Y1 = p1.Y,
                        X2 = p2.X,
                        Y2 = p2.Y,
                        Stroke = new SolidColorBrush(Color.FromArgb(20, 255, 165, 0)), // Faint orange (20 alpha)
                        StrokeThickness = 0.5
                    };
                    BrainCanvas.Children.Add(line);
                }
            }
        }

        private List<Point> DrawInputLayer(double[] activations, double x, double canvasHeight, double labelWidth)
        {
            var points = new List<Point>();
            int count = activations.Length;
            double nodeSize = Math.Min(14, (canvasHeight - 40) / count * 0.7);
            double yStep = (canvasHeight - 20) / (count);

            for (int i = 0; i < count; i++)
            {
                double y = 10 + yStep * (i + 0.5);
                double val = Math.Clamp(Math.Abs(activations[i]), 0, 1);

                // Neuron: white→orange fade
                byte r = 255, g = (byte)(255 - val * 110), b = (byte)(255 - val * 255);
                Ellipse el = new Ellipse { Width = nodeSize, Height = nodeSize, Fill = new SolidColorBrush(Color.FromRgb(r, g, b)), Stroke = Brushes.Orange, StrokeThickness = 1 };
                Canvas.SetLeft(el, x - nodeSize / 2); Canvas.SetTop(el, y - nodeSize / 2);
                BrainCanvas.Children.Add(el);

                // Track center point for connections (right side of ellipse for inputs?)
                // Actually center-to-center looks best for mesh
                points.Add(new Point(x, y));

                // Full label with value
                string valStr = $"{activations[i]:F2}";
                TextBlock tb = new TextBlock { Text = $"{InputLabels[i]} {valStr}", Foreground = Brushes.White, FontSize = 12, FontWeight = FontWeights.SemiBold };
                Canvas.SetLeft(tb, 4); Canvas.SetTop(tb, y - 8);
                BrainCanvas.Children.Add(tb);
            }
            return points;
        }

        private List<Point> DrawFullLayer(double[] activations, double x, double height, string layerName, int expectedCount, double yOffset = 0)
        {
            var points = new List<Point>();
            int count = Math.Min(activations.Length, expectedCount);
            // Use local height for spacing
            double nodeSize = Math.Min(10, Math.Max(4, (height - 20) / count * 0.6));
            double yStep = (height - 20) / count;

            // Layer title
            TextBlock title = new TextBlock { Text = layerName, Foreground = Brushes.Orange, FontSize = 13, FontWeight = FontWeights.Bold };
            Canvas.SetLeft(title, x - 25); Canvas.SetTop(title, yOffset + 1);
            BrainCanvas.Children.Add(title);

            for (int i = 0; i < count; i++)
            {
                double y = yOffset + 15 + yStep * (i + 0.5);
                double val = Math.Clamp(activations[i], 0, 1);

                // Neuron color
                byte r = 255, g = (byte)(255 - val * 110), b = (byte)(255 - val * 255);
                Ellipse el = new Ellipse { Width = nodeSize, Height = nodeSize, Fill = new SolidColorBrush(Color.FromRgb(r, g, b)), Stroke = Brushes.DarkOrange, StrokeThickness = 0.5 };
                Canvas.SetLeft(el, x - nodeSize / 2); Canvas.SetTop(el, y - nodeSize / 2);
                BrainCanvas.Children.Add(el);

                points.Add(new Point(x, y));
            }
            return points;
        }

        private List<Point> DrawOutputLayer(double[] strategy, double winProb, double x, double canvasHeight)
        {
            var points = new List<Point>();
            int totalOutputs = 7; // 6 strategies + 1 winprob
            double yStep = (canvasHeight - 30) / totalOutputs;
            double nodeSize = 18;

            // Title
            TextBlock title = new TextBlock { Text = "Outputs", Foreground = Brushes.Orange, FontSize = 13, FontWeight = FontWeights.Bold };
            Canvas.SetLeft(title, x - 15); Canvas.SetTop(title, 1);
            BrainCanvas.Children.Add(title);

            // Strategy outputs (6)
            for (int i = 0; i < 6 && i < strategy.Length; i++)
            {
                // Align Y with mesh generation: 20 + yStep*(i+0.5)
                double y = 20 + yStep * (i + 0.5);
                double val = strategy[i];

                // Color: intensity based on activation
                Color fillColor = (i == 5 && val > 0.3) ? Color.FromRgb(255, 50, 50) : Color.FromRgb((byte)(50 + val * 205), (byte)(50 + val * 150), (byte)(50 + val * 50));
                Brush strokeColor = (i == 5) ? Brushes.Red : Brushes.Orange;

                Ellipse el = new Ellipse { Width = nodeSize, Height = nodeSize, Fill = new SolidColorBrush(fillColor), Stroke = strokeColor, StrokeThickness = 2 };
                Canvas.SetLeft(el, x - nodeSize / 2); Canvas.SetTop(el, y - nodeSize / 2);
                BrainCanvas.Children.Add(el);

                points.Add(new Point(x, y));

                // Full label
                var textColor = (i == 5 && val > 0.3) ? Brushes.Red : Brushes.White;
                var fontWeight = val > 0.3 ? FontWeights.Bold : FontWeights.Normal;
                TextBlock tb = new TextBlock { Text = $"{StrategyLabelsFull[i]} {val:P0}", Foreground = textColor, FontSize = 13, FontWeight = fontWeight };
                Canvas.SetLeft(tb, x + nodeSize / 2 + 18); Canvas.SetTop(tb, y - 10);
                BrainCanvas.Children.Add(tb);
            }

            // WinProb (7th output)
            // Align Y with mesh generation: 20 + yStep*(6+0.5)
            double winY = 20 + yStep * (6 + 0.5);
            byte wg = (byte)(255 - winProb * 110), wb = (byte)(255 - winProb * 255);
            Ellipse winEl = new Ellipse { Width = 24, Height = 24, Fill = new SolidColorBrush(Color.FromRgb(255, wg, wb)), Stroke = Brushes.Gold, StrokeThickness = 3 };
            Canvas.SetLeft(winEl, x - 12); Canvas.SetTop(winEl, winY - 12);
            BrainCanvas.Children.Add(winEl);

            points.Add(new Point(x, winY));

            TextBlock winTb = new TextBlock { Text = $"🏆 Win Prob {winProb:P0}", Foreground = Brushes.Gold, FontSize = 14, FontWeight = FontWeights.Bold };
            Canvas.SetLeft(winTb, x + 30); Canvas.SetTop(winTb, winY - 10);
            BrainCanvas.Children.Add(winTb);

            return points;
        }

        // 20-input labels - FULL names
        private readonly string[] InputLabels =
        [
            "⚔️ HP Advantage", "⚡ My Power", "🎯 Target Power", "📊 Level Threat",
            "👥 Enemy Count", "🛡️ Party Size", "💀 Threat Level", "🎪 Target Control",
            "💔 Incoming DPS", "⚔️ Outgoing DPS", "👑 Is Elite", "🏰 In Instance", "🗡️ Is PVP",
            "💚 Enemy Healer", "🧑 Is Player", "🆘 Critical HP", "❤️ Target HP%",
            "🔮 Target Casting", "📏 Distance", "⏱️ Combat Duration"
        ];

        // Full strategy names
        private readonly string[] StrategyLabelsFull =
        [
            "🏃 Flee", "🛡️ Survival", "💥 Burst", "⚔️ Standard", "🌾 Farm", "⚡ Interrupt"
        ];

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

