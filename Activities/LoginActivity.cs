    using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;

namespace MathGame.Activities
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        private TextView gotoRegister;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login_screen);

            gotoRegister = FindViewById<TextView>(Resource.Id.login_gotoRegister);
            gotoRegister.Click += GotoRegister_Click;

        }

        private void GotoRegister_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(RegisterActivity)));
        }
    }
}