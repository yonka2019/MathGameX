using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MathGame.Adapters;
using Syncfusion.SfDataGrid;
using System;
using System.Data;

namespace MathGame.Activities
{
    [Activity(Label = "StatisticsActivity")]
    public class StatisticsActivity : Activity
    {
        private Button backButton;
        private GridView gv;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.statistics_screen);

            SetRefs();
            SetEvents();

            DataTable dt = new DataTable();
            dt.Columns.Add("Name");

            dt.Columns.Add("Plus");
            dt.Columns.Add("Minus");
            dt.Columns.Add("Multiply");
            dt.Columns.Add("Divide");

            dt.Rows.Add("asd", 0, 1, 2, 3);

            GridViewAdapter adapter = new GridViewAdapter(this, dt);
            gv.Adapter = adapter;
        }


        private void SetRefs()
        {
            gv = FindViewById<GridView>(Resource.Id.gridView1);
            backButton = FindViewById<Button>(Resource.Id.stats_backButton);
        }

        private void SetEvents()
        {
            backButton.Click += BackButton_Click;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(MainActivity)));
        }
    }
}