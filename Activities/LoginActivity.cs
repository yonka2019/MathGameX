using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using MathGame.Models;
using System;
using System.Threading.Tasks;

namespace MathGame.Activities
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        private EditText username, password;
        private TextView gotoRegister;
        private Button login;

        private NfcAdapter nfcAdapter;

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
        }

        private void SetEvents()
        {
            gotoRegister.Click += GotoRegister_Click;
            login.Click += Login_Click;
        }

        private async Task<bool> PasswordCorrect(string username, string password)
        {
            System.Collections.Generic.Dictionary<string, object> loginData = await FirebaseManager.GetLoginDataAsync(username);

            if (loginData == null)
                return false;
            else
                return password.GetMD5() == loginData["Password"].ToString();  // compare between already hashed password in db, and entered password hashed
        }

        private async void Login_Click(object sender, EventArgs e)
        {
            if (username.Text == "")
            {
                username.Error = "Username can't be blank";
                return;
            }

            if (password.Text == "")
            {
                password.Error = "Password can't be blank";
                return;
            }

            if (await PasswordCorrect(username.Text, password.Text))  // if password matches => successfully logged in
            {
                this.Login(username.Text);
            }
            else
            {
                password.Error = "Wrong username or password";
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

                PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);

                // Gives your current foreground activity priority in receiving NFC events over all other activities.
                nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
            }
        }

        /// <summary>
        /// If there's a new NFC tag detection, OnNewIntent will catch it
        /// </summary>
        /// <param name="intent"></param>
        protected override void OnNewIntent(Intent intent)
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


        }
    }
}