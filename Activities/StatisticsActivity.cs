using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MathGame.Adapters;
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

            dt.Rows.Add("asd", "asd", "asd");
            var list = new JavaList<IDictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                var dictionary = new Dictionary<string, object>();
                foreach (DataColumn column in dt.Columns)
                {
                    var value = row[column];
                    dictionary.Add(column.ColumnName, value != null ? value.ToString() : null);
                }

                list.Add(dictionary);
            }

            // Create a SimpleAdapter with the DataTable
            var adapter = new SimpleAdapter(
                this,
                list,
                Resource.Layout.list_item_layout,
                new string[] { "Name", "Plus", "Minus" },
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