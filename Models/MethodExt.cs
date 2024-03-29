﻿using Android.Animation;
using Android.App;
using Android.Content;
using Android.Widget;
using MathGame.Activities;
using System.Security.Cryptography;
using System.Text;

namespace MathGame.Models
{
    public static class MethodExt
    {
        /// <summary>
        /// Hash (MD5) the given string
        /// </summary>
        /// <param name="input">string to hash</param>
        /// <returns>hashed string (via MD5)</returns>
        public static string GetMD5(this string input)
        {
            using MD5 md5Hash = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new StringBuilder to collect the bytes and create a string.
            StringBuilder builder = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return builder.ToString();
        }

        /// <summary>
        /// Shortcuter to show dialog in THIS context
        /// </summary>
        public static void CreateShowDialog(this Android.Content.Context context, string title, string message, string positiveButtonText, int iconId = 0)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(context);

            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetPositiveButton(positiveButtonText, delegate { });

            if (iconId != 0)  // check if icon gived
                builder.SetIcon(iconId);

            Android.App.AlertDialog dialog = builder.Create();

            dialog.Show();
        }

        /// <summary>
        /// Shortcuter to show progress dialog in THIS context
        /// </summary>
        /// <returns>ProgressDialog control back to user in order to dismiss it when needed</returns>
        public static ProgressDialog CreateProgressDialog(this Android.Content.Context context, string message)
        {
            ProgressDialog progressDialog = new ProgressDialog(context);
            progressDialog.SetCancelable(false);
            progressDialog.SetMessage(message);
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progressDialog.Show();

            return progressDialog;
        }

        /// <summary>
        /// logins user (opens up the main activity and updates session) ; (after user successfully creation // after successfully password check
        /// </summary>
        /// <param name="username"></param>
        public static void AccountSuccessLogin(this Context context, string username)
        {
            ISharedPreferences loginSessionSP = context.GetSharedPreferences("LoginSession", FileCreationMode.Private);
            ISharedPreferencesEditor lsEditor = loginSessionSP.Edit();

            lsEditor.PutString("Username", username);
            lsEditor.PutString("LoginTime", System.DateTime.Now.ToString());

            lsEditor.Commit();

            MainActivity.Username = username;
            context.StartActivity(new Intent(context, typeof(MainActivity)));
        }

        /// <summary>
        /// Run a X animation in a X duration with the X values
        /// </summary>
        /// <param name="type">type of the animation</param>
        /// <param name="duration">duration to run in the requested animation</param>
        /// <param name="values">values which the animation should get</param>
        public static void Animate(this Java.Lang.Object target, string type, int duration, params float[] values)
        {
            ObjectAnimator animator = ObjectAnimator.OfFloat(target, type, values);
            animator.SetDuration(duration);
            animator.Start();
        }
    }
}