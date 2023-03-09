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
            sp = GetSharedPreferences("Music", FileCreationMode.Private);

            mp = MediaPlayer.Create(this, sp.GetInt("SongFile", 0));

            mp.Looping = true;
            int volume = sp.GetInt("Volume", 40);
            mp.SetVolume((float)volume / 100, (float)volume / 100);
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            int position = sp.GetInt("Position", 0);
            mp.SeekTo(position);
            mp.Start();
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutInt("Position", mp.CurrentPosition);  // save position status
            editor.Commit();

            mp.Stop();
            mp.Release();
            base.OnDestroy();
        }
    }
}