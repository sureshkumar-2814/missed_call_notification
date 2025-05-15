#if ANDROID
using Android;
using Android.Content.PM;
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Microsoft.Maui.Controls;
using MAUI_App.Platforms.Android;
using MAUI_App.Services;

using Android.Content;
using Microsoft.Maui.Controls.PlatformConfiguration;


namespace MAUI_App;

public partial class MainPage : ContentPage
{
    const int RequestPermissionsCode = 1001;

    public MainPage()
    {
        InitializeComponent();

        // Load saved email if available
        if (Preferences.ContainsKey("RecipientEmail"))
            EmailEntry.Text = Preferences.Get("RecipientEmail", "");
    }

    private void OnStartClicked(object sender, EventArgs e)
    {
        
        string? rawEmail = EmailEntry.Text;
        string email = rawEmail?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(email))
        {
            StatusLabel.Text = "Please enter a recipient email.";
            return;
        }

        Preferences.Set("RecipientEmail", email);

        // Check permissions
        if (!CheckPermissions())
        {
            RequestPermissions();
            return;
        }

        StartTrackingService();
        StatusLabel.Text = "Tracking started.";

        // 🧪 TEST EMAIL IMMEDIATELY
    Task.Run(async () =>
    {
        try
        {
            await GmailHelper.SendEmailAsync(email, "Test from MAUI", "Email sending works!");
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error("Email", "Failed to send test email: " + ex.Message);
        }
    });

    }

    private void OnStopClicked(object sender, EventArgs e)
    {
        StopTrackingService();
        StatusLabel.Text = "Tracking stopped.";
    }

    private bool CheckPermissions()
    {
        var context = Android.App.Application.Context;

        return ContextCompat.CheckSelfPermission(context, Manifest.Permission.ReadCallLog) == Permission.Granted &&
               ContextCompat.CheckSelfPermission(context, Manifest.Permission.ReadPhoneState) == Permission.Granted &&
               ContextCompat.CheckSelfPermission(context, Manifest.Permission.Internet) == Permission.Granted;
    }

    private void RequestPermissions()
    {
        var activity = Platform.CurrentActivity;
        if (activity == null)
        {
            StatusLabel.Text = "Cannot request permissions: Activity is null.";
            return;
        }
        ActivityCompat.RequestPermissions(
            activity,
            new string[]
            {
                Manifest.Permission.ReadCallLog,
                Manifest.Permission.ReadPhoneState,
                Manifest.Permission.Internet
            },
            RequestPermissionsCode
        );
    }

    private void StartTrackingService()
    {


#if ANDROID
        var context = Android.App.Application.Context;
#pragma warning disable CA1416
        var intent = new Intent(context, typeof(MissedCallTrackingService));
        context.StartForegroundService(intent);
#pragma warning restore CA1416
#endif
    }

    private void StopTrackingService()
    {
#if ANDROID
        var context = Android.App.Application.Context;
        var intent = new Intent(context, typeof(MissedCallTrackingService));
        context.StopService(intent);
#endif
    }
}
#endif