using Android.Content;
using Android.Media;

namespace MathGame.Models
{
    public static class MethodExt
    {
        public static void PlaySound(this MediaPlayer mediaPlayer, string PackageName, Context ApplicationContext, int soundId)
        {
            mediaPlayer.Stop();
            mediaPlayer.Reset();

            int id = soundId;
            Android.Net.Uri correctAnswerURI = Android.Net.Uri.Parse("android.resource://" + PackageName + "/" + id);

            mediaPlayer.SetDataSource(ApplicationContext, correctAnswerURI);
            mediaPlayer.Prepare();
            mediaPlayer.Start();
        }
    }
}