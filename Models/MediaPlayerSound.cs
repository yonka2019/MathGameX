using Android.Content;
using Android.Media;

namespace MathGame.Models
{
    // class which represents a media player which can play a sound (ambient sound) for example: correct answer, wrong answer, some sound which is especially short
    public static class MediaPlayerSound
    {
        private static float _volume;

        public static float Volume
        {
            private get => _volume;
            set => _volume = (float)value / 100;
        }

        public static void PlaySound(this MediaPlayer mediaPlayer, string PackageName, Context ApplicationContext, int soundId)
        {
            // stop and reset previous plays
            mediaPlayer.Stop();
            mediaPlayer.Reset();

            // set sound source
            int id = soundId;
            Android.Net.Uri correctAnswerURI = Android.Net.Uri.Parse("android.resource://" + PackageName + "/" + id);
            mediaPlayer.SetDataSource(ApplicationContext, correctAnswerURI);

            mediaPlayer.Prepare();
            mediaPlayer.SetVolume(_volume, _volume);

            mediaPlayer.Start();
        }
    }
}