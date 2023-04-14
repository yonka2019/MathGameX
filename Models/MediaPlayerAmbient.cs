using Android.Content;
using Android.Media;

namespace MathGame.Models
{
    // class which represents a media player which can play a sound (ambient sound) for example: correct answer, wrong answer, some sound which is especially short
    public static class MediaPlayerAmbient
    {
        private static readonly MediaPlayer mediaPlayer;
        private static float _volume;  // ambient volume

        static MediaPlayerAmbient()
        {
            mediaPlayer = new MediaPlayer();
        }

        public static float Volume
        {
            private get => _volume;
            set => _volume = (float)value / 100;
        }

        /// <summary>
        /// Plays a sound on ambient media player (which have his own volume which can be configured on songs screen)
        /// </summary>
        /// <param name="PackageName">if you in activity, just write 'PackageName', it's an base variable</param>
        /// <param name="ApplicationContext">if you in activity, just write 'ApplicationContext', it's an base variable</param>
        /// <param name="ambient_SoundID">Resourse.Raw.[file_name]</param>
        public static void PlaySound(string PackageName, Context ApplicationContext, int ambient_SoundID)
        {
            // stop and reset previous plays
            mediaPlayer.Stop();
            mediaPlayer.Reset();

            // set sound source
            int id = ambient_SoundID;
            Android.Net.Uri correctAnswerURI = Android.Net.Uri.Parse("android.resource://" + PackageName + "/" + id);
            mediaPlayer.SetDataSource(ApplicationContext, correctAnswerURI);

            mediaPlayer.Prepare();
            mediaPlayer.SetVolume(_volume, _volume);

            mediaPlayer.Start();
        }
    }
}