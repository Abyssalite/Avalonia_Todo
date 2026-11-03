# Avalonia Todo

A cross-platform **Todo / Task list** application built with **Avalonia UI**.

Supports **Desktop**, **Browser**, **Android**, and **iOS**.

🌐 Live Browser demo: [https://abyssalite.github.io/Avalonia_Todo/](https://abyssalite.github.io/Avalonia_Todo/)

## Features

- Multiple task **lists**
- **Categories / Groups** inside each list
- Tasks with:
  - Name
  - Description
  - Done state
  - Important flag
- **Quick** list (default)
- **Important** tasks view
- Archive / Unarchive lists
- Rename & edit lists
- Add / delete tasks
- Persistent storage (JSON in local app data)
- Custom navigation system (main + side + top bar)
- Event-driven architecture
- Modern UI with Semi.Avalonia + Ursa controls
- Follows system theme

## Platforms

| Platform  | Project          | Command |
|-----------|------------------|---------|
| Desktop   | `App1.Desktop`   | `dotnet run --project App1.Desktop -f net10.0` |
| Browser   | `App1.Browser`   | `dotnet run --project App1.Browser -f net10.0-browser` |
| Android   | `App1.Android`   | `dotnet run --project App1.Android -f net10.0-android` |
| iOS       | `App1.iOS`       | See below |

### iOS (Simulator example)

```bash
dotnet run --project App1.iOS -f net10.0-ios \
  -p:_DeviceName=:v2:runtime=com.apple.CoreSimulator.SimRuntime.iOS-18-3,devicetype=com.apple.CoreSimulator.SimDeviceType.iPhone-15
```

### Android Emulator

```bash
$ANDROID_SDK_ROOT/emulator/emulator -avd Android_Emulator_35
dotnet run --project App1.Android -f net10.0-android
```

### Requirements

- .NET 10 SDK
- Avalonia 11.3+

## Getting Started

```bash
git clone https://github.com/Abyssalite/Avalonia_Todo.git
cd Avalonia_Todo

# Desktop
dotnet run --project App1.Desktop -f net10.0

# Browser
dotnet run --project App1.Browser -f net10.0-browser
```

### Build

```bash
# Desktop
dotnet publish App1.Desktop -f net10.0 -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

dotnet publish App1.Desktop -f net10.0 -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true

# Android
dotnet publish App1.Android -f net10.0-android
```

## Architecture Overview

- MVVM with CommunityToolkit.Mvvm
- Custom Navigation library (Custom_Navigation)
    - Main content
    - Side panel
    - Top bar
    - Back stack
- Custom EventHub (Custom_EventHub) for decoupled communication
- Central Store that holds all application state
- JSON persistence (Tasks.json + Archive.json)

Data is saved under: `text%LocalAppData%/Custom_Todo/`

### Tech Stack

- Avalonia UI 11.3
- Semi.Avalonia + Irihi.Ursa
- CommunityToolkit.Mvvm
- Microsoft.Extensions.DependencyInjection
- System.Text.Json (source generated)
- Custom libraries:
    - Custom_Navigation
    - Custom_EventHub

## License

This project is currently unlicensed.

Built with Avalonia ❤️