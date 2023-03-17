using Android.App;
using Android.OS;
using Microcharts;
using SkiaSharp;
using System.Collections.Generic;

namespace MathGame.Activities
{
    [Activity(Label = "FinishedGameActivity")]
    public class FinishedGameActivity : Activity
    {
        private int correctAnswers, wrongAnswers;
        private readonly Dictionary<char, int> correctAnswersCounter = new Dictionary<char, int>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.finished_game);

            // restore collected data from game in order to show it
            correctAnswers = base.Intent.GetIntExtra("stats:correct", 0);
            wrongAnswers = base.Intent.GetIntExtra("stats:wrong", 0);

            correctAnswersCounter['+'] = base.Intent.GetIntExtra("stats:+", 0);
            correctAnswersCounter['-'] = base.Intent.GetIntExtra("stats:-", 0);
            correctAnswersCounter['*'] = base.Intent.GetIntExtra("stats:*", 0);
            correctAnswersCounter['/'] = base.Intent.GetIntExtra("stats:/", 0);

            ChartEntry[] entries = new[] {
                new ChartEntry(correctAnswersCounter['+'])
                {
                    Label = "+",
                    ValueLabel = correctAnswersCounter['+'].ToString(),
                    Color = SKColor.Parse("#2c3e50")
                },

                new ChartEntry(correctAnswersCounter['-'])
                {
                    Label = "-",
                    ValueLabel = correctAnswersCounter['-'].ToString(),
                    Color = SKColor.Parse("#77d065")
                },

                new ChartEntry(correctAnswersCounter['*'])
                {
                    Label = "*",
                    ValueLabel = correctAnswersCounter['*'].ToString(),
                    Color = SKColor.Parse("#b455b6")
                },

                new ChartEntry(correctAnswersCounter['/'])
                {
                    Label = "/",
                    ValueLabel = correctAnswersCounter['/'].ToString(),
                    Color = SKColor.Parse("#3498db")
                }};
        }
    }
}