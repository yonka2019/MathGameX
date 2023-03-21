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
    [Activity(Label = "UserInfoActivity")]
    public class UserInfoActivity : Activity
    {
        Button backToMenu;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.user_info_screen);
            SetRefs();
            SetEvents();
        }

        private void SetRefs()
        {

        }

        private void SetEvents()
        {

        }
    }
}