using Android.Content;
using Android.Media;

namespace MathGame.Models
{
    // class which represents a media player which can play a sound (ambient sound) for example: correct answer, wrong answer, some sound which is especially short
    public static class MediaPlayerAmbient
    {
        private static float _volume;  // ambient volume

        public static float Volume
        {
            private get => _volume;
            set => _volume = (float)value / 100;
        }

        public static void PlaySound(this MediaPlayer ambientMediaPlayer, string PackageName, Context ApplicationContext, int ambient_SoundID)
        {
            // stop and reset previous plays
            ambientMediaPlayer.Stop();
            ambientMediaPlayer.Reset();

            // set sound source
            int id = ambient_SoundID;
            Android.Net.Uri correctAnswerURI = Android.Net.Uri.Parse("android.resource://" + PackageName + "/" + id);
            ambientMediaPlayer.SetDataSource(ApplicationContext, correctAnswerURI);

            ambientMediaPlayer.Prepare();
            ambientMediaPlayer.SetVolume(_volume, _volume);

            ambientMediaPlayer.Start();
        }
    }
}