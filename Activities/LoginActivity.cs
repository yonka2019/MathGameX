using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Google.Android.Material.TextField;
using MathGame.Models;
using System;
using System.Threading.Tasks;

namespace MathGame.Activities
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        private EditText username, password;
        private TextInputLayout usernameTIL, passTIL;
        private TextView gotoRegister;
        private Button login;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login_screen);

            SetRefs();
            SetEvents();
        }
        private void SetRefs()
        {
            gotoRegister = FindViewById<TextView>(Resource.Id.login_gotoRegister);
            login = FindViewById<Button>(Resource.Id.login_loginButton);

            username = FindViewById<EditText>(Resource.Id.login_userTB);
            password = FindViewById<EditText>(Resource.Id.login_passTB);

            usernameTIL = FindViewById<TextInputLayout>(Resource.Id.login_usernametil);
            passTIL = FindViewById<TextInputLayout>(Resource.Id.login_passtil);

        }

        private void SetEvents()
        {
            gotoRegister.Click += GotoRegister_Click;
            login.Click += Login_Click;
        }

        private async Task<bool> PasswordCorrect(string username, string password)
        {
            System.Collections.Generic.Dictionary<string, object> loginData = await FirebaseManager.GetLoginDataAsync(username);

            if (loginData == null)
                return false;
            else
                return password.GetMD5() == loginData["Password"].ToString();  // compare between already hashed password in db, and entered password hashed
        }

        private async void Login_Click(object sender, EventArgs e)
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

            if (await PasswordCorrect(username.Text, password.Text))  // if password matches => successfully logged in
            {
                this.Login(username.Text);
            }
            else
            {
                password.Error = "Wrong username or password";
            }
        }

        private void GotoRegister_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(RegisterActivity)));
        }
    }
}