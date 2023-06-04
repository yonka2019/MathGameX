using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using MathGame.Models;
using System;

namespace MathGame.Activities
{
    [Activity(Label = "SettingsActivity")]
    public class GameSettingsActivity : Activity
    {
        private CheckBox oneD, doubleD, tripleD, fourthD,
            plus, minus, multiply, divide;
        private Button saveExit;

        private ISharedPreferences settingsSP;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.game_settings_screen);

            SetRefs();
            SetEvents();

            settingsSP = GetSharedPreferences("Settings", FileCreationMode.Private);

            RestoreSettingsView();  // settings manager already restored in main activity (on program startup)
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetRefs()
        {
            oneD = FindViewById<CheckBox>(Resource.Id.setting_singleDigit);
            doubleD = FindViewById<CheckBox>(Resource.Id.setting_doubleDigit);
            tripleD = FindViewById<CheckBox>(Resource.Id.setting_tripleDigit);
            fourthD = FindViewById<CheckBox>(Resource.Id.setting_fourthDigit);

            plus = FindViewById<CheckBox>(Resource.Id.setting_plus);
            minus = FindViewById<CheckBox>(Resource.Id.setting_minus);
            multiply = FindViewById<CheckBox>(Resource.Id.setting_multiply);
            divide = FindViewById<CheckBox>(Resource.Id.setting_divide);

            saveExit = FindViewById<Button>(Resource.Id.settings_saveAndExitButton);
        }

        private void SetEvents()
        {
            saveExit.Click += SaveAndExit_Click;
        }

        private void RestoreSettingsView()
        {
            plus.Checked = GameSettingsManager.Settings["operators"]['+'];
            minus.Checked = GameSettingsManager.Settings["operators"]['-'];
            multiply.Checked = GameSettingsManager.Settings["operators"]['*'];
            divide.Checked = GameSettingsManager.Settings["operators"]['/'];

            oneD.Checked = GameSettingsManager.Settings["digits"]['1'];
            doubleD.Checked = GameSettingsManager.Settings["digits"]['2'];
            tripleD.Checked = GameSettingsManager.Settings["digits"]['3'];
            fourthD.Checked = GameSettingsManager.Settings["digits"]['4'];
        }

        private void SaveAndExit_Click(object sender, EventArgs e)
        {
            ISharedPreferencesEditor editor = settingsSP.Edit();

            // save all settings into the dictionary

            // operators settings [local]
            GameSettingsManager.Settings["operators"]['+'] = plus.Checked;
            GameSettingsManager.Settings["operators"]['-'] = minus.Checked;
            GameSettingsManager.Settings["operators"]['*'] = multiply.Checked;
            GameSettingsManager.Settings["operators"]['/'] = divide.Checked;

            // digits settings [local]
            GameSettingsManager.Settings["digits"]['1'] = oneD.Checked;
            GameSettingsManager.Settings["digits"]['2'] = doubleD.Checked;
            GameSettingsManager.Settings["digits"]['3'] = tripleD.Checked;
            GameSettingsManager.Settings["digits"]['4'] = fourthD.Checked;

            // operators settings [SP]
            editor.PutBoolean("o:+", plus.Checked);
            editor.PutBoolean("o:-", minus.Checked);
            editor.PutBoolean("o:*", multiply.Checked);
            editor.PutBoolean("o:/", divide.Checked);

            // digits settings [SP]
            editor.PutBoolean("d:1", oneD.Checked);
            editor.PutBoolean("d:2", doubleD.Checked);
            editor.PutBoolean("d:3", tripleD.Checked);
            editor.PutBoolean("d:4", fourthD.Checked);

            editor.Commit();

            // back to main screen
            StartActivity(new Intent(this, typeof(MainActivity)));
        }

        [Obsolete]
        public override void OnBackPressed()
        {
            saveExit.PerformClick();  // simulate save and exit click

            base.OnBackPressed();
        }
    }
}
