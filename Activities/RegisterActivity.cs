using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathGame.Activities
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        private TextView gotoLogin;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register_screen);

            gotoLogin = FindViewById<TextView>(Resource.Id.gotoLogin);
            gotoLogin.Click += GotoLogin_Click;

        }

        private void GotoLogin_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(LoginActivity)));

        }
    }
}