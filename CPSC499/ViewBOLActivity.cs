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
    [Activity(Label = "View Orders")]
    public class ViewBOLActivity : AppCompatActivity
    {
        private ListView listview;
        public static string BOLNbr { get; set; }

        List<string> displayedInfo;
        List<string> bolNumbers;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewBOL);
            listview = FindViewById<ListView>(Resource.Id.viewBOLListview);
            displayedInfo = new List<string>();
            bolNumbers = new List<string>();

            ReloadListView();

            listview.ItemClick += (s, e) => {
                var t = displayedInfo[e.Position];
                var selected = displayedInfo[e.Position];
                if (MainMenuActivity.isViewBOL == true)
                {
                    // Intent intent = new Intent(this, typeof(ScanCasesActivity));
                    Intent intent = new Intent(this, typeof(BOLDetailsActivity));
                    BOLNbr = bolNumbers[e.Position];
                    intent.PutExtra("MyItem", BOLNbr);

                    StartActivity(intent);
                }
                else
                {
                    // Intent intent = new Intent(this, typeof(ScanCasesActivity));
                    Intent intent = new Intent(this, typeof(BOLEditDetailsActivity));
                    BOLNbr = bolNumbers[e.Position];
                    intent.PutExtra("MyItem", BOLNbr);

                    StartActivity(intent);
                }
            };

        }

        public void ReloadListView() {
            //Run SQL Query to get all BOLs
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Select BOLNumber, CustomerName From BOLS", connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                displayedInfo.Add(String.Format("{0}\n{1}", reader[1], reader[0]));
                                bolNumbers.Add(String.Format("{0}", reader[0]));
                            }
                        }
                        connection.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(ApplicationContext, "Error: " + ex.Message, ToastLength.Long).Show();
            }


            listview.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, displayedInfo);
        }

        protected override void OnRestart()
        {
            base.OnResume(); // Always call the superclass first.
            ReloadListView();
        }
    }
}