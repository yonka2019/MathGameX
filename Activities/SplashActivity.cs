using Android.App;
using Android.Content;
using Android.OS;
using System.Threading.Tasks;

namespace MathGame.Activities
{
    [Activity(Label = "@string/app_name",
            MainLauncher = true,
            Theme = "@style/AppTheme.Splash",
            NoHistory = true,
            Icon = "@mipmap/ic_launcher")]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetContentView(Resource.Layout.splash_screen);
            Task startupWork = new Task(() => { SimulateStartupAsync(); });
            startupWork.Start();
        }

        private async void SimulateStartupAsync()
        {
            await Task.Delay(1000);

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}