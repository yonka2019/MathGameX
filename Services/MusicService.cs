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
        private MediaPlayer mp;

        public override IBinder OnBind(Intent intent) { return null; }

        public override void OnCreate()
        {
            base.OnCreate();
            sp = GetSharedPreferences("MusicSP", FileCreationMode.Private);

            mp = MediaPlayer.Create(this, MusicSettings.musicFiles[sp.GetString("song", "")]);
            mp.Looping = true;
            int volume = sp.GetInt("volume", 40);
            mp.SetVolume((float)volume / 100, (float)volume / 100);
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            int position = sp.GetInt("position", 0);
            mp.SeekTo(position);
            mp.Start();
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutInt("position", mp.CurrentPosition);
            editor.Commit();

            mp.Stop();
            mp.Release();
            base.OnDestroy();
        }
    }
}