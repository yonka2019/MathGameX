using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MathGame.Models;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace MathGame.Activities
{
    [Activity(Label = "FinishedGameActivity")]
    public class FinishedGameActivity : Activity
    {
        private Button backToMenu, changeChart, screenshotButton;
        private TextView correctAnswersNumber, wrongAnswersNumber, totalGameTime, averageAnswerTime;
        private ChartView statsChart;

        private int correctAnswers, wrongAnswers, totalGameTimeSeconds;
        private double averageAnswerTimeSeconds_CURRENT;

        private readonly Dictionary<char, int> correctAnswersCounter = new Dictionary<char, int>();
        private ChartEntry[] statisticsEntries;

        private ChartTypes currentChartType;

        private const int LABEL_FONT_SIZE = 40;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.finished_game);

            SetRefs();
            SetEvents();
            SetTooltips();

            // restore collected data from game in order to show it
            correctAnswers = base.Intent.GetIntExtra("stats:correct", 0);
            wrongAnswers = base.Intent.GetIntExtra("stats:wrong", 0);
            totalGameTimeSeconds = base.Intent.GetIntExtra("stats:totalGameTime_S", 0);

            if ((correctAnswers + wrongAnswers) != 0)
                averageAnswerTimeSeconds_CURRENT = totalGameTimeSeconds / (double)(correctAnswers + wrongAnswers);
            else
                averageAnswerTimeSeconds_CURRENT = 0;

            correctAnswersCounter['+'] = base.Intent.GetIntExtra("stats:+", 0);
            correctAnswersCounter['-'] = base.Intent.GetIntExtra("stats:-", 0);
            correctAnswersCounter['*'] = base.Intent.GetIntExtra("stats:*", 0);
            correctAnswersCounter['/'] = base.Intent.GetIntExtra("stats:/", 0);

            SaveDataToDB();

            SetupChartData();
            SetupIntialChart();

            correctAnswersNumber.Text = correctAnswers.ToString();
            wrongAnswersNumber.Text = wrongAnswers.ToString();

            TimeSpan t = TimeSpan.FromSeconds(totalGameTimeSeconds);
            totalGameTime.Text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);

            averageAnswerTime.Text = $"{System.Math.Round(averageAnswerTimeSeconds_CURRENT, 2)} seconds";
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetRefs()
        {
            backToMenu = FindViewById<Button>(Resource.Id.stats_backToMenu);
            changeChart = FindViewById<Button>(Resource.Id.ChangeChart);
            screenshotButton = FindViewById<Button>(Resource.Id.cameraButton);

            correctAnswersNumber = FindViewById<TextView>(Resource.Id.stats_correct);
            wrongAnswersNumber = FindViewById<TextView>(Resource.Id.stats_wrong);
            totalGameTime = FindViewById<TextView>(Resource.Id.finish_totalTime);
            averageAnswerTime = FindViewById<TextView>(Resource.Id.finish_averageAnswerTime);

            statsChart = FindViewById<ChartView>(Resource.Id.statsChart);
        }

        private void SetEvents()
        {
            changeChart.Click += ChangeChart_Click;
            backToMenu.Click += BackToMenu_Click;
            screenshotButton.Click += ScreenshotButton_Click;
        }

        private void SetTooltips()
        {
            Android.Support.V7.Widget.TooltipCompat.SetTooltipText(averageAnswerTime, "Average time to answer per answer");
            Android.Support.V7.Widget.TooltipCompat.SetTooltipText(totalGameTime, "Total game time");
            Android.Support.V7.Widget.TooltipCompat.SetTooltipText(changeChart, "Change chart style");
            Android.Support.V7.Widget.TooltipCompat.SetTooltipText(correctAnswersNumber, "Total correct answers");
            Android.Support.V7.Widget.TooltipCompat.SetTooltipText(wrongAnswersNumber, "Total wrong answers");
            Android.Support.V7.Widget.TooltipCompat.SetTooltipText(totalGameTime, "Total game time");
            Android.Support.V7.Widget.TooltipCompat.SetTooltipText(screenshotButton, "Take a screenshot of this page");
            Android.Support.V7.Widget.TooltipCompat.SetTooltipText(statsChart, "Correct answers per arithmetic operator in current finished game");

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

        private async void SaveDataToDB()
        {
            if (MainActivity.Username != "")  // save data only if the player not in anonymous 
            {
                double bestAverageAnswerTimeSeconds;
                Dictionary<string, object> currentStats = await FirebaseManager.GetStatsDataAsync(MainActivity.Username);

                double averageAnswerTimeSeconds_DB = Convert.ToDouble(currentStats["AVG_AnswerTime_S"]);

                // checks if new AATS is better than DB value OR checks if DB value default (0) AND total correct answers more or equals to 10 (to avoid fast wrong answer and leave game)
                if (((averageAnswerTimeSeconds_CURRENT < averageAnswerTimeSeconds_DB) || (averageAnswerTimeSeconds_DB == 0)) && (correctAnswers + wrongAnswers >= 10))
                {
                    bestAverageAnswerTimeSeconds = averageAnswerTimeSeconds_CURRENT;
                    this.CreateShowDialog("You beat your own record", "You have beat your own record!", "OK", Resource.Drawable.confetti_64px);
                }
                else
                    bestAverageAnswerTimeSeconds = averageAnswerTimeSeconds_DB;


                FirebaseManager.SetStatsData(MainActivity.Username,

                // add to CURRENT STATISTICS data the new statistics data and set in firebase
                Convert.ToInt32(currentStats["Plus"]) + correctAnswersCounter['+'],
                Convert.ToInt32(currentStats["Minus"]) + correctAnswersCounter['-'],
                Convert.ToInt32(currentStats["Multiply"]) + correctAnswersCounter['*'],
                Convert.ToInt32(currentStats["Divide"]) + correctAnswersCounter['/'],
                bestAverageAnswerTimeSeconds);
            }
        }

        private void ScreenshotButton_Click(object sender, EventArgs e)
        {
            bool succeed = TakeSaveScreenshot();

            if (succeed)
            {
                MediaPlayerAmbient.PlaySound(PackageName, ApplicationContext, Resource.Raw.camera_click_sound);
                this.CreateShowDialog("Screenshot saved", "Screenshot successfully saved to your gallery!", "OK", Resource.Drawable.done_64px);
            }
            else
                this.CreateShowDialog("Something went wrong..", "Can't take a screenshot", "OK", Resource.Drawable.warning64);
        }

        /// <summary>
        /// Takes screenshot and returns true if succeed
        /// </summary>
        /// <returns>True if succeed or false if error occured</returns>
        private bool TakeSaveScreenshot()
        {
            try
            {
                // Get the root view of the current activity
                View rootView = Window.DecorView.RootView;

                // Create a bitmap with the same dimensions as the root view
                Bitmap screenshot = Bitmap.CreateBitmap(rootView.Width, rootView.Height, Bitmap.Config.Argb8888);

                // Create a canvas and draw the contents of the root view onto the bitmap
                Canvas canvas = new Canvas(screenshot);
                rootView.Draw(canvas);

                // Get the path to the device's pictures directory
                string picturesDirectoryPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;

                // Create a file to save the screenshot
                string screenshotFileName = "mathgame_screenshot_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
                string screenshotFilePath = System.IO.Path.Combine(picturesDirectoryPath, screenshotFileName);

                using (FileStream stream = new FileStream(screenshotFilePath, FileMode.Create))
                {
                    screenshot.Compress(Bitmap.CompressFormat.Png, 90, stream);
                }

                // Dispose of the bitmap
                screenshot.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }


        private void BackToMenu_Click(object sender, System.EventArgs e)
        {
            StartActivity(new Intent(this, typeof(MainActivity)));
        }

        private void ChangeChart_Click(object sender, System.EventArgs e)
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
        private void SetupChartData()
        {
            statisticsEntries = new[] {
                new ChartEntry(correctAnswersCounter['+'])
                {
                    Label = "Plus",
                    ValueLabel = correctAnswersCounter['+'].ToString(),
                    Color = SKColor.Parse("#D66E81")
                },

                new ChartEntry(correctAnswersCounter['-'])
                {
                    Label = "Minus",
                    ValueLabel = correctAnswersCounter['-'].ToString(),
                    Color = SKColor.Parse("#72BfDD")
                },

                new ChartEntry(correctAnswersCounter['*'])
                {
                    Label = "Multiply",
                    ValueLabel = correctAnswersCounter['*'].ToString(),
                    Color = SKColor.Parse("#62B58F")
                },

                new ChartEntry(correctAnswersCounter['/'])
                {
                    Label = "Divide",
                    ValueLabel = correctAnswersCounter['/'].ToString(),
                    Color = SKColor.Parse("#fAC857")
                }};
        }

        public override void OnBackPressed()
        {
            // prevent user to get back to game after finishing
        }
    }
}