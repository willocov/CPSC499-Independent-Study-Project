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
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace CPSC499
{
    [Activity(Label = "View Barcodes")]
    public class ViewBarcodeActivity : AppCompatActivity
    {
        private ListView barcodeListView;
        List<string> displayBarcodes;
        List<string> parsingIDs;
        string connectionString;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewBarcodes);

            // Create your application here
            barcodeListView = FindViewById<ListView>(Resource.Id.viewBarcodesListview);
            displayBarcodes = new List<string>();
            parsingIDs = new List<string>();
            connectionString = connectionString = @"Server=192.168.1.102;Database=CPSC499;User Id=cpsc499;Password=test;";

            //Load Listview
            ReloadListView();
        }

        public void ReloadListView() {
            try
            {
                //Clear Display and Info Lists
                displayBarcodes.Clear();
                parsingIDs.Clear();

                //Query SQL Server for Barcode Parsing Rules
                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("ListParsingRules", connection)) {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read()) {
                            //Populate List with Query Results
                            displayBarcodes.Add(string.Format("{0} - {1}", reader[1], reader[2]));
                            parsingIDs.Add(string.Format("{0}", reader[0]));                        
                        }
                    }
                    connection.Close();
                }

                //Assign Display List to List View
                barcodeListView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, displayBarcodes);
            }
            catch (System.Exception ex) {
                Toast.MakeText(ApplicationContext, "Failed to Load Listview: " + ex.Message, ToastLength.Long).Show();
            }

        }


    }
}