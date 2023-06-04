using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using MathGame.Models;
using System;
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
        private const int SESSION_MINUTES = 15;
        public static bool InternetConnection { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // check internet connection only on program loading in order to avoid problems with uninitialized database
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;

            InternetConnection = networkInfo != null && networkInfo.IsConnected;

            if (InternetConnection) // check if there is internet connection
                FirebaseManager.Init(this);  // init firebase
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetContentView(Resource.Layout.splash_screen);
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

            Task startupWork = new Task(async () => { await SimulateStartupAsync(); });
            startupWork.Start();
        }

        private async Task SimulateStartupAsync()
        {
            await Task.Delay(1000);

            Intent preActivity = new Intent(Application.Context, typeof(PreActivity));
            preActivity.PutExtra("InternetConnection", InternetConnection);

            (string username, System.DateTime lastLoginDT) = GetLastLoginDT();


            // if there is no internet connection -> must to open up pre-Activity (auto-login not takes in a count) 
            // because user must be logged in as anonymous from pre-activity
            //
            //                      --- OR ---
            //
            // Session (which is 15 minutes) had been expired and the user must to re-log in

            if (!InternetConnection || SessionExpired(lastLoginDT))
                StartActivity(preActivity);

            else  // session isn't expired
            {
                this.AccountSuccessLogin(username);

                RunOnUiThread(() =>  // to avoid 'Can't toast on a thread that has not called Looper.prepare()'
                {
                    Toast.MakeText(this, "Session restored. Welcome back, " + username, ToastLength.Short).Show();
                });
            }
        }

        private (string, System.DateTime) GetLastLoginDT()
        {
            ISharedPreferences loginSessionSP = GetSharedPreferences("LoginSession", FileCreationMode.Private);
            string lastLoginTime = loginSessionSP.GetString("LoginTime", System.DateTime.MinValue.ToString());  // if there is no last session - it will be automatically the lowest value - which will always require to re-log-in (session would be always expired)

            string username = loginSessionSP.GetString("Username", "");

            return (username, System.DateTime.Parse(lastLoginTime));
        }

        /// <summary>
        /// Checks if passed more than 15 minutes -> which means that the session expired and the user should be logged in again
        /// </summary>
        /// <param name="lastLogin">last login datetime</param>
        /// <returns>true if session is expired and user must be re-logged in</returns>
        private bool SessionExpired(System.DateTime lastLogin)
        {
            TimeSpan timeDifference = System.DateTime.Now - lastLogin;
            return timeDifference.TotalMinutes > SESSION_MINUTES;  // if total minutes is higher than 15 -> session expired
        }
    }
}