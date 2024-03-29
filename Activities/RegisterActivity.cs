﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Widget;
using MathGame.Models;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MathGame.Activities
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        private TextInputLayout usernametil, passwordtil, confpasswordtil;
        private EditText username, password, confirmPassword;
        private TextView gotoLogin;
        private Button register;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.register_screen);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

            SetRefs();
            SetEvents();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetRefs()
        {
            gotoLogin = FindViewById<TextView>(Resource.Id.register_gotoLogin);
            register = FindViewById<Button>(Resource.Id.register_registerButton);

            username = FindViewById<EditText>(Resource.Id.register_userTB);
            password = FindViewById<EditText>(Resource.Id.register_passTB);
            confirmPassword = FindViewById<EditText>(Resource.Id.register_confPassTB);

            usernametil = FindViewById<TextInputLayout>(Resource.Id.register_usernametil);
            passwordtil = FindViewById<TextInputLayout>(Resource.Id.register_passtil);
            confpasswordtil = FindViewById<TextInputLayout>(Resource.Id.register_confpasstil);
        }

        private void SetEvents()
        {
            gotoLogin.Click += GotoLogin_Click;
            register.Click += Register_Click;
            username.TextChanged += Username_TextChanged;
            password.TextChanged += Password_TextChanged;
            confirmPassword.TextChanged += ConfirmPassword_TextChanged;
        }

        private void ConfirmPassword_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            confpasswordtil.Error = "";
        }

        private void Password_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            passwordtil.Error = "";
        }

        private void Username_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            usernametil.Error = "";
        }

        /// <summary>
        /// Minimum 6 characters
        /// Atleast one uppercase english letter
        /// Atleast one lowercase english letter
        /// Atleast one digit
        /// </summary>
        /// <param name="password">password to check if he strong enough</param>
        /// <returns>true if password strong enough</returns>
        private bool IsStrongPassword(string password)
        {
            Regex strongPasswordRegex = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{6,}$");
            return strongPasswordRegex.IsMatch(password);
        }

        private async Task<bool> UserAlreadyExists(string username)
        {
            return await FirebaseManager.GetGlobalDataAsync(username) != null;  // check if user already exist
        }

        private async void RegisterUser(string username, string password)
        {
            await FirebaseManager.SetGlobalData(username);
            await FirebaseManager.SetLoginData(username, password);
            await FirebaseManager.SetStatsData(username, 0, 0, 0, 0, 0);  // fill zeros to stats
        }

        private async void Register_Click(object sender, EventArgs e)
        {
            if (username.Text == "")
            {
                usernametil.Error = "Username can't be blank";
                return;
            }

            if (password.Text == "")
            {
                passwordtil.Error = "Password can't be blank";
                return;
            }

            if (password.Text != confirmPassword.Text)
            {
                confpasswordtil.Error = "Passwords not same";
                return;
            }

            if (!IsStrongPassword(password.Text))  // check if password strong enough
            {
                passwordtil.Error = "Weak password (minimum 6 character & atleast one uppercase & atleast one lowercase & one digit)";
                return;
            }

            ProgressDialog signUpDialog = this.CreateProgressDialog("Signing up..");

            if (!await UserAlreadyExists(username.Text))  // not exist -> create user and auto login
            {
                RegisterUser(username.Text, password.Text);

                this.AccountSuccessLogin(username.Text);

                Toast.MakeText(this, "Welcome " + username.Text, ToastLength.Short).Show();
            }
            else  // already exists
            {
                usernametil.Error = "User Already Exists";
            }
            signUpDialog.Dismiss();
        }

        private void GotoLogin_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(LoginActivity)));
        }
    }
}