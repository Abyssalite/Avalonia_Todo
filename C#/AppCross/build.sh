# Run with external Terminal

APK_PATH="AppCross.Android/bin/Debug/net8.0-android/com.CompanyName.AppCross-Signed.apk"

echo "🔧 Building APK..."
dotnet clean AppCross.Android
dotnet build AppCross.Android || exit 1

echo "📦 Installing APK to Waydroid..."
waydroid app remove com.CompanyName.AppCross
waydroid app install "$APK_PATH" || exit 1

echo "🚀 Launching app..."
waydroid app launch com.CompanyName.AppCross

