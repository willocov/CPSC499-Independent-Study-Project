using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace CPSC499
{
    [Activity(Label = "View Orders")]
    public class ViewBOLActivity : AppCompatActivity
    {
        private ListView listview;
        public static string BOLNbr { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewBOL);
            listview = FindViewById<ListView>(Resource.Id.viewBOLListview);


            //Run SQL Query to get all BOLs
            List<string> displayedInfo = new List<string>();
            List<string> bolNumbers = new List<string>();
            string connectionString = @"Server=192.168.1.102;Database=CPSC499;User Id=cpsc499;Password=test;";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
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
            catch (Exception ex) {
                Toast.MakeText(ApplicationContext, "Error: " + ex.Message, ToastLength.Long).Show();
            }
          

            listview.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, displayedInfo);
            listview.ItemClick += (s, e) => {
                var t = displayedInfo[e.Position];
                Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Short).Show();
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
    }
}