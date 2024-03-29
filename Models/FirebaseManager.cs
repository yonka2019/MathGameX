﻿using Android.Content;
using Android.Content.Res;
using Android.Gms.Extensions;
using Firebase;
using Firebase.Firestore;
using Java.Util;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MathGame.Models
{
    public static class FirebaseManager
    {
        private const string SETTINGS_FILE = "google-services.json";

        // from settings file which taken from firebase project settings
        private static string SETTINGS_PROJECT_ID;
        private static string SETTINGS_APPLICATION_ID;
        private static string SETTINGS_API_KEY;
        private static string SETTINGS_STORAGE_BUCKET;


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
         *                            Fields: _________________/         \
         *                                                    /         . . .
         *                      [Timestamp] CreatedAt        /          
         *                                                  /              
         *  [Collection]                                   Data            
         *                                                  |               
         *                                                  |                
         *  [Document]                       Login: --------|------------------------- Statistics: (counter of total correct answers & average answer time)
         *                                     |                                            |
         *                                     |                                            |
         *  [Field]           [string] Password (hashed via MD5)                       [int] Plus 
         *  [Field]                                                                    [int] Minus
         *  [Field]                                                                    [int] Multiply
         *  [Field]                                                                    [int] Divide
         *  [Field]                                                                    [int] AVG_AnswerTime_S
         */

        /// <summary>
        /// Extracts settings data from the given settings file which must be stored under the Assets/ folder
        /// </summary>
        /// <param name="context">current context to access Assets object</param>
        private static void InitSettings(Context context)
        {
            // Get a reference to the AssetManager
            AssetManager assets = context.Assets;

            // Open an input stream for the JSON file in the Assets folder
            using StreamReader sr = new StreamReader(assets.Open(SETTINGS_FILE));
            // Read the contents of the file
            string contents = sr.ReadToEnd();

            // Parse the JSON data into a JObject
            JObject json = JObject.Parse(contents);

            // Access setteings in the JObject
            SETTINGS_PROJECT_ID = (string)json["project_info"]["project_id"];
            SETTINGS_APPLICATION_ID = (string)json["client"][0]["client_info"]["mobilesdk_app_id"];
            SETTINGS_API_KEY = (string)json["client"][0]["api_key"][0]["current_key"];
            SETTINGS_STORAGE_BUCKET = (string)json["project_info"]["storage_bucket"];
        }

        /// <summary>
        /// Initialize the database (should be only one time at the first launching (at SplashActivity.cs)
        /// </summary>
        /// <param name="context">current context</param>
        public static void Init(Context context)
        {
            FirebaseApp app = FirebaseApp.InitializeApp(context);
            if (app == null)
            {
                InitSettings(context);

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

            // get reference to base data '--> Users <-- / [USERNAME] / Data / [STATS|LOGIN]'
            baseReference = database.Collection(MAIN_COLLECTION);
        }

        /// <summary>
        /// returns all users which appears in the DB (all user documents)
        /// </summary>
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

        public static async Task SetLoginData(string username, string password)
        {
            HashMap newLoginData = new HashMap();

            newLoginData.Put("Password", password.GetMD5());  // hash (via MD5) the password

            await SetData(newLoginData, username, LOGIN_DOCUMENT);
        }

        public static async Task SetStatsData(string username, int plus, int minus, int multiply, int divide, double averageAnswerTimeSeconds)
        {
            HashMap newStatisticsData = new HashMap();

            newStatisticsData.Put("Plus", plus);
            newStatisticsData.Put("Minus", minus);
            newStatisticsData.Put("Multiply", multiply);
            newStatisticsData.Put("Divide", divide);
            newStatisticsData.Put("AVG_AnswerTime_S", averageAnswerTimeSeconds);

            await SetData(newStatisticsData, username, STATISTICS_DOCUMENT);
        }

        public static async Task SetGlobalData(string username)
        {
            HashMap globalData = new HashMap();

            globalData.Put("CreatedAt", new Timestamp(new Date()));

            await SetData(globalData, username);
        }

        #region Firebase interaction Functions

        /// <summary>
        /// Sets the given data in the documents which requested to be updated
        /// </summary>
        /// <param name="data">data to be uploaded</param>
        /// <param name="userDocument">user document</param>
        /// <param name="document">user sub-document (login/stats), if empty it's refers to global document</param>
        private static async Task SetData(HashMap data, string userDocument, string document = "")
        {
            try
            {
                DocumentReference documentRef;

                if (document == "")
                    documentRef = baseReference.Document(userDocument);
                else
                    documentRef = baseReference.Document(userDocument).Collection(DATA_COLLECTION).Document(document);

                await documentRef.Set(data);
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