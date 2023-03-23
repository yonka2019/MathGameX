using Android.Animation;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views.InputMethods;
using Android.Widget;
using MathGame.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MathGame.Activities
{
    [Activity(Label = "GameActivity")]
    public class GameActivity : Activity
    {
        // if EASY / MEDIUM / HARD mode selected - there is limited questions number [SHOULD BE UPDATED ALSO IN layout/game_screen.xml [questionProgressBar : max value]
        private const int QUESTIONS_NUMBER = 10;
        private const int VIBRATION_AMPLITUDE = 65;  // 1 - 255
        private const int VIBRATION_MS = 200;

        private NumberFormatInfo numberFormat;
        private MediaPlayer mediaPlayer;

        private Button submitButton, skipButton, leaveButton, negativeAnswerButton;
        private EditText answerInput;
        private TextView timer, question, correctAnswers, wrongAnswers;
        private ProgressBar questionProgressBar;

        private Game currentGame;

        private bool gameRunning;

        private readonly Dictionary<char, int> correctAnswersCounter = new Dictionary<char, int>();

        private LowBatteryReceiver lowBatteryReceiver;
        private IntentFilter lowBatteryFilter;

        /// <summary>
        /// Cancellation token which allows to stop the task remotely (for example if the user submitted the answer, or skipped)
        /// </summary>
        private CancellationTokenSource cts;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.game_screen);

            numberFormat = CultureInfo.CurrentCulture.NumberFormat;
            mediaPlayer = new MediaPlayer();  // create media player object (which will play sound when correct or wrong answer given)

            SetRefs();
            SetEvents();
            SetLowBatteryReceiver();

            ResetCounter();

            GameMode selectedDifficulty = (GameMode)base.Intent.GetIntExtra("mode", 0);

            if (selectedDifficulty == GameMode.Infinity)  // if infinity mdoe enabled - hide the progress bar
                questionProgressBar.Visibility = Android.Views.ViewStates.Invisible;

            currentGame = new Game(selectedDifficulty);

            mediaPlayer.PlaySound(PackageName, ApplicationContext, Resource.Raw.three_ticks);
            await StartCountdown(3, default);  // 3 seconds countdown before starting game

            ButtonsEnable(true);  // enable all the buttons (they are disabled by default)
            await StartGame();  // starting game
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetLowBatteryReceiver()
        {
            // Create the broadcast receiver and intent filter
            lowBatteryReceiver = new LowBatteryReceiver();
            lowBatteryFilter = new IntentFilter(Intent.ActionBatteryChanged);

            // Register the broadcast receiver
            RegisterReceiver(lowBatteryReceiver, lowBatteryFilter);
        }

        private async Task StartGame()
        {
            gameRunning = true;

            while (gameRunning &&
                (questionProgressBar.Progress < QUESTIONS_NUMBER || currentGame.GetGameMode() == GameMode.Infinity))  // check if current question number isn't higher then the max one, or, to ignore this check if infinity mode enabled
            {
                answerInput.Text = "";  // clean edit text box after answering

                question.Text = currentGame.GetRandomQuestionType().GenerateQuestion().ToString();
                questionProgressBar.Progress++;

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

                leaveButton.PerformClick();  // simulate leaving in order to show statistics screen
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
            negativeAnswerButton.Enabled = isEnabled;
            leaveButton.Enabled = isEnabled;
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

            questionProgressBar = FindViewById<ProgressBar>(Resource.Id.questionProgressbar);
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

        /// <summary>
        /// This event called on finishing game OR leaving the game
        /// </summary>
        private void LeaveButton_Click(object sender, EventArgs e)
        {
            gameRunning = false;

            if (!cts.IsCancellationRequested)  // Cancel if not already canceled
                cts.Cancel();

            // Unregister the low-battery broadcast receiver
            UnregisterReceiver(lowBatteryReceiver);

            Vibrator vibrator = (Vibrator)GetSystemService(VibratorService);
            if (vibrator.HasVibrator)
            {
                vibrator.Vibrate(VibrationEffect.CreateOneShot(VIBRATION_MS, VIBRATION_AMPLITUDE));
            }

            Intent gameActivity = new Intent(this, typeof(FinishedGameActivity));

            // send values for statistic formation
            gameActivity.PutExtra("stats:correct", int.Parse(correctAnswers.Text));
            gameActivity.PutExtra("stats:wrong", int.Parse(wrongAnswers.Text));

            gameActivity.PutExtra("stats:+", correctAnswersCounter['+']);
            gameActivity.PutExtra("stats:-", correctAnswersCounter['-']);
            gameActivity.PutExtra("stats:*", correctAnswersCounter['*']);
            gameActivity.PutExtra("stats:/", correctAnswersCounter['/']);

            StartActivity(gameActivity);
        }


        private void SkipButton_Click(object sender, EventArgs e)
        {
            WrongAnswer();

            if (!cts.IsCancellationRequested)  // Cancel if not already canceled
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


            if (!cts.IsCancellationRequested)  // Cancel if not already canceled
                cts.Cancel();
        }

        /// <summary>
        /// The answer of the player was correct
        /// </summary>
        private void CorrectAnswer()
        {
            mediaPlayer.PlaySound(PackageName, ApplicationContext, Resource.Raw.correct_answer);

            correctAnswers.Text = (int.Parse(correctAnswers.Text) + 1).ToString();

            correctAnswersCounter[currentGame.CurrentQuestion.GetOperator()]++;  // give this type of question +1 point to counter
        }

        /// <summary>
        /// The answer of the player was wrong
        /// </summary>
        private void WrongAnswer()
        {
            mediaPlayer.PlaySound(PackageName, ApplicationContext, Resource.Raw.wrong_answer);

            wrongAnswers.Text = (int.Parse(wrongAnswers.Text) + 1).ToString();
        }

        /// <summary>
        /// Start a countdown from the given seconds number
        /// </summary>
        /// <param name="seconds">begin value to count down from</param>
        /// <param name="ct">cancellation token which allows us to stop the task whenever we need</param>
        /// <returns>Task object (to have the possibility to await)</returns>
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

        public override void OnBackPressed()
        {
            leaveButton.PerformClick();
        }

        protected override void OnResume()
        {
            base.OnResume();

            try  // if receiver already registed
            {
                RegisterReceiver(lowBatteryReceiver, lowBatteryFilter);
            }
            catch { }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            try  // if receiver already unregistred
            {
                UnregisterReceiver(lowBatteryReceiver);
            }
            catch { }
        }

        private void ResetCounter()
        {
            correctAnswersCounter['+'] = 0;
            correctAnswersCounter['-'] = 0;
            correctAnswersCounter['*'] = 0;
            correctAnswersCounter['/'] = 0;
        }
    }
}