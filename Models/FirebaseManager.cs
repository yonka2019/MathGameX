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


        private const string MAIN_COLLECTION = "Users";
        private const string DATA_COLLECTION = "Data";

        private const string LOGIN_DOCUMENT = "Login";
        private const string STATISTICS_DOCUMENT = "Statistics";

        private static FirebaseFirestore database;
        private static CollectionReference baseReference;


        /*                                        $ # ## ### DATA BASE STRUCTURE ### ## # $
         *            
         *  [Collection]                                          Users 
         *                                                          |
         *                                                          |
         *                                                         / \
         *                                                        /   \
         *  [Document]                                         user1  userX
         *                                                      /       \
         *                            Fields: _________________/         \_________________ Fields:
         *                                                    /           \
         *                      [Timestamp] CreatedAt        /             \        [Timestamp] CreatedAt
         *                                                  /               \
         *  [Collection]                                   Data             Data
         *                                                  |                |
         *                                                  |                |
         *  [Document]                       Login: --------|                |-------- Statistics: (counter of total correct answers & average answer time)
         *                                     |                                            |
         *                                     |                                            |
         *  [Field]           [string] Password (hashed via MD5)                       [int] Plus 
         *  [Field]                                                                    [int] Minus
         *  [Field]                                                                    [int] Multiply
         *  [Field]                                                                    [int] Divide
         *  [Field]                                                                    [int] AVG_AnswerTime_S
         */

        /// <summary>
        /// Initialize the database (should be only one time at the first launching (at SplashActivity.cs)
        /// </summary>
        /// <param name="context">current context</param>
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
                database.FirestoreSettings = new FirebaseFirestoreSettings.Builder().SetPersistenceEnabled(false).Build();
            }
            else
            {
                database = FirebaseFirestore.GetInstance(app);
            }

            // get reference to base data 'Users / [USERNAME]'        ( / Data / [STATS|LOGIN]' )
            baseReference = database.Collection(MAIN_COLLECTION);
        }

        /// <summary>
        /// returns all users which appears in the DB (all user documents)
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>> GetUsernames()
        {
            List<string> usersList = new List<string>();

            QuerySnapshot querySnapshot = (QuerySnapshot)await baseReference.Get();
            foreach (DocumentSnapshot document in querySnapshot.Documents)
            {
                usersList.Add(document.Id);
            }

            return usersList;
        }

        public static async Task<Dictionary<string, object>> GetLoginDataAsync(string username)
        {
            return await GetDataAsync(username, LOGIN_DOCUMENT);
        }

        public static async Task<Dictionary<string, object>> GetStatsDataAsync(string username)
        {
            return await GetDataAsync(username, STATISTICS_DOCUMENT);
        }

        public static async Task<Dictionary<string, object>> GetGlobalDataAsync(string username)
        {
            return await GetDataAsync(username);
        }

        public static void SetLoginData(string username, string password)
        {
            HashMap newLoginData = new HashMap();

            newLoginData.Put("Password", password.GetMD5());  // hash (via MD5) the password

            SetData(newLoginData, username, LOGIN_DOCUMENT);
        }

        public static void SetStatsData(string username, int plus, int minus, int multiply, int divide, double averageAnswerTimeSeconds)
        {
            HashMap newStatisticsData = new HashMap();

            newStatisticsData.Put("Plus", plus);
            newStatisticsData.Put("Minus", minus);
            newStatisticsData.Put("Multiply", multiply);
            newStatisticsData.Put("Divide", divide);
            newStatisticsData.Put("AVG_AnswerTime_S", averageAnswerTimeSeconds);

            SetData(newStatisticsData, username, STATISTICS_DOCUMENT);
        }

        public static void SetGlobalData(string username)
        {
            HashMap globalData = new HashMap();

            globalData.Put("CreatedAt", new Timestamp(new Date()));

            SetData(globalData, username);
        }

        #region Help Functions

        /// <summary>
        /// Sets the given data in the documents which requested to be updated
        /// </summary>
        /// <param name="data">data to be uploaded</param>
        /// <param name="userDocument">user document</param>
        /// <param name="document">user sub-document (login/stats), if empty it's refers to global document</param>
        private static void SetData(HashMap data, string userDocument, string document = "")
        {
            try
            {
                DocumentReference documentRef;

                if (document == "")
                    documentRef = baseReference.Document(userDocument);
                else
                    documentRef = baseReference.Document(userDocument).Collection(DATA_COLLECTION).Document(document);

                documentRef.Set(data);
            }
            catch { }
        }

        /// <summary>
        /// Retreieve the required data from specific collection and specific document
        /// </summary>
        /// <param name="userDocument">user document to retreieve data from</param>
        /// <param name="requestedDocument">requested document to retreive data from, if empty it's refers to global document</param>
        /// <returns>requested data</returns>
        private static async Task<Dictionary<string, object>> GetDataAsync(string userDocument, string requestedDocument = "")
        {
            try
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                DocumentSnapshot documentSnapshot;

                if (requestedDocument == "")
                    documentSnapshot = (DocumentSnapshot)await baseReference.Document(userDocument).Get();
                else
                    documentSnapshot = (DocumentSnapshot)await baseReference.Document(userDocument).Collection(DATA_COLLECTION).Document(requestedDocument).Get();


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
            catch
            {
                return null;
            }
        }

        #endregion
    }
}