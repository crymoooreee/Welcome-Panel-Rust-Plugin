using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;
using System.Collections.Generic;
using UnityEngine;
using System;
using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
    [Info("WPWipeCycle", "crymoooreee", "1.0.0")]
    [Description("Wipe Cycle Addon Page for Welcome Panel")]

    public class WPWipeCycle : RustPlugin
    {   
        [PluginReference]
        private Plugin WipeCountdown;

        #region [Further Customization]

        private Dictionary<string, string> _mw = new Dictionary<string, string>
        {
            // [FIRST MAIN PANEL]
                //"div_mapwipe"
                { "Amin", "0.1 0.55" },     //anchors
                { "Amax", "0.9 0.85" },     //anchors
                { "bgColor", "1 1 1 0.01" },  
                // [SUB PANELS]    
                    //"mapwipe_logo_panel"
                    { "lp_Amin", "0.020 0.08" },
                    { "lp_Amax", "0.235 0.92" },
                    //"mapwipe_title_panel"
                    { "tp_Amin", "0.255 0.481" },
                    { "tp_Amax", "0.980 0.912" }, 
                    //"mapwipe_date_panel"
                    { "dp_Amin", "0.255 0.08" },
                    { "dp_Amax", "0.63 0.430" },
                    //"mapwipe_time_panel"
                    { "tip_Amin", "0.645 0.08" },
                    { "tip_Amax", "0.980 0.430" },
                    // [CONTENT]
                        //"mw_title_text"
                        { "title_Amin", "0.02 0" },
                        { "title_Amax", "1 1" },
                        //"mw_date_text"
                        { "date_Amin", "0.2 0" },
                        { "date_Amax", "1 1" },
                        //"mw_time_text"
                        { "time_Amin", "0.2 0" },
                        { "time_Amax", "1 1" },
                        //"mapwipe_logo_panel"
                        { "logo_Amin", "0 0" },
                        { "logo_Amax", "1 1" },
                        //"mapwipe_logo_date_panel"
                        { "logod_Amin", "0.05 0.25" },
                        { "logod_Amax", "0.185 0.75" },
                        //"mapwipe_logo_time_panel"
                        { "logot_Amin", "0.05 0.25" },
                        { "logot_Amax", "0.185 0.75" },
        };

        private Dictionary<string, string> _bw = new Dictionary<string, string>
        {
            // [SECOND MAIN PANEL]
                //"div_bpWipe"
                { "Amin", "0.1 0.23" },     //anchors
                { "Amax", "0.9 0.53" },     //anchors
                { "bgColor", "1 1 1 0.01" },  
                // [SUB PANELS]    
                    //"bpWipe_logo_panel"
                    { "lp_Amin", "0.020 0.08" },
                    { "lp_Amax", "0.235 0.92" },
                    //"bpWipe_title_panel"
                    { "tp_Amin", "0.255 0.481" },
                    { "tp_Amax", "0.980 0.912" }, 
                    //"bpWipe_date_panel"
                    { "dp_Amin", "0.255 0.08" },
                    { "dp_Amax", "0.63 0.430" },
                    //"bpWipe_time_panel"
                    { "tip_Amin", "0.645 0.08" },
                    { "tip_Amax", "0.980 0.430" },
                    // [CONTENT]
                        //"mw_title_text"
                        { "title_Amin", "0.02 0" },
                        { "title_Amax", "1 1" },
                        //"mw_date_text"
                        { "date_Amin", "0.2 0" },
                        { "date_Amax", "1 1" },
                        //"mw_time_text"
                        { "time_Amin", "0.2 0" },
                        { "time_Amax", "1 1" },
                        //"bpWipe_logo_panel"
                        { "logo_Amin", "0 0" },
                        { "logo_Amax", "1 1" },
                        //"bpWipe_logo_date_panel"
                        { "logod_Amin", "0.05 0.25" },
                        { "logod_Amax", "0.185 0.75" },
                        //"bpWipe_logo_time_panel"
                        { "logot_Amin", "0.05 0.25" },
                        { "logot_Amax", "0.185 0.75" },
        };

        private Dictionary<string, string> _cd = new Dictionary<string, string>
        {   
            // COUNTDOWN PANEL
            { "cdAmin", "0.1 0.13" },
            { "cdAmax", "0.9 0.21" },
                // COUNTDOWN TEXT
                { "cd_text_Amin", "0.15 0" },
                { "cd_text_Amax", "0.85 1" },
        };

        #endregion

        #region [Image Handling]

        [PluginReference] Plugin ImageLibrary;

        private void DownloadImages()
        {   
            if (ImageLibrary == null) return;

            string[] images = { $"{config.mapWipe.imgUrl}", $"{config.mapWipe.imgUrl2}", $"{config.mapWipe.imgUrl3}", $"{config.bpWipe.imgUrl}", 
            $"{config.bpWipe.imgUrl2}", $"{config.bpWipe.imgUrl3}", };

            foreach (string img in images)
                ImageLibrary.Call("AddImage", img, img); 
        }

        private string Img(string url)
        {
            if (ImageLibrary != null) {
                
                if (!(bool) ImageLibrary.Call("HasImage", url))
                    return url;
                else
                    return (string) ImageLibrary?.Call("GetImage", url);
            }
            else return url;
        }

        #endregion

        #region [Hooks]

        private void OnServerInitialized()
        {       
            LoadConfig();   
            DownloadImages();        
        }

        #endregion

        #region [Cui API]

        private void ShowWipeCycle_API(BasePlayer player)
        {
            var _wipeCycleUI = CUIClass.CreateOverlay("main", "0 0 0 0", "0 0", "0 0", false, 0.0f, $"assets/icons/iconmaterial.mat");

            CUIClass.CreateText(ref _wipeCycleUI, "text_title", "WelcomePanel_content", "1 1 1 1", config.otherSet.topTitleText, 15, "0 0.84", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
            //MAP WIPE PANEL 
            CUIClass.CreatePanel(ref _wipeCycleUI, "div_mapwipe", "WelcomePanel_content", config.mapWipe.mainColor, _mw["Amin"], _mw["Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                CUIClass.CreatePanel(ref _wipeCycleUI, "mapwipe_logo_panel", "div_mapwipe", config.mapWipe.secColor, _mw["lp_Amin"], _mw["lp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                CUIClass.CreatePanel(ref _wipeCycleUI, "mapwipe_title_panel", "div_mapwipe", config.mapWipe.secColor, _mw["tp_Amin"], _mw["tp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                CUIClass.CreatePanel(ref _wipeCycleUI, "mapwipe_date_panel", "div_mapwipe", config.mapWipe.secColor,  _mw["dp_Amin"], _mw["dp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                CUIClass.CreatePanel(ref _wipeCycleUI, "mapwipe_time_panel", "div_mapwipe", config.mapWipe.secColor, _mw["tip_Amin"], _mw["tip_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreateText(ref _wipeCycleUI, "mw_title_text", "mapwipe_title_panel", "1 1 1 1", config.mapWipe.titleText, 13, _mw["title_Amin"], _mw["title_Amax"], TextAnchor.MiddleLeft, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                    CUIClass.CreateText(ref _wipeCycleUI, "mw_date_text", "mapwipe_date_panel", "1 1 1 1", config.mapWipe.dateText, 13, _mw["date_Amin"], _mw["date_Amax"], TextAnchor.MiddleLeft, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                    CUIClass.CreateText(ref _wipeCycleUI, "mw_time_text", "mapwipe_time_panel", "1 1 1 1", config.mapWipe.timeText, 13, _mw["time_Amin"], _mw["time_Amax"], TextAnchor.MiddleLeft, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                    CUIClass.CreateImage(ref _wipeCycleUI, "mapwipe_logo_panel",  Img(config.mapWipe.imgUrl), _mw["logo_Amin"], _mw["logo_Amax"]);
                    CUIClass.CreateImage(ref _wipeCycleUI, "mapwipe_time_panel",  Img(config.mapWipe.imgUrl3), _mw["logot_Amin"], _mw["logot_Amax"]);
                    CUIClass.CreateImage(ref _wipeCycleUI, "mapwipe_date_panel",  Img(config.mapWipe.imgUrl2), _mw["logod_Amin"], _mw["logod_Amax"]);
            
            //BP WIPE PANEL
            CUIClass.CreatePanel(ref _wipeCycleUI, "div_bpWipe", "WelcomePanel_content", config.bpWipe.mainColor, _bw["Amin"], _bw["Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                CUIClass.CreatePanel(ref _wipeCycleUI, "bpWipe_logo_panel", "div_bpWipe", config.bpWipe.secColor, _bw["lp_Amin"], _bw["lp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                CUIClass.CreatePanel(ref _wipeCycleUI, "bpWipe_title_panel", "div_bpWipe", config.bpWipe.secColor, _bw["tp_Amin"], _bw["tp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                CUIClass.CreatePanel(ref _wipeCycleUI, "bpWipe_date_panel", "div_bpWipe", config.bpWipe.secColor,  _bw["dp_Amin"], _bw["dp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                CUIClass.CreatePanel(ref _wipeCycleUI, "bpWipe_time_panel", "div_bpWipe", config.bpWipe.secColor, _bw["tip_Amin"], _bw["tip_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreateText(ref _wipeCycleUI, "mw_title_text", "bpWipe_title_panel", "1 1 1 1", config.bpWipe.titleText, 13, _bw["title_Amin"], _bw["title_Amax"], TextAnchor.MiddleLeft, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                    CUIClass.CreateText(ref _wipeCycleUI, "mw_date_text", "bpWipe_date_panel", "1 1 1 1", config.bpWipe.dateText, 13, _bw["date_Amin"], _bw["date_Amax"], TextAnchor.MiddleLeft, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                    CUIClass.CreateText(ref _wipeCycleUI, "mw_time_text", "bpWipe_time_panel", "1 1 1 1", config.bpWipe.timeText, 13, _bw["time_Amin"], _bw["time_Amax"], TextAnchor.MiddleLeft, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                    CUIClass.CreateImage(ref _wipeCycleUI, "bpWipe_logo_panel",  Img(config.bpWipe.imgUrl), _bw["logo_Amin"], _bw["logo_Amax"]);
                    CUIClass.CreateImage(ref _wipeCycleUI, "bpWipe_time_panel",  Img(config.bpWipe.imgUrl3), _bw["logot_Amin"], _bw["logot_Amax"]);
                    CUIClass.CreateImage(ref _wipeCycleUI, "bpWipe_date_panel",  Img(config.bpWipe.imgUrl2), _bw["logod_Amin"], _bw["logod_Amax"]);
            //COUNTDOWN PANEL
            string _countdownText;
            if (WipeCountdown != null)
            {   
                string _getcdAPI = Convert.ToString(WipeCountdown.CallHook("GetCountdownFormated_API"));
                string _replacedText = config.otherSet.countdownText.Replace("{countdown}", _getcdAPI);
                _countdownText = _replacedText;
            } else { _countdownText = config.otherSet.countdownText; }
            
            CUIClass.CreatePanel(ref _wipeCycleUI, "div_wipeCountdown", "WelcomePanel_content", config.bpWipe.mainColor, _cd["cdAmin"], _cd["cdAmax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                CUIClass.CreateText(ref _wipeCycleUI, "cd_time_text", "div_wipeCountdown", "1 1 1 1", _countdownText, 13, _cd["cd_text_Amin"], _cd["cd_text_Amax"], TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
            //end
            CloseWipeCycle_API(player);
            CuiHelper.AddUi(player, _wipeCycleUI); 
        }    

        private void CloseWipeCycle_API(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "main");
            CuiHelper.DestroyUi(player, "div_mapwipe");
            CuiHelper.DestroyUi(player, "div_bpWipe");
            CuiHelper.DestroyUi(player, "div_wipeCountdown");
        }

        #endregion 

        #region [Cui Class]

        public class CUIClass
        {
            public static CuiElementContainer CreateOverlay(string _name, string _color, string _anchorMin, string _anchorMax, bool _cursorOn = false, float _fade = 1f, string _mat ="")
            {   

            
                var _element = new CuiElementContainer()
                {
                    {
                        new CuiPanel
                        {
                            Image = { Color = _color, Material = _mat, FadeIn = _fade},
                            RectTransform = { AnchorMin = _anchorMin, AnchorMax = _anchorMax },
                            CursorEnabled = _cursorOn
                        },
                        new CuiElement().Parent = "Overlay",
                        _name
                    }
                };
                return _element;
            }

            public static void CreatePanel(ref CuiElementContainer _container, string _name, string _parent, string _color, string _anchorMin, string _anchorMax, bool _cursorOn = false, float _fade = 1f, string _mat2 ="")
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

            public static void CreateImage(ref CuiElementContainer _container, string _parent, string _image, string _anchorMin, string _anchorMax, float _fade = 1f)
            {
                if (_image.StartsWith("http") || _image.StartsWith("www"))
                {
                    _container.Add(new CuiElement
                    {
                        Parent = _parent,
                        Components =
                        {
                            new CuiRawImageComponent { Url = _image, Sprite = "assets/content/textures/generic/fulltransparent.tga", FadeIn = _fade},
                            new CuiRectTransformComponent { AnchorMin = _anchorMin, AnchorMax = _anchorMax }
                        }
                    });
                }
                else
                {
                    _container.Add(new CuiElement
                    {
                        Parent = _parent,
                        Components =
                        {
                            new CuiRawImageComponent { Png = _image, Sprite = "assets/content/textures/generic/fulltransparent.tga", FadeIn = _fade},
                            new CuiRectTransformComponent { AnchorMin = _anchorMin, AnchorMax = _anchorMax }
                        }
                    });
                }
            }

            public static void CreateInput(ref CuiElementContainer _container, string _name, string _parent, string _color, int _size, string _anchorMin, string _anchorMax, string _font = "permanentmarker.ttf", string _command = "command.processinput", TextAnchor _align = TextAnchor.MiddleCenter)
            {
                _container.Add(new CuiElement
                {
                    Parent = _parent,
                    Name = _name,

                    Components =
                    {
                        new CuiInputFieldComponent
                        {

                            Text = "0",
                            CharsLimit = 11,
                            Color = _color,
                            IsPassword = false,
                            Command = _command,
                            Font = _font,
                            FontSize = _size,
                            Align = _align
                        },

                        new CuiRectTransformComponent
                        {
                            AnchorMin = _anchorMin,
                            AnchorMax = _anchorMax

                        }

                    },
                });
            }

            public static void CreateText(ref CuiElementContainer _container, string _name, string _parent, string _color, string _text, int _size, string _anchorMin, string _anchorMax, TextAnchor _align = TextAnchor.MiddleCenter, string _font = "robotocondensed-bold.ttf", string _outlineColor = "", string _outlineScale ="")
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
                            FadeIn = 1f,
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
                    },
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

         #region [Config] 

        private Configuration config;
        protected override void LoadConfig()
        {
            base.LoadConfig();
            config = Config.ReadObject<Configuration>();
            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            config = Configuration.CreateConfig();
        }

        protected override void SaveConfig() => Config.WriteObject(config);

        

        class Configuration
        {   

            [JsonProperty(PropertyName = "Map Wipe")]
            public MapWipe mapWipe { get; set; }

            public class MapWipe
            {   
                [JsonProperty("Title Text")]
                public string titleText { get; set; }

                [JsonProperty("Date Text")]
                public string dateText { get; set; }

                [JsonProperty("Time Text")]
                public string timeText { get; set; }

                [JsonProperty("Main Panel Color")]
                public string mainColor { get; set; }

                [JsonProperty("Secondary Panels Color")]
                public string secColor { get; set; }

                [JsonProperty("Main Icon URL")]
                public string imgUrl { get; set; }

                [JsonProperty("Date Icon URL")]
                public string imgUrl2 { get; set; }

                [JsonProperty("Time Icon URL")]
                public string imgUrl3 { get; set; }
            } 

            [JsonProperty(PropertyName = "BP Wipe")]
            public BpWipe bpWipe { get; set; }

            public class BpWipe
            {   
                [JsonProperty("Title Text")]
                public string titleText { get; set; }

                [JsonProperty("Date Text")]
                public string dateText { get; set; }

                [JsonProperty("Time Text")]
                public string timeText { get; set; }

                [JsonProperty("Main Panel Color")]
                public string mainColor { get; set; }

                [JsonProperty("Secondary Panels Color")]
                public string secColor { get; set; }

                [JsonProperty("Main Icon URL")]
                public string imgUrl { get; set; }

                [JsonProperty("Date Icon URL")]
                public string imgUrl2 { get; set; }

                [JsonProperty("Time Icon URL")]
                public string imgUrl3 { get; set; }
            }

            [JsonProperty(PropertyName = "Other")]
            public OtherSet otherSet { get; set; }

            public class OtherSet
            {   
                [JsonProperty("Top Title Text")]
                public string topTitleText { get; set; }

                [JsonProperty("Countdown Text")]
                public string countdownText { get; set; }

                [JsonProperty("Font Outline Color")]
                public string fontOutlineColor { get; set; }

                [JsonProperty("Font Outline Thickness")]
                public string fontOutlineThickness { get; set; }
            }

            public static Configuration CreateConfig()
            {
                return new Configuration
                {   

                    mapWipe = new WPWipeCycle.Configuration.MapWipe
                    {   
                        titleText = "<size=22>MAP WIPE</size> \n <size=11>Only map change, all progress will stay, currencies, blueprints, ladderboards...</size>",
                        dateText = "<size=14>  Weekly / every friday</size>",
                        timeText = "<size=14>  5:00 PM (CET)</size>",
                        mainColor = "0.25 0.25 0.25 0.65",
                        secColor = "0.19 0.19 0.19 0.85",
                        imgUrl = "https://i.imgur.com/yxg2gyM.png",
                        imgUrl2 = "https://i.ibb.co/N97bTzY/dateicon.png",
                        imgUrl3 = "https://i.ibb.co/n3FXkfn/timeicon.png",
                        
                    },

                    bpWipe = new WPWipeCycle.Configuration.BpWipe
                    {   
                        titleText = "<size=22>BLUEPRINT WIPE</size> \n <size=11>Blueprint wipe, all progress will be wiped, currencies, blueprints, ladderboards.</size>",
                        dateText = "<size=14>  Monthly / first friday</size>",
                        timeText = "<size=14>  5:00 PM (CET)</size>",
                        mainColor = "0.25 0.25 0.25 0.65",
                        secColor = "0.19 0.19 0.19 0.85",
                        imgUrl = "https://i.ibb.co/m6jw3W3/bpwipeicon.png",
                        imgUrl2 = "https://i.ibb.co/N97bTzY/dateicon.png",
                        imgUrl3 = "https://i.ibb.co/n3FXkfn/timeicon.png",
                        
                    },

                    otherSet = new WPWipeCycle.Configuration.OtherSet
                    { 
                        topTitleText = "Our Wipe Schedule",
                        countdownText = "Next wipe in (WipeCountdown plugin required)",
                        fontOutlineColor = "0 0 0 1",
                        fontOutlineThickness = "1"
                    },
                };
            }
        }

        #endregion

    }
}