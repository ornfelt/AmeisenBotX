using AmeisenBotX.Common.Math;
using AmeisenBotX.Core;
using AmeisenBotX.Core.Engines.Autopilot.Quest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace AmeisenBotX.Windows
{
    public partial class AutopilotDebugWindow : Window
    {
        private readonly AmeisenBot AmeisenBot;
        private readonly DispatcherTimer UpdateTimer;

        public AutopilotDebugWindow(AmeisenBot ameisenBot)
        {
            InitializeComponent();
            AmeisenBot = ameisenBot;

            UpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            UpdateTimer.Tick += UpdateTimer_Tick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTimer.Start();

            // Event-driven updates
            if (AmeisenBot.Bot.Autopilot?.QuestPulse != null)
            {
                AmeisenBot.Bot.Autopilot.QuestPulse.OnQuestsUpdated += OnQuestUpdateReceived;
            }

            if (AmeisenBot.Bot.Autopilot?.QuestPulse != null)
            {
                AmeisenBot.Bot.Autopilot.QuestPulse.Update();
            }

            // Initial Update
            UpdateTimer_Tick(null, null);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            UpdateTimer.Stop();

            if (AmeisenBot.Bot.Autopilot?.QuestPulse != null)
            {
                AmeisenBot.Bot.Autopilot.QuestPulse.OnQuestsUpdated -= OnQuestUpdateReceived;
            }
        }

        private void OnQuestUpdateReceived()
        {
            // Force update on UI thread
            Dispatcher.Invoke(() => UpdateTimer_Tick(null, null));
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (AmeisenBot.Bot.Autopilot?.QuestPulse != null)
            {
                AmeisenBot.Bot.Autopilot.QuestPulse.Update();
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (AmeisenBot.Bot.Autopilot == null)
            {
                txtCurrentTask.Text = "Autopilot Engine not initialized";
                return;
            }

            // Status
            // We rely on the generic State string for now, but we could cast to AutopilotEngine to get more info
            txtCurrentTask.Text = AmeisenBot.Bot.Autopilot.GetType().GetProperty("State")?.GetValue(AmeisenBot.Bot.Autopilot) as string ?? "Running";
            txtMode.Text = AmeisenBot.Config.Autopilot ? "Autopilot" : "Disabled";

            // Progression
            var player = AmeisenBot.Bot.Player;
            if (player != null)
            {
                txtLevel.Text = player.Level.ToString();
                barXp.Maximum = player.NextLevelXp;
                barXp.Value = player.Xp;
                txtXpText.Text = $"{player.Xp} / {player.NextLevelXp} ({(player.NextLevelXp > 0 ? (player.Xp * 100 / player.NextLevelXp) : 0)}%)";
            }

            // Objectives
            if (AmeisenBot.Bot.Autopilot?.QuestPulse?.ActiveQuests != null)
            {
                var viewModels = new List<ObjectiveViewModel>();
                foreach (var quest in AmeisenBot.Bot.Autopilot.QuestPulse.ActiveQuests)
                {
                    if (quest.Objectives.Count == 0)
                    {
                        // Quest has no explicit objectives (e.g. delivery or completed)
                        // Add a single entry so it appears in the list
                        viewModels.Add(new ObjectiveViewModel
                        {
                            QuestTitle = quest.Title,
                            TargetName = "No Specific Objectives", 
                            StatusText = "InProgress/Ready", 
                            OriginalText = "",
                            LocationText = "Check Log",
                            Rewards = quest.Rewards,
                            QuestId = quest.Id,
                            LogIndex = quest.LogIndex
                        });
                    }
                    else
                    {
                        foreach (var obj in quest.Objectives)
                        {
                            viewModels.Add(new ObjectiveViewModel
                            {
                                QuestTitle = obj.QuestTitle,
                                TargetName = obj.TargetName,
                                OriginalText = obj.OriginalText,
                                // If Type is TalkTo/Event, show 'Talk' or 'Event' instead of 0/1
                                StatusText = (obj.Type == QuestObjectiveType.TalkTo || obj.Type == QuestObjectiveType.Event) 
                                    ? "Talk/Interact" 
                                    : $"{obj.CurrentCount}/{obj.RequiredCount}",
                                LocationText = obj.Location != Vector3.Zero 
                                    ? $"POI: {obj.Location.X:F0}, {obj.Location.Y:F0}" 
                                    : "No POI",
                                 // attach the quest rewards to the first objective for display (simplified)
                                 Rewards = (obj == quest.Objectives.First()) ? quest.Rewards : new List<ParsedQuestReward>(),
                            // Store ID and Index for actions
                            QuestId = quest.Id,
                            LogIndex = quest.LogIndex
                        });
                        }
                    }
                }

                listObjectives.ItemsSource = viewModels;
            }
        }

        private void ButtonAbandon_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.DataContext is ObjectiveViewModel vm)
            {
                if (MessageBox.Show($"Are you sure you want to abandon quest '{vm.QuestTitle}'?", "Abandon Quest", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    string script = $"SelectQuestLogEntry({vm.LogIndex}); SetAbandonQuest(); AbandonQuest();";
                    AmeisenBot.Bot.Wow.ExecuteLuaAndRead((script, "AB_Void"), out _);
                    
                    // Trigger refresh
                    UpdateTimer_Tick(null, null);
                }
            }
        }

        public class ObjectiveViewModel
        {
            public string QuestTitle { get; set; }
            public string TargetName { get; set; }
            public string OriginalText { get; set; }
            public string StatusText { get; set; }
            public string LocationText { get; set; }
            public List<ParsedQuestReward> Rewards { get; set; }
            
            // For actions
            public int QuestId { get; set; }
            public int LogIndex { get; set; }
        }
    }
}
