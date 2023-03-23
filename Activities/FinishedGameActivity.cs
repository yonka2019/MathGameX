using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using MathGame.Models;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using System.Collections.Generic;

namespace MathGame.Activities
{
    [Activity(Label = "FinishedGameActivity")]
    public class FinishedGameActivity : Activity
    {
        private Button backToMenu, changeChart;
        private TextView correctAnswersNumber, wrongAnswersNumber;
        private ChartView statsChart;

        private int correctAnswers, wrongAnswers;
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

            // restore collected data from game in order to show it
            correctAnswers = base.Intent.GetIntExtra("stats:correct", 0);
            wrongAnswers = base.Intent.GetIntExtra("stats:wrong", 0);

            correctAnswersCounter['+'] = base.Intent.GetIntExtra("stats:+", 0);
            correctAnswersCounter['-'] = base.Intent.GetIntExtra("stats:-", 0);
            correctAnswersCounter['*'] = base.Intent.GetIntExtra("stats:*", 0);
            correctAnswersCounter['/'] = base.Intent.GetIntExtra("stats:/", 0);

            SaveDataToDB();

            SetupChartData();
            SetupIntialChart();

            correctAnswersNumber.Text = correctAnswers.ToString();
            wrongAnswersNumber.Text = wrongAnswers.ToString();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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

        private void SaveDataToDB()
        {
            FirebaseManager.SetStatsData(MainActivity.Username,
                correctAnswersCounter['+'],
                correctAnswersCounter['-'],
                correctAnswersCounter['*'],
                correctAnswersCounter['/']);
        }

        private void SetRefs()
        {
            backToMenu = FindViewById<Button>(Resource.Id.stats_backToMenu);
            changeChart = FindViewById<Button>(Resource.Id.ChangeChart);

            correctAnswersNumber = FindViewById<TextView>(Resource.Id.stats_correct);
            wrongAnswersNumber = FindViewById<TextView>(Resource.Id.stats_wrong);

            statsChart = FindViewById<ChartView>(Resource.Id.statsChart);
        }

        private void SetEvents()
        {
            changeChart.Click += ChangeChart_Click;
            backToMenu.Click += BackToMenu_Click;
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
                    Color = SKColor.Parse("#2C3E50")
                },

                new ChartEntry(correctAnswersCounter['-'])
                {
                    Label = "Minus",
                    ValueLabel = correctAnswersCounter['-'].ToString(),
                    Color = SKColor.Parse("#77D065")
                },

                new ChartEntry(correctAnswersCounter['*'])
                {
                    Label = "Multiply",
                    ValueLabel = correctAnswersCounter['*'].ToString(),
                    Color = SKColor.Parse("#B455B6")
                },

                new ChartEntry(correctAnswersCounter['/'])
                {
                    Label = "Divide",
                    ValueLabel = correctAnswersCounter['/'].ToString(),
                    Color = SKColor.Parse("#3498DB")
                }};
        }
    }
}