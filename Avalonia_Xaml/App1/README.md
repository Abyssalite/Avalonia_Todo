- dotnet run --project App1.iOS -f net9.0-ios -p:_DeviceName=:v2:runtime=com.apple.CoreSimulator.SimRuntime.iOS-18-3,devicetype=com.apple.CoreSimulator.SimDeviceType.iPhone-15

. ANDROID_SDK_ROOT/emulator/emulator -avd Android_Emulator_35

- dotnet run --project App1.Android -f net9.0-android
 
- dotnet publish App1.Android -f net9.0-android

- dotnet run --project App1.Desktop -f net9.0

- dotnet run --project App1.Browser -f net9.0-browser
