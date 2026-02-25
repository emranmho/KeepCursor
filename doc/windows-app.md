# Windows PC Application (The Listener)

This document details the implementation plan for integrating Firebase into the existing `AutoMouseActioner` Windows Forms application.

## Role of the Windows App

In this architecture, the Windows PC acts as the **Listener** and the **Worker**.

- It is responsible for physically moving the mouse and pressing the keys.
- It does NOT own the scheduling or the remote Start/Stop buttons.
- It merely listens to the Firebase Realtime Database for instructions and updates Firebase with its current true state.

## 1. Prerequisites

- **Existing Codebase:** You already have the core logic working (`MouseMoverService`, `AppController`).
- **Firebase Database:** A free Firebase project must be created, and the Realtime Database must be initialized (URL ending in `.firebaseio.com`).

## 2. Dependencies

You need to add a library to communicate with Firebase. Since this is a .NET 8 WinForms app, you can use the community-supported NuGet package:

- `FirebaseDatabase.net` (by step-up-labs)

```bash
dotnet add package FirebaseDatabase.net
```

## 3. Implementation Steps

### Step 3.1: Initialize Firebase Client

Create a new service class, e.g., `FirebaseSyncService.cs`, to handle the connection.

```csharp
using Firebase.Database;

public class FirebaseSyncService
{
    private readonly FirebaseClient _firebaseClient;
    private const string AppNode = "KeepCursorApp";

    public FirebaseSyncService(string firebaseUrl)
    {
        _firebaseClient = new FirebaseClient(firebaseUrl);
    }
}
```

### Step 3.2: Manage "Presence" (Online/Offline Status)

When the app starts, it should tell the database it is alive. When the app closes, it should tell the database it is dead.

- _Note: `FirebaseDatabase.net` does not have built-in "onDisconnect" triggers like the official JS SDK, so you must handle graceful shutdowns manually, and potentially implement a "last heartbeat" timestamp if you want the Android app to detect ungraceful crashes._

```csharp
// Call this when the app starts
public async Task SetOnlineStatus(bool isOnline)
{
    await _firebaseClient
        .Child(AppNode)
        .Child("IsOnline")
        .PutAsync(isOnline);
}
```

### Step 3.3: Syncing Local State to Cloud

Whenever the user manually clicks "Start" or "Stop" on the PC, or uses the keyboard hotkeys, the app MUST update Firebase so the phone knows what happened.

```csharp
// Bind this to your btnStart_Click and btnStop_Click
public async Task UpdateStatusAsync(string status)
{
    await _firebaseClient
        .Child(AppNode)
        .Child("Status")
        .PutAsync(status);
}
```

### Step 3.4: The Core Feature - Subscribing to Cloud Commands

This is the most critical part. The app needs to "listen" to the `Command` node in Firebase. If the Android app writes `"Start"` to this node, the PC app must react instantly.

```csharp
public void StartListeningForCommands(AppController controller)
{
    _firebaseClient
        .Child(AppNode)
        .AsObservable<AppState>() // Define a simple AppState class matching your JSON
        .Subscribe(state =>
        {
            if (state == null) return;

            // Handle incoming remote commands
            if (state.Command == "Start" && !controller.IsMoving)
            {
                // Trigger the local start logic
                controller.Start();

                // Reset the command flag so it doesn't fire again
                _firebaseClient.Child(AppNode).Child("Command").PutAsync("None");

                // Update the true status
                _firebaseClient.Child(AppNode).Child("Status").PutAsync("Running");
            }
            else if (state.Command == "Stop" && controller.IsMoving)
            {
                controller.Stop();
                _firebaseClient.Child(AppNode).Child("Command").PutAsync("None");
                _firebaseClient.Child(AppNode).Child("Status").PutAsync("Stopped");
            }
        });
}
```

## 4. Integration into Form1.cs

In your `Form1.cs` constructor or `InitializeApp` method:

1. Instantiate `FirebaseSyncService`.
2. Call `SetOnlineStatus(true)`.
3. Call `StartListeningForCommands(_controller)`.
4. Update `btnStart_Click` and `btnStop_Click` to also call `UpdateStatusAsync()`.
5. Override `OnFormClosed` or `FormClosing` to call `SetOnlineStatus(false)` and update `Status` to `"Stopped"`.

## 5. Security Note for Desktop

Because this is a desktop app running outside a browser, the Firebase Database rules must either be:

- **Public (Not Recommended long-term):** `".read": true, ".write": true`
- **Secured with Firebase Auth (Recommended):** You will need to implement a simple secret key or use Firebase Authentication (Email/Password) to sign in the desktop app invisibly in the background so it has permission to read/write the database, preventing strangers from controlling your mouse.
