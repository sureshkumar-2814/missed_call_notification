using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using MAUI_App.Platforms.Android.Broadcasts;

namespace MAUI_App.Platforms.Android
{
    [Service(ForegroundServiceType = ForegroundService.TypeDataSync)]

    public class MissedCallTrackingService : Service
    {
        private CallLogReceiver? _receiver;
        private bool _receiverRegistered = false;


        public override void OnCreate()
        {
            base.OnCreate();

            _receiver = new CallLogReceiver();
            var filter = new IntentFilter("android.intent.action.PHONE_STATE");
            RegisterReceiver(_receiver, filter);
            _receiverRegistered = true;
        }

        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            ShowNotification();
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (_receiverRegistered && _receiver != null)
            {
                UnregisterReceiver(_receiver);
                _receiverRegistered = false;
            }
        }


        private void ShowNotification()
        {
            string channelId = "missed_call_tracking_channel";
            string channelName = "Missed Call Tracking";


            var service = GetSystemService(NotificationService);
            if (service is not NotificationManager notificationManager)
                return;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.N) // API 24+
            {
#pragma warning disable CA1416
                var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Default);
                notificationManager.CreateNotificationChannel(channel);
#pragma warning restore CA1416
            }

            var notification = new NotificationCompat.Builder(this, channelId)
                .SetContentTitle("Tracking Missed Calls")
                .SetContentText("The app is monitoring missed calls.")
                .SetSmallIcon(Resource.Mipmap.appicon) // default icon
                .SetOngoing(true)
                .Build();

            StartForeground(1001, notification);
        }
    }
}
