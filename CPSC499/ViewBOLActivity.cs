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
using Android.Views;
using Android.Widget;

namespace CPSC499
{
    [Activity(Label = "ViewBOL")]
    public class ViewBOLActivity : ListActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Run SQL Query to get all BOLs
            List<string> items = new List<string>();
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
                                Console.WriteLine(String.Format("{0}, {1}",
                                    reader[0], reader[1]));

                                items.Add(String.Format("{0} - {1}",
                                    reader[0], reader[1]));
                            }
                        }
                        connection.Close();
                    }
                }
                
            }
            catch (Exception ex) { 
            
            }

            // Create your application here
            string[] mainMenuItemsBasic = { "Scan Cases", "Active BOL" };
            ListAdapter = new ArrayAdapter<string>(this, Resource.Layout.MainMenu, items);
            ListView.TextFilterEnabled = true;
            ListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                //Toast.MakeText(Application, ((TextView)args.View).Text, ToastLength.Short).Show();
                if (((TextView)args.View).Text == "Scan Cases")
                {
                    // Intent intent = new Intent(this, typeof(ScanCasesActivity));
                    Intent intent = new Intent(this, typeof(scanCases2Activity));
                    

                    StartActivity(intent);
                }
            };

        }
    }
}