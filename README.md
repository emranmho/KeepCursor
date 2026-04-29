# CursorKeep

CursorKeep is a lightweight Windows application built with .NET 10 that prevents system idle/sleep by periodically moving the mouse cursor and simulating a Shift key press. It runs unobtrusively in the system tray and can be controlled remotely from anywhere via Telegram.

---

## Features

- **Automated Mouse Movement**
  - Moves the cursor by a random offset (±100 px in both axes) from its current position every 10 seconds.
  - Simultaneously sends a `Shift` key press on each tick to reset input-idle timers.

- **System Tray Integration**
  - Closing or minimizing the window hides it to the system tray; the app keeps running in the background.
  - Double-clicking the tray icon restores the window.
  - The tray icon is **green** while automation is running and **gray** when stopped — updates automatically.
  - Right-clicking the tray icon shows a context menu with **Start**, **Stop**, and **Exit** options.

- **Telegram Remote Control**
  - Control the app from your phone via a private Telegram bot — works from anywhere in the world.
  - Send `/start`, `/stop`, or `/status` from Telegram to control the app remotely.
  - Each user connects their own private bot — no shared server required.
  - First person to message the bot is locked in as the authorized user (security).

- **Simple UI** — Windows Forms window with **Start**, **Stop**, and **⚙ Settings** buttons; connection status shown at the bottom.

---

## Architecture

```
CursorKeep/
├── Program.cs                   # Entry point
├── Form1.cs / Form1.Designer.cs # UI + event wiring
├── SettingsDialog.cs            # Telegram token configuration popup
├── Controllers/
│   └── AppController.cs         # Coordinates start/stop commands; exposes IsMoving state
├── Commands/
│   ├── IMoveCommand.cs          # Command interface (Execute)
│   ├── StartCommand.cs          # Delegates to MouseMoverService.Start()
│   └── StopCommand.cs           # Delegates to MouseMoverService.Stop()
└── Services/
    ├── MouseMoverService.cs     # Timer-driven mouse movement + Shift key press via P/Invoke
    ├── ConfigService.cs         # Loads/saves config.json (bot token + authorized chat ID)
    └── TelegramBotService.cs    # Polls Telegram API, handles commands, sends replies
```

### Key implementation details

| Component | Detail |
|---|---|
| `MouseMoverService` | Uses `SetCursorPos` (user32.dll) for cursor positioning; `SendKeys.Send("+")` for the Shift key; `System.Windows.Forms.Timer` at a 10-second interval |
| `AppController` | Holds `IMoveCommand` references and an `IsMoving` bool; used by both the UI buttons and the tray context menu |
| `ConfigService` | Saves bot token and authorized chat ID to `%AppData%\CursorKeep\config.json` |
| `TelegramBotService` | Polling-based Telegram client; validates tokens before saving; registers commands automatically |

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

### Local control

1. Launch the application — the window opens centered on screen.
2. Click **Start** to begin automation. The tray icon turns green.
3. Click **Stop** to pause. The tray icon turns gray.
4. Click **X** or minimize to hide the window to the system tray — automation continues running.
5. Double-click the tray icon to restore the window.
6. Right-click the tray icon for the **Start / Stop / Exit** context menu.
7. Use tray menu **Exit** to fully close the application.

---

### Remote control via Telegram

#### Step 1 — Create your Telegram bot

1. Open Telegram and search for **@BotFather** (official bot, blue checkmark).
2. Send `/newbot` and follow the prompts:
   - Enter a display name, e.g. `My CursorKeep`
   - Enter a username ending in `bot`, e.g. `mycursorkeep_bot`
3. BotFather replies with your bot token — looks like:
   ```
   123456789:AAFxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
   ```
   Copy it.

#### Step 2 — Connect the bot to the app

1. Open CursorKeep on your PC.
2. Click the **⚙** gear button.
3. Paste your bot token into the text box and click **OK**.
4. The app validates the token. If valid, the status at the bottom turns green:
   ```
   ● Telegram: Connected
   ```
   If the token is wrong, an error popup appears and nothing is saved.

#### Step 3 — Control from your phone

1. Open Telegram on your phone and search for your bot by its username.
2. Tap **Start** (first time only).
3. Tap the **/** button in the chat to see the command menu:

   | Command | Action |
   |---|---|
   | `/start` | Starts mouse movement |
   | `/stop` | Stops mouse movement |
   | `/status` | Shows current state (Running / Stopped) |

4. Tap any command — the app responds instantly from wherever you are.

> **Security:** The first Telegram account to message your bot is saved as the only authorized user. Messages from any other account are silently ignored. To reset this, delete `AuthorizedChatId` from `%AppData%\CursorKeep\config.json`.

---

## Acknowledgements

- [Microsoft Docs — SetCursorPos](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setcursorpos)
- [Telegram Bot API](https://core.telegram.org/bots/api)
- [Telegram.Bot .NET library](https://github.com/TelegramBots/Telegram.Bot)
