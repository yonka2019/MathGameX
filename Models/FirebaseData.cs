using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase;
using Google.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace MathGame.Models
{
    class FirebaseData
    {
        private readonly FirebaseApp app;
        private readonly FirebaseAuth auth;
        public FirebaseData()
        {
            app = FirebaseApp.InitializeApp(Application.Context);
            if (app is null)
            {
                FirebaseOptions options = GetMyOptions();
                app = FirebaseApp.InitializeApp(Application.Context, options);
            }
            auth = FirebaseAuth.Instance;
        }

        private FirebaseOptions GetMyOptions()
        {
            //הערכים בסוגריים לוקחים מקובץ json
            return new FirebaseOptions.Builder()
                .SetProjectId("task6-firebase")
                .SetApplicationId("task6-firebase")
                .SetApiKey("AIzaSyAHEzbE0msQVHZIKNnQxB8jVYp4_W-7Ul4")
                .SetStorageBucket("task6-firebase.appspot.com")
                .Build();
        }

        public Android.Gms.Tasks.Task CreateUser(string email, string password)
        {
            return auth.CreateUserWithEmailAndPassword(email, password);
        }

        public Android.Gms.Tasks.Task SignIn(string email, string password)
        {
            return auth.SignInWithEmailAndPassword(email, password);
        }

        public Android.Gms.Tasks.Task ResetPassword(string email)
        {
            return auth.SendPasswordResetEmail(email);
        }
    }

}