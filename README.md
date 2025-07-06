# 🔒 Winlock Service  
🚀 **Enhanced Windows security with HID Device Protection**  
👦 *I created this at the beginning of 2025.*

---

## 🛡️ USB Lock Service

**USB Lock Service** is a Windows service that strengthens security by automatically locking the system when a USB device (flash drive, mouse, or keyboard) is detected. The lock can be bypassed if a specific file with a predefined name is found on the connected USB flash drive.

---

## ✨ Features

- ✅ **Monitors USB connections and detects unauthorized devices**  
- 🔒 **Automatically locks the Windows session upon detection**  
- 🗝️ **Allows unlocking with a designated file on the USB drive**  
- 👀 **Runs silently in the background**  
- 🖥️ **TeamViewer connections remain unaffected**, ensuring seamless remote access  
- ⚡ **Automatically starts with Windows and runs on system startup for continuous protection**

---

## ⚙️ Installation & Setup

To use the service, follow these steps:

1. **FIRST BUILD THE SERVICE TO THE FOLDER CALLED `BuildHere`**
2. **Then run the provided `.bat` scripts** – These scripts set up the scheduled task and install the Windows service automatically.

### 🛠️ Manual Setup (if needed)

You can manually create a scheduled task via **Windows Task Scheduler** and install the service separately if desired.

### ❌ Removal

The provided **uninstall `.bat`** script removes both the service and scheduled task cleanly.

---

## 🏢 Use Cases

- 🔹 **Workplaces** – Prevent unauthorized physical access to office computers  
- 🔹 **Educational Institutions** – Enhance security for public-use computers in libraries and labs  
- 🔹 **Information Screens & Digital Signage** – Keep Windows-based display systems secure and prevent unauthorized interference

---

## 💡 Summary

Perfect for organizations looking for an **extra layer of protection without compromise**!
