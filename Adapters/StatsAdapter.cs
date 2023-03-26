using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MathGame.Activities;
using System.Collections.Generic;

namespace MathGame.Adapters
{
    internal class StatsAdapter : SimpleAdapter
    {

        private readonly int[] mTo;
        private readonly string[] mFrom;
        private readonly LayoutInflater mInflater;

        public StatsAdapter(Context context, JavaList<IDictionary<string, object>> data,
            int resource, string[] from, int[] to) : base(context, data, resource, from, to)
        {
            mFrom = from;
            mTo = to;
            mInflater = LayoutInflater.FromContext(context);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = base.GetView(position, convertView, parent);

            TextView text = (TextView)view.FindViewById(mTo[0]);
            if (text.Text == MainActivity.Username)
            {
                text.SetTypeface(null, TypefaceStyle.Bold);

                for (int i = 1; i < mTo.Length; i++)
                {
                    TextView tv = (TextView)view.FindViewById(mTo[i]);
                    tv.SetTypeface(null, TypefaceStyle.Bold);
                }
            }

            return view;
        }
    }
}