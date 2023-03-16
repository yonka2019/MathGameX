using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using MathGame.Activities;
using MathGame.Models;

namespace MathGame
{
    [Activity(Label = "@string/app_name",
        Theme = "@style/AppTheme.Splash",
        Icon = "@mipmap/ic_launcher")]
    public class MainActivity : AppCompatActivity
    {
        private Button Stats, Start, Settings;
        private TextView Login, Register;
        private ImageButton SetSong;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main_screen);

            SetRefs();
            SetEvents();
            RestoreAmbientSound();
            RestoreGameSettings();
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

            MediaPlayerSound.Volume = ambientSP.GetInt("Volume", 100);  // restore volume setting [ambient sound]
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
                ShowAlertDialog("Warning", "You must set up the settings before starting game", Resource.Drawable.warning64);
            }
        }

        /// <summary>
        /// Shortcuter to show alert dialog in THIS context
        /// </summary>
        private void ShowAlertDialog(string title, string message, int iconId)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetPositiveButton("OK", delegate { });
            builder.SetIcon(iconId);
            Android.App.AlertDialog dialog = builder.Create();

            dialog.Show();
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

        private void SetEvents()
        {
            Stats.Click += Stats_Click;
            Start.Click += Start_Click;
            Settings.Click += Settings_Click;
            SetSong.Click += SetSong_Click;
            Login.Click += Login_Click;
            Register.Click += Register_Click;
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
    }
}
