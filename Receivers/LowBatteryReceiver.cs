using Android.App;
using Android.Content;
using Android.OS;
using MathGame.Models;

[BroadcastReceiver(Enabled = true, Label = "Low Battery Receiver")]
[IntentFilter(new[] { Intent.ActionBatteryChanged })]
public class LowBatteryReceiver : BroadcastReceiver
{
    private const int LOW_PERCENT_WARNING = 10;

    public override void OnReceive(Context context, Intent intent)
    {
        int level = intent.GetIntExtra(BatteryManager.ExtraLevel, -1);
        int scale = intent.GetIntExtra(BatteryManager.ExtraScale, -1);

        float batteryPercent = level / (float)scale * 100.0f;

        if (batteryPercent <= LOW_PERCENT_WARNING)
        {
            context.CreateShowDialog("Low Battery", "Your battery is getting low!", "Understand");
        }
    }
}