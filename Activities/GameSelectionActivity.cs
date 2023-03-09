﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MathGame.Models;
using System;

namespace MathGame.Activities
{
    [Activity(Label = "GameSelectionActivity")]
    public class GameSelectionActivity : Activity
    {
        internal static GameDifficulty gameDifficulty;
        private Button infinity, easy, medium, hard, online;

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
            easy = FindViewById<Button>(Resource.Id.gSelection_easy);
            medium = FindViewById<Button>(Resource.Id.gSelection_medium);
            hard = FindViewById<Button>(Resource.Id.gSelection_hard);
            online = FindViewById<Button>(Resource.Id.gSelection_online);
        }
    }
}