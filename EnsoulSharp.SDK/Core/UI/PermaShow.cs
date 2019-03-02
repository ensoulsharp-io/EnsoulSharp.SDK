namespace EnsoulSharp.SDK.Core.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Utils;
    using IMenu;
    using IMenu.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     The PermaShow class allows you to add important items to permashow easily.
    /// </summary>
    public static class PermaShow
    {
        #region Static Fields

        /// <summary>
        ///     The default perma show width
        /// </summary>
        private static readonly float DefaultPermaShowWidth = 250f;

        /// <summary>
        ///     The default small box width
        /// </summary>
        private static readonly float DefaultSmallBoxWidth = 45f;

        /// <summary>
        ///     List of items for PermaShow
        /// </summary>
        private static readonly List<PermaShowItem> PermaShowItems = new List<PermaShowItem>();

        /// <summary>
        ///     The x factor
        /// </summary>
        private static readonly float XFactor = Drawing.Width / 1366f;

        /// <summary>
        ///     The y factor
        /// </summary>
        private static readonly float YFactor = Drawing.Height / 768f;

        /// <summary>
        ///     The line for the box.
        /// </summary>
        private static Line BoxLine;

        /// <summary>
        ///     The box position
        /// </summary>
        private static Vector2 BoxPosition;

        /// <summary>
        ///     The default position
        /// </summary>
        private static Vector2 DefaultPosition = new Vector2(
            Drawing.Width - (0.682f * Drawing.Width),
            Drawing.Height - (0.965f * Drawing.Height));

        /// <summary>
        ///     The dragging
        /// </summary>
        private static bool Dragging;

        /// <summary>
        ///     The menu to save position and bool for moving the menu
        /// </summary>
        private static Menu placetosave;

        /// <summary>
        ///     The sprite
        /// </summary>
        private static Sprite sprite;

        /// <summary>
        ///     <c>true</c> if this instance has subscribed to the L# events.
        /// </summary>
        private static bool subbed;

        /// <summary>
        ///     The font for text.
        /// </summary>
        private static Font Text;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="PermaShow" /> class.
        /// </summary>
        public static void Initialize(Menu menu)
        {
            Events.OnLoad += (sender, args) =>
            {
                CreateMenu(menu);
                BoxPosition = GetPosition();
                PrepareDrawing();
            };
        }

        #endregion

        #region Enums

        /// <summary>
        ///     Represents a direction.
        /// </summary>
        private enum Direction
        {
            /// <summary>
            ///     The X direction. (Horizontal)
            /// </summary>
            X,

            /// <summary>
            ///     The Y direction. (Vertical)
            /// </summary>
            Y
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the width of the perma show.
        /// </summary>
        /// <value>The width of the perma show.</value>
        private static float PermaShowWidth
        {
            get
            {
                return ScaleValue(placetosave["bwidth"].GetValue<MenuSlider>().Value, Direction.X);
            }
        }

        /// <summary>
        ///     Gets the width of the small box.
        /// </summary>
        /// <value>The width of the small box.</value>
        private static float SmallBoxWidth
        {
            get
            {
                return ScaleValue(placetosave["swidth"].GetValue<MenuSlider>().Value, Direction.X);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds a menuitem to PermaShow, can be used without any arguements or with if you want to customize. The bool can be
        ///     set to false to remove the item from permashow.
        ///     When removing, you can simply set the bool parameter to false and everything else can be null. The default color is
        ///     White.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="enabled">if set to <c>true</c> the instance will be enabled.</param>
        /// <param name="customdisplayname">The customdisplayname.</param>
        /// <param name="col">The color.</param>
        public static void Permashow(
            this MenuItem item,
            bool enabled = true,
            string customdisplayname = null,
            Color? col = null)
        {
            if (!IsEnabled())
            {
                return;
            }
            if (enabled && PermaShowItems.All(x => x.Item != item))
            {
                if (!PermaShowItems.Any())
                {
                    Sub();
                }
                var dispName = customdisplayname ?? item.DisplayName;
                var itemType = item is MenuBool ? MenuValueType.Boolean
                    : (item is MenuColor ? MenuValueType.Color
                    : (item is MenuKeyBind ? MenuValueType.KeyBind
                    : (item is MenuList ? MenuValueType.List
                    : (item is MenuSlider ? MenuValueType.Slider
                    : MenuValueType.None))));
                Color? color = col ?? new ColorBGRA(255, 255, 255, 255);
                PermaShowItems.Add(
                    new PermaShowItem
                        { DisplayName = dispName, Item = item, Color = (Color)color, ItemType = itemType });
            }
            else if (!enabled)
            {
                var itemtoremove = PermaShowItems.FirstOrDefault(x => x.Item == item);
                if (itemtoremove != null)
                {
                    PermaShowItems.Remove(itemtoremove);
                    if (!PermaShowItems.Any())
                    {
                        Unsub();
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Create the menu
        /// </summary>
        private static void CreateMenu(Menu menu)
        {
            placetosave = new Menu("PermaShow", "Permashow");

            var enablepermashow = new MenuBool("enablepermashow", "Enable PermaShow", true);
            placetosave.Add(enablepermashow);
            placetosave.Add(new MenuSlider("X", "X", (int)DefaultPosition.X, 0, Drawing.Width));
            placetosave.Add(new MenuSlider("Y", "Y", (int)DefaultPosition.Y, 0, Drawing.Height));
            placetosave.Add(new MenuSlider("bwidth", "Width", (int)DefaultPermaShowWidth, 100, 400));
            placetosave.Add(new MenuSlider("swidth", "Indicator Width", (int)DefaultSmallBoxWidth, 30, 90));
            placetosave.Add(new MenuBool("moveable", "Moveable", true));

            var def = new MenuButton("defaults", "Default", "Reset")
            {
                Action = () =>
                {
                    var bwidth = placetosave.GetValue<MenuSlider>("bwidth");
                    if (bwidth != null)
                    {
                        bwidth.Value = (int)DefaultPermaShowWidth;
                        bwidth.MinValue = 100;
                        bwidth.MaxValue = 400;
                    }
                    var swidth = placetosave.GetValue<MenuSlider>("swidth");
                    if (swidth != null)
                    {
                        swidth.Value = (int)DefaultSmallBoxWidth;
                        swidth.MinValue = 30;
                        swidth.MaxValue = 90;
                    }
                }
            };

            placetosave.Add(def);

            menu.Add(placetosave);

            enablepermashow.ValueChanged += (sender, args) =>
                {
                    var boolean = sender as MenuBool;
                    if (boolean != null)
                    {
                        if (boolean.Value && !subbed)
                        {
                            Sub();
                        }
                        else if (!boolean.Value)
                        {
                            Unsub();
                        }
                    }
                };
        }

        /// <summary>
        ///     Fired when the AppDomain is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            BoxLine.OnLostDevice();
            Text.OnLostDevice();
            sprite.OnLostDevice();
            sprite.Dispose();
            Text.Dispose();
            BoxLine.Dispose();
        }

        /// <summary>
        ///     Draws a colored box to indicate booleans true or false with green and red respectively
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="ison">if set to <c>true</c> [ison].</param>
        private static void DrawBox(Vector2 pos, bool ison)
        {
            BoxLine.Width = SmallBoxWidth;

            BoxLine.Begin();

            var positions = new[]
                                {
                                    new Vector2(pos.X, pos.Y),
                                    new Vector2(pos.X, pos.Y + (Text.Description.Height * 1.4f))
                                };

            var col = ison ? Color.DarkGreen : Color.DarkRed;
            col.A = 150;
            BoxLine.Draw(positions, col);

            BoxLine.End();
        }

        /// <summary>
        ///     Fired when everything has been drawn.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="Exception">[PermaShow] - MenuItem not supported</exception>
        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
            {
                return;
            }

            if (!placetosave["enablepermashow"].GetValue<MenuBool>().Value)
            {
                Unsub();
                return;
            }

            PermaArea();

            var halfwidth = 0.96f * (PermaShowWidth / 2);

            var baseposition = new Vector2(BoxPosition.X - halfwidth, BoxPosition.Y);

            var boxx = BoxPosition.X + (PermaShowWidth / 2) - (SmallBoxWidth / 2);

            foreach (var permaitem in PermaShowItems)
            {
                var index = PermaShowItems.IndexOf(permaitem);
                var boxpos = new Vector2(boxx, baseposition.Y + (Text.Description.Height * 1.4f * index));
                var endpos = new Vector2(
                    BoxPosition.X + (PermaShowWidth / 2),
                    baseposition.Y + (Text.Description.Height * 1.4f * index));
                var itempos = new Vector2(baseposition.X, baseposition.Y + (Text.Description.Height * 1.4f * index) + Text.Description.Height * 0.2f);
                var textpos = (int)(endpos.X - (SmallBoxWidth / 1.25f));

                switch (permaitem.ItemType)
                {
                    case MenuValueType.Boolean:
                        DrawBox(boxpos, permaitem.Item.GetValue<MenuBool>().Value);
                        Text.DrawText(
                            null,
                            permaitem.DisplayName + ":",
                            (int)itempos.X,
                            (int)itempos.Y,
                            permaitem.Color);
                        Text.DrawText(
                            null,
                            permaitem.Item.GetValue<MenuBool>().Value.ToString(),
                            textpos,
                            (int)itempos.Y,
                            permaitem.Color);
                        break;
                    case MenuValueType.Slider:
                        Text.DrawText(
                            null,
                            permaitem.DisplayName + ":",
                            (int)itempos.X,
                            (int)itempos.Y,
                            permaitem.Color);
                        Text.DrawText(
                            null,
                            permaitem.Item.GetValue<MenuSlider>().Value.ToString(),
                            textpos,
                            (int)itempos.Y,
                            permaitem.Color);
                        break;
                    case MenuValueType.KeyBind:
                        DrawBox(boxpos, permaitem.Item.GetValue<MenuKeyBind>().Active);
                        Text.DrawText(
                            null,
                            permaitem.DisplayName + " [" + KeyConvert.KeyToText((uint)permaitem.Item.GetValue<MenuKeyBind>().Key)
                            + "] :",
                            (int)itempos.X,
                            (int)itempos.Y,
                            permaitem.Color);
                        Text.DrawText(
                            null,
                            permaitem.Item.GetValue<MenuKeyBind>().Active.ToString(),
                            textpos,
                            (int)itempos.Y,
                            permaitem.Color);
                        break;
                    case MenuValueType.List:
                        Text.DrawText(
                            null,
                            permaitem.DisplayName + ":",
                            (int)itempos.X,
                            (int)itempos.Y,
                            permaitem.Color);
                        var dimen = Text.MeasureText(sprite, permaitem.Item.GetValue<MenuList<string>>().SelectedValue, FontDrawFlags.Left);
                        Text.DrawText(
                            null,
                            permaitem.Item.GetValue<MenuList<string>>().SelectedValue,
                            (int)(textpos + dimen.Width < endpos.X ? textpos : endpos.X - dimen.Width),
                            (int)itempos.Y,
                            permaitem.Color);
                        break;
                    case MenuValueType.None:
                        break;
                    default:
                        throw new Exception("[PermaShow] - MenuItem not supported");
                }
            }
        }

        /// <summary>
        ///     Fired after the DirectX device has been reset.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void DrawingOnOnPostReset(EventArgs args)
        {
            sprite.OnResetDevice();
            Text.OnResetDevice();
            BoxLine.OnResetDevice();
        }

        /// <summary>
        ///     Fired before the DirectX device has been reset.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void DrawingOnPreReset(EventArgs args)
        {
            sprite.OnLostDevice();
            Text.OnLostDevice();
            BoxLine.OnLostDevice();
        }

        /// <summary>
        ///     Return current positions from file
        /// </summary>
        /// <returns>Vector2.</returns>
        private static Vector2 GetPosition()
        {
            return new Vector2(
                placetosave.GetValue<MenuSlider>("X").Value,
                placetosave.GetValue<MenuSlider>("Y").Value);
        }

        /// <summary>
        ///     Determines whether this instance is enabled.
        /// </summary>
        /// <returns><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</returns>
        private static bool IsEnabled()
        {
            return placetosave.GetValue<MenuBool>("enablepermashow").Value;
        }

        /// <summary>
        ///     Gets if the mouse of over the perma show.
        /// </summary>
        /// <returns><c>true</c> if the mouse of over the perma show, <c>false</c> otherwise.</returns>
        private static bool MouseOverArea()
        {
            var pos = Cursor.Position;
            return ((pos.X >= BoxPosition.X - (PermaShowWidth / 2f)) && pos.X <= (BoxPosition.X + (PermaShowWidth / 2f))
                    && pos.Y >= BoxPosition.Y
                    && pos.Y <= (BoxPosition.Y + PermaShowItems.Count * (Text.Description.Height * 1.2f)));
        }

        /// <summary>
        ///     Fired when the window receives a message.
        /// </summary>
        /// <param name="args">The <see cref="WndEventArgs" /> instance containing the event data.</param>
        private static void OnWndProc(WndEventArgs args)
        {
            if (MenuManager.Instance.MenuVisible || !placetosave.GetValue<MenuBool>("moveable").Value)
            {
                Dragging = false;
                return;
            }

            if (Dragging)
            {
                BoxPosition = Cursor.Position;
            }

            if (MouseOverArea() && args.Msg == (uint)WindowsMessages.LBUTTONDOWN)
            {
                if (!Dragging)
                {
                    Dragging = true;
                }
            }
            else if (Dragging && args.Msg == (uint)WindowsMessages.LBUTTONUP)
            {
                Dragging = false;
                BoxPosition = Cursor.Position;
                SavePosition();
            }
        }

        /// <summary>
        ///     Draw area where text will be drawn
        /// </summary>
        private static void PermaArea()
        {
            BoxLine.Width = PermaShowWidth;

            BoxLine.Begin();

            var pos = BoxPosition;

            var positions = new[]
                                {
                                    new Vector2(pos.X, pos.Y),
                                    new Vector2(pos.X, pos.Y + PermaShowItems.Count * (Text.Description.Height * 1.4f))
                                };

            var col = Color.Black;

            BoxLine.Draw(positions, new ColorBGRA(col.B, col.G, col.R, 0.4f));

            BoxLine.End();
        }

        /// <summary>
        ///     Initialize the Drawing tools
        /// </summary>
        private static void PrepareDrawing()
        {
            try
            {
                Text = Constants.EnsoulSharpFont;
                sprite = new Sprite(Drawing.Direct3DDevice);
                BoxLine = new Line(Drawing.Direct3DDevice) { Width = 1 };
            }
            catch (DllNotFoundException ex)
            {
                if (ex.Message.Contains("d3dx9_43"))
                {
                    var msg = "Drawings won't work because DirectX (June 2010) is not installed, install https://www.microsoft.com/en-us/download/details.aspx?id=8109 to fix this problem.";
                    Console.WriteLine(msg);
                    Chat.Print(msg);
                }
            }
        }

        /// <summary>
        ///     Saves the current position
        /// </summary>
        private static void SavePosition()
        {
            var x = placetosave.Components["X"].GetValue<MenuSlider>();
            if (x != null)
            {
                x.Value = (int)BoxPosition.X;
                x.MinValue = 0;
                x.MaxValue = Drawing.Width;
            }
            var y = placetosave.Components["Y"].GetValue<MenuSlider>();
            if (y != null)
            {
                y.Value = (int)BoxPosition.Y;
                y.MinValue = 0;
                y.MaxValue = Drawing.Height;
            }
        }

        /// <summary>
        ///     Scales the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>System.Single.</returns>
        private static float ScaleValue(float value, Direction direction)
        {
            var returnvalue = direction == Direction.X ? value * XFactor : value * YFactor;
            return returnvalue;
        }

        /// <summary>
        ///     Subscribes this instance to L# events.
        /// </summary>
        private static void Sub()
        {
            subbed = true;
            Drawing.OnPreReset += DrawingOnPreReset;
            Drawing.OnPostReset += DrawingOnOnPostReset;
            Drawing.OnDraw += Drawing_OnEndScene;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
            Game.OnWndProc += OnWndProc;
        }

        /// <summary>
        ///     Unsubscribes this instance to L# events.
        /// </summary>
        private static void Unsub()
        {
            subbed = false;
            Drawing.OnPreReset -= DrawingOnPreReset;
            Drawing.OnPostReset -= DrawingOnOnPostReset;
            Drawing.OnDraw -= Drawing_OnEndScene;
            AppDomain.CurrentDomain.DomainUnload -= CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit -= CurrentDomainOnDomainUnload;
            Game.OnWndProc -= OnWndProc;
        }

        #endregion

        /// <summary>
        ///     Class PermaShowItem.
        /// </summary>
        internal class PermaShowItem
        {
            #region Properties

            /// <summary>
            ///     Gets or sets the color.
            /// </summary>
            /// <value>The color.</value>
            internal Color Color { get; set; }

            /// <summary>
            ///     Gets or sets the display name.
            /// </summary>
            /// <value>The display name.</value>
            internal string DisplayName { get; set; }

            /// <summary>
            ///     Gets or sets the item.
            /// </summary>
            /// <value>The item.</value>
            internal MenuItem Item { get; set; }

            /// <summary>
            ///     Gets or sets the type of the item.
            /// </summary>
            /// <value>The type of the item.</value>
            internal MenuValueType ItemType { get; set; }

            #endregion
        }
    }
}