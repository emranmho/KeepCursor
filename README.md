# CursorKeep

CursorKeep is a lightweight Windows application built with .NET 10 that prevents system idle/sleep by periodically moving the mouse cursor and simulating a Shift key press. It runs unobtrusively in the system tray and supports global hotkeys for hands-free control.

---

## Features

- **Automated Mouse Movement**
  - Moves the cursor by a random offset (±100 px in both axes) from its current position every 10 seconds.
  - Simultaneously sends a `Shift` key press on each tick to reset input-idle timers.

- **System Tray Integration**
  - Closing or minimizing the window hides it to the system tray; the app keeps running in the background.
  - Double-clicking the tray icon restores the window.
  - The tray icon is **green** while automation is running and **gray** when stopped — updates automatically.
  - Right-clicking the tray icon shows a context menu with **Start**, **Stop**, and **Exit** options (context-aware enable/disable state).

- **Simple UI** — Windows Forms window with **Start** and **Stop** buttons; fixed size, centered on screen.

---

## Architecture

The project follows the **Command** pattern with a thin controller layer:

```
CursorKeep/
├── Program.cs                   # Entry point
├── Form1.cs / Form1.Designer.cs # UI + event wiring
├── Controllers/
│   └── AppController.cs         # Coordinates start/stop commands; exposes IsMoving state
├── Commands/
│   ├── IMoveCommand.cs          # Command interface (Execute)
│   ├── StartCommand.cs          # Delegates to MouseMoverService.Start()
│   └── StopCommand.cs           # Delegates to MouseMoverService.Stop()
└── Services/
    └── MouseMoverService.cs     # Timer-driven mouse movement + Shift key press via P/Invoke
```

### Key implementation details

| Component | Detail |
|---|---|
| `MouseMoverService` | Uses `SetCursorPos` (user32.dll) for cursor positioning; `SendKeys.Send("+")` for the Shift key; `System.Windows.Forms.Timer` at a 10-second interval |
| `AppController` | Holds `IMoveCommand` references and an `IsMoving` bool; used by both the UI buttons and the tray context menu |

---

## Prerequisites

- **OS**: Windows 10 or Windows 11
- **.NET Runtime**: [.NET 10 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

---

## Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/emranmho/AutoMouseAction.git
   cd AutoMouseAction
   ```

2. **Build and run**

   *Visual Studio:* Open `CursorKeep.slnx`, build, and run.

   *Command line:*
   ```bash
   dotnet build
   dotnet run --project CursorKeep
   ```

---

## Usage

1. Launch the application — the window opens centered on screen.
2. Click **Start** to begin automation. The tray icon turns **green**.
3. Click **Stop** to pause. The tray icon turns **gray**.
4. Click **X** or minimize to hide the window to the system tray — automation continues running.
5. Double-click the tray icon to restore the window.
6. Right-click the tray icon for the **Start / Stop / Exit** context menu.
7. Use tray menu **Exit** to fully close the application.

---

## Acknowledgements

- [Microsoft Docs — SetCursorPos](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setcursorpos)
