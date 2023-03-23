using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Firebase;
using Java.Util;
using MathGame.Models;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MathGame.Activities
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        private EditText username, password, confirmPassword;
        private TextView gotoLogin;
        private Button register;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.register_screen);

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
        }

        private void SetEvents()
        {
            gotoLogin.Click += GotoLogin_Click;
            register.Click += Register_Click;
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

        private void RegisterUser(string username, string password)
        {
            FirebaseManager.SetGlobalData(username);
            FirebaseManager.SetLoginData(username, password);
            FirebaseManager.SetStatsData(username, 0, 0, 0, 0);  // fill zeros to stats
        }

        private async void Register_Click(object sender, EventArgs e)
        {
            if (username.Text == "")
            {
                username.Error = "Username can't be blank";
                return;
            }

            if (password.Text == "")
            {
                password.Error = "Password can't be blank";
                return;
            }

            if (password.Text != confirmPassword.Text)
            {
                confirmPassword.Error = "Not the same";
                return;
            }

            if (!IsStrongPassword(password.Text))  // check if password strong enough
            {
                password.Error = "Weak password";
                return;
            }


            if (!await UserAlreadyExists(username.Text))  // not exist -> create user and auto login
            {
                RegisterUser(username.Text, password.Text);

                this.Login(username.Text);
            }
            else  // already exists
            {
                username.Error = "Already Exists";
            }
        }

        private void GotoLogin_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(LoginActivity)));
        }
    }
}