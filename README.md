<p align="center">
  <img 
    src="https://github.com/user-attachments/assets/36fa27dc-6774-45b4-8805-a30bf7eb9fda"
    alt="image"
    width="70%"
  />
</p>

<hr>

<p align="center">
  <img 
    src="https://github.com/user-attachments/assets/76ddc7e8-2d86-459d-a4a4-52fe607d2e2a"
    alt="image"
  />
</p>


<h2 align="center">Every text tab in WelcomePanel has section for addon name.</h3>
<h3>Examples:</h3>

| Addon | Name |
|-----:|-----------|
|     Addon (plugin name)| "vipranks"|
|     Addon (plugin name)| "wipecycle"|
|     Addon (plugin name)| "sociallinks"|
|Addon (plugin name)| "kits" |
|Addon (plugin name)| "stats" |
|Addon (plugin name) | "home" |

<br>
<hr>

<p align="center">
  <img width="334" height="76" alt="image" src="https://github.com/user-attachments/assets/2b019158-f13d-422e-a087-7857ccf8e4b1" />
</p>


<a href="https://umod.org/plugins/rust-kits">RustKits</a> is required. <a href="https://www.youtube.com/watch?v=WqGH_GCIOls&t=961s">(tutorial for creating kits)</a>

Importing your kits
You can import your kits through ingame interface by typing /import_kits or manually through json file oxide/data/WPKitsData.json

Changing ui colors
Every color option of user interface can be found in config file oxide/config/WPKits.json.
For correct color code use this tool  https://tools.rustplugins.io/colors


Changing texts
Text displayed on user interface can be change inside lang file oxide/lang/WPKits.json.

<br>
<hr>

<p align="center">
  <img width="335" height="76" alt="image" src="https://github.com/user-attachments/assets/ce9a41c5-ea7c-4dc6-915d-2e28232f36d2" />
</p>

<h3>Features</h3>

- List up to 6 of your social media.
- URL copying based on note system (see last screenshot).
- Fully customizable appearance; for advanced users, additional position settings are inside the <code>.cs</code> file under the <b>Further Customization</b> region.

<details>
  <summary><b>Config Example</b></summary>

<br>

```json
{
  "Panel #1": {
    "Enabled": true,
    "Text Field": "<size=22>DISCORD</size> \n <size=9>Link to our Discord server</size>",
    "Icon Image": "https://i.ibb.co/pRYmCBx/discordicon.png",
    "Button Text": "Get Link",
    "Note Text": "\n\n https://discord.gg/RBbV8sY3wS \n\n Copy the link from the note.",
    "Note Name": "DISCORD",
    "Note Skin ID / Icon": 2391705198,
    "Main Panel Color": "0.25 0.25 0.25 0.65",
    "Secondary Panel Color": "0.19 0.19 0.19 0.85",
    "Button Color": "0.16 0.34 0.49 0.80"
  },
  "Panel #2": {
    "Enabled": true,
    "Text Field": "<size=22>Store</size> \n <size=9>Our in-game shop</size>",
    "Icon Image": "https://i.ibb.co/x3ydpLw/shopicon.png",
    "Button Text": "Get Link",
    "Note Text": "\n\n steamgroup.link \n\n Copy the link from the note.",
    "Note Name": "Store Link",
    "Note Skin ID / Icon": 2543321437,
    "Main Panel Color": "0.25 0.25 0.25 0.65",
    "Secondary Panel Color": "0.19 0.19 0.19 0.85",
    "Button Color": "0.16 0.34 0.49 0.80"
  }
}
```
</details>

<br>
<hr>

<p align="center">
  <img width="335" height="76" alt="image" src="https://github.com/user-attachments/assets/08ccdea4-bbd1-4463-9cf0-845135cf7fa6" />
</p>

<h3>Features</h3>

- List up to 6 VIP  Ranks.
- Buy button provides custom note with url to you web store.
- Appearance is fully customizable, for advanced users position settings provided inside .cs file under Further Customization region.

<details>
  <summary><b>Config Example</b></summary>

<br>

```json
{
  "Panel #1": {
    "Enabled": true,
    "Title Text": "<color=#3498DB><size=15>VIP</size></color>",
    "Text Field": "\n✔ Queue priority\n✔ Chat prefix\n✔ Free skin changes\n✔ Unique kits\n✔ Rates x7.5\n<color=#696969>✘ Enhanced metabolism\n✘ Personal minicopter\n✘ Pocket recycler\n</color>",
    "Icon Image": "https://i.ibb.co/PwFtbs9/vip.png",
    "Button Text": "<size=9>Buy for</size> <size=15>200 ₽</size>",
    "Main Panel Color": "0.25 0.25 0.25 0.65",
    "Secondary Panel Color": "0.19 0.19 0.19 0.85",
    "Button Color": "0.16 0.34 0.49 0.80"
  },

  "Panel #2": {
    "Enabled": true,
    "Title Text": "<color=#E16B00FF><size=15>ELITE</size></color>",
    "Text Field": "\n✔ Queue priority\n✔ Chat prefix\n✔ Free skin changes\n✔ Unique kits\n✔ Rates x9\n✔ Enhanced metabolism\n<color=#696969>✘ Personal minicopter\n✘ Pocket recycler\n</color>",
    "Icon Image": "https://i.ibb.co/r22Nvg9/elite.png",
    "Button Text": "<size=9>Buy for</size> <size=15>400 ₽</size>",
    "Main Panel Color": "0.25 0.25 0.25 0.65",
    "Secondary Panel Color": "0.19 0.19 0.19 0.85",
    "Button Color": "0.88 0.42 0.00 0.80"
  },

  "Panel #3": {
    "Enabled": true,
    "Title Text": "<color=#C70039FF><size=15>LEGEND</size></color>",
    "Text Field": "\n✔ Queue priority\n✔ Chat prefix\n✔ Free skin changes\n✔ Unique kits\n✔ Rates x10\n✔ Enhanced metabolism\n✔ Personal minicopter\n✔ Pocket recycler\n",
    "Icon Image": "https://i.ibb.co/QC8h4jY/legend.png",
    "Button Text": "<size=9>Buy for</size> <size=15>600 ₽</size>",
    "Main Panel Color": "0.25 0.25 0.25 0.65",
    "Secondary Panel Color": "0.19 0.19 0.19 0.85",
    "Button Color": "0.78 0.00 0.22 0.80"
  },

  "Panel #4": {
    "Enabled": false,
    "Title Text": "<color=#C70039FF><size=15>LEGEND</size></color>",
    "Text Field": "\n✔ Queue priority\n✔ Chat prefix\n✔ Free skin changes\n✔ Unique kits\n✔ Rates x10\n✔ Enhanced metabolism\n✔ Personal minicopter\n✔ Pocket recycler\n",
    "Icon Image": "https://i.ibb.co/QC8h4jY/legend.png",
    "Button Text": "<size=9>Buy for</size> <size=15>600 ₽</size>",
    "Main Panel Color": "0.25 0.25 0.25 0.65",
    "Secondary Panel Color": "0.19 0.19 0.19 0.85",
    "Button Color": "0.78 0.00 0.22 0.80"
  },

  "Panel #5": {
    "Enabled": false,
    "Title Text": "<color=#C70039FF><size=15>LEGEND</size></color>",
    "Text Field": "\n✔ Queue priority\n✔ Chat prefix\n✔ Free skin changes\n✔ Unique kits\n✔ Rates x10\n✔ Enhanced metabolism\n✔ Personal minicopter\n✔ Pocket recycler\n",
    "Icon Image": "https://i.ibb.co/QC8h4jY/legend.png",
    "Button Text": "<size=9>Buy for</size> <size=15>600 ₽</size>",
    "Main Panel Color": "0.25 0.25 0.25 0.65",
    "Secondary Panel Color": "0.19 0.19 0.19 0.85",
    "Button Color": "0.78 0.00 0.22 0.80"
  },

  "Panel #6": {
    "Enabled": false,
    "Title Text": "<color=#C70039FF><size=15>LEGEND</size></color>",
    "Text Field": "\n✔ Queue priority\n✔ Chat prefix\n✔ Free skin changes\n✔ Unique kits\n✔ Rates x10\n✔ Enhanced metabolism\n✔ Personal minicopter\n✔ Pocket recycler\n",
    "Icon Image": "https://i.ibb.co/QC8h4jY/legend.png",
    "Button Text": "<size=9>Buy for</size> <size=15>600 ₽</size>",
    "Main Panel Color": "0.25 0.25 0.25 0.65",
    "Secondary Panel Color": "0.19 0.19 0.19 0.85",
    "Button Color": "0.78 0.00 0.22 0.80"
  }
}
```
</details>

<br>
<hr>

<p align="center">
  <img width="335" height="76" alt="image" src="https://github.com/user-attachments/assets/2166bc15-ed07-4b68-825f-8c5f8cd02d8a" />
</p>

<h3>WipeCountdown is required</h3>

<h3>Features</h3>

- Two simple panels made for showing wipe cycle of your server.
- Enable countdown in text by using  {countdown}. Check example down bellow.
- Appearance is fully customizable, for advanced users position settings provided inside .cs file under Further Customization region.

<details>
  <summary><b>Config Example</b></summary>

```json
{
  "General Settings": {
    "Countdown (seconds)": 259200,
    "Global Response Enabled": false,
    "Global Response Text": "<size=20><color=#FF5733>Your server name</color></size> \n Next wipe in <color=#FFE49B>{countdown}</color>",
    "Global Response Icon": 76561199136807947,
    "Global Response Cooldown": 3600,
    "Keyword To Trigger Response": "wipe",
    "Reset Countdown on Map Change": true
  }
}
```
</details>

<br>
<hr>

<h1>FaQ</h1>

<h2 style="color:green;">Can I add multiple pages to WelcomePanel?</h2>
<p>Yes, you can add an unlimited number of pages.</p>

<h2 style="color:green;">Can I add images to WelcomePanel?</h2>
<p>Yes, you can add an image to any panel and set a background for each tab.</p>

<h2 style="color:green;">What image sizes should I use?</h2>
<p>The sizes vary for each panel in different templates. Basically, images that are parent to any panel are stretched to its size. It is best to take a screenshot of the panel and try to fit the image to its size before uploading.</p>

