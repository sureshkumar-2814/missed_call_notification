using Android.Content;
using Android.Database;
using Android.Provider;
using Android.Util;
using Java.Lang;
using MAUI_App.Services;

namespace MAUI_App.Platforms.Android.Broadcasts
{
    [BroadcastReceiver(Enabled = true)]
    public class CallLogReceiver : BroadcastReceiver
    {
        private static DateTime _lastNotifiedTime = DateTime.MinValue;

        public override void OnReceive(Context? context, Intent? intent)
        {
            Log.Info("CallLog", $"Received PHONE_STATE: {intent?.GetStringExtra("state")}");

            if (context == null || intent == null) return;


            if (intent.Action != "android.intent.action.PHONE_STATE")
                return;

            string? state = intent.GetStringExtra("state");
            if (state != "IDLE") return;

            if (state == "IDLE")
            {
                // Delay slightly to allow log to update
                System.Threading.Thread.Sleep(2000);

                try
                {
                    var uri = CallLog.Calls.ContentUri;
                    var cursor = context.ContentResolver?.Query(
                        CallLog.Calls.ContentUri!,
                        null,
                        $"{CallLog.Calls.Type} = {((int)CallType.Missed)}",
                        null,
                        $"{CallLog.Calls.Date} DESC"
                    );

                    if (cursor != null && cursor.MoveToFirst())
                    {
                        long dateMillis = cursor.GetLong(cursor.GetColumnIndex(CallLog.Calls.Date));
                        DateTime callTime = DateTimeOffset.FromUnixTimeMilliseconds(dateMillis).LocalDateTime;

                        if ((DateTime.Now - callTime).TotalSeconds < 30 && callTime > _lastNotifiedTime)
                        {
                            _lastNotifiedTime = callTime;

                            string? number = cursor.GetString(cursor.GetColumnIndex(CallLog.Calls.Number));
                            if (string.IsNullOrEmpty(number)) return;
                            string timeStr = callTime.ToString("g");

                            // Get email from Preferences
                            string recipient = Preferences.Get("RecipientEmail", "");
                            if (!string.IsNullOrEmpty(recipient))
                            {
                                Task.Run(async () =>
                                {
                                    var subject = "Missed Call Alert";
                                    var body = $"Missed call from: {number}\nTime: {timeStr}";
                                    try
                                    {
                                        await GmailHelper.SendEmailAsync(recipient, subject, body);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        Log.Error("EmailSend", ex.ToString());
                                    }
                                    
                                });
                            }
                        }

                        cursor.Close();
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("CallLogReceiver", "Failed to read call log: " + ex.Message);
                }
            }
        }
    }
}
