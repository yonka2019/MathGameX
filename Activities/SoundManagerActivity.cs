using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MathGame.Adapters;
using MathGame.Models;
using MathGame.Services;
using System.Collections.Generic;

namespace MathGame.Activities
{
    [Activity(Label = "SoundManagerActivity")]
    public class SoundManagerActivity : Activity, AdapterView.IOnItemClickListener
    {
        public static List<Song> SongList { get; set; }
        public static Intent MusicServiceIntent { get; private set; }

        private SongAdapter songAdapter;
        private Button back;
        private ListView lv;
        private SeekBar musicSB, ambientSB;

        private ISharedPreferences musicSP;
        private ISharedPreferences ambientSP;

        private MediaPlayer mediaPlayer;

        private bool _musicPlaying;

        private bool musicPlaying
        {
            get => _musicPlaying;
            set
            {
                _musicPlaying = value;

                // save into SP
                ISharedPreferencesEditor editor = musicSP.Edit();
                editor.PutBoolean("Playing", _musicPlaying);
                editor.Commit();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.sound_manager_screen);

            SetRefs();
            SetEvents();

            mediaPlayer = new MediaPlayer();

            musicSP = GetSharedPreferences("Music", FileCreationMode.Private);
            ambientSP = GetSharedPreferences("Ambient", FileCreationMode.Private);

            musicPlaying = musicSP.GetBoolean("Playing", false);

            musicSB.Progress = musicSP.GetInt("Volume", 40);  // restore volume setting [music sound]
            ambientSB.Progress = ambientSP.GetInt("Volume", 100);  // restore volume setting [ambient sound]

            MusicServiceIntent = new Intent(this, typeof(MusicService));

            Song song1 = new Song("OFF", default);

            Song song2 = new Song("Glossy Heart", Resource.Raw.Glossy_heart);
            Song song3 = new Song("Give And Take", Resource.Raw.Give_And_Take);
            Song song4 = new Song("Wanderland", Resource.Raw.Wanderland);
            Song song5 = new Song("Silent nights", Resource.Raw.Silent_Nights);

            SongList = new List<Song> { song1, song2, song3, song4, song5 };

            songAdapter = new SongAdapter(this, SongList, GetSongPosition(musicSP.GetInt("SongFile", default)));
            lv.Adapter = songAdapter;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private int GetSongPosition(int songFile)
        {
            return SongList.FindIndex(x => x.FileName == songFile);
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            Song selectedSong = SongList[position];
            Toast.MakeText(this, $"Selected: {selectedSong.Name}", ToastLength.Short).Show();

            ISharedPreferencesEditor editor = musicSP.Edit();
            editor.PutInt("SongFile", selectedSong.FileName);
            editor.Commit();

            songAdapter = new SongAdapter(this, SongList, GetSongPosition(musicSP.GetInt("SongFile", default)));
            lv.Adapter = songAdapter;

            if (selectedSong.Name == "OFF")
            {
                musicPlaying = false;
                StopService(MusicServiceIntent);
            }
            else
            {
                if (musicPlaying)  // if already playing - stop
                    StopService(MusicServiceIntent);

                musicPlaying = true;
                StartService(MusicServiceIntent);
            }
        }

        private void Sb_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (e.FromUser && musicPlaying)
            {
                ISharedPreferencesEditor editor = musicSP.Edit();
                editor.PutInt("Volume", e.Progress);
                editor.Commit();

                MusicService.SetVolume(e.Progress);
            }
        }

        private void AmbientSB_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (e.FromUser)
            {
                ISharedPreferencesEditor editor = ambientSP.Edit();
                editor.PutInt("Volume", e.Progress);
                editor.Commit();

                MediaPlayerAmbient.Volume = e.Progress;

                mediaPlayer.PlaySound(PackageName, ApplicationContext, Resource.Raw.notification);
            }
        }

        private void SetRefs()
        {
            lv = FindViewById<ListView>(Resource.Id.music_lvMusic);
            back = FindViewById<Button>(Resource.Id.music_backButton);
            musicSB = FindViewById<SeekBar>(Resource.Id.music_seekbar);
            ambientSB = FindViewById<SeekBar>(Resource.Id.ambient_seekbar);
        }

        private void SetEvents()
        {
            back.Click += Back_Click;
            musicSB.ProgressChanged += Sb_ProgressChanged;
            ambientSB.ProgressChanged += AmbientSB_ProgressChanged;
            lv.OnItemClickListener = this;
        }

        private void Back_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(MainActivity)));
        }
    }
}