using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;
using System.Collections.Generic;
using Oxide.Core.Plugins;
using UnityEngine;
using System;


namespace Oxide.Plugins
{
    [Info("WPVipRanks", "crymoooreee", "1.0.0")]
    [Description("Dedicated Vip Ranks Addon for Welcome Panel")]

    public class WPVipRanks : RustPlugin
    { 
        #region [Further Customization]

        private Dictionary<string, string> _p = new Dictionary<string, string>
        {
                // PANEL 1
                { "p1_Amin", "0.07 0.10" },    
                { "p1_Amax", "0.330 0.9" },   
                // PANEL 2
                { "p2_Amin", "0.370 0.1" },    
                { "p2_Amax", "0.630 0.9" }, 
                // PANEL 3
                { "p3_Amin", "0.670 0.1" },    
                { "p3_Amax", "0.93 0.9" }, 
                // PANEL 4 
                { "p4_Amin", "0.07 0.10" },    
                { "p4_Amax", "0.330 0.9" },  
                // PANEL 5
                { "p5_Amin", "0.370 0.1" },    
                { "p5_Amax", "0.630 0.9" }, 
                // PANEL 6
                { "p6_Amin", "0.670 0.1" },    
                { "p6_Amax", "0.93 0.9" }, 
                // "NEXT" BUTTON
                { "bn_Amin", "0.94 0.4" },    
                { "bn_Amax", "0.98 0.6" }, 
                // "PREVIOUS" BUTTON
                { "bp_Amin", "0.02 0.4" },    
                { "bp_Amax", "0.06 0.6" }, 
                // SUB PANELS
                    //LOGO PANEL
                    { "lp_Amin", "0.05 0.70" },
                    { "lp_Amax", "0.95 0.975" },
                    //TITLE PANEL
                    { "ttp_Amin", "0.05 0.70" },
                    { "ttp_Amax", "0.95 0.975" },
                    //TEXT PANEL
                    { "tp_Amin", "0.05 0.025" },
                    { "tp_Amax", "0.95 0.675" },
                    //BUY BUTTON
                    { "btn_Amin", "0.1 0.05" },
                    { "btn_Amax", "0.9 0.15" },      
                //------------------------------//
                //WHEN ONLY 2 ENABLED
                    // PANEL 1
                    { "pp1_Amin", "0.170 0.10" },    
                    { "pp1_Amax", "0.450 0.9" },   
                    // PANEL 2
                    { "pp2_Amin", "0.550 0.1" },    
                    { "pp2_Amax", "0.830 0.9" }, 
                //WHEN ONLY 1 ENABLED
                    // PANEL 1
                    { "ps1_Amin", "0.360 0.10" },    
                    { "ps1_Amax", "0.640 0.9" },         
        };

        #endregion

        #region [Dependencies]

        [PluginReference]
        private Plugin WelcomePanel;

        private void Loaded()
        {
            if (WelcomePanel == null)
            {
               Puts(" Core plugin WelcomePanel is not loaded.");
            }
        }

        #endregion

        #region [Image Handling]

        [PluginReference] Plugin ImageLibrary;

        private void DownloadImages()
        {   
            if (ImageLibrary == null) return;

            string[] images = { $"{config.panel1.iconURL}", $"{config.panel2.iconURL}", $"{config.panel3.iconURL}", $"{config.panel4.iconURL}", $"{config.panel5.iconURL}",
             $"{config.panel6.iconURL}" };

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

        #region [Commands/CustomNote]

        [ConsoleCommand("create1_note")]
        private void create1_note(ConsoleSystem.Arg arg)
        { 
            var player = arg?.Player(); 
            var args = arg.Args;
            if (arg.Player() == null) return;
            if (args.Length < 1) return;
            if (args[0] == "1") { CreateNot(player); NoteCreateLink(player, config.otherSet.noteSkinID, config.otherSet.noteName, config.otherSet.noteText); return; }
        }

        [ConsoleCommand("vip_page_open")]
        private void vip_page_open(ConsoleSystem.Arg arg)
        {   
            var player = arg?.Player(); 
            var args = arg.Args;
            if (arg.Player() == null) return;
            if (args.Length < 1) return;
            if (args[0] == "1") { CloseVip_API(player); ShowVipRanks_3_API(player); return; } 
            if (args[0] == "2") { CloseVip_API(player); ShowVipRanks_6_API(player); return; }  
        }

        [ConsoleCommand("not1_cmd")]
        private void not1_cmd(ConsoleSystem.Arg arg)
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
            Item _customNote = ItemManager.CreateByPartialName("note", 1 , _skinId);
            _customNote.name = _noteName;
            _customNote.text = _noteText;
            _customNote.MarkDirty();
            player.inventory.GiveItem(_customNote);
        }

        #endregion

        #region [Cui API]

        private void ShowVipRanks_LessThan3_API(BasePlayer player)
        {
            var _vipRanksUI = new CuiElementContainer();
            //TITLE
            CUIClass.CreateText(ref _vipRanksUI, "text_title", "WelcomePanel_content", "1 1 1 1", config.otherSet.topTitleText, 15, "0 0.84", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
            //SHOW TWO
            if (config.panel2.enabled)
            {   
                //PANEL1
                CUIClass.CreatePanel(ref _vipRanksUI, "div_panel1", "WelcomePanel_content", config.panel1.mainColor, _p["pp1_Amin"], _p["pp1_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel1_logo_panel", "div_panel1", config.panel1.secColor, _p["lp_Amin"], _p["lp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel1_text_panel", "div_panel1", config.panel1.secColor, _p["tp_Amin"], _p["tp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                        CUIClass.CreateImage(ref _vipRanksUI, "panel1_logo_panel", Img(config.panel1.iconURL), "0 0", "1 1");
                        CUIClass.CreateText(ref _vipRanksUI, "panel1_title", "panel1_logo_panel", "1 1 1 1", config.panel1.titleText, 10, "0.025 0.05", "1 1", TextAnchor.LowerCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateText(ref _vipRanksUI, "panel1_text", "panel1_text_panel", "1 1 1 1", config.panel1.textField, 13, "0.0 0.15", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateButton(ref _vipRanksUI, "panel1_btn_panel", "div_panel1", config.panel1.btnColor, config.panel1.btnText, 13, _p["btn_Amin"], _p["btn_Amax"], $"create1_note 1", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");
                //PANEL2
                CUIClass.CreatePanel(ref _vipRanksUI, "div_panel2", "WelcomePanel_content", config.panel2.mainColor, _p["pp2_Amin"], _p["pp2_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel2_logo_panel", "div_panel2", config.panel2.secColor, _p["lp_Amin"], _p["lp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel2_text_panel", "div_panel2", config.panel2.secColor, _p["tp_Amin"], _p["tp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                        CUIClass.CreateImage(ref _vipRanksUI, "panel2_logo_panel", Img(config.panel2.iconURL), "0 0", "1 1");
                        CUIClass.CreateText(ref _vipRanksUI, "panel2_title", "panel2_logo_panel", "1 1 1 1", config.panel2.titleText, 10, "0.025 0.05", "1 1", TextAnchor.LowerCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateText(ref _vipRanksUI, "panel2_text", "panel2_text_panel", "1 1 1 1", config.panel2.textField, 13, "0.0 0.15", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateButton(ref _vipRanksUI, "panel2_btn_panel", "div_panel2", config.panel2.btnColor, config.panel2.btnText, 13, _p["btn_Amin"], _p["btn_Amax"], $"create1_note 1", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");
            }
            //SHOW ONLY ONE
            if (!config.panel2.enabled)
            {
                CUIClass.CreatePanel(ref _vipRanksUI, "div_panel1", "WelcomePanel_content", config.panel1.mainColor, _p["ps1_Amin"], _p["ps1_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel1_logo_panel", "div_panel1", config.panel1.secColor, _p["lp_Amin"], _p["lp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel1_text_panel", "div_panel1", config.panel1.secColor, _p["tp_Amin"], _p["tp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                        CUIClass.CreateImage(ref _vipRanksUI, "panel1_logo_panel", Img(config.panel1.iconURL), "0 0", "1 1");
                        CUIClass.CreateText(ref _vipRanksUI, "panel1_title", "panel1_logo_panel", "1 1 1 1", config.panel1.titleText, 10, "0.025 0.05", "1 1", TextAnchor.LowerCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateText(ref _vipRanksUI, "panel1_text", "panel1_text_panel", "1 1 1 1", config.panel1.textField, 13, "0.0 0.15", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateButton(ref _vipRanksUI, "panel1_btn_panel", "div_panel1", config.panel1.btnColor, config.panel1.btnText, 13, _p["btn_Amin"], _p["btn_Amax"], $"create1_note 1", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");     
            }
            //FOOTER
            CUIClass.CreateText(ref _vipRanksUI, "text_footer", "WelcomePanel_content", "1 1 1 1", config.otherSet.footerText, 15, "0 0.0.5", "1 0.10", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
            //end
            CloseVip_API(player);
            CuiHelper.AddUi(player, _vipRanksUI); 
        }  

        private void ShowVipRanks_3_API(BasePlayer player)
        {
            var _vipRanksUI = new CuiElementContainer();
            //TITLE
            CUIClass.CreateText(ref _vipRanksUI, "text_title", "WelcomePanel_content", "1 1 1 1", config.otherSet.topTitleText, 15, "0 0.84", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
            //PANEL 1 
            if (config.panel1.enabled)
            {
                CUIClass.CreatePanel(ref _vipRanksUI, "div_panel1", "WelcomePanel_content", config.panel1.mainColor, _p["p1_Amin"], _p["p1_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel1_logo_panel", "div_panel1", config.panel1.secColor, _p["lp_Amin"], _p["lp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel1_text_panel", "div_panel1", config.panel1.secColor, _p["tp_Amin"], _p["tp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                        CUIClass.CreateImage(ref _vipRanksUI, "panel1_logo_panel", Img(config.panel1.iconURL), "0 0", "1 1");
                        CUIClass.CreateText(ref _vipRanksUI, "panel1_title", "panel1_logo_panel", "1 1 1 1", config.panel1.titleText, 10, "0.025 0.05", "1 1", TextAnchor.LowerCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateText(ref _vipRanksUI, "panel1_text", "panel1_text_panel", "1 1 1 1", config.panel1.textField, 13, "0.0 0.15", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateButton(ref _vipRanksUI, "panel1_btn_panel", "div_panel1", config.panel1.btnColor, config.panel1.btnText, 13, _p["btn_Amin"], _p["btn_Amax"], $"create1_note 1", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");
                   
            }
            //PANEL 2
            if (config.panel2.enabled)
            {
                CUIClass.CreatePanel(ref _vipRanksUI, "div_panel2", "WelcomePanel_content", config.panel2.mainColor, _p["p2_Amin"], _p["p2_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel2_logo_panel", "div_panel2", config.panel2.secColor, _p["lp_Amin"], _p["lp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel2_text_panel", "div_panel2", config.panel2.secColor, _p["tp_Amin"], _p["tp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                        CUIClass.CreateImage(ref _vipRanksUI, "panel2_logo_panel", Img(config.panel2.iconURL), "0 0", "1 1");
                        CUIClass.CreateText(ref _vipRanksUI, "panel2_title", "panel2_logo_panel", "1 1 1 1", config.panel2.titleText, 10, "0.025 0.05", "1 1", TextAnchor.LowerCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateText(ref _vipRanksUI, "panel2_text", "panel2_text_panel", "1 1 1 1", config.panel2.textField, 13, "0.0 0.15", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateButton(ref _vipRanksUI, "panel2_btn_panel", "div_panel2", config.panel2.btnColor, config.panel2.btnText, 13, _p["btn_Amin"], _p["btn_Amax"], $"create1_note 1", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");
            }
            //PANEL 3
            if (config.panel3.enabled)
            {
                 CUIClass.CreatePanel(ref _vipRanksUI, "div_panel3", "WelcomePanel_content", config.panel3.mainColor, _p["p3_Amin"], _p["p3_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel3_logo_panel", "div_panel3", config.panel3.secColor, _p["lp_Amin"], _p["lp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel3_text_panel", "div_panel3", config.panel3.secColor, _p["tp_Amin"], _p["tp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                        CUIClass.CreateImage(ref _vipRanksUI, "panel3_logo_panel", Img(config.panel3.iconURL), "0 0", "1 1");
                        CUIClass.CreateText(ref _vipRanksUI, "panel3_title", "panel3_logo_panel", "1 1 1 1", config.panel3.titleText, 10, "0.025 0.05", "1 1", TextAnchor.LowerCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateText(ref _vipRanksUI, "panel3_text", "panel3_text_panel", "1 1 1 1", config.panel3.textField, 13, "0.0 0.15", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateButton(ref _vipRanksUI, "panel3_btn_panel", "div_panel3", config.panel3.btnColor, config.panel3.btnText, 13, _p["btn_Amin"], _p["btn_Amax"], $"create1_note 1", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");
            }   
            //FOOTER
            CUIClass.CreateText(ref _vipRanksUI, "text_footer", "WelcomePanel_content", "1 1 1 1", config.otherSet.footerText, 15, "0 0.0.5", "1 0.10", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
            //NEXT BUTTON
            if (config.panel4.enabled || config.panel5.enabled || config.panel6.enabled)
            {
                CUIClass.CreateButton(ref _vipRanksUI, "btn_next1", "WelcomePanel_content", config.btnSet.btnColor, ">", 12, _p["bn_Amin"], _p["bn_Amax"], $"vip_page_open 2", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");
            }
            //end
            CloseVip_API(player);
            CuiHelper.AddUi(player, _vipRanksUI); 
        }    

        private void ShowVipRanks_6_API(BasePlayer player)
        {
            var _vipRanksUI = new CuiElementContainer();
            //TITLE
            CUIClass.CreateText(ref _vipRanksUI, "text_title", "WelcomePanel_content", "1 1 1 1", config.otherSet.topTitleText, 15, "0 0.84", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
            //PANEL 4
            if (config.panel4.enabled)
            {
                CUIClass.CreatePanel(ref _vipRanksUI, "div_panel4", "WelcomePanel_content", config.panel4.mainColor, _p["p4_Amin"], _p["p4_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel4_logo_panel", "div_panel4", config.panel4.secColor, _p["lp_Amin"], _p["lp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel4_text_panel", "div_panel4", config.panel4.secColor, _p["tp_Amin"], _p["tp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                        CUIClass.CreateImage(ref _vipRanksUI, "panel4_logo_panel", Img(config.panel4.iconURL), "0 0", "1 1");
                        CUIClass.CreateText(ref _vipRanksUI, "panel4_title", "panel4_logo_panel", "1 1 1 1", config.panel4.titleText, 10, "0.025 0.05", "1 1", TextAnchor.LowerCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateText(ref _vipRanksUI, "panel4_text", "panel4_text_panel", "1 1 1 1", config.panel4.textField, 13, "0.0 0.15", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateButton(ref _vipRanksUI, "panel4_btn_panel", "div_panel4", config.panel4.btnColor, config.panel4.btnText, 13, _p["btn_Amin"], _p["btn_Amax"], $"create1_note 1", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");
            }   
            //PANEL 5
            if (config.panel5.enabled)
            {
                CUIClass.CreatePanel(ref _vipRanksUI, "div_panel5", "WelcomePanel_content", config.panel5.mainColor, _p["p5_Amin"], _p["p5_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel5_logo_panel", "div_panel5", config.panel5.secColor, _p["lp_Amin"], _p["lp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel5_text_panel", "div_panel5", config.panel5.secColor, _p["tp_Amin"], _p["tp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                        CUIClass.CreateImage(ref _vipRanksUI, "panel5_logo_panel", Img(config.panel5.iconURL), "0 0", "1 1");
                        CUIClass.CreateText(ref _vipRanksUI, "panel5_title", "panel5_logo_panel", "1 1 1 1", config.panel5.titleText, 10, "0.025 0.05", "1 1", TextAnchor.LowerCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateText(ref _vipRanksUI, "panel5_text", "panel5_text_panel", "1 1 1 1", config.panel5.textField, 13, "0.0 0.15", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateButton(ref _vipRanksUI, "panel5_btn_panel", "div_panel5", config.panel5.btnColor, config.panel5.btnText, 13, _p["btn_Amin"], _p["btn_Amax"], $"create1_note 1", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");
            }
            //PANEL 6
            if (config.panel6.enabled)
            {
                CUIClass.CreatePanel(ref _vipRanksUI, "div_panel6", "WelcomePanel_content", config.panel6.mainColor, _p["p6_Amin"], _p["p6_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel6_logo_panel", "div_panel6", config.panel6.secColor, _p["lp_Amin"], _p["lp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                    CUIClass.CreatePanel(ref _vipRanksUI, "panel6_text_panel", "div_panel6", config.panel6.secColor, _p["tp_Amin"], _p["tp_Amax"], false, 0f, "assets/content/ui/uibackgroundblur.mat");
                        CUIClass.CreateImage(ref _vipRanksUI, "panel6_logo_panel", Img(config.panel6.iconURL), "0 0", "1 1");
                        CUIClass.CreateText(ref _vipRanksUI, "panel6_title", "panel6_logo_panel", "1 1 1 1", config.panel6.titleText, 10, "0.025 0.05", "1 1", TextAnchor.LowerCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateText(ref _vipRanksUI, "panel6_text", "panel6_text_panel", "1 1 1 1", config.panel6.textField, 13, "0.0 0.15", "1 1", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
                        CUIClass.CreateButton(ref _vipRanksUI, "panel6_btn_panel", "div_panel6", config.panel6.btnColor, config.panel6.btnText, 13, _p["btn_Amin"], _p["btn_Amax"], $"create1_note 1", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");
            }
            //FOOTER
            CUIClass.CreateText(ref _vipRanksUI, "text_footer", "WelcomePanel_content", "1 1 1 1", config.otherSet.footerText, 15, "0 0.0.5", "1 0.10", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
            //PREVIOUS BUTTON
            CUIClass.CreateButton(ref _vipRanksUI, "btn_next", "WelcomePanel_content", config.btnSet.btnColor, "<", 12, _p["bp_Amin"], _p["bp_Amax"], $"vip_page_open 1", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");
            //end
            CloseVip_API(player);
            CuiHelper.AddUi(player, _vipRanksUI); 
        } 

        private void ShowVipRanks_API(BasePlayer player)
        {
            if (config.panel3.enabled)
            {
                ShowVipRanks_3_API(player);
            }
            else if(config.panel2.enabled || config.panel1.enabled)
            {
                ShowVipRanks_LessThan3_API(player);
            }
        }

        private void CloseVip_API(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "main");
            CuiHelper.DestroyUi(player, "btn_next");
            CuiHelper.DestroyUi(player, "btn_next1");
            for (int i = 1; i < 7; i++)
            {
                CuiHelper.DestroyUi(player, $"div_panel{i}");
            }

        }

        private void CreateNot(BasePlayer player)
        {
            var _uiNotification = CUIClass.CreateOverlay("main_not", "0.19 0.19 0.19 0.90", "0 0", "1 1", true, 0.0f, $"assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreatePanel(ref _uiNotification, "blurr2", "main_not", "0 0 0 0.95", "0 0", "1 1", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            
            CUIClass.CreatePanel(ref _uiNotification, "main_panel", "main_not", "0.19 0.19 0.19 0.99", "0.4 0.4", "0.6 0.6", false, 0f, "assets/content/ui/uibackgroundblur.mat");
            CUIClass.CreateText(ref _uiNotification, "text_title", "main_panel", "1 1 1 1", config.otherSet.notePuMsg, 15, "0.1 0", "0.9 0.8", TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf", config.otherSet.fontOutlineColor, $"{config.otherSet.fontOutlineThickness} {config.otherSet.fontOutlineThickness}");
            CUIClass.CreateImage(ref _uiNotification, "main_panel", "https://rustlabs.com/img/items180/note.png", "0.4 0.60", "0.6 0.90");
            CUIClass.CreateButton(ref _uiNotification, "close_btn", "main_panel", "0.56 0.20 0.15 1.0", "✘", 11, "0.85 0.8", "0.97 0.95", "not1_cmd close", "", "1 1 1 1", 0f, TextAnchor.MiddleCenter, $"robotocondensed-bold.ttf");       
            
            CloseNot(player);
            CuiHelper.AddUi(player, _uiNotification); 
        }

        private void CloseNot(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "main_not");
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

            [JsonProperty(PropertyName = "Panel #1")]
            public Panel1 panel1 { get; set; }

            public class Panel1
            {   
                [JsonProperty("Enabled")]
                public bool enabled { get; set; }

                [JsonProperty("Title Text")]
                public string titleText { get; set; }

                [JsonProperty("Text Field")]
                public string textField { get; set; }

                [JsonProperty("Icon Image")]
                public string iconURL { get; set; }

                [JsonProperty("Button Text")]
                public string btnText { get; set; }

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

                [JsonProperty("Title Text")]
                public string titleText { get; set; }

                [JsonProperty("Text Field")]
                public string textField { get; set; }

                [JsonProperty("Icon Image")]
                public string iconURL { get; set; }

                [JsonProperty("Button Text")]
                public string btnText { get; set; }

                [JsonProperty("Main Panel Color")]
                public string mainColor { get; set; }

                [JsonProperty("Secondary Panel Color")]
                public string secColor { get; set; }

                [JsonProperty("Button Color")]
                public string btnColor { get; set; }
            }  

            [JsonProperty(PropertyName = "Panel #3")]
            public Panel3 panel3 { get; set; }

            public class Panel3
            {   
                [JsonProperty("Enabled")]
                public bool enabled { get; set; }

                [JsonProperty("Title Text")]
                public string titleText { get; set; }

                [JsonProperty("Text Field")]
                public string textField { get; set; }

                [JsonProperty("Icon Image")]
                public string iconURL { get; set; }

                [JsonProperty("Button Text")]
                public string btnText { get; set; }

                [JsonProperty("Main Panel Color")]
                public string mainColor { get; set; }

                [JsonProperty("Secondary Panel Color")]
                public string secColor { get; set; }

                [JsonProperty("Button Color")]
                public string btnColor { get; set; }
            } 

            [JsonProperty(PropertyName = "Panel #4")]
            public Panel4 panel4 { get; set; }

            public class Panel4
            {   
                [JsonProperty("Enabled")]
                public bool enabled { get; set; }

                [JsonProperty("Title Text")]
                public string titleText { get; set; }

                [JsonProperty("Text Field")]
                public string textField { get; set; }

                [JsonProperty("Icon Image")]
                public string iconURL { get; set; }

                [JsonProperty("Button Text")]
                public string btnText { get; set; }

                [JsonProperty("Main Panel Color")]
                public string mainColor { get; set; }

                [JsonProperty("Secondary Panel Color")]
                public string secColor { get; set; }

                [JsonProperty("Button Color")]
                public string btnColor { get; set; }
            }  

            [JsonProperty(PropertyName = "Panel #5")]
            public Panel5 panel5 { get; set; }

            public class Panel5
            {   
                [JsonProperty("Enabled")]
                public bool enabled { get; set; }

                [JsonProperty("Title Text")]
                public string titleText { get; set; }

                [JsonProperty("Text Field")]
                public string textField { get; set; }

                [JsonProperty("Icon Image")]
                public string iconURL { get; set; }

                [JsonProperty("Button Text")]
                public string btnText { get; set; }

                [JsonProperty("Main Panel Color")]
                public string mainColor { get; set; }

                [JsonProperty("Secondary Panel Color")]
                public string secColor { get; set; }

                [JsonProperty("Button Color")]
                public string btnColor { get; set; }
            }  

            [JsonProperty(PropertyName = "Panel #6")]
            public Panel6 panel6 { get; set; }

            public class Panel6
            {   
                [JsonProperty("Enabled")]
                public bool enabled { get; set; }

                [JsonProperty("Title Text")]
                public string titleText { get; set; }

                [JsonProperty("Text Field")]
                public string textField { get; set; }

                [JsonProperty("Icon Image")]
                public string iconURL { get; set; }

                [JsonProperty("Button Text")]
                public string btnText { get; set; }

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
                [JsonProperty("Title Text")]
                public string topTitleText { get; set; }

                [JsonProperty("Footer Text")]
                public string footerText { get; set; }

                [JsonProperty("Font Outline Color")]
                public string fontOutlineColor { get; set; }

                [JsonProperty("Font Outline Thickness")]
                public string fontOutlineThickness { get; set; }

                [JsonProperty("Note Text")]
                public string noteText { get; set; }

                [JsonProperty("Note Name")]
                public string noteName { get; set; }

                [JsonProperty("Note Skin ID / Icon")]
                public ulong noteSkinID { get; set; }

                [JsonProperty("Note Pop-up window message")]
                public string notePuMsg { get; set; }
            }

            [JsonProperty(PropertyName = "Next/Previous Button")]
            public BtnSet btnSet { get; set; }

            public class BtnSet
            {   
                [JsonProperty("Color")]
                public string btnColor { get; set; }
            }
            

            public static Configuration CreateConfig()
            {
                return new Configuration
                {   

                    panel1 = new WPVipRanks.Configuration.Panel1
                    {   
                        enabled = true,
                        titleText = "<color=#3498DB><size=15>VIP</size></color>",
                        textField = "\n✔ Que Skip\n✔ Discord Role\n✔ Sign Artist\n <color=#696969>✘ SkinBox\n✘ Better Metabolism\n✘ Daily Special Kit\n✘ My Mini\n✘ Access to Bgrade\n</color>",
                        iconURL = "https://i.ibb.co/PwFtbs9/vip.png",
                        btnText = "<size=9>BUY FOR</size> <size=15>5$</size>  ",
                        mainColor = "0.25 0.25 0.25 0.65",
                        secColor = "0.19 0.19 0.19 0.85",
                        btnColor = "0.16 0.34 0.49 0.80",
                    },

                    panel2 = new WPVipRanks.Configuration.Panel2
                    {   
                        enabled = true,
                        titleText = "<color=#E16B00FF><size=15>ELITE</size></color>",
                        textField = "\n✔ Que Skip\n✔ Discord Role\n✔ Sign Artist\n✔ SkinBox\n✔ Better Metabolism\n✔ Daily Special Kit\n<color=#696969>✘ My Mini\n✘ Access to Bgrade\n</color>",
                        iconURL = "https://i.ibb.co/r22Nvg9/elite.png",
                        btnText = "<size=9>BUY FOR</size> <size=15>10$</size>  ",
                        mainColor = "0.25 0.25 0.25 0.65",
                        secColor = "0.19 0.19 0.19 0.85",
                        btnColor = "0.88 0.42 0.00 0.80",
                    },

                    panel3 = new WPVipRanks.Configuration.Panel3
                    {   
                        enabled = true,
                        titleText = "<color=#C70039FF><size=15>LEGEND</size></color>",
                        textField = "\n✔ Que Skip\n✔ Discord Role\n✔ Sign Artist\n✔ SkinBox\n✔ Better Metabolism\n✔ Daily Special Kit\n✔ My Mini\n✔ Access to Bgrade\n",
                        iconURL = "https://i.ibb.co/QC8h4jY/legend.png",
                        btnText = "<size=9>BUY FOR</size> <size=15>17$</size>  ",
                        mainColor = "0.25 0.25 0.25 0.65",
                        secColor = "0.19 0.19 0.19 0.85",
                        btnColor = "0.78 0.00 0.22 0.80",
                    },

                    panel4 = new WPVipRanks.Configuration.Panel4
                    {   
                        enabled = true,
                        titleText = "",
                        textField = "✔ Que Skip\n✔ Discord Role\n✔ Sign Artist\n✔ SkinBox\n✔ Better Metabolism\n✔ Daily Special Kit\n✔ My Mini\n✔ Access to Bgrade",
                        iconURL = "https://i.ibb.co/6t92Nyt/vipletters.png",
                        btnText = "<size=9>BUY FOR</size> <size=15>17$</size>  ",
                        mainColor = "0.19 0.19 0.19 0.85",
                        secColor = "0.16 0.34 0.49 0.65",
                        btnColor = "0.25 0.25 0.25 0.85",
                    },

                    panel5 = new WPVipRanks.Configuration.Panel5
                    {   
                        enabled = true,
                        titleText = "",
                        textField = "✔ Que Skip\n✔ Discord Role\n✔ Sign Artist\n✔ SkinBox\n✔ Better Metabolism\n✔ Daily Special Kit\n✔ My Mini\n✔ Access to Bgrade",
                        iconURL = "https://i.ibb.co/vZ9y1K1/eliteletters.png",
                        btnText = "<size=9>BUY FOR</size> <size=15>17$</size>  ",
                        mainColor = "0.19 0.19 0.19 0.85",
                        secColor = "0.88 0.42 0.00 0.65",
                        btnColor = "0.25 0.25 0.25 0.85",
                    },

                    panel6 = new WPVipRanks.Configuration.Panel6
                    {   
                        enabled = true,
                        titleText = "",
                        textField = "✔ Que Skip\n✔ Discord Role\n✔ Sign Artist\n✔ SkinBox\n✔ Better Metabolism\n✔ Daily Special Kit\n✔ My Mini\n✔ Access to Bgrade",
                        iconURL = "https://i.ibb.co/XJ6cz6z/legendletters.png",
                        btnText = "<size=9>BUY FOR</size> <size=15>17$</size>  ",
                        mainColor = "0.19 0.19 0.19 0.85",
                        secColor = "0.78 0.00 0.22 0.65",
                        btnColor = "0.25 0.25 0.25 0.85",
                    },

                    otherSet = new WPVipRanks.Configuration.OtherSet
                    { 
                        topTitleText = "",
                        footerText = "<size=8>By purchasing stuff at our store you directly supporting our servers.</size>",
                        noteText = "\n \n ourvipshop.link \n \n you can copy paste link from here",
                        noteName = "VIP SHOP",
                        noteSkinID = 2543322586,
                        fontOutlineColor = "0 0 0 1",
                        fontOutlineThickness = "1",
                        notePuMsg = "You've got a note with url in your inventory",

                    },
                    btnSet = new WPVipRanks.Configuration.BtnSet
                    { 
                        btnColor = "0.19 0.19 0.19 0.85",
                    },

                };
            }
        }

        #endregion
    }
}