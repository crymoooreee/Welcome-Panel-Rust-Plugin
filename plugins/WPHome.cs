using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("WPHome", "crymoooreee", "1.0.0")]
    [Description("Home Information Panel for Welcome Panel")]

    public class WPHome : RustPlugin
    {
        #region Configuration

        private Configuration config;

        protected override void LoadConfig()
        {
            base.LoadConfig();
            config = Config.ReadObject<Configuration>();
            SaveConfig();
        }

        protected override void LoadDefaultConfig() => config = Configuration.CreateConfig();

        protected override void SaveConfig() => Config.WriteObject(config);

        class Configuration
        {
            [JsonProperty("Home Panel")]
            public HomePanel homePanel { get; set; }

            public class HomePanel
            {
                [JsonProperty("Main Panel Color")]
                public string mainColor { get; set; }

                [JsonProperty("Secondary Panel Color")]
                public string secColor { get; set; }

                [JsonProperty("Welcome Message")]
                public string welcomeMessage { get; set; }

                [JsonProperty("QR Discord Code")]
                public string qrDs { get; set; }
                
                [JsonProperty("QR VK Code")]
                public string qrVk { get; set; }

                [JsonProperty("QR TG Code")]
                public string qrTg { get; set; }
            }

            public static Configuration CreateConfig()
            {
                return new Configuration
                {
                    homePanel = new HomePanel
                    {
                        mainColor = "0.25 0.25 0.25 0.65",
                        secColor = "0.19 0.19 0.19 0.85",
                        welcomeMessage = "ВЫ ИГРАЕТЕ НА СЕРВЕРЕ RUST CHILL\nСЕРВЕР БЕЗ ЛИМИТА ИГРОКОВ В КОМАНДЕ",
                        qrDs = "https://i.imgur.com/SaG9KSg.png",
                        qrVk = "https://i.imgur.com/gQIYC5w.png",
                        qrTg = "https://i.imgur.com/gAGDruE.png",
                    }
                };
            }
        }
        #endregion

        #region UI Handling
        private void ShowHome_API(BasePlayer player)
        {
            var homeUI = CUIClass.CreateOverlay("main", "0 0 0 0", "0 0", "0 0", false, 0.0f, "assets/icons/iconmaterial.mat");

            int currentPlayerCount = BasePlayer.activePlayerList.Count;
            int maxPlayerCount = 100; // Example maximum player count place here your own maximum
            float fillPercentage = (float)currentPlayerCount / maxPlayerCount;

            CUIClass.CreatePanel(ref homeUI, "div_home", "WelcomePanel_content", "0 0 0 0", "0 0", "1 1", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreatePanel(ref homeUI, "home_panel", "div_home", config.homePanel.mainColor, "0 0.2", "1 1", false, 0f, "assets/content/ui/uibackgroundblur.mat");

            CUIClass.CreatePanel(ref homeUI, "welcomeMessagePanel", "home_panel", config.homePanel.secColor, "0.027 0.75", "0.975 0.95", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateText(ref homeUI, "welcomeMessage", "welcomeMessagePanel", "1 1 1 1", config.homePanel.welcomeMessage, 20, "0.025 0", "1 1", TextAnchor.MiddleLeft, "robotocondensed-bold.ttf", "0 0 0 1", "1 1");


            CUIClass.CreatePanel(ref homeUI, "playerCountPanel", "welcomeMessagePanel", config.homePanel.mainColor, "0.75 0.1", "0.975 0.9", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateText(ref homeUI, "playerCountText", "playerCountPanel", "1 1 1 1", $"Игоков онайлн: {currentPlayerCount}", 14, "0 0", "1 1", TextAnchor.MiddleCenter, "robotocondensed-bold.ttf", "0 0 0 1", "1 1"); // Added left margin
            CUIClass.CreatePanel(ref homeUI, "smallCircle", "playerCountPanel", "0.55 0.78 0.24 1.00", "0.06 0.45", "0.1 0.55", false, 0f);
            
            // Online progress bar
            CUIClass.CreatePanel(ref homeUI, "progressBar", "home_panel", config.homePanel.secColor, "0.027 0.95", "0.974 0.98", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateProgressBar(ref homeUI, "progressBar", fillPercentage, "0.0 0", "1 1");

            // QR Codes
            CUIClass.CreatePanel(ref homeUI, "qrDs_panel", "home_panel", config.homePanel.secColor, "0.027 0.2", "0.325 0.7", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateImage(ref homeUI, "qrDs_panel", config.homePanel.qrDs, "0.05 0.05", "0.95 0.95");

            CUIClass.CreatePanel(ref homeUI, "qrDs_text_panel", "home_panel", config.homePanel.secColor, "0.027 0.05", "0.325 0.15", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateText(ref homeUI, "qrds_text", "qrDs_text_panel", "1 1 1 1", "Наш Discord канал", 20, "0 0", "1 1", TextAnchor.MiddleCenter, "robotocondensed-bold.ttf", "0 0 0 1", "1 1");


            CUIClass.CreatePanel(ref homeUI, "qrVk_panel", "home_panel", config.homePanel.secColor, "0.35 0.2", "0.65 0.7", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateImage(ref homeUI, "qrVk_panel", config.homePanel.qrVk, "0.05 0.05", "0.95 0.95");

            CUIClass.CreatePanel(ref homeUI, "qrVk_text_panel", "home_panel", config.homePanel.secColor, "0.35 0.05", "0.65 0.15", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateText(ref homeUI, "qrVk_text", "qrVk_text_panel", "1 1 1 1", "Наша группа ВК", 20, "0 0", "1 1", TextAnchor.MiddleCenter, "robotocondensed-bold.ttf", "0 0 0 1", "1 1");

            
            CUIClass.CreatePanel(ref homeUI, "qrDs_panel", "home_panel", config.homePanel.secColor, "0.675 0.2", "0.975 0.7", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateImage(ref homeUI, "qrDs_panel", config.homePanel.qrTg, "0.05 0.05", "0.95 0.95");

            CUIClass.CreatePanel(ref homeUI, "qrTg_text_panel", "home_panel", config.homePanel.secColor, "0.675 0.05", "0.975 0.15", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateText(ref homeUI, "qrTg_text", "qrTg_text_panel", "1 1 1 1", "Наш Telegram канал", 20, "0 0", "1 1", TextAnchor.MiddleCenter, "robotocondensed-bold.ttf", "0 0 0 1", "1 1");

            
            CuiHelper.AddUi(player, homeUI);
        }
        #endregion

        #region CUI Helper Class

        public class CUIClass
        {
            public static void CreateProgressBar(ref CuiElementContainer _container, string _parent, float fillPercentage, string _anchorMin, string _anchorMax)
            {
                string progressBarName = "progressBar";
                string fillColor = "0.55 0.78 0.24 1.00"; // Green color for filled part
                string backgroundColor = "0.5 0.5 0.5 1"; // Grey color for background

                // Create background panel for the Progress Bar
                _container.Add(new CuiPanel
                {
                    Image = { Color = backgroundColor },
                    RectTransform = { AnchorMin = _anchorMin, AnchorMax = _anchorMax },
                    CursorEnabled = false
                }, _parent, progressBarName);

                // Create filled part of the Progress Bar
                _container.Add(new CuiPanel
                {
                    Image = { Color = fillColor },
                    RectTransform = { AnchorMin = _anchorMin, AnchorMax = $"{Mathf.Clamp(fillPercentage, 0, 1) * 1f} 0.94" }, // Adjust width based on fill percentage, reducing height of the green bar

                    CursorEnabled = false
                }, progressBarName, "progressBarFill");
            }


            public static CuiElementContainer CreateOverlay(string _name, string _color, string _anchorMin, string _anchorMax, bool _cursorOn = false, float _fade = 1f, string _mat = "")
            {
                var _element = new CuiElementContainer()
                {
                    {
                        new CuiPanel
                        {
                            Image = { Color = _color, Material = _mat, FadeIn = _fade },
                            RectTransform = { AnchorMin = _anchorMin, AnchorMax = _anchorMax },
                            CursorEnabled = _cursorOn
                        },
                        new CuiElement().Parent = "Overlay",
                        _name
                    }
                };
                return _element;
            }

            public static void CreatePanel(ref CuiElementContainer _container, string _name, string _parent, string _color, string _anchorMin, string _anchorMax, bool _cursorOn = false, float _fade = 1f, string _mat2 = "")
            {
                _container.Add(new CuiPanel
                {
                    Image = { Color = _color, Material = _mat2, FadeIn = _fade },
                    RectTransform = { AnchorMin = _anchorMin, AnchorMax = _anchorMax },
                    CursorEnabled = _cursorOn
                },
                _parent,
                _name);
            }

            public static void CreateText(ref CuiElementContainer _container, string _name, string _parent, string _color, string _text, int _size, string _anchorMin, string _anchorMax, TextAnchor _align = TextAnchor.MiddleCenter, string _font = "robotocondensed-bold.ttf", string _outlineColor = "", string _outlineScale = "")
            {
                _container.Add(new CuiElement
                {
                    Parent = _parent,
                    Name = _name,
                    Components =
                    {
                        new CuiTextComponent
                        {
                            Text = _text,
                            FontSize = _size,
                            Font = _font,
                            Align = _align,
                            Color = _color,
                            FadeIn = 1f
                        },
                        new CuiOutlineComponent
                        {
                            Color = _outlineColor,
                            Distance = _outlineScale
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = _anchorMin,
                            AnchorMax = _anchorMax
                        }
                    }
                });
            }

            public static void CreateImage(ref CuiElementContainer _container, string _parent, string _image, string _anchorMin, string _anchorMax, float _fade = 1f)
            {
                _container.Add(new CuiElement
                {
                    Parent = _parent,
                    Components =
                    {
                        new CuiRawImageComponent
                        {
                            Url = _image,
                            Sprite = "assets/content/textures/generic/fulltransparent.tga",
                            FadeIn = _fade
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = _anchorMin,
                            AnchorMax = _anchorMax
                        }
                    }
                });
            }

            public static void CreateButton(ref CuiElementContainer _container, string _name, string _parent, string _color, string _text, int _size, string _anchorMin, string _anchorMax, string _command = "", string _close = "", string _textColor = "0.843 0.816 0.78 1", float _fade = 1f, TextAnchor _align = TextAnchor.MiddleCenter, string _font = "")
            {       
                _container.Add(new CuiButton
                {
                    Button = { Close = _close, Command = _command, Color = _color, Material = "assets/content/ui/uibackgroundblur-ingamemenu.mat", FadeIn = _fade},
                    RectTransform = { AnchorMin = _anchorMin, AnchorMax = _anchorMax },
                    Text = { Text = _text, FontSize = _size, Align = _align, Color = _textColor, Font = _font, FadeIn = _fade}
                },
                _parent,
                _name);
            }
        }
        #endregion
    }
}
