using Android.Content;
using Android.Views;
using Android.Widget;
using System.Data;

namespace MathGame.Adapters
{
    public class GridViewAdapter : BaseAdapter
    {
        private readonly Context _context;
        private readonly DataTable _dataTable;

        public GridViewAdapter(Context context, DataTable dataTable)
        {
            _context = context;
            _dataTable = dataTable;
        }

        public override int Count => _dataTable.Rows.Count;

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                LayoutInflater inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
                view = inflater.Inflate(Resource.Layout.grid_layout, parent, false);
            }

            if (position == 0)
            {
                // This is the first row, which contains the column headers

                TextView textView1 = view.FindViewById<TextView>(Resource.Id.name_text_view);
                textView1.Text = "Header 1";
                textView1.SetBackgroundColor(Android.Graphics.Color.LightGray);

                TextView textView2 = view.FindViewById<TextView>(Resource.Id.age_text_view);
                textView2.Text = "Header 2";
                textView2.SetBackgroundColor(Android.Graphics.Color.LightGray);

                return view;
            }
            else
            {
                // This is a data row, which contains the data for one row in the DataTable

                DataRow row = _dataTable.Rows[position - 1];

                TextView textView1 = view.FindViewById<TextView>(Resource.Id.name_text_view);
                textView1.Text = row["Name"].ToString();

                TextView textView2 = view.FindViewById<TextView>(Resource.Id.age_text_view);
                textView2.Text = row["Plus"].ToString();


                return view;
            }
        }
    }
}