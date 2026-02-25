# Android Application (The Controller)

This document details the implementation plan for the Android application responsible for remotely controlling the `AutoMouseActioner` Windows app.

## Role of the Android App

In this architecture, the phone acts as a **Remote Control Dashboard**.

- It does NOT execute any automation on the phone itself.
- It ONLY reads the current state from Firebase to display the UI.
- It ONLY writes intention commands (e.g., "I want the PC to Start") to Firebase.

## 1. Prerequisites

- **Android Development Environment:** Since you are a .NET developer, **.NET MAUI** (Multi-platform App UI) is highly recommended. It allows you to write the Android app entirely in C# and XAML. Alternatively, you can use native Android Studio (Java/Kotlin) or Flutter.
- **Firebase Project:** Must point to the exact same Firebase Realtime Database URL as the Windows application.

## 2. Dependencies (.NET MAUI Approach)

If using .NET MAUI, you will need a library to communicate with Firebase Realtime Database.

- `FirebaseDatabase.net` (by step-up-labs) - The same library used on the Windows side works perfectly in MAUI.

```bash
dotnet add package FirebaseDatabase.net
```

## 3. User Interface (UI) Design

The UI should be very simple and clear:

- **Status Indicator:** A large icon or text at the top (e.g., a green circle if "Running", a red circle if "Stopped", or gray if "Offline").
- **Start Button:** A large, prominent button. Disabled if the app is already "Running" or if the PC is "Offline".
- **Stop Button:** A large button. Disabled if the app is already "Stopped".

## 4. Implementation Steps

### Step 4.1: Initialize Firebase Connection

In your main ViewModel or Service layer, connect to the database.

```csharp
using Firebase.Database;

public class RemoteControlViewModel : INotifyPropertyChanged
{
    private readonly FirebaseClient _firebaseClient;
    private const string AppNode = "KeepCursorApp";

    public RemoteControlViewModel()
    {
        _firebaseClient = new FirebaseClient("https://your-project.firebaseio.com/");
        ListenToAppStatus();
    }
}
```

### Step 4.2: Listening for Status Changes (Updating the UI)

The phone app must constantly monitor the `Status` and `IsOnline` fields. Whenever the PC updates them, the phone UI must change.

```csharp
private void ListenToAppStatus()
{
    _firebaseClient
        .Child(AppNode)
        .AsObservable<AppState>() // Same model class used in Windows
        .Subscribe(state =>
        {
            if (state == null) return;

            // Use MainThread to update UI properties bound to XAML
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsPcOnline = state.IsOnline;
                CurrentStatus = state.Status; // e.g., bound to a Label

                // Determine button states based on variables
                CanStart = state.IsOnline && state.Status == "Stopped" && state.Command == "None";
                CanStop = state.IsOnline && state.Status == "Running" && state.Command == "None";
            });
        });
}
```

### Step 4.3: Sending Commands (The Start/Stop Buttons)

When the user taps a button, the app DOES NOT update the `Status`. It only updates the `Command` node, asking the PC to execute the action.

```csharp
// Bound to the "Start" button click event in UI
public async Task SendStartCommandAsync()
{
    // Write the intention to start
    await _firebaseClient
        .Child(AppNode)
        .Child("Command")
        .PutAsync("Start");

    // The UI will naturally update when the PC reacts and changes the 'Status' to "Running".
}

// Bound to the "Stop" button click event in UI
public async Task SendStopCommandAsync()
{
    // Write the intention to stop
    await _firebaseClient
        .Child(AppNode)
        .Child("Command")
        .PutAsync("Stop");
}
```

## 5. Handling Edge Cases

- **PC is Offline:** If `IsOnline` is false, hide or disable the Start/Stop buttons and display a message indicating the AutoMove app is closed on the desktop.
- **Command Delay (Spinner):** When the "Start" button is pressed, there might be a 1-2 second delay before the PC detects it, starts, and updates the `Status`. You may want to show a loading spinner on the button immediately upon pressing it, which hides once the `Status` actually changes to "Running".

## 6. Authentication (Optional but Recommended)

If you secure your Firebase Database rules (so strangers cannot control your mouse), your Android app must authenticate.

- You can use the official `FirebaseAuthentication.net` package to sign in silently with an Email/Password tied to your Firebase project before you start reading/writing to the database.
