using Android.Animation;
using Android.App;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;
using MathGame.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MathGame
{
    [Activity(Label = "GameActivity")]
    public class GameActivity : Activity
    {
        private NumberFormatInfo numberFormat;

        private Button submitButton, skipButton, leaveButton, negativeAnswerButton;
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

            numberFormat = CultureInfo.CurrentCulture.NumberFormat;

            SetRefs();
            SetEvents();

            Models.GameMode selectDifficulty = (Models.GameMode)base.Intent.GetIntExtra("mode", 0);

            currentGame = new Game(selectDifficulty);

            await StartCountdown(3, default);  // 3 seconds countdown before starting game

            ButtonsEnable(true);  // enable all the buttons (they are disabled by default)

            await StartGame();  // starting game
        }

        private async Task StartGame()
        {
            gameRunning = true;

            while (gameRunning)
            {
                answerInput.Text = "";  // clean edit text box after answering

                question.Text = currentGame.GetRandomQuestionType().GenerateQuestion().ToString();

                RunAnimation(question, AnimationType.TranslationY, 750, 300f, 0f);

                cts = new CancellationTokenSource();

                try
                {
                    await StartCountdown(currentGame.AnswerTime, cts.Token);
                    submitButton.PerformClick();  // time passed - simulate submission

                }
                catch (TaskCanceledException)  // SUBMIT / SKIP button pressed -> continue to next question (answer already checked)
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
            negativeAnswerButton = FindViewById<Button>(Resource.Id.makeNegativeAnswer);

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
            negativeAnswerButton.Click += NegativeAnswerButton_Click;
            answerInput.EditorAction += AnswerInput_EditorAction;
        }

        private void AnswerInput_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done)  // clicked on OK (on keyboard)
            {
                submitButton.PerformClick();
            }
        }

        private void NegativeAnswerButton_Click(object sender, EventArgs e)
        {
            if (answerInput.Text.Contains('-'))  // if negative sign already exist
                answerInput.Text = answerInput.Text.Replace("-", "");  // remove the negative (-)

           else if (answerInput.Text.All(x => x == '0') && answerInput.Text != "")  // zero AND not blank
                return;  // not add negative (-) sign

            else
                answerInput.Text = "-" + answerInput.Text;  // add negative (-) sign into left from the number

            answerInput.SetSelection(answerInput.Text.Length);  // move crusor to end
        }

        private void LeaveButton_Click(object sender, EventArgs e)
        {
            gameRunning = false;

            Toast.MakeText(this, $"leaved", ToastLength.Short).Show();
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            WrongAnswer();
            cts.Cancel();
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            // if the cultureinfo setted that the decimal separator is '.' or ',', replace current separator to the right one (to prevent exception in double.Parse(XX))
            answerInput.Text = answerInput.Text.Replace(',', numberFormat.NumberDecimalSeparator[0]).Replace('.', numberFormat.NumberDecimalSeparator[0]);


            if (answerInput.Text == "")  // no input
                WrongAnswer();  // count as wrong answer

            else if (double.TryParse(answerInput.Text, out double userAnswer))
            {
                if (currentGame.CurrentQuestion.CheckAnswer(userAnswer))
                    CorrectAnswer();
                else
                    WrongAnswer();
            }
            else  // can't parse (bad answer -> clean answer)
                answerInput.Text = "";

            cts.Cancel();
        }

        private void CorrectAnswer()
        {
            correctAnswers.Text = (int.Parse(correctAnswers.Text) + 1).ToString();
        }

        private void WrongAnswer()
        {
            wrongAnswers.Text = (int.Parse(wrongAnswers.Text) + 1).ToString();
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