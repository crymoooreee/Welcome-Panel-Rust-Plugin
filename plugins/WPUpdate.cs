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
    [Info("WPUpdate", "crymoooreee", "1.0.0")]
    [Description("Update Information Panel for Welcome Panel")]

    public class WPUpdate : RustPlugin
    {
        #region Commands
        [ConsoleCommand("wpupdate.showdetails")]
        private void ShowDetailsCommand(ConsoleSystem.Arg arg)
        {
            var player = arg.Player();
            if (player != null)
            {
                ShowDetailedUpdateInfo_API(player);
                CloseUpdate_API(player);
            }
        }

        [ConsoleCommand("wpupdate.closedetails")]
        private void CloseDetailsCommand(ConsoleSystem.Arg arg)
        {
            var player = arg.Player();
            if (player != null)
            {
                ShowUpdate_API(player);
                CloseDetails_API(player);
            }
        }
        #endregion

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
            [JsonProperty("Update Panel")]
            public UpdatePanel updatePanel { get; set; }

            public class UpdatePanel
            {
                [JsonProperty("Title Text")]
                public string titleText { get; set; }

                [JsonProperty("Description Text")]
                public string descriptionText { get; set; }

                [JsonProperty("Main Panel Color")]
                public string mainColor { get; set; }

                [JsonProperty("Secondary Panel Color")]
                public string secColor { get; set; }

                [JsonProperty("Icon URL")]
                public string iconUrl { get; set; }

                // Detailed update info

                [JsonProperty("Detailed Title Text")]
                public string DetailedTitleText { get; set; }

                [JsonProperty("Second Icon URL")]
                public string SecondiconUrl { get; set; }
                
                [JsonProperty("Detailed First String")]
                public string DetailedFirstString { get; set; }
            }

            public static Configuration CreateConfig()
            {
                return new Configuration
                {
                    updatePanel = new UpdatePanel
                    {
                        titleText = "<size=22>LATEST UPDATE</size>",
                        descriptionText = "<size=14>New features and improvements added!</size>",
                        mainColor = "0.25 0.25 0.25 0.65",
                        secColor = "0.19 0.19 0.19 0.85",
                        iconUrl = "https://i.imgur.com/example.png"
                    }
                };
            }
        }
        #endregion

        #region UI Handling
        private void ShowUpdate_API(BasePlayer player)
        {
            var updateUI = CUIClass.CreateOverlay("main", "0 0 0 0", "0 0", "0 0", false, 0.0f, "assets/icons/iconmaterial.mat");


            CUIClass.CreatePanel(ref updateUI, "div_update", "WelcomePanel_content", config.updatePanel.mainColor, "0.1 0.45", "0.9 0.85", false, 0f, "assets/content/ui/uibackgroundblur.mat");

            
            CUIClass.CreatePanel(ref updateUI, "update_title_panel", "div_update", config.updatePanel.secColor, "0.255 0.55", "0.980 0.8", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateText(ref updateUI, "update_title_text", "update_title_panel", "1 1 1 1", config.updatePanel.titleText, 11, "0.02 0", "1 1", TextAnchor.MiddleLeft, "robotocondensed-bold.ttf", "0 0 0 1", "1 1");


            CUIClass.CreatePanel(ref updateUI, "update_desc_panel", "div_update", config.updatePanel.secColor, "0.255 0.3", "0.980 0.5", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateText(ref updateUI, "update_desc_text", "update_desc_panel", "1 1 1 1", config.updatePanel.descriptionText, 11, "0.02 0", "1 1", TextAnchor.MiddleLeft, "robotocondensed-bold.ttf", "0 0 0 1", "1 1");


            CUIClass.CreatePanel(ref updateUI, "update_icon_panel", "div_update", config.updatePanel.secColor, "0.020 0.3", "0.240 0.794", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateImage(ref updateUI, "update_icon_panel", config.updatePanel.iconUrl, "0 0", "1 1", 0.5f);


            CUIClass.CreateButton(ref updateUI, "btn_details", "div_update", "0.2 0.2 0.2 1", "Подробнее", 22, "0.02 0.02", "0.98 0.25", "wpupdate.showdetails", "", "1 1 1 1");


            CuiHelper.AddUi(player, updateUI);

        }

        private void CloseUpdate_API(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "main");
            CuiHelper.DestroyUi(player, "div_update");
        }
        private void CloseDetails_API(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "main");
            CuiHelper.DestroyUi(player, "div_detail");
        }

        private void ShowDetailedUpdateInfo_API(BasePlayer player)
        {
            var detailUI = CUIClass.CreateOverlay("main", "0 0 0 0", "0 0", "0 0", false, 0.0f, "assets/icons/iconmaterial.mat");

            CUIClass.CreatePanel(ref detailUI, "div_detail", "WelcomePanel_content", "0.2 0.2 0.2 0.8", "0.05 0.1", "0.95 0.9", false, 0f, "");


            CUIClass.CreatePanel(ref detailUI, "detail_icon_panel", "div_detail", config.updatePanel.secColor, "0.05 0.5", "0.95 0.95", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateImage(ref detailUI, "detail_icon_panel", config.updatePanel.SecondiconUrl, "0 0", "1 1", 0f);


            CUIClass.CreatePanel(ref detailUI, "detail_title_panel", "div_detail", config.updatePanel.secColor, "0.05 0.5", "0.953 0.6", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateText(ref detailUI, "detail_title_text", "detail_title_panel", "1 1 1 1", config.updatePanel.DetailedTitleText, 13, "0.02 0", "1 1", TextAnchor.MiddleCenter, "robotocondensed-bold.ttf", "0 0 0 1", "1 1");


            CUIClass.CreatePanel(ref detailUI, "detail_title_panel", "div_detail", config.updatePanel.secColor, "0.05 0.35", "0.953 0.45", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateText(ref detailUI, "detail_title_text", "detail_title_panel", "1 1 1 1", config.updatePanel.DetailedFirstString, 13, "0.02 0", "1 1", TextAnchor.MiddleLeft, "robotocondensed-bold.ttf", "0 0 0 1", "1 1");


            CUIClass.CreateButton(ref detailUI, "btn_close", "div_detail", "0.2 0.2 0.2 1", "Закрыть", 12, "0.8 0.02", "0.98 0.12", "wpupdate.closedetails", "", "1 1 1 1");



            CuiHelper.AddUi(player, detailUI);
        }

        #endregion

        #region CUI Helper Class
        public class CUIClass
        {
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
