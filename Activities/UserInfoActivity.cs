﻿using Android.App;
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
        private Button backToMenu, changeChart;
        private ChartView statsChart;
        private TextView playerName, createdAt;

        private ChartEntry[] statisticsEntries;
        private ChartTypes currentChartType;

        private const int LABEL_FONT_SIZE = 45;

        private NfcAdapter nfcAdapter;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.user_info_screen);

            SetRefs();
            SetEvents();

            nfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            Dictionary<string, object> userStatisticsData = await FirebaseManager.GetStatsDataAsync(MainActivity.Username);
            Dictionary<string, object> userRegisterData = await FirebaseManager.GetRegisterDataAsync(MainActivity.Username);

            playerName.Text = MainActivity.Username;

            createdAt.Text = FormatTimestamp(userRegisterData["CreatedAt"]);

            SetupChartData(userStatisticsData);
            SetupIntialChart();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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

        private void SetRefs()
        {
            backToMenu = FindViewById<Button>(Resource.Id.info_backToMenu);
            changeChart = FindViewById<Button>(Resource.Id.info_changeChart);

            statsChart = FindViewById<ChartView>(Resource.Id.info_statsChart);

            playerName = FindViewById<TextView>(Resource.Id.info_playerName);
            createdAt = FindViewById<TextView>(Resource.Id.info_createdAt);
        }

        private void SetEvents()
        {
            backToMenu.Click += BackToMenu_Click;
            changeChart.Click += ChangeChart_Click;
        }

        private void SetupChartData(Dictionary<string, object> userStatisticsData)
        {
            statisticsEntries = new[] {
                new ChartEntry(float.Parse(userStatisticsData["Plus"].ToString()))
                {
                    Label = "Plus",
                    ValueLabel = userStatisticsData["Plus"].ToString(),
                    Color = SKColor.Parse("#2C3E50")
                },

                new ChartEntry(float.Parse(userStatisticsData["Minus"].ToString()))
                {
                    Label = "Minus",
                    ValueLabel = userStatisticsData["Minus"].ToString(),
                    Color = SKColor.Parse("#77D065")
                },

                new ChartEntry(float.Parse(userStatisticsData["Multiply"].ToString()))
                {
                    Label = "Multiply",
                    ValueLabel = userStatisticsData["Multiply"].ToString(),
                    Color = SKColor.Parse("#B455B6")
                },

                new ChartEntry(float.Parse(userStatisticsData["Divide"].ToString()))
                {
                    Label = "Divide",
                    ValueLabel = userStatisticsData["Divide"].ToString(),
                    Color = SKColor.Parse("#3498DB")
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

                PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);

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
            // save login data to NFC Card in order to easily login
            string hashedPassword = (await FirebaseManager.GetLoginDataAsync(MainActivity.Username))["Password"].ToString();
            WriteToNFCTag(intent, hashedPassword);
        }

        public void WriteToNFCTag(Intent intent, string content)
        {
            if (!(intent.GetParcelableExtra(NfcAdapter.ExtraTag) is Tag tag)) return;
            Ndef ndef = Ndef.Get(tag);
            if (ndef == null || !ndef.IsWritable) return;
            byte[] payload = Encoding.ASCII.GetBytes(content);
            byte[] mimeBytes = Encoding.ASCII.GetBytes("text/plain");
            NdefRecord record = new NdefRecord(NdefRecord.TnfWellKnown, mimeBytes, new byte[0], payload);
            NdefMessage ndefMessage = new NdefMessage(new[] { record });

            ndef.Connect();
            ndef.WriteNdefMessage(ndefMessage);
            ndef.Close();
        }
    }
}