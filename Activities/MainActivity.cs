using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using MathGame.Activities;

namespace MathGame
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button Stats, Start, Settings;
        private TextView Login, Register;
        private ImageButton SetSong;

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
            SetSong.Click += SetSong_Click;
            Login.Click += Login_Click;
            Register.Click += Register_Click;
        }

        private void Register_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(RegisterActivity)));
        }

        private void Login_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(LoginActivity)));

        }

        private void Stats_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(StatisticsActivity)));
        }

        private void Settings_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(SettingsActivity)));
        }

        private void SetSong_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(MusicSelectorActivity)));
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

        private void SetRefs()
        {
            Stats = FindViewById<Button>(Resource.Id.main_statsButton);
            Start = FindViewById<Button>(Resource.Id.main_startGameButton);
            Settings = FindViewById<Button>(Resource.Id.main_settingsButton);
            SetSong = FindViewById<ImageButton>(Resource.Id.main_setSongButton);
            Login = FindViewById<TextView>(Resource.Id.main_loginButton);
            Register = FindViewById<TextView>(Resource.Id.main_registerButton);
        }
    }
}
