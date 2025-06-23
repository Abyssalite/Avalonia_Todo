#if __ANDROID__
    using Android;
    using Android.App;
    using Android.Content.PM;
    using Android.OS;
    using AndroidX.Core.App;
    using AndroidX.Core.Content;
    public class PermissionsService
    {
        public async Task RequestStoragePermissionsAsync()
        {
            var activity = (Activity)Uno.UI.ContextHelper.Current;

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (ContextCompat.CheckSelfPermission(activity, Manifest.Permission.WriteExternalStorage)
                    != (int)Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(activity, new string[]
                    {
                        Manifest.Permission.WriteExternalStorage,
                        Manifest.Permission.ReadExternalStorage
                    }, 1);
                }
            }
        }
    }
#endif
