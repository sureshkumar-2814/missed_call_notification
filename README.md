# Missed Call Notifier – .NET MAUI Android App

**Missed Call Notifier** is a .NET MAUI-based Android application that automatically detects missed calls and sends an email notification containing the caller's number and timestamp. The app is designed for users who may not have direct access to their phones but want to stay informed about missed communications in real time.

---

## Features

- Detects missed calls using Android's `CallLog` content provider
- Sends automatic email notifications with:
  - Caller’s phone number
  - Timestamp of the missed call
- Requests runtime permissions for call log access and phone state
- Built using .NET MAUI with platform-specific code for Android
- Uses `BroadcastReceiver` to hook into Android’s native phone state events

---


## Technologies Used

- [.NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/)
- C# / XAML
- Android Telephony & Call Log APIs
- SMTP for email sending
- Gmail App Password for secure mail delivery

---

## Permissions Used

| Permission | Purpose |
|-----------|---------|
| `READ_PHONE_STATE` | To listen for call state changes |
| `READ_CALL_LOG`    | To access the latest missed call details |

Permissions are requested at runtime using Android’s recommended `ActivityCompat` mechanism.

---

## How It Works

1. **BroadcastReceiver (`CallReceiver`)** is registered for `PHONE_STATE` intents.
2. When the call ends, it queries the latest entry in the Android Call Log.
3. If the call type is `MISSED`, it extracts the number and time.
4. A simple SMTP client sends an email to a predefined recipient.

---

## Setup Instructions

1. Clone the repository and open in **Visual Studio 2022 or newer**
2. Ensure the **.NET MAUI workload** is installed
3. Replace email credentials in `CallReceiver.cs`:
   ```csharp
   new NetworkCredential("your.email@gmail.com", "your-16-digit-app-password")
