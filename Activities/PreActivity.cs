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
        private Button login, register, anon;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.pre_screen);

            SetRefs();
            SetEvents();

            // if there is no internet connection allow only play anonymously (because can not connect to the database)
            if (!base.Intent.GetBooleanExtra("InternetConnection", false))
            {
                login.Enabled = false;
                register.Enabled = false;
            }

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetRefs()
        {
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
    }
}