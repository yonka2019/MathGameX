using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MathGame.Adapters;
using MathGame.Services;
using System.Collections.Generic;

namespace MathGame
{
    [Activity(Label = "MusicSelectorActivity")]
    public class MusicSelectorActivity : Activity, AdapterView.IOnItemClickListener
    {
        public static List<Song> SongList { get; set; }

        private SongAdapter songAdapter;
        private Button back;
        private ListView lv;
        private SeekBar sb;
        private Intent musicService;
        private ISharedPreferences sp;
        private bool musicPlaying;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.music_select);

            SetRefs();

            sp = GetSharedPreferences("Music", FileCreationMode.Private);

            musicPlaying = sp.GetBoolean("Playing", false);

            musicService = new Intent(this, typeof(MusicService));

            Song song1 = new Song("OFF", default);
            Song song2 = new Song("another", Resource.Raw.another_one_bites_the_dust);
            Song song3 = new Song("my", Resource.Raw.my_song);

            SongList = new List<Song> { song1, song2, song3 };

            songAdapter = new SongAdapter(this, SongList, GetSongPosition(sp.GetInt("SongFile", default)));
            lv.Adapter = songAdapter;

            back.Click += Back_Click;
            sb.ProgressChanged += Sb_ProgressChanged;
            lv.OnItemClickListener = this;
        }

        private int GetSongPosition(int songFile)
        {
            return SongList.FindIndex(x => x.FileName == songFile);
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            Song selectedSong = SongList[position];
            Toast.MakeText(this, $"Selected: {selectedSong.Name}", ToastLength.Short).Show();

            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutInt("SongFile", selectedSong.FileName);
            editor.Commit();

            songAdapter = new SongAdapter(this, SongList, GetSongPosition(sp.GetInt("SongFile", default)));
            lv.Adapter = songAdapter;

            if (selectedSong.Name == "OFF")
            {
                musicPlaying = false;

                StopService(musicService);
            }
            else
            {
                if (musicPlaying)  // if already playing - stop
                    StopService(musicService);

                musicPlaying = true;
                StartService(musicService);

            }
        }

        private void Sb_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (e.FromUser)
            {
                ISharedPreferencesEditor editor = sp.Edit();
                editor.PutInt("Volume", e.Progress);
                editor.Commit();

                if (musicPlaying)
                {
                    StopService(musicService);
                    StartService(musicService);
                }

            }
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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnDestroy()
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutBoolean("Playing", false);
            editor.Commit();

            base.OnDestroy();
        }

    }
}