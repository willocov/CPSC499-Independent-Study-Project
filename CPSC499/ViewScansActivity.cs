using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace CPSC499
{
    [Activity(Label = "View Scans")]
    public class ViewScansActivity : AppCompatActivity
    {
        public static int SelectedScanID { get; set; }
        private ListView listview;
        List<int> scanIDs;
        List<string> displayedInfo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewScans);
            listview = FindViewById<ListView>(Resource.Id.viewScansListView);
            scanIDs = new List<int>();
            displayedInfo = new List<string>();

            // Query server for scans
            reloadListView();

            listview.ItemClick += (s, e) => {
                // Intent intent = new Intent(this, typeof(ScanCasesActivity));
                Intent intent = new Intent(this, typeof(ScanDetailsActivity));
                SelectedScanID = scanIDs[e.Position];
                intent.PutExtra("MyItem", SelectedScanID);

                StartActivity(intent);
            };

        }

        protected override void OnRestart()
        {
            base.OnResume(); // Always call the superclass first.
            reloadListView();
        }
        private void reloadListView() {
            try
            {
                scanIDs.Clear();
                displayedInfo.Clear();
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("ListScans", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                displayedInfo.Add(String.Format("{0}\n{1} - {2}", reader[1], reader[2], reader[3]));
                                scanIDs.Add(int.Parse(reader[0].ToString()));
                            }
                        }
                        connection.Close();
                    }
                }
                listview.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, displayedInfo);

            }
            catch (Exception ex) {
                Toast.MakeText(ApplicationContext, "Error: " + ex.Message, ToastLength.Long).Show();
            }
        }
    }
}