using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MAUI_App
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
#pragma warning disable CA1416
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
#pragma warning restore CA1416
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }

}
