using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Google.Android.Material.TextField;
using MathGame.Models;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MathGame.Activities
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        private TextInputLayout usernametil, passwordtil;
        private EditText username, password;
        private TextView gotoRegister;
        private Button login;

        private NfcAdapter nfcAdapter;
        private Regex loginDataExpression;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.login_screen);

            nfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            SetRefs();
            SetEvents();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetRefs()
        {
            gotoRegister = FindViewById<TextView>(Resource.Id.login_gotoRegister);
            login = FindViewById<Button>(Resource.Id.login_loginButton);

            username = FindViewById<EditText>(Resource.Id.login_userTB);
            password = FindViewById<EditText>(Resource.Id.login_passTB);

            usernametil = FindViewById<TextInputLayout>(Resource.Id.login_usernametil);
            passwordtil = FindViewById<TextInputLayout>(Resource.Id.login_passtil);
        }

        private void SetEvents()
        {
            gotoRegister.Click += GotoRegister_Click;
            login.Click += Login_Click;
            username.TextChanged += Username_TextChanged;
            password.TextChanged += Password_TextChanged;
        }

        private void Password_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            passwordtil.Error = "";
        }

        private void Username_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            usernametil.Error = "";
        }

        /// <summary>
        /// Checks if the given password is correct compared the given user
        /// </summary>
        /// <param name="username">username which is password should be tested</param>
        /// <param name="password">(probably) the password of the given username</param>
        /// <param name="MD5_required">if MD5 required to check, if the password already the hash, for example like in NFC tag auto-login), the hashes from the NFC Tag and from the DB got compared, and when the user logins via regular method, the password got hashed and after that get checked with the DB hashed password</param>
        /// <returns>true if the local password and DB password matched according the MD5 rules</returns>
        private async Task<bool> PasswordCorrect(string username, string password, bool MD5_required)
        {
            System.Collections.Generic.Dictionary<string, object> loginData = await FirebaseManager.GetLoginDataAsync(username);

            if (loginData == null)
                return false;
            else
            {
                if (MD5_required)
                    password = password.GetMD5();
            }

            return password == loginData["Password"].ToString();  // compare between already hashed password in db, and entered password hashed
        }

        private async void Login_Click(object sender, EventArgs e)
        {
            if (username.Text == "")
            {
                usernametil.Error = "Username can't be blank";
                return;
            }

            if (password.Text == "")
            {
                passwordtil.Error = "Password can't be blank";
                return;
            }

            if (await PasswordCorrect(username.Text, password.Text, true))  // if password matches => successfully logged in
            {
                this.AccountLogin(username.Text);
            }
            else
            {
                passwordtil.Error = "Wrong username or password";
            }
        }

        private void GotoRegister_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(RegisterActivity)));
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (nfcAdapter != null)
            {
                //Set events and filters
                IntentFilter tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
                IntentFilter ndefDetected = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
                IntentFilter techDetected = new IntentFilter(NfcAdapter.ActionTechDiscovered);
                IntentFilter[] filters = new[] { ndefDetected, tagDetected, techDetected };

                Intent intent = new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop);

                PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.Mutable);

                // Gives your current foreground activity priority in receiving NFC events over all other activities.
                nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
            }
        }

        /// <summary>
        /// If there's a new NFC tag detection, OnNewIntent will catch it
        /// </summary>
        /// <param name="intent"></param>
        protected override async void OnNewIntent(Intent intent)
        {
            if (intent.Action != NfcAdapter.ActionTagDiscovered) return;
            Tag NFCTag = (Tag)intent.GetParcelableExtra(NfcAdapter.ExtraTag);

            if (NFCTag == null) return;

            IParcelable[] rawMessages = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
            if (rawMessages == null) return;

            NdefMessage msg = (NdefMessage)rawMessages[0];

            // Get NdefRecord which contains the actual data
            NdefRecord record = msg.GetRecords()[0];

            if (record == null) return;

            // The data is defined by the Record Type Definition (RTD) specification available from http://members.nfc-forum.org/specs/spec_list/
            if (record.Tnf != NdefRecord.TnfWellKnown) return;

            // Get the transmitted data
            string data = System.Text.Encoding.ASCII.GetString(record.GetPayload());

            (string nfc_username, string nfc_password) = ExtractNFCData(data);
            if (nfc_username != null && nfc_password != null)
            {
                username.Text = nfc_username;
                password.Text = nfc_password;

                if (await PasswordCorrect(nfc_username, nfc_password, false))  // if password matches => successfully logged in
                {
                    this.AccountLogin(nfc_username);
                }
                else
                {
                    passwordtil.Error = "Wrong username or password";
                }
            }
        }

        /// <summary>
        /// Extracts the username and the password from the given data which returned from NFC Tag (if it's not hold the right information the functions returns NULLs
        /// </summary>
        /// <param name="data">data which got from the scanned NFC tag</param>
        /// <returns>username and password from this data if the data matches according application sign</returns>
        private (string, string) ExtractNFCData(string data)
        {
            loginDataExpression = new Regex(@"^\[com\.yonka\.mathgame\]\$LOGIN_DATA\{(.+?):(.+?)\}\$$");
            Match dataMatch = loginDataExpression.Match(data);

            if (dataMatch.Success)
            {
                return (dataMatch.Groups[1].Value, dataMatch.Groups[2].Value);
            }
            else
                return (null, null);
        }
    }
}