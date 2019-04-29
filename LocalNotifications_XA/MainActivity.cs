using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.V4.App;
using TaskScheduler = Android.Support.V4.App.TaskStackBuilder;
using Android.Content;
using Java.Lang;

// Base on the code in this document:
//    https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/notifications/local-notifications-walkthrough

namespace LocalNotifications_XA
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        static readonly int NOTIFICATION_ID = 1000;
        static readonly string CHANNEL_ID = "location_notification";
        internal static readonly string COUNT_KEY = "count";

        int count = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            CreateNotificationChannel();

            // Display the "Hello World, Click Me!" button and register its event handler:
            var button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += ButtonOnClick;
        }

        private void ButtonOnClick(object sender, System.EventArgs e)
        {
            // Pass the current button press count value to the next activity:
            var valuesForActivity = new Bundle();
            valuesForActivity.PutInt(COUNT_KEY, count);

            // When the user clicks the notification, SecondActivity will start up.
            var resultIntent = new Intent(this, typeof(SecondActivity));

            // Pass some values to SecondActivity:
            resultIntent.PutExtras(valuesForActivity);

            // Construct a back stack for cross-task navigation:
            //var stackBuilder = TaskStackBuilder.Create(this);
            var stackBuilder = TaskScheduler.Create(this);
            stackBuilder.AddParentStack(Class.FromType(typeof(SecondActivity)));
            stackBuilder.AddNextIntent(resultIntent);

            // Create the PendingIntent with the back stack:
            var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            // Build the notification:
            var builder = new NotificationCompat.Builder(this, CHANNEL_ID)
                          .SetAutoCancel(true)                                            // Dismiss the notification from the notification area when the user clicks on it
                          .SetContentIntent(resultPendingIntent)                          // Start up this activity when the user clicks the intent.
                          .SetContentTitle("Button Clicked")                              // Set the title
                          .SetNumber(count)                                               // Display the count in the Content Info
                          .SetSmallIcon(Resource.Drawable.ic_stat_button_click)           // This is the icon to display
                          .SetContentText($"The button has been clicked {count} times."); // the message to display.

            // Finally, publish the notification:
            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(NOTIFICATION_ID, builder.Build());

            // Increment the button press count:
            count++;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        // Create the notification channel
        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var name = Resources.GetString(Resource.String.channel_name);
            var description = GetString(Resource.String.channel_description);
            var channel = new NotificationChannel(CHANNEL_ID, name, NotificationImportance.Default)
            {
                Description = description
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}