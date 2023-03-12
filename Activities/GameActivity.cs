using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MathGame.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MathGame
{
    [Activity(Label = "GameActivity")]
    public class GameActivity : Activity
    {
        private Button submitButton, skipButton, leaveButton;
        private EditText answerInput;
        private TextView timer, question, correctAnswers, wrongAnswers;

        private Game currentGame;

        /// <summary>
        /// Cancellation token which allows to stop the task remotely (for example if the user submitted the answer, or skipped)
        /// </summary>
        private CancellationTokenSource cts;

        private bool gameRunning;


        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.game_screen);
            // count down on screen 3 seconds before start

            SetRefs();
            SetEvents();

            Models.GameMode selectDifficulty = (Models.GameMode)base.Intent.GetIntExtra("mode", 0);

            currentGame = new Game(selectDifficulty);

            await StartCountdown(3, default);

            ButtonsEnable(true);

            await StartGame();
        }

        private async Task StartGame()
        {
            gameRunning = true;

            while (gameRunning)
            {
                question.Text = currentGame.GetRandomQuestionType().GetRandomQuestion();

                RunAnimation(question, AnimationType.TranslationY, 750, 300f, 0f);

                cts = new CancellationTokenSource();

                try
                {
                    await StartCountdown(currentGame.AnswerTime, cts.Token);

                }
                catch (TaskCanceledException)
                {
                    continue;
                }
            }

        }

        /// <summary>
        /// Run a X animation in a X duration with the X values
        /// </summary>
        /// <param name="type">type of the animation</param>
        /// <param name="duration">duration to run in the requested animation</param>
        /// <param name="values">values which the animation should get</param>
        private void RunAnimation(Java.Lang.Object target, string type, int duration, params float[] values)
        {
            ObjectAnimator animator = ObjectAnimator.OfFloat(target, type, values);
            animator.SetDuration(duration);
            animator.Start();
        }

        private void ButtonsEnable(bool isEnabled)
        {
            submitButton.Enabled = isEnabled;
            skipButton.Enabled = isEnabled;
            answerInput.Enabled = isEnabled;
        }


        private void SetRefs()
        {
            submitButton = FindViewById<Button>(Resource.Id.SubmitButton);
            skipButton = FindViewById<Button>(Resource.Id.skipButton);
            leaveButton = FindViewById<Button>(Resource.Id.leaveButton);

            answerInput = FindViewById<EditText>(Resource.Id.inputAnswer);

            timer = FindViewById<TextView>(Resource.Id.TimerText);
            question = FindViewById<TextView>(Resource.Id.questionText);
            correctAnswers = FindViewById<TextView>(Resource.Id.correctAnswers);
            wrongAnswers = FindViewById<TextView>(Resource.Id.wrongAnswers);
        }

        private void SetEvents()
        {
            submitButton.Click += SubmitButton_Click;
            skipButton.Click += SkipButton_Click;
            leaveButton.Click += LeaveButton_Click;
        }

        private void LeaveButton_Click(object sender, EventArgs e)
        {
            gameRunning = false;

            Toast.MakeText(this, $"leaved", ToastLength.Short).Show();
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            cts.Cancel();
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            cts.Cancel();
        }


        private async Task StartCountdown(int seconds, CancellationToken ct)
        {
            TimeSpan beginTime = TimeSpan.FromSeconds(seconds);  // show begin value
            timer.Text = beginTime.ToString(@"mm\:ss");

            int i = seconds;

            while (i > 0)
            {
                await Task.Delay(1000, ct);  // wait 1 sec
                i--;

                TimeSpan time = TimeSpan.FromSeconds(i);
                timer.Text = time.ToString(@"mm\:ss");
            }
        }
    }
}