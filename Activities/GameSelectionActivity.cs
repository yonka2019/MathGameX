using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using System;

namespace MathGame.Activities
{
    [Activity(Label = "GameSelectionActivity")]
    public class GameSelectionActivity : Activity
    {
        private Button infinity, easy, medium, hard, online;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.game_selection);

            SetRefs();
            SetEvents();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void Online_Click(object sender, EventArgs e)
        {
            Intent gameActivity = new Intent(this, typeof(GameActivity));
            gameActivity.PutExtra("mode", "4");

            StartActivity(gameActivity);
        }

        private void Hard_Click(object sender, EventArgs e)
        {
            Intent gameActivity = new Intent(this, typeof(GameActivity));
            gameActivity.PutExtra("mode", 3);

            StartActivity(gameActivity);
        }

        private void Medium_Click(object sender, EventArgs e)
        {
            Intent gameActivity = new Intent(this, typeof(GameActivity));
            gameActivity.PutExtra("mode", 2);

            StartActivity(gameActivity);
        }

        private void Easy_Click(object sender, EventArgs e)
        {
            Intent gameActivity = new Intent(this, typeof(GameActivity));
            gameActivity.PutExtra("mode", 1);

            StartActivity(gameActivity);
        }

        private void Infinity_Click(object sender, EventArgs e)
        {
            Intent gameActivity = new Intent(this, typeof(GameActivity));
            gameActivity.PutExtra("mode", 0);

            StartActivity(gameActivity);
        }

        private void SetRefs()
        {
            infinity = FindViewById<Button>(Resource.Id.gSelection_infinity);
            easy = FindViewById<Button>(Resource.Id.gSelection_easy);
            medium = FindViewById<Button>(Resource.Id.gSelection_medium);
            hard = FindViewById<Button>(Resource.Id.gSelection_hard);
            online = FindViewById<Button>(Resource.Id.gSelection_online);
        }

        private void SetEvents()
        {
            infinity.Click += Infinity_Click;
            easy.Click += Easy_Click;
            medium.Click += Medium_Click;
            hard.Click += Hard_Click;
            online.Click += Online_Click;
        }
    }
}