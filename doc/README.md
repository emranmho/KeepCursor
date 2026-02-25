# Remote Control Architecture (Firebase)

This document outlines the proposed architecture and implementation plan for adding remote control capabilities to the `AutoMouseActioner` Windows desktop application via an Android app using Firebase Realtime Database.

## Overview

The goal is to allow a user to monitor the status (running/stopped) and control (start/stop) the `AutoMouseActioner` desktop application from an Android phone, from anywhere in the world over the internet.

To achieve this without dealing with complex network configurations (like port forwarding or firewalls blocking incoming connections to the PC), we will use a **Cloud Middleman** approach. Both the Windows PC and the Android phone will communicate with a central cloud database.

## Technology Stack

- **Cloud Middleman:** Firebase Realtime Database (Free Tier / Spark Plan)
- **Desktop App (Listener/Worker):** Existing Windows Forms Application (.NET 8 C#)
- **Mobile App (Controller):** Cross-platform Mobile Application (Flutter/Dart)

## Why Firebase Realtime Database?

Firebase is ideal for this scenario because it provides **real-time synchronization** using WebSockets under the hood.

- **Serverless:** No need to build or host a custom backend API (like ASP.NET Core).
- **Instant:** Changes made on the phone are pushed to the PC in milliseconds.
- **Free:** The Free Tier (100 simultaneous connections, 1GB storage, 10GB/month bandwidth) is vastly more than enough for this personal project.

## Database Structure (JSON)

We will use a very simple structure in the Firebase Realtime Database:

```json
{
  "KeepCursorApp": {
    "Status": "Stopped", // Managed by the PC (Running, Stopped)
    "Command": "None", // Managed by the Phone (Start, Stop, None)
    "IsOnline": false, // Managed by the PC (true when app is open)
    "Interval": 10000 // Managed by Phone, read by PC (Optional feature)
  }
}
```

## How It Works (The Flow)

The core principle is that **Firebase owns the truth**. The phone writes commands, and the PC executes them and updates its status.

### 1. The Windows PC (The Listener)

The WinForms app will maintain a constant, lightweight connection to Firebase while it is running (even if minimized to the system tray).

- **On Startup:** It connects to Firebase and sets `IsOnline: true` and `Status: Stopped` (or `Running` if it auto-starts).
- **Listening:** It subscribes to the `KeepCursorApp` node.
- **Execution:** If it sees the `Command` value change from `"None"` to `"Start"`, it will:
  1. Call `_controller.Start()` (simulating the physical hotkey).
  2. Write back to Firebase: `Command: "None"` (resetting the command).
  3. Write back to Firebase: `Status: "Running"`.
- **On Exit:** It sets `IsOnline: false`.

### 2. The Android Phone (The Controller)

The Android app acts as the remote control, reading the status and sending commands.

- **On Startup:** It connects to Firebase and subscribes to the `KeepCursorApp` node.
- **Display UI:** It reads `Status` and `IsOnline`. If `IsOnline` is false, it might show "PC Offline". If it is "Running", it shows a green indicator.
- **Sending Commands:** When the user taps the "Start" button on the phone, the app simply writes `"Start"` to the `Command` field in Firebase. It does _not_ write to the `Status` field. It waits for the PC to update the `Status` field, and then the phone's UI will naturally update.

## Scenarios Discussed

1. **Start on PC, Check Phone:**
   User starts the app on the laptop -> PC updates Firebase `Status: "Running"` -> User opens phone app -> Phone reads Firebase -> UI shows "Running" instantly.
2. **Minimize on PC, Start on Phone:**
   User opens PC app and minimizes it -> PC connects to Firebase, waiting -> User opens phone, taps "Start" -> Phone updates Firebase `Command: "Start"` -> Firebase pushes update to minimized PC app -> PC app triggers "Start" and updates Firebase `Status: "Running"` -> Phone sees status change and UI updates.

## Next Steps for Implementation

If you are ready to begin implementation, follow these steps:

1. **Create Firebase Project:**
   - Go to the Firebase Console (console.firebase.google.com).
   - Create a new project.
   - Add a "Realtime Database".
   - Set the database rules (initially to public for testing, later secure it).
2. **Update Windows App (.NET 8):**
   - Install a Firebase Realtime Database NuGet package (e.g., `FirebaseDatabase.net`).
   - Add connection logic to `Form1.cs` or a new `FirebaseService.cs`.
   - Implement the listener to watch for `Command` changes and trigger `_controller.Start()` / `_controller.Stop()`.
   - Ensure the UI updates Firebase `Status` when buttons are clicked manually or hotkeys are pressed.
3. **Build Mobile App:**
   - Create a new Flutter project.
   - Add the necessary Firebase packages (e.g., `firebase_core`, `firebase_database`).
   - Build a simple UI with Start/Stop buttons and a Status indicator.
   - Wire the buttons to update the `Command` node in Firebase.

## Advanced Features (Future Ideas)

Because the database syncs instantly, you can add many features by simply adding fields to the JSON:

- **Change Movement Interval:** Add a slider on the phone to modify 'Interval' in Firebase. The PC listens and updates its internal timer.
- **Scheduled Start/Stop:** Add timestamps to Firebase that the PC constantly checks.
- **Custom Keys:** Change the key pressed (Shift, Ctrl, etc.) by updating a field in Firebase.
- **Multi-PC Support:** Change the root JSON node from `KeepCursorApp` to a list of devices (e.g., `Devices/WorkLaptop`, `Devices/HomePC`) to control multiple computers independently.

## Project Structure

This project uses a **Polyglot Monorepo** approach to house both the .NET 8 Windows application and the Flutter mobile application in a single Git repository. While code cannot be directly shared between C# and Dart, this structure simplifies source control and versioning.

```text
KeepCursor/                            # Root of your Git repository
├── .git/
├── .gitignore                         # Will contain both C# and Flutter ignores
├── README.md                          # The main project overview
│
├── KeepCursor.Windows/                # 1. WINDOWS DESKTOP APP (.NET 8 C#)
│   ├── KeepCursor.Windows.sln         # Note: The .sln file moves inside here!
│   ├── KeepCursor.Windows.csproj
│   ├── Program.cs
│   ├── Models/
│   │   └── AppState.cs                # The C# version of the Firebase JSON
│   └── (rest of your WinForms code)
│
└── keep_cursor_mobile/                # 2. FLUTTER APP (Dart)
    ├── pubspec.yaml                   # Flutter's equivalent of a .csproj file
    ├── android/                       # Flutter generated Android shell
    ├── ios/                           # Flutter generated iOS shell
    └── lib/                           # Your Dart Source Code!
        ├── main.dart
        ├── models/
        │   └── app_state.dart         # The Dart version of the Firebase JSON
        └── screens/
            └── home_screen.dart       # The Remote Control UI
```
