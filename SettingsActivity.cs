﻿using Android.App;
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

        }

        private void SaveExit_Click(object sender, EventArgs e)
        {
            // save all settings into the dictionary

            // operators settings
            SettingsManager.Settings["operators"]["plus"] = plus.Checked;
            SettingsManager.Settings["operators"]["minus"] = minus.Checked;
            SettingsManager.Settings["operators"]["multiply"] = multiply.Checked;
            SettingsManager.Settings["operators"]["divide"] = divide.Checked;

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
            oneD = FindViewById<CheckBox>(Resource.Id.setting_single_digit);
            doubleD = FindViewById<CheckBox>(Resource.Id.setting_double_digit);
            tripleD = FindViewById<CheckBox>(Resource.Id.setting_triple_digit);
            fourthD = FindViewById<CheckBox>(Resource.Id.setting_fourth_digit);

            plus = FindViewById<CheckBox>(Resource.Id.setting_plus);
            minus = FindViewById<CheckBox>(Resource.Id.setting_minus);
            multiply = FindViewById<CheckBox>(Resource.Id.setting_multiply);
            divide = FindViewById<CheckBox>(Resource.Id.setting_divide);

            saveExit = FindViewById<Button>(Resource.Id.SaveExitButton);
        }
    }
}