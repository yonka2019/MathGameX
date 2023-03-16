﻿using Android.Content;
using Android.Graphics;
using Android.Views;
using MathGame.Models;
using MathGame.Activities;
using Android.Widget;
using System.Collections.Generic;

namespace MathGame.Adapters
{
    class SongAdapter : BaseAdapter<Song>
    {
        private Context context;
        private List<Song> songs;
        private int selectedPosition;

        public List<Song> GetList()
        {
            return songs;
        }

        public SongAdapter(Context context, List<Song> songs, int selectedPosition)
        {
            this.context = context;
            this.songs = songs;
            this.selectedPosition = selectedPosition;
        }

        public override Song this[int position] => songs[position];

        public override int Count => songs.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater layoutInflater = ((SoundManagerActivity)context).LayoutInflater;
            View view = layoutInflater.Inflate(Resource.Layout.music_layout, parent, false);

            TextView tvSong = view.FindViewById<TextView>(Resource.Id.musicl_musicName);

            if (selectedPosition == position)
                tvSong.SetTypeface(null, TypefaceStyle.Bold);
            else
                tvSong.SetTypeface(null, TypefaceStyle.Normal);


            Song _song = songs[position];

            if (_song != null)
                tvSong.Text = _song.Name;

            return view;
        }
    }
}