using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using MathGame.Models;
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
        public static bool InternetConnection { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // check internet connection only on program loading in order to avoid problems with uninitialized database
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;

            InternetConnection = networkInfo != null && networkInfo.IsConnected;

            if (InternetConnection) // check if there is internet connection
                FirebaseManager.Init(this);  // init firebase
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
            // CHECK IF ALRERADY LOGGED

            Intent preActivity = new Intent(Application.Context, typeof(PreActivity));
            preActivity.PutExtra("InternetConnection", InternetConnection);

            StartActivity(preActivity);
        }
    }
}