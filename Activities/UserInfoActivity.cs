using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Java.Text;
using MathGame.Models;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace MathGame.Activities
{
    [Activity(Label = "UserInfoActivity")]
    public class UserInfoActivity : Activity
    {
        private Button backToMenu, changeChart, logout;
        private ChartView statsChart;
        private TextView playerName, createdAt, averageAnswerTime;
        private CheckBox clearTag;

        private ChartEntry[] statisticsEntries;
        private ChartTypes currentChartType;

        private const int LABEL_FONT_SIZE = 40;

        private NfcAdapter nfcAdapter;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.user_info_screen);

            SetRefs();
            SetEvents();
            SetTooltips();

            nfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            Dictionary<string, object> userStatisticsData = await FirebaseManager.GetStatsDataAsync(MainActivity.Username);
            Dictionary<string, object> userRegisterData = await FirebaseManager.GetGlobalDataAsync(MainActivity.Username);

            playerName.Text = MainActivity.Username;

            createdAt.Text = FormatTimestamp(userRegisterData["CreatedAt"]);
            averageAnswerTime.Text = $"{System.Math.Round(Convert.ToDouble(userStatisticsData["AVG_AnswerTime_S"]), 2)} seconds";

            SetupChartData(userStatisticsData);
            SetupIntialChart();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetRefs()
        {
            backToMenu = FindViewById<Button>(Resource.Id.info_backToMenu);
            changeChart = FindViewById<Button>(Resource.Id.info_changeChart);
            logout = FindViewById<Button>(Resource.Id.info_logout);

            statsChart = FindViewById<ChartView>(Resource.Id.info_statsChart);

            playerName = FindViewById<TextView>(Resource.Id.info_playerName);
            createdAt = FindViewById<TextView>(Resource.Id.info_createdAt);
            averageAnswerTime = FindViewById<TextView>(Resource.Id.info_averageAnswerTime);

            clearTag = FindViewById<CheckBox>(Resource.Id.cbClearTag);
        }

        private void SetEvents()
        {
            backToMenu.Click += BackToMenu_Click;
            changeChart.Click += ChangeChart_Click;
            logout.Click += Logout_Click;
        }

        private void SetTooltips()
        {
            Android.Support.V7.Widget.TooltipCompat.SetTooltipText(averageAnswerTime, "Average time to answer per answer");
            Android.Support.V7.Widget.TooltipCompat.SetTooltipText(changeChart, "Change chart type");
        }

        /// <summary>
        /// formats the timestamp object of Firebase as dd.MM.yy HH:mm
        /// </summary>
        /// <param name="timestampObject">object to reformat in string</param>
        /// <returns>reformatted string as 'dd.MM.yy HH:mm'</returns>
        private string FormatTimestamp(object timestampObject)
        {
            Java.Util.Date date = ((Firebase.Timestamp)timestampObject).ToDate();
            SimpleDateFormat dateFormat = new SimpleDateFormat("dd.MM.yy HH:mm");
            return dateFormat.Format(date);
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            MainActivity.Username = "";  // remove username

            GetSharedPreferences("LoginSession", FileCreationMode.Private).Edit().Clear().Commit();  // removes current session (even if expired)

            Intent preActivity = new Intent(this, typeof(PreActivity));
            StartActivity(preActivity);

            Toast.MakeText(this, "Logged out", ToastLength.Short).Show();
        }

        private void SetupChartData(Dictionary<string, object> userStatisticsData)
        {
            statisticsEntries = new[] {
                new ChartEntry(float.Parse(userStatisticsData["Plus"].ToString()))
                {
                    Label = "Plus",
                    ValueLabel = userStatisticsData["Plus"].ToString(),
                    Color = SKColor.Parse("#D66E81")
                },

                new ChartEntry(float.Parse(userStatisticsData["Minus"].ToString()))
                {
                    Label = "Minus",
                    ValueLabel = userStatisticsData["Minus"].ToString(),
                    Color = SKColor.Parse("#72BfDD")
                },

                new ChartEntry(float.Parse(userStatisticsData["Multiply"].ToString()))
                {
                    Label = "Multiply",
                    ValueLabel = userStatisticsData["Multiply"].ToString(),
                    Color = SKColor.Parse("#62B58F")
                },

                new ChartEntry(float.Parse(userStatisticsData["Divide"].ToString()))
                {
                    Label = "Divide",
                    ValueLabel = userStatisticsData["Divide"].ToString(),
                    Color = SKColor.Parse("#FAC857")
                }};
        }

        private void SetupIntialChart()
        {
            DonutChart donutChart = new DonutChart
            {
                Entries = statisticsEntries,
                LabelTextSize = LABEL_FONT_SIZE,
                GraphPosition = GraphPosition.Center
            };

            statsChart.Chart = donutChart;
            currentChartType = ChartTypes.Donut;
        }

        private void ChangeChart_Click(object sender, EventArgs e)
        {
            if (currentChartType == ChartTypes.Donut)  // if current chart DONUT, change to PIE
            {
                Drawable pieChartImage = Resources.GetDrawable(Resource.Drawable.bar_chart_35px);  // the image means the next chart change type
                changeChart.SetCompoundDrawablesRelativeWithIntrinsicBounds(null, null, null, pieChartImage);

                PieChart pieChart = new PieChart
                {
                    Entries = statisticsEntries,
                    LabelTextSize = LABEL_FONT_SIZE,
                    GraphPosition = GraphPosition.Center
                };

                statsChart.Chart = pieChart;
                currentChartType = ChartTypes.Pie;
            }

            else if (currentChartType == ChartTypes.Pie)  // if current chart PIE, change to BAR
            {
                Drawable barChartImage = Resources.GetDrawable(Resource.Drawable.radar_35px);  // the image means the next chart change type
                changeChart.SetCompoundDrawablesRelativeWithIntrinsicBounds(null, null, null, barChartImage);

                BarChart barChart = new BarChart
                {
                    Entries = statisticsEntries,
                    LabelTextSize = LABEL_FONT_SIZE,
                    LabelOrientation = Microcharts.Orientation.Horizontal
                };

                statsChart.Chart = barChart;
                currentChartType = ChartTypes.Bar;
            }

            else if (currentChartType == ChartTypes.Bar)  // if current chart BAR, change to RADAR
            {
                Drawable radarChartImage = Resources.GetDrawable(Resource.Drawable.donut_chart_35px);  // the image means the next chart change type
                changeChart.SetCompoundDrawablesRelativeWithIntrinsicBounds(null, null, null, radarChartImage);

                RadarChart radarChart = new RadarChart
                {
                    Entries = statisticsEntries,
                    LabelTextSize = LABEL_FONT_SIZE,
                };

                statsChart.Chart = radarChart;
                currentChartType = ChartTypes.Radar;
            }

            else if (currentChartType == ChartTypes.Radar)  // if current chart RADAR, change to DONUT
            {
                Drawable donutChartImage = Resources.GetDrawable(Resource.Drawable.pie_chart_35px);  // the image means the next chart change type
                changeChart.SetCompoundDrawablesRelativeWithIntrinsicBounds(null, null, null, donutChartImage);

                DonutChart donutChart = new DonutChart
                {
                    Entries = statisticsEntries,
                    LabelTextSize = LABEL_FONT_SIZE,
                    GraphPosition = GraphPosition.Center
                };

                statsChart.Chart = donutChart;
                currentChartType = ChartTypes.Donut;
            }
        }

        private void BackToMenu_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(MainActivity)));
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
        /// If there's NFC detection OnNewIntent will catch it
        /// </summary>
        /// <param name="intent"></param>
        protected override async void OnNewIntent(Intent intent)
        {
            if (clearTag.Checked)  // user checked to clean the tag data
            {
                bool succeed = WriteToNFCTag(intent, "");

                if (succeed)
                    this.CreateShowDialog("NFC tag cleaned", "Your NFC tag cleaned successfully", "OK", Resource.Drawable.done_64px);
                else
                    this.CreateShowDialog("Something went wrong..", "Can't clean your NFC tag", "OK", Resource.Drawable.warning64);
            }
            else
            {
                // save login data to NFC Card in order to allow easily login
                string hashedPassword = (await FirebaseManager.GetLoginDataAsync(MainActivity.Username))["Password"].ToString();

                string dataToWrite = $"[com.yonka.mathgame]$LOGIN_DATA{{{MainActivity.Username}:{hashedPassword}}}$";
                bool succeed = WriteToNFCTag(intent, dataToWrite);

                if (succeed)
                    this.CreateShowDialog("NFC Login Data Copied", "Your authentication data successfuly saved on the NFC tag!", "OK", Resource.Drawable.done_64px);
                else
                    this.CreateShowDialog("Something went wrong..", "Can't copy your authentication data to the NFC tag", "OK", Resource.Drawable.warning64);
            }
        }

        public bool WriteToNFCTag(Intent intent, string content)
        {
            try
            {
                if (!(intent.GetParcelableExtra(NfcAdapter.ExtraTag) is Tag tag)) return false;
                Ndef ndef = Ndef.Get(tag);
                if (ndef == null || !ndef.IsWritable) return false;

                byte[] payload = Encoding.ASCII.GetBytes(content);
                byte[] mimeBytes = Encoding.ASCII.GetBytes("text/plain");

                NdefRecord record = new NdefRecord(NdefRecord.TnfWellKnown, mimeBytes, new byte[0], payload);
                NdefMessage ndefMessage = new NdefMessage(new[] { record });

                ndef.Connect();
                ndef.WriteNdefMessage(ndefMessage);
                ndef.Close();

                return true;
            }
            catch { return false; }
        }
    }
}