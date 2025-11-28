using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;
using System.Collections.Generic;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("WPSocialLinks", "David", "1.2.21")]
    [Description("Social Media Links Addon for Welcome Panel")]

    public class WPSocialLinks : RustPlugin
    {   

        #region [Further Customization]

        private Dictionary<string, string> _p = new Dictionary<string, string>
        {
                // PANEL 1
                { "p1_Amin", "0.1 0.65" },    
                { "p1_Amax", "0.475 0.85" },   
                // PANEL 2
                { "p2_Amin", "0.525 0.65" },    
                { "p2_Amax", "0.900 0.85" }, 
                // PANEL 3
                { "p3_Amin", "0.1 0.43" },    
                { "p3_Amax", "0.475 0.63" }, 
                // PANEL 4 
                { "p4_Amin", "0.525 0.43" },    
                { "p4_Amax", "0.900 0.63" }, 
                // PANEL 5
                { "p5_Amin", "0.1 0.21" },    
                { "p5_Amax", "0.475 0.41" }, 
                // PANEL 6
                { "p6_Amin", "0.525 0.21" },    
                { "p6_Amax", "0.900 0.41" }, 
                // SUB PANELS
                    //LOGO PANEL
                    { "lp_Amin", "0.03 0.09" },
                    { "lp_Amax", "0.32 0.91" },
                    //TEXT PANEL
                    { "tp_Amin", "0.34 0.41" },
                    { "tp_Amax", "0.97 0.91" },
                    //BUTTON
                    { "btn_Amin", "0.34 0.09" },
                    { "btn_Amax", "0.97 0.35" },      
        };

        #endregion

        #region [Image Handling]

        [PluginReference] Plugin ImageLibrary;

        private void DownloadImages()
        {   
            if (ImageLibrary == null) return;

            string[] images = { $"{config.panel1.iconURL}", $"{config.panel2.iconURL}"};

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

        #region [Custom Note]

        [ConsoleCommand("create_note")]
        private void create_note(ConsoleSystem.Arg arg)
        { 
            var player = arg?.Player(); 
            var args = arg.Args;
            if (arg.Player() == null) return;

            if (args.Length < 1) return;

           
           
            if (args[0] == "1") { CreateNot(player); NoteCreateLink(player, config.panel1.noteSkinID, config.panel1.noteName, config.panel1.noteText); return; } 
            if (args[0] == "2") { CreateNot(player); NoteCreateLink(player, config.panel2.noteSkinID, config.panel2.noteName, config.panel2.noteText); return; } 
            
        }

        [ConsoleCommand("not_cmd")]
        private void not_cmd(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player(); 
            var args = arg.Args;
            if (arg.Player() == null) return;

            if (args.Length < 1) return;
            if (args[0] == "show") { CreateNot(player); return; }
            if (args[0] == "close") { CloseNot(player); return; }
        }
        

        private void NoteCreateLink(BasePlayer player, ulong _skinId, string _noteName, string _noteText )
        {   
            if (config.otherSet.chatTextEnabled && config.otherSet.chatTextEnabled != null)
            {   
                player.SendConsoleCommand($"chat.add 0 1 \"{_noteText}\"");
                return;
            }

            Item _customNote = ItemManager.CreateByPartialName("note", 1 , _skinId);
            _customNote.name = _noteName;
            _customNote.text = _noteText;
            _customNote.MarkDirty();
            player.inventory.GiveItem(_customNote);
        }

        #endregion

        #region [Cui API]

        private void ShowLinks_API(BasePlayer player)
        {
            var _wipeCycleUI = new CuiElementContainer();
            //TITLE
            CUIClass.CreateText(ref _wipeCycleUI, "text_title", "WelcomePanel_content", "1 1 1 1", config.otherSet.topTitleText, 15, "0 0.84", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-regular.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
            //PANEL 1 
            if (config.panel1.enabled)
            {
                CUIClass.CreatePanel(ref _wipeCycleUI, "div_panel1", "WelcomePanel_content", config.panel1.mainColor, _p["p1_Amin"], _p["p1_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _wipeCycleUI, "panel1_logo_panel", "div_panel1", config.panel1.secColor, _p["lp_Amin"], _p["lp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _wipeCycleUI, "panel1_text_panel", "div_panel1", config.panel1.secColor, _p["tp_Amin"], _p["tp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreateButton(ref _wipeCycleUI, "panel1_btn_panel", "div_panel1", config.panel1.btnColor, config.panel1.btnText, 13, _p["btn_Amin"], _p["btn_Amax"], $"create_note 1", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");
                        CUIClass.CreateText(ref _wipeCycleUI, "panel1_text", "panel1_text_panel", "1 1 1 1", config.panel1.textField, 8, "0.025 0", "1 1", TextAnchor.MiddleLeft, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateImage(ref _wipeCycleUI, "panel1_logo_panel", Img($"{config.panel1.iconURL}"), "0.05 0.05", "0.95 0.95");
            }
            //PANEL 2
            if (config.panel2.enabled)
            {
                CUIClass.CreatePanel(ref _wipeCycleUI, "div_panel2", "WelcomePanel_content", config.panel2.mainColor, _p["p2_Amin"], _p["p2_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _wipeCycleUI, "panel2_logo_panel", "div_panel2", config.panel2.secColor, _p["lp_Amin"], _p["lp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _wipeCycleUI, "panel2_text_panel", "div_panel2", config.panel2.secColor, _p["tp_Amin"], _p["tp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreateButton(ref _wipeCycleUI, "panel2_btn_panel", "div_panel2", config.panel2.btnColor, config.panel2.btnText, 13, _p["btn_Amin"], _p["btn_Amax"], $"create_note 2", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");
                        CUIClass.CreateText(ref _wipeCycleUI, "panel2_text", "panel2_text_panel", "1 1 1 1", config.panel2.textField, 8, "0.025 0", "1 1", TextAnchor.MiddleLeft, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateImage(ref _wipeCycleUI, "panel2_logo_panel", Img($"{config.panel2.iconURL}"), "0.05 0.05", "0.95 0.95");
            }
            //FOOTER
            CUIClass.CreateText(ref _wipeCycleUI, "text_footer", "WelcomePanel_content", "1 1 1 1", config.otherSet.footerText, 15, "0 0.0.5", "1 0.17", TextAnchor.MiddleCenter, $"robotocondensed-regular.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
            //end

            CloseLinks_API(player);
            CuiHelper.AddUi(player, _wipeCycleUI); 
        }    

        private void CloseLinks_API(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "main");
            for (int i = 1; i < 7; i++)
            {
                CuiHelper.DestroyUi(player, $"div_panel{i}");
            }
        }

        private void CreateNot(BasePlayer player)
        {   
            string notText = "Pop up window text is missing, please change it in config file.";
            if (config.otherSet.notePopMsg != null) notText = config.otherSet.notePopMsg;
            

            var _uiNotification = CUIClass.CreateOverlay("main_not", "0.19 0.19 0.19 0.90", "0 0", "1 1", true, 0.0f, $"assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreatePanel(ref _uiNotification, "blurr2", "main_not", "0 0 0 0.95", "0 0", "1 1", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            
            CUIClass.CreatePanel(ref _uiNotification, "main_panel", "main_not", "0.19 0.19 0.19 0.99", "0.4 0.4", "0.6 0.6", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateText(ref _uiNotification, "text_title", "main_panel", "1 1 1 1", $"{notText}", 15, "0.1 0", "0.9 0.8", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
            CUIClass.CreateImage(ref _uiNotification, "main_panel", "https://rustlabs.com/img/items180/note.png", "0.4 0.60", "0.6 0.90");
            CUIClass.CreateButton(ref _uiNotification, "close_btn", "main_panel", "0.56 0.20 0.15 1.0", "✘", 11, "0.85 0.8", "0.97 0.95", "not_cmd close", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");       
            
            CloseNot(player);
            CuiHelper.AddUi(player, _uiNotification); 

            if (config.otherSet.closePanel && config.otherSet.closePanel != null)
                player.SendConsoleCommand($"welcomepanel_close");
           
        }

        private void CloseNot(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "main_not");
        }



        #endregion 

        #region [Cui Class]

        public class CUIClass
        {
            public static CuiElementContainer CreateOverlay(string _name, string _color, string _anchorMin, string _anchorMax, bool _cursorOn = false, float _fade = 0f, string _mat ="")
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

            public static void CreatePanel(ref CuiElementContainer _container, string _name, string _parent, string _color, string _anchorMin, string _anchorMax, bool _cursorOn = false, float _fade = 0f, string _mat2 ="")
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

            public static void CreateImage(ref CuiElementContainer _container, string _parent, string _image, string _anchorMin, string _anchorMax, float _fade = 0f)
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
                            FadeIn = 0f,
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

            public static void CreateButton(ref CuiElementContainer _container, string _name, string _parent, string _color, string _text, int _size, string _anchorMin, string _anchorMax, string _command = "", string _close = "", string _textColor = "0.843 0.816 0.78 1", float _fade = 0f, TextAnchor _align = TextAnchor.MiddleCenter, string _font = "")
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

            [JsonProperty(PropertyName = "Panel #1")]
            public Panel1 panel1 { get; set; }

            public class Panel1
            {   
                [JsonProperty("Enabled")]
                public bool enabled { get; set; }

                [JsonProperty("Text Field")]
                public string textField { get; set; }

                [JsonProperty("Icon Image")]
                public string iconURL { get; set; }

                [JsonProperty("Button Text")]
                public string btnText { get; set; }

                [JsonProperty("Note Text")]
                public string noteText { get; set; }

                [JsonProperty("Note Name")]
                public string noteName { get; set; }

                [JsonProperty("Note Skin ID / Icon")]
                public ulong noteSkinID { get; set; }

                [JsonProperty("Main Panel Color")]
                public string mainColor { get; set; }

                [JsonProperty("Secondary Panel Color")]
                public string secColor { get; set; }

                [JsonProperty("Button Color")]
                public string btnColor { get; set; }
            } 

            [JsonProperty(PropertyName = "Panel #2")]
            public Panel2 panel2 { get; set; }

            public class Panel2
            {   
                [JsonProperty("Enabled")]
                public bool enabled { get; set; }

                [JsonProperty("Text Field")]
                public string textField { get; set; }

                [JsonProperty("Icon Image")]
                public string iconURL { get; set; }

                [JsonProperty("Button Text")]
                public string btnText { get; set; }

                [JsonProperty("Note Text")]
                public string noteText { get; set; }

                [JsonProperty("Note Name")]
                public string noteName { get; set; }

                [JsonProperty("Note Skin ID / Icon")]
                public ulong noteSkinID { get; set; }

                [JsonProperty("Main Panel Color")]
                public string mainColor { get; set; }

                [JsonProperty("Secondary Panel Color")]
                public string secColor { get; set; }

                [JsonProperty("Button Color")]
                public string btnColor { get; set; }
            }  

            [JsonProperty(PropertyName = "Other")]
            public OtherSet otherSet { get; set; }

            public class OtherSet
            {       
                [JsonProperty("Send Chat Message instead of Note")]
                public bool chatTextEnabled { get; set; }

                [JsonProperty("Close WelcomePanel after clicking GET LINK button")]
                public bool closePanel { get; set; }
                
                [JsonProperty("Pop up window message")]
                public string notePopMsg { get; set; }
            
                [JsonProperty("Title Text")]
                public string topTitleText { get; set; }

                [JsonProperty("Footer Text")]
                public string footerText { get; set; }

                [JsonProperty("Font Outline Color")]
                public string fontOutlineColor { get; set; }

                [JsonProperty("Font Outline Thickness")]
                public string fontOutlineThickness { get; set; }
            }

            

            public static Configuration CreateConfig()
            {
                return new Configuration
                {   

                    panel1 = new WPSocialLinks.Configuration.Panel1
                    {   
                        enabled = true,
                        textField = "<size=22>DISCORD</size> \n <size=9>Link your discord with game account!</size>",
                        iconURL = "https://i.ibb.co/pRYmCBx/discordicon.png",
                        btnText = "GET LINK",
                        noteText = "\n \n discord.gg/invitelink \n \n you can copy paste link from here",
                        noteName = "DISCORD",
                        noteSkinID = 2391705198,
                        mainColor = "0.25 0.25 0.25 0.65",
                        secColor = "0.19 0.19 0.19 0.85",
                        btnColor = "0.16 0.34 0.49 0.80",
                    },

                    panel2 = new WPSocialLinks.Configuration.Panel2
                    {   
                        enabled = true,
                        textField = "<size=22>STEAM GROUP</size> \n <size=9>Join our steam group and get rewarded!</size>",
                        iconURL = "https://i.ibb.co/8c4sVfQ/steamicon.png",
                        btnText = "GET LINK",
                        noteText = "\n \n steamgroup.link \n \n you can copy paste link from here",
                        noteName = "STEAM GROUP",
                        noteSkinID = 2543322586,
                        mainColor = "0.25 0.25 0.25 0.65",
                        secColor = "0.19 0.19 0.19 0.85",
                        btnColor = "0.16 0.34 0.49 0.80",
                    },

                    otherSet = new WPSocialLinks.Configuration.OtherSet
                    {   
                        chatTextEnabled = false,
                        closePanel = false,
                        notePopMsg = "You've got a note with url in your inventory.",
                        topTitleText = "You can find us here",
                        footerText = "Follow us on social media and claim your rewards!",
                        fontOutlineColor = "0 0 0 1",
                        fontOutlineThickness = "1",
                    },

                };
            }
        }

        #endregion

    }
}