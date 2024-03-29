﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using MathGame.Models;
using System;

namespace MathGame.Activities
{
    [Activity(Label = "PreActivity")]
    public class PreActivity : Activity
    {
        private TextView welcomeText1;
        private TextView welcomeText2;
        private Button login, register, anon;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.pre_screen);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

            SetRefs();
            SetEvents();

            welcomeText1.Animate(AnimationType.TranslationX, 500, -500f, 0f);
            welcomeText2.Animate(AnimationType.TranslationX, 500, 500f, 0f);

            // if there is no internet connection allow only play anonymously (because can not connect to the database)
            if (Intent != null)
            {
                if (!base.Intent.GetBooleanExtra("InternetConnection", true))
                {
                    login.Enabled = false;
                    register.Enabled = false;
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetRefs()
        {
            welcomeText1 = FindViewById<TextView>(Resource.Id.pre_welcome1);
            welcomeText2 = FindViewById<TextView>(Resource.Id.pre_welcome2);

            login = FindViewById<Button>(Resource.Id.pre_login_button);
            register = FindViewById<Button>(Resource.Id.pre_register_button);
            anon = FindViewById<Button>(Resource.Id.pre_anonymously_button);
        }

        private void SetEvents()
        {
            login.Click += Login_Click;
            register.Click += Register_Click;
            anon.Click += Anon_Click;
        }

        private void Anon_Click(object sender, EventArgs e)
        {
            Intent mainActivity = new Intent(this, typeof(MainActivity));

            MainActivity.Username = "";
            StartActivity(mainActivity);
        }

        private void Login_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(LoginActivity)));
        }

        private void Register_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(RegisterActivity)));
        }

        public override void OnBackPressed()
        {
            // prevent user to get back because he can be after log-out
        }
    }
}