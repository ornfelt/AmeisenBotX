#nullable enable
using AmeisenBotX.BehaviorTree;
using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace AmeisenBotX
{
    public partial class BehaviorTreeDebugWindow : Window
    {
        private readonly AmeisenBotInterfaces bot;
        private readonly DispatcherTimer timer;
        private readonly List<NodeTraceEntry> traceHistory = [];
        private const int MaxHistoryCount = 100;

        // Pre-frozen brushes to prevent memory allocation during updates
        private static readonly SolidColorBrush SuccessBrush;
        private static readonly SolidColorBrush FailedBrush;
        private static readonly SolidColorBrush OngoingBrush;
        private static readonly SolidColorBrush DefaultBrush;
        private static readonly SolidColorBrush SuccessBackgroundBrush;
        private static readonly SolidColorBrush FailedBackgroundBrush;
        private static readonly SolidColorBrush OngoingBackgroundBrush;
        private static readonly SolidColorBrush DefaultBackgroundBrush;
        private static readonly SolidColorBrush GrayBrush;

        private bool isPaused;
        private int updateCounter;
        private string? lastNodeKey; // Track last node to avoid redundant UI updates

        static BehaviorTreeDebugWindow()
        {
            // Freeze all brushes for thread safety and performance
            SuccessBrush = new SolidColorBrush(Color.FromRgb(78, 201, 176));
            SuccessBrush.Freeze();

            FailedBrush = new SolidColorBrush(Color.FromRgb(244, 135, 113));
            FailedBrush.Freeze();

            OngoingBrush = new SolidColorBrush(Color.FromRgb(86, 156, 214));
            OngoingBrush.Freeze();

            DefaultBrush = Brushes.White;

            SuccessBackgroundBrush = new SolidColorBrush(Color.FromArgb(60, 0, 180, 0));
            SuccessBackgroundBrush.Freeze();

            FailedBackgroundBrush = new SolidColorBrush(Color.FromArgb(60, 180, 0, 0));
            FailedBackgroundBrush.Freeze();

            OngoingBackgroundBrush = new SolidColorBrush(Color.FromArgb(60, 0, 120, 200));
            OngoingBackgroundBrush.Freeze();

            DefaultBackgroundBrush = new SolidColorBrush(Color.FromArgb(40, 80, 80, 80));
            DefaultBackgroundBrush.Freeze();

            GrayBrush = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            GrayBrush.Freeze();
        }

        public BehaviorTreeDebugWindow(AmeisenBotInterfaces bot)
        {
            InitializeComponent();
            this.bot = bot;

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            timer.Tick += UpdateTick;
            timer.Start();
        }

        private void UpdateTick(object? sender, EventArgs e)
        {
            if (isPaused)
            {
                return;
            }

            try
            {
                updateCounter++;

                // Get current bot mode
                string mode = bot.Config?.Autopilot == true ? "🤖 Autopilot" : "🎮 Manual";
                ModeText.Text = mode;

                // Get behavior tree execution info
                if (bot.BehaviorTree == null)
                {
                    StatusText.Text = "Tree not available";
                    return;
                }

                // Check tree status
                Tree tree = bot.BehaviorTree;
                StatusText.Text = tree.LastStatus.ToString();

                // Build execution path from tree's last executed nodes
                UpdateExecutionPath(tree);

                // Only refresh node list every 4 ticks (1 second) to reduce UI churn
                if (updateCounter % 4 == 0)
                {
                    RefreshNodeList();
                }
            }
            catch
            {
                // Silently ignore errors to keep debugger stable
                StatusText.Text = "⚠️ Update error";
            }
        }

        private void UpdateExecutionPath(BehaviorTree.Tree tree)
        {
            INode lastNode = tree.LastExecutedNode;
            BtStatus lastStatus = tree.LastStatus;

            if (lastNode == null)
            {
                CurrentGoalText.Text = "Waiting...";
                CurrentGoalSubText.Text = "No tree execution yet";
                ExecutionPathText.Text = "⏳ Waiting for execution...";
                return;
            }

            string nodeName = GetNodeName(lastNode);
            string cleanName = lastNode.Name ?? lastNode.GetType().Name;
            string nodeKey = $"{nodeName}|{lastStatus}";

            // Skip if nothing changed
            if (nodeKey == lastNodeKey)
            {
                return;
            }

            lastNodeKey = nodeKey;

            string statusIcon = GetStatusIcon(lastStatus);

            // Set the prominent goal display
            CurrentGoalText.Text = cleanName;
            CurrentGoalSubText.Text = $"{statusIcon} {lastStatus} at {DateTime.Now:HH:mm:ss}";

            // Color based on status (using frozen brushes)
            CurrentGoalText.Foreground = lastStatus switch
            {
                BtStatus.Success => SuccessBrush,
                BtStatus.Failed => FailedBrush,
                BtStatus.Ongoing => OngoingBrush,
                _ => DefaultBrush
            };

            // Add to history
            NodeTraceEntry entry = new()
            {
                NodeName = nodeName,
                Status = lastStatus,
                Timestamp = DateTime.Now
            };

            traceHistory.Insert(0, entry);
            while (traceHistory.Count > MaxHistoryCount)
            {
                traceHistory.RemoveAt(traceHistory.Count - 1);
            }

            ExecutionPathText.Text = $"{statusIcon} {nodeName} → {lastStatus}";
        }

        private void RefreshNodeList()
        {
            // Batch update: only recreate children if count changed significantly
            // This reduces UI churn during long sessions
            int currentCount = TreePanel.Children.Count;
            int targetCount = Math.Min(traceHistory.Count, 30); // Only show last 30 for performance

            // Clear and rebuild only if needed
            TreePanel.Children.Clear();

            for (int i = 0; i < targetCount; i++)
            {
                NodeTraceEntry entry = traceHistory[i];
                Border border = new()
                {
                    Margin = new Thickness(0, 2, 0, 2),
                    Padding = new Thickness(8, 4, 8, 4),
                    CornerRadius = new CornerRadius(4),
                    Background = GetStatusBackground(entry.Status)
                };

                StackPanel stack = new() { Orientation = Orientation.Horizontal };

                // Status icon
                stack.Children.Add(new TextBlock
                {
                    Text = GetStatusIcon(entry.Status),
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 8, 0)
                });

                // Node name
                stack.Children.Add(new TextBlock
                {
                    Text = entry.NodeName,
                    Foreground = Brushes.White,
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center
                });

                // Timestamp
                stack.Children.Add(new TextBlock
                {
                    Text = $" ({entry.Timestamp:HH:mm:ss})",
                    Foreground = GrayBrush,
                    FontSize = 11,
                    VerticalAlignment = VerticalAlignment.Center
                });

                border.Child = stack;
                TreePanel.Children.Add(border);
            }
        }

        private static string GetNodeName(BehaviorTree.Objects.INode node)
        {
            // Use the node's Name property if set (preferred for debugging)
            if (!string.IsNullOrEmpty(node.Name))
            {
                return $"📌 {node.Name}";
            }

            // Fallback to type name with icon
            string typeName = node.GetType().Name;

            // Remove generic parameters for cleaner display
            int genericIndex = typeName.IndexOf('`');
            if (genericIndex > 0)
            {
                typeName = typeName[..genericIndex];
            }

            // Recursive Logic for Decorators: Show what they are wrapping!
            if (node is BehaviorTree.Objects.TimeLimit timeLimit)
            {
                return $"⏱️ {GetNodeName(timeLimit.Child)}"; // Just show child with timer icon
            }
            return node is BehaviorTree.Objects.Cooldown cooldown
                ? $"❄️ {GetNodeName(cooldown.Child)}"
                : node is BehaviorTree.Objects.AbortIf abortIf
                ? $"🛑 {GetNodeName(abortIf.Child)}"
                : node is BehaviorTree.Objects.Annotator annotator
                ? $"📝 {GetNodeName(annotator.Child)}"
                : typeName switch
                {
                    "Waterfall" => "🌊 Waterfall",
                    "Selector" => "🔀 Selector",
                    "Sequence" => "📋 Sequence",
                    "Leaf" => "🍃 Leaf",
                    "SuccessLeaf" => "✨ SuccessLeaf",
                    "BoolLeaf" => "🔘 BoolLeaf",
                    "Annotator" => "📝 Annotator",
                    "AbortIf" => "🛑 AbortIf",
                    "TimeLimit" => "⏱️ TimeLimit",
                    "Cooldown" => "❄️ Cooldown",
                    "Repeater" => "🔁 Repeater",
                    "Parallel" => "⚡ Parallel",
                    "DualSelector" => "⚖️ DualSelector",
                    "InteractWithUnitLeaf" => "👋 InteractWithUnit",
                    "InteractWithGobjectLeaf" => "📦 InteractWithGobject",
                    _ => $"📦 {typeName}"
                };
        }

        private static string GetStatusIcon(BtStatus status)
        {
            return status switch
            {
                BtStatus.Success => "✅",
                BtStatus.Failed => "❌",
                BtStatus.Ongoing => "🔄",
                _ => "⏸️"
            };
        }

        private static Brush GetStatusBackground(BtStatus status)
        {
            return status switch
            {
                BtStatus.Success => SuccessBackgroundBrush,
                BtStatus.Failed => FailedBackgroundBrush,
                BtStatus.Ongoing => OngoingBackgroundBrush,
                _ => DefaultBackgroundBrush
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            timer.Stop();
            traceHistory.Clear(); // Free memory
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            isPaused = !isPaused;
            ButtonPause.Content = isPaused ? "▶️" : "⏸️";
            ButtonPause.ToolTip = isPaused ? "Resume updates" : "Pause updates";
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            traceHistory.Clear();
            TreePanel.Children.Clear();
            lastNodeKey = null;
        }

        private sealed class NodeTraceEntry
        {
            public string NodeName { get; init; } = "";
            public BtStatus Status { get; init; }
            public DateTime Timestamp { get; init; }
        }
    }
}
