using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using TaskScheduler = Android.Support.V4.App.TaskStackBuilder;
using Java.Lang;

namespace LocalNotifications_XA
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var message = intent.GetStringExtra("message");
            var title = intent.GetStringExtra("title");

            // Pass the current button press count value to the next activity:
            var valuesForActivity = new Bundle();
            valuesForActivity.PutInt(MainActivity.COUNT_KEY, MainActivity.count);

            // When the user clicks the notification, SecondActivity will start up.
            var resultIntent = new Intent(context, typeof(SecondActivity));

            // Pass some values to SecondActivity:
            resultIntent.PutExtras(valuesForActivity);

            // Construct a back stack for cross-task navigation:
            //var stackBuilder = TaskStackBuilder.Create(this);
            var stackBuilder = TaskScheduler.Create(context);
            stackBuilder.AddParentStack(Class.FromType(typeof(SecondActivity)));
            stackBuilder.AddNextIntent(resultIntent);

            // Create the PendingIntent with the back stack:
            var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            // Build the notification:
            var notificationBuilder = new NotificationCompat.Builder(context, MainActivity.CHANNEL_ID)
                          .SetAutoCancel(true)                                   // Dismiss the notification from the notification area when the user clicks on it
                          .SetContentIntent(resultPendingIntent)                 // Start up this activity when the user clicks the intent.
                          .SetContentTitle(title)                                // Set the title
                          .SetNumber(MainActivity.count)                         // Display the count in the Content Info
                          .SetSmallIcon(Resource.Drawable.ic_stat_button_click)  // This is the icon to display
                          .SetContentText(message);                              // the message to display.

            // Finally, publish the notification:
            var notificationManager = NotificationManagerCompat.From(context);
            notificationManager.Notify(MainActivity.NOTIFICATION_ID, notificationBuilder.Build());
        }
    }
}