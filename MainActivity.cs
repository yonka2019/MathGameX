using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using AndroidX.AppCompat.App;

namespace MathGame
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button Stats, Start, Settings;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource

            SetContentView(Resource.Layout.main_screen);
            SetRefs();

            Stats.Click += Stats_Click;
            Start.Click += Start_Click;
            Settings.Click += Settings_Click;
        }

        private void Settings_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(SettingsActivity)));
        }

        private void Start_Click(object sender, System.EventArgs e)
        {
            if (SettingsManager.ReadyToPlay)
            {
                StartActivity(new Intent(this, typeof(GameActivity)));
            }
            else
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                builder.SetTitle("Warning");
                builder.SetMessage("You must set up the settings before starting game");
                builder.SetPositiveButton("OK", delegate { });
                builder.SetIcon(Resource.Drawable.warning64);
                Android.App.AlertDialog dialog = builder.Create();
                dialog.Show();
            }
        }

        private void Stats_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void SetRefs()
        {
            Stats = FindViewById<Button>(Resource.Id.statsButton);
            Start = FindViewById<Button>(Resource.Id.startGameButton);
            Settings = FindViewById<Button>(Resource.Id.settingsButton);
        }
    }
}
