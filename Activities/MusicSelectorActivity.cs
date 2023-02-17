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
        public static List<Song> SongList { get; set; }

        private MusicAdapter songAdapter;
        private Button back;
        private ListView lv;
        private SeekBar sb;
        private ISharedPreferences sp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.music_select);

            SetRefs();

            Song song1 = new Song("Song1");
            Song song2 = new Song("Song2");
            Song song3 = new Song("Song3");

            SongList = new List<Song> { song1, song2, song3 };

            songAdapter = new MusicAdapter(this, SongList);

            back.Click += Back_Click;
            lv.Adapter = songAdapter;
            lv.OnItemClickListener = this;
        }

        private void SetRefs()
        {
            lv = FindViewById<ListView>(Resource.Id.music_lvMusic);
            back = FindViewById<Button>(Resource.Id.music_backButton);
            sb = FindViewById<SeekBar>(Resource.Id.music_seekbar);
        }

        private void Back_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(MainActivity)));
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            Song selectedSong = SongList[position];
            Toast.MakeText(this, $"Selected: {selectedSong.Name}", ToastLength.Short).Show();
        }
    }
}