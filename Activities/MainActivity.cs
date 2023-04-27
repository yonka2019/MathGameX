using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using MathGame.Models;

namespace MathGame.Activities
{
    [Activity(Label = "@string/app_name",
        Theme = "@style/AppTheme.Splash",
        Icon = "@mipmap/ic_launcher")]
    public class MainActivity : AppCompatActivity
    {
        public static string Username { set; get; }

        private Button Stats, Start, Settings;
        private TextView Login, Register, UsernameTV;
        private ImageButton SetSong;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.main_screen);

            SetRefs();
            SetEvents();
            SetTooltips();

            RestoreAmbientSound();
            RestoreGameSettings();

            if (Username != "")  // not anonymous mode and there is internet (if there is internet lack - the mode automatically will be anonymous
            {
                UsernameTV.Text = Username;

                UsernameTV.Visibility = Android.Views.ViewStates.Visible;
                Login.Visibility = Android.Views.ViewStates.Gone;
                Register.Visibility = Android.Views.ViewStates.Gone;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetRefs()
        {
            Stats = FindViewById<Button>(Resource.Id.main_statsButton);
            Start = FindViewById<Button>(Resource.Id.main_startGameButton);
            Settings = FindViewById<Button>(Resource.Id.main_settingsButton);

            SetSong = FindViewById<ImageButton>(Resource.Id.main_setSongButton);

            Login = FindViewById<TextView>(Resource.Id.main_loginButton);
            Register = FindViewById<TextView>(Resource.Id.main_registerButton);
            UsernameTV = FindViewById<TextView>(Resource.Id.main_username);
        }

        private void SetEvents()
        {
            Stats.Click += Stats_Click;
            Start.Click += Start_Click;
            Settings.Click += Settings_Click;

            SetSong.Click += SetSong_Click;

            Login.Click += Login_Click;
            Register.Click += Register_Click;

            UsernameTV.Click += UsernameTV_Click;
        }

        private void SetTooltips()
        {
            Android.Support.V7.Widget.TooltipCompat.SetTooltipText(SetSong, "Song / ambient sounds configuration");
        }

        private void RestoreGameSettings()
        {
            ISharedPreferences settingsSp = GetSharedPreferences("Settings", FileCreationMode.Private);

            SettingsManager.Settings["operators"]['+'] = settingsSp.GetBoolean("o:+", false);
            SettingsManager.Settings["operators"]['-'] = settingsSp.GetBoolean("o:-", false);
            SettingsManager.Settings["operators"]['*'] = settingsSp.GetBoolean("o:*", false);
            SettingsManager.Settings["operators"]['/'] = settingsSp.GetBoolean("o:/", false);

            SettingsManager.Settings["digits"]['1'] = settingsSp.GetBoolean("d:1", false);
            SettingsManager.Settings["digits"]['2'] = settingsSp.GetBoolean("d:2", false);
            SettingsManager.Settings["digits"]['3'] = settingsSp.GetBoolean("d:3", false);
            SettingsManager.Settings["digits"]['4'] = settingsSp.GetBoolean("d:4", false);
        }

        private void RestoreAmbientSound()
        {
            ISharedPreferences ambientSP = GetSharedPreferences("Ambient", FileCreationMode.Private);

            MediaPlayerAmbient.Volume = ambientSP.GetInt("Volume", 100);  // restore volume setting [ambient sound]
        }

        private void Register_Click(object sender, System.EventArgs e)
        {
            if (SplashActivity.InternetConnection)  // check if there is internet connection
                StartActivity(new Intent(this, typeof(RegisterActivity)));
            else
                this.CreateShowDialog("No internet connection", "Can't register due internet lack", "OK", Resource.Drawable.wifi_off_64px);
        }

        private void Login_Click(object sender, System.EventArgs e)
        {
            if (SplashActivity.InternetConnection)  // check if there is internet connection
                StartActivity(new Intent(this, typeof(LoginActivity)));
            else
                this.CreateShowDialog("No internet connection", "Can't login due internet lack", "OK", Resource.Drawable.wifi_off_64px);
        }

        private void Stats_Click(object sender, System.EventArgs e)
        {
            if (SplashActivity.InternetConnection)  // check if there is internet connection
                StartActivity(new Intent(this, typeof(StatisticsActivity)));
            else
                this.CreateShowDialog("No internet connection", "Can't show statistics table due internet lack", "OK", Resource.Drawable.wifi_off_64px);
        }

        private void Settings_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(SettingsActivity)));
        }

        private void SetSong_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(SoundManagerActivity)));
        }

        private void Start_Click(object sender, System.EventArgs e)
        {
            if (SettingsManager.ReadyToPlay)
            {
                StartActivity(new Intent(this, typeof(GameSelectionActivity)));
            }
            else
            {
                this.CreateShowDialog("Warning", "You must set up the settings before starting game", "OK", Resource.Drawable.warning64);
            }
        }

        private void UsernameTV_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(UserInfoActivity)));
        }

        protected override void OnDestroy()
        {
            ISharedPreferences sp;

            sp = GetSharedPreferences("Music", FileCreationMode.Private);

            if (sp.GetBoolean("Playing", false))  // if running - stop music
                StopService(SoundManagerActivity.MusicServiceIntent);

            ISharedPreferencesEditor editor = sp.Edit();

            editor.PutInt("SongFile", 0);  // reset song file in SP to OFF
            editor.PutBoolean("Playing", false);  // reset playing status in SP to OFF
            editor.Commit();

            base.OnDestroy();
        }

        public override void OnBackPressed()
        {
            // prevent user to get back to splash (sessioned)/login/register after he done
        }
    }
}
