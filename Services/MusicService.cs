using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;

namespace MathGame.Services
{
    [Service]
    public class MusicService : Service
    {
        private ISharedPreferences sp;
        private static MediaPlayer mp;

        public static void SetVolume(float newVolume)
        {
            if (mp != null)
                mp.SetVolume(newVolume / 100, newVolume / 100);
        }

        public override IBinder OnBind(Intent intent) { return null; }

        public override void OnCreate()
        {
            base.OnCreate();

            sp = GetSharedPreferences("Music", FileCreationMode.Private);

            mp = MediaPlayer.Create(this, sp.GetInt("SongFile", 0));

            mp.Looping = true;

            int volume = sp.GetInt("Volume", 40);
            SetVolume(volume);
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            mp.Start();

            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            mp.Stop();
            mp.Release();

            base.OnDestroy();
        }
    }
}