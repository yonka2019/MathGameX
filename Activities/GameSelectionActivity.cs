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
    [Activity(Label = "GameSelectionActivity")]
    public class GameSelectionActivity : Activity
    {
        private Button infinity, level, online;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.game_selection);
            SetRefs();

            infinity.Click += Infinity_Click;
        }

        private void Infinity_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(GameActivity)));
        }

        private void SetRefs()
        {
            infinity = FindViewById<Button>(Resource.Id.gSelection_infinity);
            level = FindViewById<Button>(Resource.Id.gSelection_level);
            online = FindViewById<Button>(Resource.Id.gSelection_online);
        }
    }
}