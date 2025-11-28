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
    [Info("WelcomePanel", "crymoooreee", "1.0.0")]

    public class WelcomePanel : RustPlugin
    {

        #region Dependencies / Api

        [PluginReference] Plugin ImageLibrary, WPKits, WPSocialLinks, WPVipRanks, WPWipeCycle, VoteMap, Shop, ServerRewards, Economics, WipeCountdown, WPUpdate, WPHome;

        private string Img(string url)
        {
            if (ImageLibrary != null)
            {
                if (!(bool)ImageLibrary.Call("HasImage", url))
                    return url;
                else
                    return (string)ImageLibrary?.Call("GetImage", url);
            }
            else
                return url;
        }

        #endregion        

        #region Hooks

        void OnServerInitialized()
        {
            if (config == null)
            {
                NextTick(() => {
                    PrintWarning("Configuration file is not valid, this is usually user error. Please before asking developer for help make sure you validate your json file at https://jsonformatter.curiousconcept.com/");
                    PrintWarning("Unloading WelcomePanel now...");
                    Interface.Oxide.UnloadPlugin("WelcomePanel");
                });
                return;
            }

            if (!CheckConfig())
            {
                NextTick(() => {
                    PrintWarning("Unloading WelcomePanel now...");
                    Interface.Oxide.UnloadPlugin("WelcomePanel");
                });
                return;
            }

            if (config.main.onceWipe)
                LoadData();

            RegisterCommands();
            DownloadImages();
            BuildUi();

            /* foreach (var player in BasePlayer.activePlayerList)
            {

                OpenMain(player);
                OpenTab(player, config.main.openAt);
            } */

        }

        void OnPlayerConnected(BasePlayer player)
        {
            if (config.main.open)
            {
                if (config.main.onceWipe)
                {
                    if (playerData.Contains(player.userID))
                        return;

                    playerData.Add(player.userID);
                }

                OpenMain(player);
                OpenTab(player, config.main.openAt);
            }
        }

        void Unload()
        {
            if (config.main.onceWipe)
                SaveData();

            foreach (var player in BasePlayer.activePlayerList)
                CuiHelper.DestroyUi(player, $"{Name}_background");
        }

        void OnNewSave()
        {
            LoadData();
            playerData.Clear();
            SaveData();
        }

        #endregion

        #region Functions

        void OpenMain(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, $"{Name}_background");

            if (config.main.tags)
                CuiHelper.AddUi(player, ReplaceTags(player, $"{_main}"));
            else
                CuiHelper.AddUi(player, _main);

            CuiHelper.AddUi(player, _buttons);
        }

        void OpenTab(BasePlayer player, int tab)
        {
            CuiHelper.DestroyUi(player, $"{Name}_focus");
            CuiHelper.DestroyUi(player, $"{Name}_content");
            CuiHelper.AddUi(player, _contentPanel);

            if (config.tabs[tab - 1].addon != null && config.tabs[tab - 1].addon != "")
            {
                OpenAddon(player, config.tabs[tab - 1].addon);
            }
            else
            {
                CuiHelper.AddUi(player, ReplaceTags(player, $"{tabs[tab][0]}"));
            }

            tab--;
            var btnFocus = new CuiElementContainer();
            Create.Panel(ref btnFocus, $"{Name}_focus", $"{Name}_offset_container", config.main.selectedColor, config.buttons[tab].anchorMin, config.buttons[tab].anchorMax, false, config.baseUi["side"].fade, config.baseUi["side"].fade, config.buttons[tab].mat, "0 0", "0 0");
            Create.Text(ref btnFocus, $"{Name}_focus_text", $"{Name}_focus", "1 1 1 1", config.tabs[tab].name, 12, "0 0", "1 1", config.buttons[tab].align, config.buttons[tab].font, config.baseUi["side"].fade, config.baseUi["side"].fade, "0 0 0 0", "0 0");
            if (config.tabs[tab].icon != null && config.tabs[tab].icon != "")
                Create.Image(ref btnFocus, $"{Name}_focus_img", $"{Name}_focus", Img(config.tabs[tab].icon), "0 0", "1 1", config.baseUi["side"].fade, config.baseUi["side"].fade);

            CuiHelper.AddUi(player, btnFocus);

            if (tabs[tab + 1].Count > 1)
            {
                CuiHelper.DestroyUi(player, $"{Name}_next_button");
                CuiHelper.AddUi(player, $"{_nextButton}".Replace("%tab%", $"{tab + 1}").Replace("%page%", "1"));
            }
        }

        void OpenPage(BasePlayer player, int tab, int page)
        {
            CuiHelper.DestroyUi(player, $"{Name}_content");
            CuiHelper.DestroyUi(player, $"{Name}_text");
            CuiHelper.DestroyUi(player, $"{Name}_next_button");
            CuiHelper.DestroyUi(player, $"{Name}_back_button");

            CuiHelper.AddUi(player, _contentPanel);
            CuiHelper.AddUi(player, ReplaceTags(player, $"{tabs[tab][page]}"));

            if (tabs[tab].Count > page + 1)
            {
                CuiHelper.DestroyUi(player, $"{Name}_next_button");
                CuiHelper.AddUi(player, $"{_nextButton}".Replace("%tab%", $"{tab}").Replace("%page%", $"{page + 1}"));
            }

            if (page > 0)
            {
                CuiHelper.DestroyUi(player, $"{Name}_back_button");
                CuiHelper.AddUi(player, $"{_backButton}".Replace("%tab%", $"{tab}").Replace("%page%", $"{page - 1}"));
            }
        }

        void OpenAddon(BasePlayer player, string addon)
        {
            switch (addon.ToLower())
            {
                case "kits":
                    if (WPKits == null)
                    {
                        PrintWarning("Kits addon is not installed or not loaded...");
                        CuiHelper.AddUi(player, $"{_addonWarning}".Replace("%replace%", "Kits addon is not installed or not loaded..."));
                    }
                    else
                        WPKits.Call("ShowKits_Page1_API", player);
                    break;

                case "sociallinks":
                    if (WPSocialLinks == null)
                    {
                        PrintWarning("SocialLinks addon is not installed or not loaded...");
                        CuiHelper.AddUi(player, $"{_addonWarning}".Replace("%replace%", "SocialLinks addon is not installed or not loaded..."));
                    }
                    else
                        WPSocialLinks.Call("ShowLinks_API", player);
                    break;

                case "vipranks":
                    if (WPVipRanks == null)
                    {
                        PrintWarning("VipRanks addon is not installed or not loaded...");
                        CuiHelper.AddUi(player, $"{_addonWarning}".Replace("%replace%", "VipRanks addon is not installed or not loaded..."));
                    }
                    else
                        WPVipRanks.Call("ShowVipRanks_API", player);
                    break;

                case "wipecycle":
                    if (WPWipeCycle == null)
                    {
                        PrintWarning("WipeCycle addon is not installed or not loaded...");
                        CuiHelper.AddUi(player, $"{_addonWarning}".Replace("%replace%", "WipeCycle addon is not installed or not loaded..."));
                    }
                    else
                        WPWipeCycle.Call("ShowWipeCycle_API", player);
                    break;

                case "votemap":
                    if (VoteMap == null)
                    {
                        PrintWarning("VoteMap addon is not installed or not loaded...");
                        CuiHelper.AddUi(player, $"{_addonWarning}".Replace("%replace%", "VoteMap is not installed or not loaded..."));
                    }
                    else
                        VoteMap.Call("ContentCui", player, 1, 0, true);
                    break;

                case "home":
                    if (WPHome == null)
                    {
                        PrintWarning("Hom is not installed or not loaded...");
                        CuiHelper.AddUi(player, $"{_addonWarning}".Replace("%replace%", "Home is not installed or not loaded..."));
                    }
                    else
                        WPHome.Call("ShowHome_API", player);
                    break;
                case "update":
                    if (WPUpdate == null)
                    {
                        PrintWarning("Update is not installed or not loaded...");
                        CuiHelper.AddUi(player, $"{_addonWarning}".Replace("%replace%", "Update is not installed or not loaded..."));
                    }
                    else
                        WPUpdate.Call("ShowUpdate_API", player);
                    break;

                default:
                    CuiHelper.AddUi(player, $"{_addonWarning}".Replace("%replace%", $"Addon '{addon}' does not exist."));
                    PrintWarning($"Addon '{addon}' does not exist.");
                    break;
            }
        }

        string ReplaceTags(BasePlayer player, string _text)
        {
            if (!config.main.tags)
                return _text;

            string text = _text;

            if (text.Contains("{playername}"))
            {
                string playerName = player.displayName;
                text = text.Replace("{playername}", playerName);
            }
            if (text.Contains("{pvp/pve}"))
            {
                bool pve = ConVar.Server.pve;
                if (pve)
                    text = text.Replace("{pvp/pve}", "PVE");
                else
                    text = text.Replace("{pvp/pve}", "PVP");
            }
            if (text.Contains("{maxplayers}"))
            {
                string max = $"{(int)ConVar.Server.maxplayers}";
                text = text.Replace("{maxplayers}", max);
            }
            if (text.Contains("{online}"))
            {
                string online = $"{(int)BasePlayer.activePlayerList.Count()}";
                text = text.Replace("{online}", online);
            }
            if (text.Contains("{sleeping}"))
            {
                string sleeping = $"{(int)BasePlayer.sleepingPlayerList.Count()}";
                text = text.Replace("{sleeping}", sleeping);
            }
            if (text.Contains("{joining}"))
            {
                string joining = $"{(int)ServerMgr.Instance.connectionQueue.Joining}";
                text = text.Replace("{joining}", joining);
            }
            if (text.Contains("{queued}"))
            {
                string queued = $"{(int)ServerMgr.Instance.connectionQueue.Queued}";
                text = text.Replace("{queued}", queued);
            }
            if (text.Contains("{worldsize}"))
            {
                string worldsize = $"{(int)ConVar.Server.worldsize}";
                text = text.Replace("{worldsize}", worldsize);
            }
            if (text.Contains("{hostname}"))
            {
                string hostname = ConVar.Server.hostname;
                text = text.Replace("{hostname}", hostname);
            }
            if (WipeCountdown != null)
            {
                if (text.Contains("{wipecountdown}"))
                {
                    string wipe = Convert.ToString(WipeCountdown.CallHook("GetCountdownFormated_API"));
                    text = text.Replace("{wipecountdown}", wipe);
                }
            }
            if (Economics != null)
            {
                if (text.Contains("{economics}"))
                {
                    string playersBalance = $"{Economics.Call<double>("Balance", player.UserIDString)}";
                    text = text.Replace("{economics}", playersBalance);
                }
            }
            if (ServerRewards != null)
            {
                if (text.Contains("{rp}"))
                {
                    string playersRP = $"{ServerRewards?.Call<int>("CheckPoints", player.userID)}";
                    text = text.Replace("{rp}", playersRP);
                }
            }
            return text;
        }


        #endregion

        #region Commands

        void RegisterCommands()
        {
            cmd.AddChatCommand(config.main.cmd, this, "ChatCommand");

            foreach (string command in config.main.addCmds.Keys)
                cmd.AddChatCommand(command, this, "ChatCommand");
        }

        void ChatCommand(BasePlayer player, string command, string[] args)
        {
            if (command.ToLower() == config.main.cmd)
            {
                OpenMain(player);
                OpenTab(player, config.main.openAt);
            }

            if (config.main.addCmds.ContainsKey(command.ToLower()))
            {
                OpenMain(player);
                OpenTab(player, config.main.addCmds[command.ToLower()]);
            }
        }

        [ConsoleCommand("welcomepanellite_tab")]
        private void welcomepanellite_tab(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player();
            if (player == null) return;
            var args = arg.Args;
            if (args.Length != 1) return;

            int tab = int.Parse(args[0]);
            OpenTab(player, tab);
        }

        [ConsoleCommand("welcomepanellite_page")]
        private void welcomepanellite_page(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player();
            if (player == null) return;
            var args = arg.Args;
            if (args.Length != 2) return;

            int tab = int.Parse(args[0]);
            int page = int.Parse(args[1]);
            OpenPage(player, tab, page);
        }

        [ConsoleCommand("welcomepanel_close")]
        private void welcomepanel_close(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player();
            if (player == null) return;
        
            CuiHelper.DestroyUi(player, $"{Name}_background");
        }


        #endregion

        #region Build

        CuiElementContainer _main;
        CuiElementContainer _buttons;
        CuiElementContainer _nextButton;
        CuiElementContainer _backButton;
        CuiElementContainer _contentPanel;
        Dictionary<int, List<CuiElementContainer>> tabs = new Dictionary<int, List<CuiElementContainer>>();

        CuiElementContainer _addonWarning;

        void BuildUi()
        {

            string logo = config.main.logo;
            if (config.main.logo == null || config.main.logo == "")
                logo = config.baseUi["logo"].image;

            string title = config.main.title;
            if (config.main.title == null || config.main.title == "")
                logo = config.baseUi["title"].text;

            var ui = new CuiElementContainer();
            Create.Panel(ref ui, $"{Name}_background", "Overlay", config.baseUi["background"].color, config.baseUi["background"].anchorMin, config.baseUi["background"].anchorMax, true, config.baseUi["background"].fade, config.baseUi["background"].fade, config.baseUi["background"].mat, config.baseUi["background"].offsetMin, config.baseUi["background"].offsetMax, config.main.blockInput);
            if (config.baseUi["background"].image != null && config.baseUi["background"].image != "")
                Create.Image(ref ui, $"{Name}_panel_img", $"{Name}_background", Img(config.baseUi["background"].image), "0 0", "1 1", config.baseUi["background"].fade, config.baseUi["background"].fade);
            Create.Panel(ref ui, $"{Name}_offset_container", $"{Name}_background", config.baseUi["offset_container"].color, config.baseUi["offset_container"].anchorMin, config.baseUi["offset_container"].anchorMax, false, config.baseUi["offset_container"].fade, config.baseUi["offset_container"].fade, config.baseUi["offset_container"].mat, config.baseUi["offset_container"].offsetMin, config.baseUi["offset_container"].offsetMax);
            if (config.baseUi["offset_container"].image != null && config.baseUi["offset_container"].image != "")
                Create.Image(ref ui, $"{Name}_panel_img", $"{Name}_offset_container", Img(config.baseUi["offset_container"].image), "0 0", "1 1", config.baseUi["offset_container"].fade, config.baseUi["offset_container"].fade);
            Create.Panel(ref ui, $"{Name}_main", $"{Name}_offset_container", config.baseUi["main"].color, config.baseUi["main"].anchorMin, config.baseUi["main"].anchorMax, false, config.baseUi["main"].fade, config.baseUi["main"].fade, config.baseUi["main"].mat, config.baseUi["main"].offsetMin, config.baseUi["main"].offsetMax);
            if (config.baseUi["main"].image != null && config.baseUi["main"].image != "")
                Create.Image(ref ui, $"{Name}_panel_img", $"{Name}_main", Img(config.baseUi["main"].image), "0 0", "1 1", config.baseUi["main"].fade, config.baseUi["main"].fade);
            Create.Panel(ref ui, $"{Name}_side", $"{Name}_offset_container", config.baseUi["side"].color, config.baseUi["side"].anchorMin, config.baseUi["side"].anchorMax, false, config.baseUi["side"].fade, config.baseUi["side"].fade, config.baseUi["side"].mat, config.baseUi["side"].offsetMin, config.baseUi["side"].offsetMax);
            if (config.baseUi["side"].image != null && config.baseUi["side"].image != "")
                Create.Image(ref ui, $"{Name}_panel_img", $"{Name}_side", Img(config.baseUi["side"].image), "0 0", "1 1", config.baseUi["side"].fade, config.baseUi["side"].fade);

            if (logo != null && logo != "")
            {
                Create.Panel(ref ui, $"{Name}_logo", $"{Name}_offset_container", config.baseUi["logo"].color, config.baseUi["logo"].anchorMin, config.baseUi["logo"].anchorMax, false, config.baseUi["logo"].fade, config.baseUi["logo"].fade, config.baseUi["logo"].mat, config.baseUi["logo"].offsetMin, config.baseUi["logo"].offsetMax);
                Create.Image(ref ui, $"{Name}_logo_img", $"{Name}_logo", Img(logo), "0 0", "1 1", config.baseUi["side"].fade, config.baseUi["side"].fade);
            }

            Create.Panel(ref ui, $"{Name}_title", $"{Name}_offset_container", config.baseUi["title"].color, config.baseUi["title"].anchorMin, config.baseUi["title"].anchorMax, false, config.baseUi["title"].fade, config.baseUi["title"].fade, config.baseUi["title"].mat, config.baseUi["title"].offsetMin, config.baseUi["title"].offsetMax);
               
            if (title != null && title != "")
            {
                Create.Text(ref ui, $"{Name}_title_text", $"{Name}_title", "1 1 1 1", title, 12, "0 0", "1 1", config.baseUi["title"].align, config.baseUi["title"].font, config.baseUi["title"].fade, config.baseUi["title"].fade, "0 0 0 0", "0 0");
            }

            if (config.baseUi["title"].image != null && config.baseUi["title"].image != "")
                Create.Image(ref ui, $"{Name}_title_img", $"{Name}_title", Img(config.baseUi["title"].image), "0 0", "1 1", config.baseUi["title"].fade, config.baseUi["title"].fade);

            Create.Button(ref ui, $"{Name}_closebtn", $"{Name}_offset_container", config.baseUi["close_button"].color, config.baseUi["close_button"].text, 12, config.baseUi["close_button"].anchorMin, config.baseUi["close_button"].anchorMax, "", $"{Name}_background", "1 1 1 1", config.baseUi["close_button"].fade, config.baseUi["close_button"].align, config.baseUi["close_button"].font, config.baseUi["close_button"].mat);
            if (config.baseUi["close_button"].image != null && config.baseUi["close_button"].image != "")
                Create.Image(ref ui, $"{Name}_panel_img", $"{Name}_closebtn", Img(config.baseUi["close_button"].image), "0 0", "1 1", config.baseUi["close_button"].fade, config.baseUi["close_button"].fade);

            _main = ui;

            var btns = new CuiElementContainer();
            int count = 0;

            foreach (var btn in config.buttons)
            {
                if (count >= config.tabs.Count() || count >= config.buttons.Count())
                    break;

                Create.Button(ref btns, $"{Name}_btn{count}", $"{Name}_offset_container", btn.color, config.tabs[count].name, 12, btn.anchorMin, btn.anchorMax, $"welcomepanellite_tab {count + 1}", "", "1 1 1 1", config.baseUi["side"].fade, btn.align, btn.font, btn.mat);
                Create.Image(ref btns, $"btn_img", $"{Name}_btn{count}", Img(config.tabs[count].icon), "0 0", "1 1", config.baseUi["side"].fade, 0);
                count++;
            }

            _buttons = btns;

            int tabCount = 1;
            foreach (var tab in config.tabs)
            {
                var pages = new List<CuiElementContainer>();

                foreach (var page in tab.text)
                {
                    string _text = "";

                    foreach (var line in page)
                        _text += line + "\n";

                    var textUi = new CuiElementContainer();

                    if (tab.image != null && tab.image != "")
                    {
                        //Create.Panel(ref textUi, $"{Name}_content_imagepanel", $"{Name}_content", "0 0 0 0", "0 0", "1 1", false, config.baseUi["content"].fade, config.baseUi["content"].fade, "assets/icons/iconmaterial.mat");
                        Create.Image(ref textUi, $"{Name}_tab_img", $"{Name}_content", Img(tab.image), "0 0", "1 1", config.baseUi["content"].fade, config.baseUi["content"].fade);
                    }
                    Create.Text(ref textUi, $"{Name}_text", $"{Name}_content", tab.color, _text, tab.size, "0.02 0.02", "0.98 0.98", tab.align, tab.font, config.baseUi["content"].fade, config.baseUi["content"].fade, tab.outlineColor, $"{tab.outline} {tab.outline}");
                    pages.Add(textUi);
                }

                tabs.Add(tabCount, pages);
                tabCount++;
            }

            var content = new CuiElementContainer();
            Create.Panel(ref content, $"{Name}_content", $"{Name}_offset_container", config.baseUi["content"].color, config.baseUi["content"].anchorMin, config.baseUi["content"].anchorMax, false, config.baseUi["content"].fade, config.baseUi["content"].fade, config.baseUi["content"].mat, config.baseUi["content"].offsetMin, config.baseUi["content"].offsetMax);
            if (config.baseUi["content"].image != null && config.baseUi["content"].image != "")
                Create.Image(ref content, $"{Name}_panel_img", $"{Name}_content", Img(config.baseUi["content"].image), "0 0", "1 1", config.baseUi["content"].fade, config.baseUi["content"].fade);

            _contentPanel = content;

            var nextButton = new CuiElementContainer();
            Create.Button(ref nextButton, $"{Name}_next_button", $"{Name}_content", config.baseUi["next_button"].color, config.baseUi["next_button"].text, 12, config.baseUi["next_button"].anchorMin, config.baseUi["next_button"].anchorMax, $"welcomepanellite_page %tab% %page%", "", "1 1 1 1", config.baseUi["next_button"].fade, config.baseUi["next_button"].align, config.baseUi["next_button"].font, config.baseUi["next_button"].mat);
            if (config.baseUi["next_button"].image != null && config.baseUi["next_button"].image != "")
                Create.Image(ref nextButton, $"{Name}_panel_img", $"{Name}_next_button", Img(config.baseUi["next_button"].image), "0 0", "1 1", config.baseUi["next_button"].fade, config.baseUi["next_button"].fade);

            _nextButton = nextButton;

            var backButton = new CuiElementContainer();
            Create.Button(ref backButton, $"{Name}_back_button", $"{Name}_content", config.baseUi["back_button"].color, config.baseUi["back_button"].text, 12, config.baseUi["back_button"].anchorMin, config.baseUi["back_button"].anchorMax, $"welcomepanellite_page %tab% %page%", "", "1 1 1 1", config.baseUi["back_button"].fade, config.baseUi["back_button"].align, config.baseUi["back_button"].font, config.baseUi["back_button"].mat);
            if (config.baseUi["back_button"].image != null && config.baseUi["back_button"].image != "")
                Create.Image(ref backButton, $"{Name}_panel_img", $"{Name}_back_button", Img(config.baseUi["back_button"].image), "0 0", "1 1", config.baseUi["back_button"].fade, config.baseUi["back_button"].fade);

            _backButton = backButton;

            var addonWarning = new CuiElementContainer();
            Create.Text(ref addonWarning, $"{Name}_warning", $"{Name}_content", "1 1 1 1", "%replace%", 15, "0.02 0.02", "0.98 0.98", TextAnchor.MiddleCenter, "robotocondensed-bold.ttf", config.baseUi["content"].fade, config.baseUi["content"].fade, "0 0 0 1", $"1 1");
            _addonWarning = addonWarning;
        }


        void DownloadImages()
        {   
            if (ImageLibrary == null)
            {
                Puts("ImageLibrary not found.");
                return;
            }

            if (config.main.logo != null && config.main.logo != "")
                ImageLibrary.Call("AddImage", config.main.logo, config.main.logo);

            foreach (var tab in config.tabs)
            {
                if (tab.icon != null || tab.icon != "")
                {
                    if (tab.icon.StartsWith("http") || tab.icon.StartsWith("www"))
                        ImageLibrary.Call("AddImage", tab.icon, tab.icon);
                }

                if (tab.image != null || tab.image != "")
                {
                    if (tab.image.StartsWith("http") || tab.image.StartsWith("www"))
                        ImageLibrary.Call("AddImage", tab.image, tab.image);
                }
            }

            foreach (string component in config.baseUi.Keys)
            {
                if (config.baseUi[component].image != null && config.baseUi[component].image != "")
                {
                    if (config.baseUi[component].image.StartsWith("http") || config.baseUi[component].image.StartsWith("www"))
                        ImageLibrary.Call("AddImage", config.baseUi[component].image, config.baseUi[component].image);
                }
            }
        }

        bool CheckConfig()
        {
            string[] required_panels = new string[] {
                "background", "offset_container",
                "main", "side", "content", "title",
                "logo", "close_button", "next_button",
                "back_button",
            };

            foreach (string component in required_panels)
            {
                if (!config.baseUi.ContainsKey(component))
                {
                    NextTick(() => {
                        PrintWarning($"Build failed! Missing \"{component}\" panel.");
                    });
                    return false;
                }
            }

            if (config.main.openAt > config.tabs.Count)
            {
                NextTick(() => {
                    PrintWarning($"Build failed! Panel is set to open at {config.main.openAt}. tab but config contains only {config.tabs.Count} tabs.");
                });
                return false;
            }

            if (config.tabs.Count() > config.buttons.Count())
            {
                NextTick(() => {
                    PrintWarning($"Warning! Your config contains {config.tabs.Count()} tabs while your template supports only {config.buttons.Count()} buttons. Plugin will load but some tabs will not be displayed.");
                });
            }

            return true;
        }

        #endregion

        #region Cui

        public class Create
        {
            public static void Panel(ref CuiElementContainer container, string name, string parent, string color, string anchorMinx, string anchorMax, bool cursorOn = false, float fade = 0f, float fadeOut = 0f, string material = "", string offsetMin = "", string offsetMax = "", bool keyboard = false)
            {
                container.Add(new CuiPanel
                {

                    Image = { Color = color, Material = material, FadeIn = fade },
                    RectTransform = { AnchorMin = anchorMinx, AnchorMax = anchorMax, OffsetMin = offsetMin, OffsetMax = offsetMax },
                    FadeOut = 0f,
                    CursorEnabled = cursorOn,
                    KeyboardEnabled = keyboard,

                },
                parent,
                name);
            }

            public static void Image(ref CuiElementContainer container, string name, string parent, string image, string anchorMinx, string anchorMax, float fade = 0f, float fadeOut = 0f, string offsetMin = "", string offsetMax = "")
            {
                if (image.StartsWith("http") || image.StartsWith("www"))
                {
                    container.Add(new CuiElement
                    {
                        Name = name,
                        Parent = parent,
                        FadeOut = 0f,
                        Components =
                        {
                            new CuiRawImageComponent { Url = image, Sprite = "assets/content/textures/generic/fulltransparent.tga", FadeIn = fade},
                            new CuiRectTransformComponent { AnchorMin = anchorMinx, AnchorMax = anchorMax, OffsetMin = offsetMin, OffsetMax = offsetMax }
                        }
                    });
                }
                else
                {
                    container.Add(new CuiElement
                    {
                        Parent = parent,
                        Components =
                        {
                            new CuiRawImageComponent { Png = image, Sprite = "assets/content/textures/generic/fulltransparent.tga", FadeIn = fade},
                            new CuiRectTransformComponent { AnchorMin = anchorMinx, AnchorMax = anchorMax }
                        }
                    });
                }
            }

            public static void Text(ref CuiElementContainer container, string name, string parent, string color, string text, int size, string anchorMinx, string anchorMax, TextAnchor align = TextAnchor.MiddleCenter, string font = "robotocondensed-regular.ttf", float fade = 0f, float fadeOut = 0f, string _outlineColor = "0 0 0 0", string _outlineScale = "0 0")
            {
                container.Add(new CuiElement
                {
                    Parent = parent,
                    Name = name,
                    Components =
                    {
                        new CuiTextComponent
                        {
                            Text = text,
                            FontSize = size,
                            Font = font,
                            Align = align,
                            Color = color,
                            FadeIn = fade,
                        },

                        new CuiOutlineComponent
                        {

                            Color = _outlineColor,
                            Distance = _outlineScale

                        },

                        new CuiRectTransformComponent
                        {
                             AnchorMin = anchorMinx,
                             AnchorMax = anchorMax
                        }
                    },
                    FadeOut = 0f
                });
            }

            public static void Button(ref CuiElementContainer container, string name, string parent, string color, string text, int size, string anchorMinx, string anchorMax, string command = "", string _close = "", string textColor = "0.843 0.816 0.78 1", float fade = 1f, TextAnchor align = TextAnchor.MiddleCenter, string font = "robotocondensed-regular.ttf", string material = "assets/content/ui/uibackgroundblur-ingamemenu.mat")
            {
                container.Add(new CuiButton
                {
                    Button = { Close = _close, Command = command, Color = color, Material = material, FadeIn = fade },
                    RectTransform = { AnchorMin = anchorMinx, AnchorMax = anchorMax },
                    Text = { Text = text, FontSize = size, Align = align, Color = textColor, Font = font, FadeIn = fade }
                },
                parent,
                name);
            }
        }

        #endregion

        #region Player Data

        private void SaveData()
        {
            if (playerData != null)
                Interface.Oxide.DataFileSystem.WriteObject($"{Name}/PlayerData", playerData);
        }

        private List<ulong> playerData;

        private void LoadData()
        {
            if (Interface.Oxide.DataFileSystem.ExistsDatafile($"{Name}/PlayerData"))
            {
                playerData = Interface.Oxide.DataFileSystem.ReadObject<List<ulong>>($"{Name}/PlayerData");
            }
            else
            {
                playerData = new List<ulong>();
                SaveData();
            }
        }

        #endregion

        #region Config

        private Configuration config;

        protected override void LoadConfig()
        {
            base.LoadConfig();

            try
            {
                config = Config.ReadObject<Configuration>();
            }
            catch
            {
                //
            }
            SaveConfig();
        }

        protected override void LoadDefaultConfig() => config = Configuration.CreateConfig();

        protected override void SaveConfig() => Config.WriteObject(config);

        class Configuration
        {
            [JsonProperty(PropertyName = "Main Settings")]
            public Main main { get; set; }

            public class Main
            {
                [JsonProperty("Open when player joins")]
                public bool open { get; set; }

                [JsonProperty("Open once per wipe")]
                public bool onceWipe { get; set; }

                [JsonProperty("Open at tab")]
                public int openAt { get; set; }

                [JsonProperty("Block movement when browsing panel")]
                public bool blockInput { get; set; }

                [JsonProperty("Enable text tags")]
                public bool tags { get; set; }

                [JsonProperty("Server Title")]
                public string title { get; set; }

                [JsonProperty("Server Logo")]
                public string logo { get; set; }

                [JsonProperty("Selected button color")]
                public string selectedColor { get; set; }

                [JsonProperty("Base Command")]
                public string cmd { get; set; }

                [JsonProperty("Additional Commands")]
                public Dictionary<string, int> addCmds { get; set; }

            }

            [JsonProperty(PropertyName = "Text Tabs")]
            public List<Tab> tabs { get; set; }

            public class Tab
            {
                [JsonProperty("Name")]
                public string name { get; set; }

                [JsonProperty("Icon")]
                public string icon { get; set; }

                [JsonProperty("Font Size")]
                public int size { get; set; }

                [JsonProperty("Font Color")]
                public string color { get; set; }

                [JsonProperty("Font Outline Color")]
                public string outlineColor { get; set; }

                [JsonProperty("Font Outline Thickness")]
                public string outline { get; set; }

                [JsonProperty("Font")]
                public string font { get; set; }

                [JsonProperty("Text Background Image")]
                public string image { get; set; }

                [JsonProperty("Text Alignment")]
                public TextAnchor align { get; set; }

                [JsonProperty("Text Lines")]
                public List<string[]> text { get; set; }

                [JsonProperty("Addon (plugin name)")]
                public string addon { get; set; }

            }

            [JsonProperty(PropertyName = "Base Ui Elements")]
            public Dictionary<string, BaseUi> baseUi { get; set; }

            public class BaseUi
            {
                [JsonProperty("Color")]
                public string color { get; set; }

                [JsonProperty("Material")]
                public string mat { get; set; }

                [JsonProperty("Image")]
                public string image { get; set; }

                [JsonProperty("Anchor Min")]
                public string anchorMin { get; set; }

                [JsonProperty("Anchor Max")]
                public string anchorMax { get; set; }

                [JsonProperty("Offset Min")]
                public string offsetMin { get; set; }

                [JsonProperty("Offset Max")]
                public string offsetMax { get; set; }

                [JsonProperty("Fade")]
                public float fade { get; set; }

                [JsonProperty("Text (not for panels)")]
                public string text { get; set; }

                [JsonProperty("Text Alignment (not for panels)")]
                public TextAnchor align { get; set; }

                [JsonProperty("Text Font (not for panels)")]
                public string font { get; set; }

            }

            [JsonProperty(PropertyName = "Tab Buttons")]
            public List<TabButtons> buttons { get; set; }

            public class TabButtons
            {
                [JsonProperty("Color")]
                public string color { get; set; }

                [JsonProperty("Text Alignment")]
                public TextAnchor align { get; set; }

                [JsonProperty("Text Font")]
                public string font { get; set; }

                [JsonProperty("Anchor Min")]
                public string anchorMin { get; set; }

                [JsonProperty("Anchor Max")]
                public string anchorMax { get; set; }

                [JsonProperty("Material")]
                public string mat { get; set; }

            }

            public static Configuration CreateConfig()
            {
                return new Configuration
                {
                    main = new WelcomePanel.Configuration.Main
                    {
                        open = true,
                        onceWipe = false,
                        openAt = 1,
                        blockInput = true,
                        tags = true,
                        selectedColor = "0.161 0.384 0.569 1",
                        title = "<size=60>RUSTSERVER.NET</size>",
                        logo = "https://rustplugins.net/products/welcomepanellite/1/logo.png",
                        cmd = "menu",
                        addCmds = new Dictionary<string, int>()
                        {
                            {"rules", 2},
                            {"help", 3}
                        }
                    },
                    tabs = new List<WelcomePanel.Configuration.Tab>
                    {
                        new WelcomePanel.Configuration.Tab
                        {
                            name = "HOME",
                            icon = "https://rustplugins.net/products/welcomepanellite/1/home_button.png",
                            size = 12,
                            color = "1 1 1 1",
                            outlineColor = "0 0 0 1",
                            outline = "0.5",
                            font = "robotocondensed-bold.ttf",
                            image = "",
                            align = TextAnchor.MiddleCenter,
                            text = new List<string[]>{
                                new string[]
                                {
                                    "<size=45><color=#4A95CC>RUSTSERVERNAME</color> #4 </size> ",
                                    "<size=25>WIPE SCHEDULE <color=#83b8c7>WEEKLY</color> @ <color=#83b8c7>4:00PM</color> (CET)</size>",
                                    "<size=25>RATES <color=#83b8c7>2x GATHER</color> | <color=#83b8c7>1.5x LOOT</color></size> ",
                                    "<size=25>GROUP LIMIT <color=#83b8c7>MAX 5</color></size>",
                                    "<size=25>MAPSIZE <color=#83b8c7>3500</color></size> ",
                                    "\n",
                                    "\n",
                                    "<size=15>Server is located in EU. Blueprints are wiped monthly. Feel free to browse our infomation panel to find out more about the server. If you have more questions, please join our discord and we will happy to help you.</size>",
                                    "\n",
                                    "<size=15><color=#83b8c7>\n This is demo page for Welcome Panel, you can find more examples by checking other tabs.</color></size>"
                                }
                            },
                            addon = "",
                        },
                        new WelcomePanel.Configuration.Tab
                        {
                            name = " RULES",
                            icon = "https://rustplugins.net/products/welcomepanellite/1/rules_button.png",
                            size = 12,
                            color = "1 1 1 1",
                            outlineColor = "0 0 0 1",
                            outline = "0.5",
                            font = "robotocondensed-regular.ttf",
                            image = "",
                            align = TextAnchor.MiddleCenter,
                            text = new List<string[]>{
                                new string[]
                                {
                                    "<size=45><color=#4A95CC>Text Alignment</color></size>",
                                    "",
                                    "<size=18>You can set various text alignments inside config file.</size>",
                                    "<size=18>There is 9 available settings, each one is defined by number (0 to 8)</size>",
                                    "",
                                    "<size=17>UpperLeft - <color=#4A95CC>0</color></size>\n<size=17>UpperCenter - <color=#4A95CC>1</color></size>\n<size=17>UpperRight - <color=#4A95CC>2</color></size>",
                                    "<size=17>MiddleLeft - <color=#4A95CC>3</color></size>\n<size=17>MiddleCenter - <color=#4A95CC>4</color></size>\n<size=17>MiddleRight - <color=#4A95CC>5</color></size>",
                                    "<size=17>LowerLeft - <color=#4A95CC>6</color></size>\n<size=17>LowerCenter - <color=#4A95CC>7</color></size>\n<size=17>LowerRight - <color=#4A95CC>8</color></size>",
                                    "",
                                    ""
                                }
                            },
                            addon = "",
                        },
                        new WelcomePanel.Configuration.Tab
                        {
                            name = "   WIPE CYCLE",
                            icon = "https://rustplugins.net/products/welcomepanellite/1/wipe_button.png",
                            size = 12,
                            color = "1 1 1 1",
                            outlineColor = "0 0 0 1",
                            outline = "0.5",
                            font = "robotocondensed-regular.ttf",
                            image = "https://rustplugins.net/products/welcomepanellite/1/richtext.png",
                            align = TextAnchor.MiddleLeft,
                            text = new List<string[]>{
                                new string[]
                                {
                                    "<size=45><color=#4A95CC><b>Text Style</b></color></size>",
                                    "Text in Rust plugins can be styled with standard rich text tags, it's similar to HTML. Every tag must be closed at the end of text line, otherwise none of the tags will work and they will be shown as regular text. If you ever encounter this issue, please do not reach out to support for help and double check your text lines instead.",
                                    "",
                                    "  Available Tags",
                                    "\n\n\n\n\n\n\n\n\n",
                                    "  Available Fonts <size=9>(fonts are applied to whole page)</size>\n",
                                    "    <b>robotocondensed-bold.ttf</b>",
                                    "    <b>robotocondensed-regular.ttf</b>",
                                    "    <b>permanentmarker.ttf</b>",
                                    "    <b>droidsansmono.ttf</b>"
                                }
                            },
                            addon = "",
                        },
                        new WelcomePanel.Configuration.Tab
                        {
                            name = "  SUPPORT",
                            icon = "https://rustplugins.net/products/welcomepanellite/1/admin_button.png",
                            size = 12,
                            color = "1 1 1 1",
                            outlineColor = "0 0 0 1",
                            outline = "0.5",
                            font = "robotocondensed-regular.ttf",
                            image = "https://rustplugins.net/products/welcomepanellite/1/enableaddons.png",
                            align = TextAnchor.UpperLeft,
                            text = new List<string[]>{
                                new string[]
                                {
                                    "<size=45><color=#4A95CC><b>How to enable addons</b></color></size>",
                                    "Each tab has addon option right under text line. Simply put addon name in there.",
                                    "You can find list of addon names in plugin description."
                                }
                            },
                            addon = "",
                        },
                        new WelcomePanel.Configuration.Tab
                        {
                            name = "KITS",
                            icon = "https://rustplugins.net/products/welcomepanellite/1/kits_button.png",
                            size = 12,
                            color = "1 1 1 1",
                            outlineColor = "0 0 0 1",
                            outline = "0.5",
                            font = "robotocondensed-regular.ttf",
                            image = "",
                            align = TextAnchor.MiddleLeft,
                            text = new List<string[]>{
                                new string[]
                                {
                                    "This text won't be displayed because",
                                    "addon is assigned to this tab."
                                }
                            },
                            addon = "kits",
                        },
                        new WelcomePanel.Configuration.Tab
                        {
                            name = "   COMMANDS",
                            icon = "https://rustplugins.net/products/welcomepanellite/1/star_button.png",
                            size = 12,
                            color = "1 1 1 1",
                            outlineColor = "0 0 0 1",
                            outline = "0.5",
                            font = "robotocondensed-regular.ttf",
                            image = "https://rustplugins.net/products/welcomepanellite/1/multiplepages.png",
                            align = TextAnchor.UpperLeft,
                            text = new List<string[]>{
                                new string[]
                                {
                                    "<size=45><color=#4A95CC><b>How to add multiple pages</b></color></size>",
                                    "You can add unlimited amount of pages, check image bellow or config file for example."
                                },
                                new string[]
                                {
                                    "<size=45><color=#4A95CC><b>This is page number 2</b></color></size>",
                                    "You can add unlimited amount of pages, check image bellow or config file for example."
                                },
                                new string[]
                                {
                                    "<size=45><color=#4A95CC><b>This is page number 3</b></color></size>",
                                    "You can add unlimited amount of pages, check image bellow or config file for example."
                                }
                            },
                            addon = "",
                        },
                        new WelcomePanel.Configuration.Tab
                        {
                            name = " DISCORD",
                            icon = "https://rustplugins.net/products/welcomepanellite/1/discord.png",
                            size = 12,
                            color = "1 1 1 1",
                            outlineColor = "0 0 0 1",
                            outline = "1",
                            font = "robotocondensed-regular.ttf",
                            image = "",
                            align = TextAnchor.MiddleCenter,
                            text = new List<string[]>{
                                new string[]
                                {
                                    "<size=25>discord.gg/<color=#c14229><b><size=22>RUSTPLUGINS</size></b></color></size>",
                                    "Become verified customer and get faster support.",
                                    "\n\n\n",
                                    "Full documentation at <b>docs.rustplugins.net/welcomepanellite</b>"
                                }
                            },
                            addon = "",
                        },
                    },
                    baseUi = new Dictionary<string, WelcomePanel.Configuration.BaseUi>
                    {
                        {"background", new WelcomePanel.Configuration.BaseUi
                            {
                                color = "0 0 0 0.8",
                                mat = "assets/content/ui/uibackgroundblur.mat",
                                image = "",
                                anchorMin = "0 0",
                                anchorMax = "1 1",
                                offsetMin = "0 0",
                                offsetMax = "0 0",
                                fade = 0.2f,
                                text = null,
                                align = TextAnchor.MiddleCenter,
                                font = null
                            }
                        },
                        {"offset_container", new WelcomePanel.Configuration.BaseUi
                            {
                                color = "0 0 0 0",
                                mat = "assets/icons/iconmaterial.mat",
                                image = "",
                                anchorMin = "0.5 0.5",
                                anchorMax = "0.5 0.5",
                                offsetMin = "-680 -360",
                                offsetMax = "680 360",
                                fade = 0.2f,
                                text = null,
                                align = TextAnchor.MiddleCenter,
                                font = null
                            }
                        },
                        {"main", new WelcomePanel.Configuration.BaseUi
                            {
                                color = "0.70 0.67 0.65 0.07",
                                mat = "assets/content/ui/uibackgroundblur-ingamemenu.mat",
                                image = "",
                                anchorMin = "0.315 0.175",
                                anchorMax = "0.80 0.748",
                                offsetMin = "0 0",
                                offsetMax = "0 0",
                                fade = 0.2f,
                                text = null,
                                align = TextAnchor.MiddleCenter,
                                font = null
                            }
                        },
                        {"side", new WelcomePanel.Configuration.BaseUi
                            {
                                color = "0.70 0.67 0.65 0.07",
                                mat = "assets/content/ui/uibackgroundblur-ingamemenu.mat",
                                image = "",
                                anchorMin = "0.187 0.175",
                                anchorMax = "0.308 0.748",
                                offsetMin = "0 0",
                                offsetMax = "0 0",
                                fade = 0.2f,
                                text = null,
                                align = TextAnchor.MiddleCenter,
                                font = null
                            }
                        },
                        {"content", new WelcomePanel.Configuration.BaseUi
                            {
                                color = "0 0 0 0.0",
                                mat = "assets/icons/iconmaterial.mat",
                                image = "",
                                anchorMin = "0.32 0.182",
                                anchorMax = "0.795 0.740",
                                offsetMin = "0 0",
                                offsetMax = "0 0",
                                fade = 0.0f,
                                text = null,
                                align = TextAnchor.MiddleCenter,
                                font = null
                            }
                        },
                        {"title", new WelcomePanel.Configuration.BaseUi
                            {
                                color = "0 0 0 0",
                                mat = "assets/icons/iconmaterial.mat",
                                image = "",
                                anchorMin = "0.185 0.745",
                                anchorMax = "0.9 0.85",
                                offsetMin = "0 0",
                                offsetMax = "0 0",
                                fade = 0.2f,
                                text = null,
                                align = TextAnchor.MiddleLeft,
                                font = "robotocondensed-bold.ttf"
                            }
                        },
                        {"logo", new WelcomePanel.Configuration.BaseUi
                            {
                                color = "0 0 0 0",
                                mat = "assets/icons/iconmaterial.mat",
                                image = null,
                                anchorMin = "0.507 0.760",
                                anchorMax = "0.545 0.833",
                                offsetMin = "0 0",
                                offsetMax = "0 0",
                                fade = 0.2f,
                                text = null,
                                align = TextAnchor.MiddleCenter,
                                font = null,
                            }
                        },
                        {"close_button", new WelcomePanel.Configuration.BaseUi
                            {
                                color = "0.757 0.259 0.161 1.00",
                                mat = "assets/content/ui/uibackgroundblur-ingamemenu.mat",
                                image = "",
                                anchorMin = "0.73 0.125",
                                anchorMax = "0.80 0.16",
                                offsetMin = "0 0",
                                offsetMax = "0 0",
                                fade = 0.2f,
                                text = "✘ CLOSE",
                                align = TextAnchor.MiddleCenter,
                                font = "robotocondensed-bold.ttf",
                            }
                        },
                        {"next_button", new WelcomePanel.Configuration.BaseUi
                            {
                                color = "1 1 1 0.30",
                                mat = "assets/icons/iconmaterial.mat",
                                image = "https://rustplugins.net/products/welcomepanellite/1/next.png",
                                anchorMin = "0.953 0.013",
                                anchorMax = "0.995 0.06",
                                offsetMin = "0 0",
                                offsetMax = "0 0",
                                fade = 0.0f,
                                text = "",
                                align = TextAnchor.MiddleCenter,
                                font = "robotocondensed-bold.ttf",
                            }
                        },
                        {"back_button", new WelcomePanel.Configuration.BaseUi
                            {
                                color = "1 1 1 0.30",
                                mat = "assets/icons/iconmaterial.mat",
                                image = "https://rustplugins.net/products/welcomepanellite/1/back.png",
                                anchorMin = "0.902 0.013",
                                anchorMax = "0.943 0.06",
                                offsetMin = "0 0",
                                offsetMax = "0 0",
                                fade = 0.0f,
                                text = "",
                                align = TextAnchor.MiddleCenter,
                                font = "robotocondensed-bold.ttf",
                            }
                        }
                    },
                    buttons = new List<WelcomePanel.Configuration.TabButtons>
                    {
                        new WelcomePanel.Configuration.TabButtons
                        {
                            color = "0 0 0 0",
                            align = TextAnchor.MiddleCenter,
                            font = "robotocondensed-bold.ttf",
                            anchorMin = "0.1845 0.700",
                            anchorMax = "0.3095 0.748",
                            mat = "assets/icons/iconmaterial.mat"
                        },
                        new WelcomePanel.Configuration.TabButtons
                        {
                            color = "0 0 0 0",
                            align = TextAnchor.MiddleCenter,
                            font = "robotocondensed-bold.ttf",
                            anchorMin = "0.1845 0.650",
                            anchorMax = "0.3095 0.697",
                            mat = "assets/icons/iconmaterial.mat"
                        },
                        new WelcomePanel.Configuration.TabButtons
                        {
                            color = "0 0 0 0",
                            align = TextAnchor.MiddleCenter,
                            font = "robotocondensed-bold.ttf",
                            anchorMin = "0.1845 0.600",
                            anchorMax = "0.3095 0.647",
                            mat = "assets/icons/iconmaterial.mat"
                        },
                        new WelcomePanel.Configuration.TabButtons
                        {
                            color = "0 0 0 0",
                            align = TextAnchor.MiddleCenter,
                            font = "robotocondensed-bold.ttf",
                            anchorMin = "0.1845 0.550",
                            anchorMax = "0.3095 0.597",
                            mat = "assets/icons/iconmaterial.mat"
                        },
                        new WelcomePanel.Configuration.TabButtons
                        {
                            color = "0 0 0 0",
                            align = TextAnchor.MiddleCenter,
                            font = "robotocondensed-bold.ttf",
                            anchorMin = "0.1845 0.500",
                            anchorMax = "0.3095 0.547",
                            mat = "assets/icons/iconmaterial.mat"
                        },
                        new WelcomePanel.Configuration.TabButtons
                        {
                            color = "0 0 0 0",
                            align = TextAnchor.MiddleCenter,
                            font = "robotocondensed-bold.ttf",
                            anchorMin = "0.1845 0.450",
                            anchorMax = "0.3095 0.497",
                            mat = "assets/icons/iconmaterial.mat"
                        },
                        new WelcomePanel.Configuration.TabButtons
                        {
                            color = "0 0 0 0",
                            align = TextAnchor.MiddleCenter,
                            font = "robotocondensed-bold.ttf",
                            anchorMin = "0.1845 0.400",
                            anchorMax = "0.3095 0.447",
                            mat = "assets/icons/iconmaterial.mat"
                        },
                        new WelcomePanel.Configuration.TabButtons
                        {
                            color = "0 0 0 0",
                            align = TextAnchor.MiddleCenter,
                            font = "robotocondensed-bold.ttf",
                            anchorMin = "0.1845 0.350",
                            anchorMax = "0.3095 0.397",
                            mat = "assets/icons/iconmaterial.mat"
                        },
                        new WelcomePanel.Configuration.TabButtons
                        {
                            color = "0 0 0 0",
                            align = TextAnchor.MiddleCenter,
                            font = "robotocondensed-bold.ttf",
                            anchorMin = "0.1845 0.300",
                            anchorMax = "0.3095 0.347",
                            mat = "assets/icons/iconmaterial.mat"
                        },
                        new WelcomePanel.Configuration.TabButtons
                        {
                            color = "0 0 0 0",
                            align = TextAnchor.MiddleCenter,
                            font = "robotocondensed-bold.ttf",
                            anchorMin = "0.1845 0.250",
                            anchorMax = "0.3095 0.297",
                            mat = "assets/icons/iconmaterial.mat"
                        },
                    }
                };
            }
        }
        #endregion

    }
}