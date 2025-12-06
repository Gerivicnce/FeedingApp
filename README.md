# FeedingApp

## Camera capture on Android emulators

The app uses the [`CommunityToolkit.Maui.Camera`](https://learn.microsoft.com/dotnet/communitytoolkit/maui/camera) control to capture photos. The camera page automatically requests runtime permission before starting the camera and stores captured photos inside the app data directory.

To try camera capture on an emulated Android device:

1. Install the .NET MAUI Android workload (if not already installed):
   ```bash
   dotnet workload install maui-android
   ```
2. Create or start an Android emulator from the Android SDK (API level 34+ is recommended) and ensure it has a virtual camera enabled.
3. Deploy the app to the emulator:
   ```bash
   dotnet build -t:Run -f net9.0-android
   ```
4. From the Calendar tab, start creating or editing a feeding entry, choose **Fotó hozzáadása** → **Új fotó készítése**, and grant the camera permission when prompted.

Captured photos are saved to the app's private storage and linked to the feeding entry you are working on.
