using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using MathGame.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace MathGame.Activities
{
    [Activity(Label = "StatisticsActivity")]
    public class StatisticsActivity : Activity
    {
        private Button backButton;
        private ListView listView;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.statistics_screen);

            SetRefs();
            SetEvents();

            DataTable sortedDataTable = SortTable(await SetupDataTable());
            SimpleAdapter adapter = SetAdapter(sortedDataTable);

            // Set the adapter for the ListView
            listView.Adapter = adapter;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private SimpleAdapter SetAdapter(DataTable dataTable)
        {
            JavaList<IDictionary<string, object>> totalDataList = new JavaList<IDictionary<string, object>>();

            foreach (DataRow row in dataTable.Rows) // each row
            {
                JavaDictionary<string, object> userDictionary = new JavaDictionary<string, object>();

                foreach (DataColumn column in dataTable.Columns)  // each column
                {
                    object value = row[column];
                    userDictionary.Add(column.ColumnName, value?.ToString());
                }
                totalDataList.Add(userDictionary);
            }

            // Create a SimpleAdapter with the DataTable
            SimpleAdapter simpleAdapter = new SimpleAdapter(
                this,
                totalDataList,
                Resource.Layout.statistics_layout,
                new string[] { "Name", "+", "-", "*", "/", "Total", "Time" },
                new int[] { Resource.Id.column1_playername, Resource.Id.column2_plusTotal, Resource.Id.column3_minusTotal, Resource.Id.column4_multiplyTotal, Resource.Id.column5_divideTotal, Resource.Id.column6_totalPoints, Resource.Id.column7_averageTime }); ;

            return simpleAdapter;
        }

        private async System.Threading.Tasks.Task<DataTable> SetupDataTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Name");

            dataTable.Columns.Add("+");
            dataTable.Columns.Add("-");
            dataTable.Columns.Add("*");
            dataTable.Columns.Add("/");
            dataTable.Columns.Add("Total");
            dataTable.Columns.Add("Time");

            dataTable.Rows.Add("Name", "+", "-", "*", "/", "Total", "Time");

            List<string> usernames = await FirebaseManager.GetUsernames();

            foreach (string username in usernames)
            {
                Dictionary<string, object> statisticsData = await FirebaseManager.GetStatsDataAsync(username);
                dataTable.Rows.Add(username, statisticsData["Plus"], statisticsData["Minus"], statisticsData["Multiply"], statisticsData["Divide"],
                    GetTotalPoints(statisticsData), $"{Math.Round(Convert.ToDouble(statisticsData["AVG_AnswerTime_S"]), 2)}s");
            }

            return dataTable;
        }

        private DataTable SortTable(DataTable table)
        {
            // assuming you have a DataTable called 'table'
            DataTable sortedTable = table.Clone(); // create a new table with the same schema as the original table
            DataRow firstRow = table.Rows[0]; // store the first row in a separate variable
            table.Rows.Remove(firstRow); // remove the first row from the original table
            table.DefaultView.Sort = "Total ASC"; // sort the remaining rows by the ColumnName column in ascending order
            DataTable sortedRows = table.DefaultView.ToTable(); // create a new table with the sorted rows
            DataRow newRow = sortedTable.NewRow(); // create a new DataRow in the target DataTable
            newRow.BeginEdit();
            newRow.ItemArray = firstRow.ItemArray; // copy the values from the firstRow to the new DataRow
            sortedRows.Rows.InsertAt(newRow, 0); // add the new DataRow back to the beginning of the sorted rows
            sortedTable.Merge(sortedRows); // merge the sorted rows into the new table

            // the sortedTable DataTable now contains all the rows sorted, with the first row in its original 

            return sortedTable;
        }

        private int GetTotalPoints(Dictionary<string, object> stats)
        {
            int totalPoints = 0;

            totalPoints += Convert.ToInt32(stats["Plus"]);
            totalPoints += Convert.ToInt32(stats["Minus"]);
            totalPoints += Convert.ToInt32(stats["Multiply"]);
            totalPoints += Convert.ToInt32(stats["Divide"]);

            return totalPoints;
        }

        private void SetRefs()
        {
            backButton = FindViewById<Button>(Resource.Id.stats_backButton);
            listView = FindViewById<ListView>(Resource.Id.statistics_listview);
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