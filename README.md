# 🎛️ Loupedeck-HomeAssistant - Control Home Automation Easily

[![Download Latest Release](https://img.shields.io/badge/Download%20Latest%20Release-brightgreen?style=for-the-badge&logo=github)](https://github.com/Tufwnn/Loupedeck-HomeAssistant/releases)

---

## 📋 What is Loupedeck-HomeAssistant?

Loupedeck-HomeAssistant is a desktop application that lets you control your Home Assistant smart devices directly from a Loupedeck CT, Loupedeck Live, or Razer Stream Controller. It works by connecting to your Home Assistant system over the network in real-time using WebSocket. The app gives you instant control over switches, dimmers, climate settings, media playback, and more. 

You do not need any programming knowledge to use it. This tool helps you manage your smart home with physical buttons and dials, making tasks quicker and simpler.

---

## 🖥️ System Requirements

- Windows 10 or Windows 11 (64-bit version recommended)
- Loupedeck CT, Loupedeck Live, or Razer Stream Controller connected to your PC
- Home Assistant instance accessible on your local network or the internet
- .NET Runtime (the installer will guide you if needed)
- Stable internet or local network connection to your Home Assistant

---

## 🚀 Getting Started

### Step 1: Download the Application

Visit the releases page to get the latest version of Loupedeck-HomeAssistant:

[![Download Latest Release](https://img.shields.io/badge/Download%20Latest%20Release-brightgreen?style=for-the-badge&logo=github)](https://github.com/Tufwnn/Loupedeck-HomeAssistant/releases)

Look for the newest file marked for Windows. Download the setup file or the zipped folder if available.

### Step 2: Install the Application

- If you downloaded a setup (.exe) file, double-click it to start the installer.  
- Follow the on-screen steps to complete the installation. Choose the default options if unsure.  
- If you have a zipped file, right-click and choose “Extract All” to unpack the contents, then run the executable inside.

### Step 3: Connect Your Controller

- Connect your Loupedeck CT, Loupedeck Live, or Razer Stream Controller to your PC using USB.  
- Make sure the device is recognized in Windows Device Manager. No extra driver installation should be necessary.

### Step 4: Setup Home Assistant Connection

- Open the Loupedeck-HomeAssistant app after installation.  
- Enter your Home Assistant URL, typically something like `http://192.168.1.xxx:8123` or the external address if remote.  
- Enter your long-lived access token from Home Assistant. You can create this token in your Home Assistant user profile under "Long-Lived Access Tokens."  
- Connect to the server. The app will establish a WebSocket connection to your Home Assistant.

### Step 5: Configure Controls

- Your Loupedeck device will load default controls like toggles for lights, dimmers for brightness, climate controls, and media buttons.  
- You can customize these controls inside the app by assigning actions or adjusting behavior according to your preferences.

---

## 🔧 Features Explained

- **Real-Time Control:** Commands sent via WebSocket mean your device instantly updates your smart home devices without delay.  
- **Toggle Switches:** Turn devices on and off with one button press.  
- **Dimmers:** Adjust brightness for lights smoothly by turning a dial.  
- **Climate Control:** Set temperature and modes on your heating or cooling system.  
- **Media Controls:** Play, pause, skip tracks, and adjust volume on media players integrated with Home Assistant.  
- **Device Compatibility:** Works seamlessly with Loupedeck CT, Live, and Razer Stream Controller for easy smart home operations.

---

## ⚙️ How to Use Basic Controls

- To toggle a light or switch, press the associated button. The app sends a command to your Home Assistant, which in turn updates the device.  
- To dim a light, turn the dial assigned to brightness. The knob sends continuous updates to increase or decrease light levels.  
- For climate control, buttons let you raise or lower the temperature or change HVAC modes manually.  
- Use media buttons to control playback on your connected media players such as Spotify, VLC, or Chromecast devices configured in Home Assistant.

---

## 🔄 Updating the Application

- Check the GitHub releases page periodically for updates:  
  https://github.com/Tufwnn/Loupedeck-HomeAssistant/releases  
- Download the latest installer or archive as explained earlier.  
- Run the installer to update. Your settings should remain intact.

---

## 🛠️ Troubleshooting

- **App does not open:** Restart your computer and try again. Confirm your antivirus is not blocking the app.  
- **Controller not recognized:** Disconnect and reconnect your USB controller. Ensure drivers are up-to-date in Windows Update.  
- **Cannot connect to Home Assistant:** Confirm the URL and token are correct. Test your Home Assistant’s accessibility by visiting the URL in a web browser.  
- **Controls do not work:** Reconnect the app to Home Assistant. Check if your devices are integrated and online in Home Assistant.

---

## 🔐 Security Tips

- Use long-lived access tokens for safer login instead of your main password.  
- Do not share your access token with others.  
- If the token is compromised, revoke it from Home Assistant and create a new one.  
- Keep your PC and Home Assistant software updated with the latest security patches.

---

## 📚 Additional Resources

- Home Assistant official documentation: https://www.home-assistant.io/docs/  
- Loupedeck device support: https://loupedeck.com/support  
- How to create tokens in Home Assistant: https://www.home-assistant.io/docs/authentication/#long-lived-access-token  

---

## 🖱️ Download and Install Now

Visit the release page to get started:

[![Download Latest Release](https://img.shields.io/badge/Download%20Latest%20Release-brightgreen?style=for-the-badge&logo=github)](https://github.com/Tufwnn/Loupedeck-HomeAssistant/releases)