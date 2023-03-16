using Android.App;
using Android.OS;
using Microcharts;
using SkiaSharp;

namespace MathGame.Activities
{
    [Activity(Label = "FinishedGameActivity")]
    public class FinishedGameActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.finished_game);


            Models.GameMode selectedDifficulty = (Models.GameMode)base.Intent.GetIntExtra("mode", 0);

            ChartEntry[] entries = new[] {
                new ChartEntry(212)
                {
                    Label = "+",
                    ValueLabel = "112",
                    Color = SKColor.Parse("#2c3e50")
                },
                new ChartEntry(248)
                {
                    Label = "-",
                    ValueLabel = "648",
                    Color = SKColor.Parse("#77d065")
                },
                new ChartEntry(128)
                {
                    Label = "*",
                    ValueLabel = "428",
                    Color = SKColor.Parse("#b455b6")
                },
                new ChartEntry(514)
                {
                    Label = "/",
                    ValueLabel = "214",
                    Color = SKColor.Parse("#3498db")
                }};
        }
    }
}