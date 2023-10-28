using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core;
using AmeisenBotX.Utils;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

/// <summary>
/// Represents a window that displays a map and is associated with an AmeisenBot instance.
/// </summary>
namespace AmeisenBotX
{
    /// <summary>
    /// Represents a window that displays a map and is associated with an AmeisenBot instance.
    /// </summary>
    public partial class MapWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MapWindow class.
        /// </summary>
        /// <param name="ameisenBot">The AmeisenBot instance to associate with the MapWindow.</param>
        public MapWindow(AmeisenBot ameisenBot)
        {
            AmeisenBot = ameisenBot;

            MapTimer = new(250, MapTimerTick);

            MeBrush = new SolidBrush((Color)new ColorConverter().ConvertFromString("#FFFFFFFF"));
            EnemyBrush = new SolidBrush((Color)new ColorConverter().ConvertFromString("#FFFF5D6C"));
            DeadBrush = new SolidBrush((Color)new ColorConverter().ConvertFromString("#FFACACAC"));
            FriendBrush = new SolidBrush((Color)new ColorConverter().ConvertFromString("#FF8CBA51"));
            NeutralBrush = new SolidBrush((Color)new ColorConverter().ConvertFromString("#FFFFE277"));
            DefaultEntityBrush = new SolidBrush((Color)new ColorConverter().ConvertFromString("#FFB4F2E1"));

            DungeonNodeBrush = new SolidBrush((Color)new ColorConverter().ConvertFromString("#FF808080"));
            DungeonNodePen = new((Color)new ColorConverter().ConvertFromString("#FFFFFFFF"), 1);

            PathNodeBrush = new SolidBrush((Color)new ColorConverter().ConvertFromString("#FF00FFFF"));
            PathNodePen = new((Color)new ColorConverter().ConvertFromString("#FFE0FFFF"), 1);

            BlacklistNodeBrush = new SolidBrush((Color)new ColorConverter().ConvertFromString("#FFFF0000"));
            BlacklistNodePen = new((Color)new ColorConverter().ConvertFromString("#FFFF0000"), 1);

            TextBrush = new SolidBrush((Color)new ColorConverter().ConvertFromString("#FFFFFFFF"));
            TextFont = new("Bahnschrift Light", 6, System.Drawing.FontStyle.Regular);

            SubTextBrush = new SolidBrush((Color)new ColorConverter().ConvertFromString("#DCDCDC"));
            SubTextFont = new("Bahnschrift Light", 5, System.Drawing.FontStyle.Regular);

            OreBrush = new SolidBrush((Color)new ColorConverter().ConvertFromString("#FF6F4E37"));
            HerbBrush = new SolidBrush((Color)new ColorConverter().ConvertFromString("#FF7BB661"));

            AmeisenBot.Bot.Objects.OnObjectUpdateComplete += (IEnumerable<IWowObject> wowObjects) => { NeedToUpdateMap = true; };

            InitializeComponent();
        }

        ///<summary>
        ///This method disposes all the resources used by the MapWindow object.
        ///</summary>
        ~MapWindow()
        {
            Bitmap.Dispose();
            Graphics.Dispose();

            BlacklistNodeBrush.Dispose();
            BlacklistNodePen.Dispose();
            BlacklistNodePen.Dispose();
            DeadBrush.Dispose();
            DefaultEntityBrush.Dispose();
            DungeonNodeBrush.Dispose();
            DungeonNodePen.Dispose();
            EnemyBrush.Dispose();
            FriendBrush.Dispose();
            HerbBrush.Dispose();
            MeBrush.Dispose();
            NeutralBrush.Dispose();
            OreBrush.Dispose();
            PathNodeBrush.Dispose();
            PathNodePen.Dispose();
            SubTextBrush.Dispose();
            SubTextFont.Dispose();
            TextBrush.Dispose();
            TextFont.Dispose();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Enabled property is enabled or not.
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Gets or sets the AmeisenBot object.
        /// </summary>
        private AmeisenBot AmeisenBot { get; set; }

        /// <summary>
        /// Gets or sets the Bitmap image property.
        /// </summary>
        private Bitmap Bitmap { get; set; }

        /// <summary>
        /// Gets or sets the brush used for blacklisting nodes.
        /// </summary>
        private Brush BlacklistNodeBrush { get; set; }

        /// <summary>
        /// The pen used for the blacklist node.
        /// </summary>
        private Pen BlacklistNodePen { get; set; }

        /// <summary>
        /// Gets or sets the Brush used to represent dead objects.
        /// </summary>
        private Brush DeadBrush { get; set; }

        /// <summary>
        /// Gets or sets the default brush for entities.
        /// </summary>
        private Brush DefaultEntityBrush { get; set; }

        /// <summary>
        /// Gets or sets the brush used for the dungeon nodes.
        /// </summary>
        private Brush DungeonNodeBrush { get; set; }

        ///<summary>
        ///Gets or sets the pen object used for drawing the dungeon nodes.
        ///</summary>
        private Pen DungeonNodePen { get; set; }

        /// <summary>
        /// Gets or sets the brush used for rendering enemies.
        /// </summary>
        private Brush EnemyBrush { get; set; }

        /// <summary>
        /// Gets or sets the brush used for Friend.
        /// </summary>
        private Brush FriendBrush { get; set; }

        /// <summary>
        /// Gets or sets the Graphics object used for drawing.
        /// </summary>
        private Graphics Graphics { get; set; }

        /// <summary>
        /// Gets or sets the brush for the herb.
        /// </summary>
        private Brush HerbBrush { get; set; }

        /// <summary>
        /// Gets the locked timer used for mapping.
        /// </summary>
        private LockedTimer MapTimer { get; }

        /// <summary>
        /// Gets or sets the brush associated with the object.
        /// </summary>
        private Brush MeBrush { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the map needs to be updated.
        /// </summary>
        private bool NeedToUpdateMap { get; set; }

        /// <summary>
        /// Gets or sets the brush used for neutral color.
        /// </summary>
        private Brush NeutralBrush { get; set; }

        /// <summary>
        /// Gets or sets the brush used for Ore.
        /// </summary>
        private Brush OreBrush { get; set; }

        /// <summary>
        /// Gets or sets the brush used to draw the path nodes.
        /// </summary>
        private Brush PathNodeBrush { get; set; }

        /// <summary>
        /// The pen used to draw the path nodes.
        /// </summary>
        private Pen PathNodePen { get; set; }

        /// <summary>
        /// Gets or sets the scale value for the private float property.
        /// </summary>
        private float Scale { get; set; }

        /// <summary>
        /// Gets or sets the brush used for the subtext.
        /// </summary>
        private Brush SubTextBrush { get; set; }

        /// <summary>
        /// Gets or sets the font used for subtext.
        /// </summary>
        private Font SubTextFont { get; set; }

        /// <summary>
        /// Gets or sets the brush used for text rendering.
        /// </summary>
        private Brush TextBrush { get; set; }

        /// <summary>
        /// Gets or sets the font used for text.
        /// </summary>
        private Font TextFont { get; set; }

        /// <summary>
        /// Calculates the relative position of a point based on the given parameters.
        /// </summary>
        /// <param name="posA">The first position vector.</param>
        /// <param name="posB">The second position vector.</param>
        /// <param name="rotation">The rotation in radians.</param>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        /// <param name="scale">The scaling factor (optional, default value is 1.0).</param>
        /// <returns>The relative position as a Point object.</returns>
        private static Point GetRelativePosition(Vector3 posA, Vector3 posB, float rotation, int x, int y, float scale = 1.0f)
        {
            float relativeX = x + ((posA.Y - posB.Y) * scale);
            float relativeY = y + ((posA.X - posB.X) * scale);

            float originX = relativeX - x;
            float originY = relativeY - y;

            float rSin = MathF.Sin(rotation);
            float cSin = MathF.Cos(rotation);

            float newX = originX * cSin - originY * rSin;
            float newY = originX * rSin + originY * cSin;

            return new((int)(newX + x), (int)(newY + y));
        }

        /// <summary>
        /// Renders a blacklisted node on the graphics surface at the specified (x, y) coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the node.</param>
        /// <param name="y">The y-coordinate of the node.</param>
        /// <param name="blacklistNodeBrush">The brush used to fill the node.</param>
        /// <param name="blacklistNodePen">The pen used to draw the node's border.</param>
        /// <param name="graphics">The graphics object used to render the node.</param>
        /// <param name="size">The size of the node.</param>
        /// <param name="radius">The radius of the node's border.</param>
        private static void RenderBlacklistNode(int x, int y, Brush blacklistNodeBrush, Pen blacklistNodePen, Graphics graphics, int size, int radius)
        {
            int offsetStart = (int)(size / 2.0);
            graphics.FillRectangle(blacklistNodeBrush, new(x - offsetStart, y - offsetStart, size, size));
            graphics.DrawEllipse(blacklistNodePen, new(x - radius, y - radius, radius * 2, radius * 2));
        }

        /// <summary>
        /// Renders a game object with a specified width, height, name, dot brush, text brush, text font, graphics, and optional size.
        /// </summary>
        /// <param name="width">The width of the game object.</param>
        /// <param name="height">The height of the game object.</param>
        /// <param name="name">The name of the game object.</param>
        /// <param name="dotBrush">The brush used to paint the dot of the game object.</param>
        /// <param name="textBrush">The brush used to paint the text of the game object.</param>
        /// <param name="textFont">The font used for the text of the game object.</param>
        /// <param name="graphics">The graphics object used to render the game object.</param>
        /// <param name="size">The optional size of the dot of the game object.</param>
        private static void RenderGameobject(int width, int height, string name, Brush dotBrush, Brush textBrush, Font textFont, Graphics graphics, int size = 3)
        {
            int offsetStart = (int)(size / 2.0);
            graphics.FillRectangle(dotBrush, new(width - offsetStart, height - offsetStart, size, size));

            if (!string.IsNullOrEmpty(name))
            {
                float nameWidth = graphics.MeasureString(name, textFont).Width;
                graphics.DrawString(name, textFont, textBrush, width - (nameWidth / 2F), height + 8);
            }
        }

        /// <summary>
        /// Renders a node with specified coordinates, dot brush, line pen, graphics, and size.
        /// </summary>
        /// <param name="x1">The x-coordinate of the first point.</param>
        /// <param name="y1">The y-coordinate of the first point.</param>
        /// <param name="x2">The x-coordinate of the second point.</param>
        /// <param name="y2">The y-coordinate of the second point.</param>
        /// <param name="dotBrush">The brush used to fill the dot.</param>
        /// <param name="linePen">The pen used to draw the line.</param>
        /// <param name="graphics">The graphics object used for rendering.</param>
        /// <param name="size">The size of the dot.</param>
        private static void RenderNode(int x1, int y1, int x2, int y2, Brush dotBrush, Pen linePen, Graphics graphics, int size)
        {
            int offsetStart = (int)(size / 2.0);
            graphics.FillRectangle(dotBrush, new(x1 - offsetStart, y1 - offsetStart, size, size));
            graphics.FillRectangle(dotBrush, new(x2 - offsetStart, y2 - offsetStart, size, size));
            graphics.DrawLine(linePen, x1, y1, x2, y2);
        }

        /// <summary>
        /// Renders a unit on the graphics object with the specified width, height, name, subtext, dot brush, text brush, text font, subtext font, subtext brush, and size.
        /// </summary>
        /// <param name="width">The width of the unit.</param>
        /// <param name="height">The height of the unit.</param>
        /// <param name="name">The name of the unit.</param>
        /// <param name="subtext">The subtext of the unit.</param>
        /// <param name="dotBrush">The brush used to fill the dot.</param>
        /// <param name="textBrush">The brush used to draw the text.</param>
        /// <param name="textFont">The font used for the text.</param>
        /// <param name="subtextFont">The font used for the subtext.</param>
        /// <param name="subTextBrush">The brush used to draw the subtext.</param>
        /// <param name="graphics">The graphics object to render on.</param>
        /// <param name="size">The size of the dot.</param>
        private static void RenderUnit(int width, int height, string name, string subtext, Brush dotBrush, Brush textBrush, Font textFont, Font subtextFont, Brush subTextBrush, Graphics graphics, int size = 3)
        {
            int offsetStart = (int)(size / 2.0);
            graphics.FillRectangle(dotBrush, new(width - offsetStart, height - offsetStart, size, size));

            if (!string.IsNullOrEmpty(name))
            {
                float nameWidth = graphics.MeasureString(name, textFont).Width;
                graphics.DrawString(name, textFont, textBrush, width - (nameWidth / 2F), height + 8);

                float subtextWidth = graphics.MeasureString(subtext, subtextFont).Width;
                graphics.DrawString(subtext, subtextFont, subTextBrush, width - (subtextWidth / 2F), height + 20);
            }
        }

        /// <summary>
        /// Event handler for when the Exit button is clicked.
        /// Disables the button and hides the current form.
        /// </summary>
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Enabled = false;
            Hide();
        }

        /// <summary>
        /// Toggles the visibility of the gridSidemenu element.
        /// If the gridSidemenu is currently visible, it will be collapsed.
        /// If the gridSidemenu is currently collapsed, it will be made visible.
        /// </summary>
        private void ButtonSidebar_Click(object sender, RoutedEventArgs e)
        {
            if (gridSidemenu.Visibility == Visibility.Visible)
            {
                gridSidemenu.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridSidemenu.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Sets the MapRenderCurrentPath property to true when the CheckboxRenderCurrentPath is checked.
        /// </summary>
        private void CheckboxRenderCurrentPath_Checked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderCurrentPath = true;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderCurrentPath is unchecked.
        /// Sets the MapRenderCurrentPath property in the AmeisenBot.Config class to false.
        /// </summary>
        private void CheckboxRenderCurrentPath_Unchecked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderCurrentPath = false;
        }

        /// <summary>
        /// Event handler for when the Dungeon Path checkbox is checked.
        /// Sets the MapRenderDungeonNodes property in the AmeisenBot Config to true.
        /// </summary>
        private void CheckboxRenderDungeonPath_Checked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderDungeonNodes = true;
        }

        /// <summary>
        /// Event handler for the Unchecked event of the CheckboxRenderDungeonPath.
        /// Sets the MapRenderDungeonNodes property of the AmeisenBot.Config class to false.
        /// </summary>
        private void CheckboxRenderDungeonPath_Unchecked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderDungeonNodes = false;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderHerbs is checked.
        /// Sets the MapRenderHerbs property of the AmeisenBot.Config class to true.
        /// </summary>
        private void CheckboxRenderHerbs_Checked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderHerbs = true;
        }

        /// <summary>
        /// Sets the value of the MapRenderHerbs property in the AmeisenBot.Config class to false
        /// when the CheckboxRenderHerbs is unchecked.
        /// </summary>
        private void CheckboxRenderHerbs_Unchecked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderHerbs = false;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderMe is checked.
        /// Sets the MapRenderMe property in the AmeisenBot.Config class to true.
        /// </summary>
        private void CheckboxRenderMe_Checked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderMe = true;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderMe is unchecked.
        /// Sets the MapRenderMe property in the AmeisenBot.Config to false.
        /// </summary>
        private void CheckboxRenderMe_Unchecked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderMe = false;
        }

        /// <summary>
        /// Event handler for when the "CheckboxRenderOres" checkbox is checked. Sets the <see cref="AmeisenBot.Config.MapRenderOres"/> property to true.
        /// </summary>
        private void CheckboxRenderOres_Checked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderOres = true;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderOres is unchecked. It sets the MapRenderOres property in the AmeisenBot.Config class to false.
        /// </summary>
        private void CheckboxRenderOres_Unchecked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderOres = false;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderPlayerInfo is checked.
        /// Sets the MapRenderPlayerExtra property in the AmeisenBot.Config class to true.
        /// </summary>
        private void CheckboxRenderPlayerInfo_Checked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderPlayerExtra = true;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderPlayerInfo is unchecked.
        /// Sets the configuration property MapRenderPlayerExtra to false.
        /// </summary>
        private void CheckboxRenderPlayerInfo_Unchecked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderPlayerExtra = false;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderPlayerNames is checked. 
        /// Sets the MapRenderPlayerNames property in the AmeisenBot.Config to true.
        /// </summary>
        private void CheckboxRenderPlayerNames_Checked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderPlayerNames = true;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderPlayerNames is unchecked.
        /// Sets the MapRenderPlayerNames property of AmeisenBot.Config to false.
        /// </summary>
        private void CheckboxRenderPlayerNames_Unchecked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderPlayerNames = false;
        }

        /// <summary>
        /// Enables the rendering of player names and player information when the checkbox "CheckboxRenderPlayers" is checked.
        /// </summary>
        private void CheckboxRenderPlayers_Checked(object sender, RoutedEventArgs e)
        {
            checkboxRenderPlayerNames.IsEnabled = true;
            checkboxRenderPlayerInfo.IsEnabled = true;
            AmeisenBot.Config.MapRenderPlayers = true;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderPlayers is unchecked.
        /// Disables the checkboxRenderPlayerNames and checkboxRenderPlayerInfo checkboxes.
        /// Sets AmeisenBot.Config.MapRenderPlayers to false.
        /// </summary>
        private void CheckboxRenderPlayers_Unchecked(object sender, RoutedEventArgs e)
        {
            checkboxRenderPlayerNames.IsEnabled = false;
            checkboxRenderPlayerInfo.IsEnabled = false;
            AmeisenBot.Config.MapRenderPlayers = false;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderUnitInfo is checked.
        /// Sets the AmeisenBot.Config.MapRenderUnitExtra property to true.
        /// </summary>
        private void CheckboxRenderUnitInfo_Checked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderUnitExtra = true;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderUnitInfo is unchecked.
        /// Sets the MapRenderUnitExtra property in the AmeisenBot.Config class to false.
        /// </summary>
        private void CheckboxRenderUnitInfo_Unchecked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderUnitExtra = false;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderUnitNames is checked.
        /// Sets the MapRenderUnitNames property in AmeisenBot.Config to true.
        /// </summary>
        private void CheckboxRenderUnitNames_Checked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderUnitNames = true;
        }

        /// <summary>
        /// Event handler for when the CheckboxRenderUnitNames is unchecked.
        /// It sets the MapRenderUnitNames property in the AmeisenBot.Config class to false.
        /// </summary>
        private void CheckboxRenderUnitNames_Unchecked(object sender, RoutedEventArgs e)
        {
            AmeisenBot.Config.MapRenderUnitNames = false;
        }

        /// <summary>
        /// Event handler for when the "CheckboxRenderUnits" is checked.
        /// Enables the "checkboxRenderUnitNames" and "checkboxRenderUnitInfo" checkboxes.
        /// Sets the "MapRenderUnits" property of the "AmeisenBot.Config" object to true.
        /// </summary>
        private void CheckboxRenderUnits_Checked(object sender, RoutedEventArgs e)
        {
            checkboxRenderUnitNames.IsEnabled = true;
            checkboxRenderUnitInfo.IsEnabled = true;
            AmeisenBot.Config.MapRenderUnits = true;
        }

        /// <summary>
        /// Event handler for when the "CheckboxRenderUnits" is unchecked.
        /// Disables the "checkboxRenderUnitNames" and "checkboxRenderUnitInfo" checkboxes.
        /// Sets the "MapRenderUnits" property in the "AmeisenBot.Config" class to false.
        /// </summary>
        private void CheckboxRenderUnits_Unchecked(object sender, RoutedEventArgs e)
        {
            checkboxRenderUnitNames.IsEnabled = false;
            checkboxRenderUnitInfo.IsEnabled = false;
            AmeisenBot.Config.MapRenderUnits = false;
        }

        /// <summary>
        /// Generates a bitmap image of a map using the provided bitmap, graphics, width, and height.
        /// The map image is rendered based on the player's position and rotation.
        /// The map can include various elements such as dungeon nodes, movement path, blacklisted nodes,
        /// game objects (ores and herbs), units/players, and the player's own position.
        /// </summary>
        /// <param name="bitmap">The bitmap object to save the map image.</param>
        /// <param name="graphics">The graphics object used for rendering.</param>
        /// <param name="width">The width of the map image.</param>
        /// <param name="height">The height of the map image.</param>
        /// <returns>A BitmapImage object representing the generated map image.</returns>
        private BitmapImage GenerateMapImage(Bitmap bitmap, Graphics graphics, int width, int height)
        {
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            Graphics.Clear(Color.Transparent);

            if (AmeisenBot.Bot.Player != null)
            {
                Vector3 playerPosition = AmeisenBot.Bot.Player.Position;
                float playerRotation = AmeisenBot.Bot.Player.Rotation;

                // Render current dungeon nodes
                // ---------------------------- >

                if (AmeisenBot.Config.MapRenderDungeonNodes && AmeisenBot.Bot.Dungeon.Nodes?.Count > 0)
                {
                    RenderDungeonNodes(halfWidth, halfHeight, graphics, Scale, playerPosition, playerRotation);
                }

                // Render current movement path
                // ---------------------------- >

                if (AmeisenBot.Config.MapRenderCurrentPath && AmeisenBot.Bot.Movement.Path.Any())
                {
                    RenderCurrentPath(halfWidth, halfHeight, graphics, Scale, playerPosition, playerRotation);
                }

                // Render blacklisted nodes
                // ------------------------ >

                // if
                // (AmeisenBot.Bot.BotCache.TryGetBlacklistPosition((int)AmeisenBot.Bot.ObjectManager.MapId,
                // playerPosition, 64, out List<Vector3> blacklistNodes)) { for (int i = 0; i <
                // blacklistNodes.Count; ++i) { Vector3 node = blacklistNodes[i]; Point
                // nodePositionOnMap = GetRelativePosition(playerPosition, node, playerRotation,
                // halfWidth, halfHeight, scale);
                //
                // RenderBlacklistNode(nodePositionOnMap.X, nodePositionOnMap.Y, BlacklistNodeBrush,
                // BlacklistNodePen, graphics, 3, 32); } }

                // Render Gameobjects
                // ------------------ >

                if (AmeisenBot.Config.MapRenderOres)
                {
                    RenderOres(halfWidth, halfHeight, graphics, Scale, playerPosition, playerRotation);
                }

                if (AmeisenBot.Config.MapRenderHerbs)
                {
                    RenderHerbs(halfWidth, halfHeight, graphics, Scale, playerPosition, playerRotation);
                }

                // Render Units/Players
                // -------------------- >

                if (AmeisenBot.Config.MapRenderUnits || AmeisenBot.Config.MapRenderPlayers)
                {
                    RenderUnits(halfWidth, halfHeight, graphics, Scale, playerPosition, playerRotation);
                }

                if (AmeisenBot.Config.MapRenderMe)
                {
                    RenderUnit(halfWidth, halfHeight, AmeisenBot.Bot.Db.GetUnitName(AmeisenBot.Bot.Player, out string name) ? name : "unknown", "<Me>", MeBrush, TextBrush, TextFont, SubTextFont, SubTextBrush, graphics, 7);
                }
            }

            using MemoryStream memory = new();
            bitmap.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            BitmapImage bitmapImageMap = new();
            bitmapImageMap.BeginInit();
            bitmapImageMap.StreamSource = memory;
            bitmapImageMap.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImageMap.EndInit();

            return bitmapImageMap;
        }

        /// <summary>
        /// Executes when the MapTimer ticks and updates the map canvas if enabled and a map update is needed.
        /// </summary>
        private void MapTimerTick()
        {
            if (Enabled && NeedToUpdateMap)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    int width = (int)mapCanvasBackground.ActualWidth;
                    int height = (int)mapCanvasBackground.ActualHeight;

                    mapCanvas.Source = GenerateMapImage(Bitmap, Graphics, width, height);
                });

                NeedToUpdateMap = false;
            }
        }

        /// <summary>
        /// Renders the current path for a player on the graphics object.
        /// </summary>
        /// <param name="halfWidth">Half the width of the graphics object.</param>
        /// <param name="halfHeight">Half the height of the graphics object.</param>
        /// <param name="graphics">The graphics object used for rendering.</param>
        /// <param name="scale">The scale of the graphics object.</param>
        /// <param name="playerPosition">The position of the player.</param>
        /// <param name="playerRotation">The rotation of the player.</param>
        private void RenderCurrentPath(int halfWidth, int halfHeight, Graphics graphics, float scale, Vector3 playerPosition, float playerRotation)
        {
            List<Vector3> path = AmeisenBot.Bot.Movement.Path.ToList();

            for (int i = 0; i < path.Count; ++i)
            {
                Vector3 node = path[i];
                Vector3 prevNode = i == 0 ? playerPosition : path[i - 1];

                Point nodePositionOnMap = GetRelativePosition(playerPosition, node, playerRotation, halfWidth, halfHeight, scale);
                Point prevNodePositionOnMap = GetRelativePosition(playerPosition, prevNode, playerRotation, halfWidth, halfHeight, scale);

                RenderNode(nodePositionOnMap.X, nodePositionOnMap.Y, prevNodePositionOnMap.X, prevNodePositionOnMap.Y, PathNodeBrush, PathNodePen, graphics, 3);
            }
        }

        ///<summary>
        ///Renders the dungeon nodes on the graphics object.
        ///</summary>
        private void RenderDungeonNodes(int halfWidth, int halfHeight, Graphics graphics, float scale, Vector3 playerPosition, float playerRotation)
        {
            for (int i = 1; i < AmeisenBot.Bot.Dungeon.Nodes.Count; ++i)
            {
                Vector3 node = AmeisenBot.Bot.Dungeon.Nodes[i].Position;
                Vector3 prevNode = AmeisenBot.Bot.Dungeon.Nodes[i - 1].Position;

                Point nodePositionOnMap = GetRelativePosition(playerPosition, node, playerRotation, halfWidth, halfHeight, scale);
                Point prevNodePositionOnMap = GetRelativePosition(playerPosition, prevNode, playerRotation, halfWidth, halfHeight, scale);

                RenderNode(nodePositionOnMap.X, nodePositionOnMap.Y, prevNodePositionOnMap.X, prevNodePositionOnMap.Y, DungeonNodeBrush, DungeonNodePen, graphics, 3);
            }
        }

        ///<summary>
        ///Renders the herbs on the graphics using the specified parameters.
        ///</summary>
        private void RenderHerbs(int halfWidth, int halfHeight, Graphics graphics, float scale, Vector3 playerPosition, float playerRotation)
        {
            IEnumerable<IWowGameobject> herbNodes = AmeisenBot.Bot.Objects.All
                .OfType<IWowGameobject>()
                .Where(e => Enum.IsDefined(typeof(WowHerbId), e.DisplayId));

            for (int i = 0; i < herbNodes.Count(); ++i)
            {
                IWowGameobject gameobject = herbNodes.ElementAt(i);
                Point positionOnMap = GetRelativePosition(playerPosition, gameobject.Position, playerRotation, halfWidth, halfHeight, scale);
                RenderGameobject(positionOnMap.X, positionOnMap.Y, ((WowHerbId)gameobject.DisplayId).ToString(), HerbBrush, TextBrush, TextFont, graphics);
            }
        }

        /// <summary>
        /// Renders the ores in the game.
        /// </summary>
        /// <param name="halfWidth">The half width of the game screen.</param>
        /// <param name="halfHeight">The half height of the game screen.</param>
        /// <param name="graphics">The graphics object used for rendering.</param>
        /// <param name="scale">The scale of the game screen.</param>
        /// <param name="playerPosition">The position of the player.</param>
        /// <param name="playerRotation">The rotation of the player.</param>
        private void RenderOres(int halfWidth, int halfHeight, Graphics graphics, float scale, Vector3 playerPosition, float playerRotation)
        {
            IEnumerable<IWowGameobject> oreNodes = AmeisenBot.Bot.Objects.All
                .OfType<IWowGameobject>()
                .Where(e => Enum.IsDefined(typeof(WowOreId), e.DisplayId));

            for (int i = 0; i < oreNodes.Count(); ++i)
            {
                IWowGameobject gameobject = oreNodes.ElementAt(i);
                Point positionOnMap = GetRelativePosition(playerPosition, gameobject.Position, playerRotation, halfWidth, halfHeight, scale);
                RenderGameobject(positionOnMap.X, positionOnMap.Y, ((WowOreId)gameobject.DisplayId).ToString(), OreBrush, TextBrush, TextFont, graphics);
            }
        }

        /// <summary>
        /// Renders the units on the map with the given parameters.
        /// </summary>
        /// <param name="halfWidth">The half width of the map.</param>
        /// <param name="halfHeight">The half height of the map.</param>
        /// <param name="graphics">The graphics object used for rendering.</param>
        /// <param name="scale">The scale of the map.</param>
        /// <param name="playerPosition">The position of the player.</param>
        /// <param name="playerRotation">The rotation of the player.</param>
        private void RenderUnits(int halfWidth, int halfHeight, Graphics graphics, float scale, Vector3 playerPosition, float playerRotation)
        {
            IEnumerable<IWowUnit> wowUnits = AmeisenBot.Bot.Objects.All
                .OfType<IWowUnit>();

            for (int i = 0; i < wowUnits.Count(); ++i)
            {
                IWowUnit unit = wowUnits.ElementAt(i);

                Brush selectedBrush = unit.IsDead ? DeadBrush : AmeisenBot.Bot.Db.GetReaction(AmeisenBot.Bot.Player, unit) switch
                {
                    WowUnitReaction.Hated => EnemyBrush,
                    WowUnitReaction.Hostile => EnemyBrush,
                    WowUnitReaction.Neutral => NeutralBrush,
                    WowUnitReaction.Friendly => FriendBrush,
                    _ => DefaultEntityBrush,
                };

                Point positionOnMap = GetRelativePosition(playerPosition, unit.Position, playerRotation, halfWidth, halfHeight, scale);

                if (unit.GetType() == typeof(IWowPlayer))
                {
                    if (AmeisenBot.Config.MapRenderPlayers)
                    {
                        string playerName = AmeisenBot.Config.MapRenderPlayerNames && AmeisenBot.Bot.Db.GetUnitName(unit, out string name) ? name : string.Empty;
                        string playerExtra = AmeisenBot.Config.MapRenderPlayerExtra ? $"<{unit.Level} {unit.Race} {unit.Class}>" : string.Empty;

                        RenderUnit(positionOnMap.X, positionOnMap.Y, playerName, playerExtra, selectedBrush, WowColorsDrawing.GetClassPrimaryBrush(unit.Class), TextFont, SubTextFont, SubTextBrush, graphics, 7);
                    }
                }
                else
                {
                    if (AmeisenBot.Config.MapRenderUnits)
                    {
                        string unitName = AmeisenBot.Config.MapRenderUnitNames && AmeisenBot.Bot.Db.GetUnitName(unit, out string name) ? name : string.Empty;
                        string unitExtra = AmeisenBot.Config.MapRenderPlayerExtra ? $"<{unit.Level}>" : string.Empty;

                        RenderUnit(positionOnMap.X, positionOnMap.Y, unitName, unitExtra, selectedBrush, TextBrush, TextFont, SubTextFont, SubTextBrush, graphics);
                    }
                }
            }
        }

        /// <summary>
        /// This method sets up the graphics for the application. It disposes the current graphics and bitmap objects, then creates a new bitmap and graphics object based on the actual width and height of the map canvas background.
        /// </summary>
        private void SetupGraphics()
        {
            if (Graphics != null)
            {
                Graphics.Dispose();
            }

            if (Bitmap != null)
            {
                Bitmap.Dispose();
            }

            int width = (int)mapCanvasBackground.ActualWidth;
            int height = (int)mapCanvasBackground.ActualHeight;

            Bitmap = new(width, height);
            Graphics = Graphics.FromImage(Bitmap);
        }

        /// <summary>
        /// Event handler for when the value of the SliderZoom has changed.
        /// Updates the Scale property with the new value.
        /// </summary>
        private void SliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Scale = (float)sliderZoom.Value;
        }

        /// <summary>
        /// Event handler for the Window_IsVisibleChanged event. Sets the Enabled property to match the visibility of the window.
        /// </summary>
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Enabled = IsVisible;
        }

        /// <summary>
        /// This method is called when the window is loaded. 
        /// It sets up the graphics for the application.
        /// It also sets the visibility of the gridSidemenu to Collapsed. 
        /// It initializes and checks the status of various checkboxes for rendering different elements on the map.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupGraphics();

            Enabled = true;
            gridSidemenu.Visibility = Visibility.Collapsed;

            checkboxRenderCurrentPath.IsChecked = AmeisenBot.Config.MapRenderCurrentPath;
            checkboxRenderDungeonPath.IsChecked = AmeisenBot.Config.MapRenderDungeonNodes;
            checkboxRenderHerbs.IsChecked = AmeisenBot.Config.MapRenderHerbs;
            checkboxRenderMe.IsChecked = AmeisenBot.Config.MapRenderMe;
            checkboxRenderOres.IsChecked = AmeisenBot.Config.MapRenderOres;
            checkboxRenderPlayerInfo.IsChecked = AmeisenBot.Config.MapRenderPlayerExtra;
            checkboxRenderPlayerNames.IsChecked = AmeisenBot.Config.MapRenderPlayerNames;
            checkboxRenderPlayers.IsChecked = AmeisenBot.Config.MapRenderPlayers;
            checkboxRenderUnitInfo.IsChecked = AmeisenBot.Config.MapRenderUnitExtra;
            checkboxRenderUnitNames.IsChecked = AmeisenBot.Config.MapRenderUnitNames;
            checkboxRenderUnits.IsChecked = AmeisenBot.Config.MapRenderUnits;
        }

        /// <summary>
        /// Event handler for the mouse left button down event in the Window. Invokes the method DragMove() which allows the window to be dragged by the mouse.
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// This method is called when the size of the window is changed.
        /// It sets up the graphics for the window.
        /// </summary>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetupGraphics();
        }

        /// <summary>
        /// Event handler for when the window is unloaded.
        /// Disables the window.
        /// </summary>
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Enabled = false;
        }
    }
}