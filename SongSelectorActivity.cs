using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MathGame.Adapters;
using System.Collections.Generic;

namespace MathGame
{
    [Activity(Label = "MusicSelectorActivity")]
    public class SongSelectorActivity : Activity, AdapterView.IOnItemClickListener
    {
        public static List<Song> songList { get; set; }

        private SongAdapter songAdapter;
        private ListView lv;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.song_layout);

            Song song1 = new Song("Song1");
            Song song2 = new Song("Song2");
            Song song3 = new Song("Song3");
            songList = new List<Song> { song1, song2, song3 };

            songAdapter = new SongAdapter(this, songList);
            lv = FindViewById<ListView>(Resource.Id.lvSong);
            lv.Adapter = songAdapter;

            lv.OnItemClickListener = this;



        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            Song selectedSong = songList[position];
            Toast.MakeText(this, $"Selected: {selectedSong.Name}", ToastLength.Short).Show();
        }
    }
}