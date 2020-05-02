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
        List<int> parsingIDs;
        Button btnNew;

        public static int ParsingID { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ViewBarcodes);

            // Create your application here
            barcodeListView = FindViewById<ListView>(Resource.Id.viewBarcodesListview);
            btnNew = FindViewById<Button>(Resource.Id.btnBarcodeAdd);
            displayBarcodes = new List<string>();
            parsingIDs = new List<int>();
            
            //Load Listview
            ReloadListView();

            //Handle Item Click from List
            barcodeListView.ItemClick += (s, e) =>
            {
                var t = displayBarcodes[e.Position];
                var selected = displayBarcodes[e.Position];
                Intent intent = new Intent(this, typeof(BarcodeAddEditActivity));
                ParsingID = parsingIDs[e.Position];
                intent.PutExtra("MyItems", ParsingID);
                StartActivity(intent);

                this.Recreate();
            };

            btnNew.Click += (s, e) =>
            {
                ParsingID = -1;
                Intent intent = new Intent(this, typeof(BarcodeAddEditActivity));
                intent.PutExtra("MyItems", ParsingID);
                StartActivity(intent);

                this.Recreate();

            };
        }

        public void ReloadListView() {
            try
            {
                //Clear Display and Info Lists
                displayBarcodes.Clear();
                parsingIDs.Clear();

                //Query SQL Server for Barcode Parsing Rules
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("ListParsingRules", connection)) {
                        command.CommandType = CommandType.StoredProcedure;
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read()) {
                            //Populate List with Query Results
                            displayBarcodes.Add(string.Format("{0}\n{1} - {2}", reader[2], reader[0], reader[1]));
                            parsingIDs.Add(Int32.Parse(reader[0].ToString()));                        
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

        protected override void OnRestart()
        {
            base.OnResume(); // Always call the superclass first.
            ReloadListView();
        }
    }
}