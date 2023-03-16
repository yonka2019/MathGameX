using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.Activity;
using System;
using MathGame.Models;

namespace MathGame.Activities
{
    [Activity(Label = "SettingsActivity")]
    public class SettingsActivity : Activity
    {
        private CheckBox oneD, doubleD, tripleD, fourthD,
            plus, minus, multiply, divide;
        private Button saveExit;

        private ISharedPreferences settingsSP;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.settings_screen);

            SetRefs();
            SetEvents();

            settingsSP = GetSharedPreferences("Settings", FileCreationMode.Private);

            RestoreSettingsView();  // settings manager already restored in main activity (on program startup)
        }

        private void RestoreSettingsView()
        {
            plus.Checked = SettingsManager.Settings["operators"]['+'];
            minus.Checked = SettingsManager.Settings["operators"]['-'];
            multiply.Checked = SettingsManager.Settings["operators"]['*'];
            divide.Checked = SettingsManager.Settings["operators"]['/'];

            oneD.Checked = SettingsManager.Settings["digits"]['1'];
            doubleD.Checked = SettingsManager.Settings["digits"]['2'];
            tripleD.Checked = SettingsManager.Settings["digits"]['3'];
            fourthD.Checked = SettingsManager.Settings["digits"]['4'];
        }

        private void SaveAndExit_Click(object sender, EventArgs e)
        {
            ISharedPreferencesEditor editor = settingsSP.Edit();

            // save all settings into the dictionary

            // operators settings [local]
            SettingsManager.Settings["operators"]['+'] = plus.Checked;
            SettingsManager.Settings["operators"]['-'] = minus.Checked;
            SettingsManager.Settings["operators"]['*'] = multiply.Checked;
            SettingsManager.Settings["operators"]['/'] = divide.Checked;

            // digits settings [local]
            SettingsManager.Settings["digits"]['1'] = oneD.Checked;
            SettingsManager.Settings["digits"]['2'] = doubleD.Checked;
            SettingsManager.Settings["digits"]['3'] = tripleD.Checked;
            SettingsManager.Settings["digits"]['4'] = fourthD.Checked;

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

        [Obsolete]
        public override void OnBackPressed()
        {
            saveExit.PerformClick();  // simulate save and exit click

            base.OnBackPressed();
        }
    }
}
