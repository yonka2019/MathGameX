using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MathGame.Adapters;
using System.Collections.Generic;

namespace MathGame
{
    [Activity(Label = "MusicSelectorActivity")]
    public class MusicSelectorActivity : Activity, AdapterView.IOnItemClickListener
    {
        public static List<Music> SongList { get; set; }

        private MusicAdapter songAdapter;
        private Button back;
        private ListView lv;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.music_select);

            Music song1 = new Music("Song1");
            Music song2 = new Music("Song2");
            Music song3 = new Music("Song3");
            SongList = new List<Music> { song1, song2, song3 };

            songAdapter = new MusicAdapter(this, SongList);

            lv = FindViewById<ListView>(Resource.Id.lvMusic);
            back = FindViewById<Button>(Resource.Id.backButton);

            back.Click += Back_Click;
            lv.Adapter = songAdapter;
            lv.OnItemClickListener = this;
        }

        private void Back_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(MainActivity)));
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            Music selectedSong = SongList[position];
            Toast.MakeText(this, $"Selected: {selectedSong.Name}", ToastLength.Short).Show();
        }
    }
}