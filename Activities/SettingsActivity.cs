using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;

namespace MathGame
{
    [Activity(Label = "SettingsActivity")]
    public class SettingsActivity : Activity
    {
        CheckBox oneD, doubleD, tripleD, fourthD,
            plus, minus, multiply, divide;
        Button saveExit;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.settings_screen);

            SetRefs();

            saveExit.Click += SaveExit_Click;
            RestoreSettings();
        }

        private void RestoreSettings()
        {
            plus.Checked = SettingsManager.Settings["operators"]["+"];
            minus.Checked = SettingsManager.Settings["operators"]["-"];
            multiply.Checked = SettingsManager.Settings["operators"]["*"];
            divide.Checked = SettingsManager.Settings["operators"]["/"];

            oneD.Checked = SettingsManager.Settings["digits"]["one"];
            doubleD.Checked = SettingsManager.Settings["digits"]["double"];
            tripleD.Checked = SettingsManager.Settings["digits"]["triple"];
            fourthD.Checked = SettingsManager.Settings["digits"]["fourth"];
        }

        private void SaveExit_Click(object sender, EventArgs e)
        {
            // save all settings into the dictionary

            // operators settings
            SettingsManager.Settings["operators"]["+"] = plus.Checked;
            SettingsManager.Settings["operators"]["-"] = minus.Checked;
            SettingsManager.Settings["operators"]["*"] = multiply.Checked;
            SettingsManager.Settings["operators"]["/"] = divide.Checked;

            // digits settings
            SettingsManager.Settings["digits"]["one"] = oneD.Checked;
            SettingsManager.Settings["digits"]["double"] = doubleD.Checked;
            SettingsManager.Settings["digits"]["triple"] = tripleD.Checked;
            SettingsManager.Settings["digits"]["fourth"] = fourthD.Checked;

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
    }
}
