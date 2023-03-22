using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MathGame.Adapters;
using Syncfusion.SfDataGrid;
using System;
using System.Collections.Generic;
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
            var list = new List<Dictionary<string, string>>();

            foreach (DataRow row in dt.Rows)
            {
                var dictionary = new Dictionary<string, string>();
                foreach (DataColumn column in dt.Columns)
                {
                    dictionary.Add(column.ColumnName, row[column].ToString());
                }
                list.Add(dictionary);
            }

            // Create a SimpleAdapter with the DataTable
            var adapter = new SimpleAdapter(
                this,
                list,
                Resource.Layout.list_item_layout,
                new string[] { "Column 1", "Column 2", "Column 3" },
                new int[] { Resource.Id.column1_textview, Resource.Id.column2_textview, Resource.Id.column3_textview });

            // Set the adapter for the ListView
            var listView = FindViewById<ListView>(Resource.Id.my_listview);
            listView.Adapter = adapter;
        }


        private void SetRefs()
        {
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