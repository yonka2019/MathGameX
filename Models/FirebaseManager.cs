
using Android.Content;
using Android.Gms.Extensions;
using Firebase;
using Firebase.Firestore;
using Java.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathGame.Models
{
    public static class FirebaseManager
    {
        private const string SETTINGS_PROJECT_ID = "mathgame-x";
        private const string SETTINGS_APPLICATION_ID = "1:909793803468:android:7052c15697ed01fae55669";
        private const string SETTINGS_API_KEY = "AIzaSyDqJoZcLu-IVCkp-_ULJLIHAnNFiluL0cs";
        private const string SETTINGS_STORAGE_BUCKET = "mathgame-x.appspot.com";


        private const string MainCollection = "MainDB";
        private const string MainDocument = "Users";

        private const string LoginDocument = "LoginData";
        private const string StatisticsDocument = "Statistics";

        private static FirebaseFirestore database;
        private static DocumentReference baseReference;

        public static void Init(Context context)
        {
            FirebaseApp app = FirebaseApp.InitializeApp(context);
            if (app == null)
            {
                FirebaseOptions options = new FirebaseOptions.Builder()
                    .SetProjectId(SETTINGS_PROJECT_ID)
                    .SetApplicationId(SETTINGS_APPLICATION_ID)
                    .SetApiKey(SETTINGS_API_KEY)
                    .SetStorageBucket(SETTINGS_STORAGE_BUCKET)
                    .Build();

                app = FirebaseApp.InitializeApp(context, options);
                database = FirebaseFirestore.GetInstance(app);
            }
            else
            {
                database = FirebaseFirestore.GetInstance(app);
            }

            // get reference to base data 'MainDB / Users / [USERNAME] / [STAT|LOGIN]'
            baseReference = database.Collection(MainCollection).Document(MainDocument);
        }

        public static async Task<Dictionary<string, object>> GetLoginDataAsync(string username)
        {
            return await GetDataAsync(username, LoginDocument);
        }

        public static async Task<Dictionary<string, object>> GetStatsDataAsync(string username)
        {
            return await GetDataAsync(username, StatisticsDocument);
        }

        public static void SetLoginData(string username, string password)
        {
            HashMap newLoginData = new HashMap();

            newLoginData.Put("Password", password.GetMD5());  // hash password
            newLoginData.Put("CreatedOn", new Timestamp(new Date()));

            SetData(username, LoginDocument, newLoginData);
        }

        public static void SetStatsData(string username, int plus, int minus, int multiply, int divide)
        {
            HashMap newStatisticsData = new HashMap();

            newStatisticsData.Put("Plus", plus);
            newStatisticsData.Put("Minus", minus);
            newStatisticsData.Put("Multiply", multiply);
            newStatisticsData.Put("Divide", divide);

            SetData(username, StatisticsDocument, newStatisticsData);
        }

        #region Help Functions
        private static void SetData(string collection, string document, HashMap data)
        {
            DocumentReference documentRef = baseReference.Collection(collection).Document(document);
            documentRef.Set(data);
        }

        /// <summary>
        /// Retreieve the required data from specific collection and specific document for example:
        /// Users (Collection)/yonka2019 (Document)
        /// </summary>
        private static async Task<Dictionary<string, object>> GetDataAsync(string collection, string document)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            DocumentSnapshot documentSnapshot = (DocumentSnapshot)await baseReference.Collection(collection).Document(document).Get();

            if (documentSnapshot != null && documentSnapshot.Exists())
            {
                foreach (KeyValuePair<string, Java.Lang.Object> item in documentSnapshot.Data)
                {
                    dictionary.Add(item.Key, item.Value);
                }

                return dictionary;
            }
            else
                return null;
        }
        #endregion
    }
}