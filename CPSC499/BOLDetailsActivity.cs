using System;
using System.Collections.Generic;
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
    [Activity(Label = "Order Details")]
    public class BOLDetailsActivity : AppCompatActivity
    {
        private ListView listview;
        EditText editTextBOL;
        EditText editTextCustomerName;
        EditText editTextCustomerNbr;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BOLDetails);
            listview = FindViewById<ListView>(Resource.Id.BOLDetailsListview);
            editTextBOL = FindViewById<EditText>(Resource.Id.BOLDetailsEditText1);
            editTextCustomerName = FindViewById<EditText>(Resource.Id.BOLDetailsEditText2);
            editTextCustomerNbr = FindViewById<EditText>(Resource.Id.BOLDetailsEditText3);

            editTextBOL.SetFocusable(ViewFocusability.NotFocusable);
            editTextCustomerName.SetFocusable(ViewFocusability.NotFocusable);
            editTextCustomerNbr.SetFocusable(ViewFocusability.NotFocusable);

            //Run SQL Query to get all BOLs
            string selectedBOLNbr = ViewBOLActivity.BOLNbr;
            List<string> displayedInfo = new List<string>();
            List<string> itemNumbers = new List<string>();
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    //Get General BOL information
                    using (SqlCommand cmd = new SqlCommand(@"
                        Select BOLNumber, CustomerName, CustomerNbr from BOLS Where BOLNumber = '" + selectedBOLNbr + "'"
                        , connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                editTextBOL.Text = reader[0].ToString();
                                editTextCustomerName.Text = reader[1].ToString();
                                editTextCustomerNbr.Text = reader[2].ToString();

                                //displayedInfo.Add(String.Format("{0}: [{1}/{2}]", reader[1], reader[2], reader[3]));
                                //itemNumbers.Add(String.Format("{0}", reader[0])); 
                            }
                        }
                        connection.Close();
                    }
                    //Get BOL Items and Scans information
                    using (SqlCommand cmd = new SqlCommand(@"
                        Select 
                            d.ItemNumber,
                            i.ItemName,
                            d.ItemsScanned,
                            d.ItemQuantity,
                            i.WhsLoc
                        From BOLDetails d
                        Left Join items i on i.ItemNumber = d.ItemNumber
                        Where d.BOLNumber = '" + selectedBOLNbr + "'"
                       , connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                displayedInfo.Add(String.Format("[{0}/{1}] - {2}", reader[2], reader[3], reader[1]));
                                itemNumbers.Add(String.Format("{0}", reader[0]));
                            }
                        }
                        connection.Close();
                    }
                }

            }
            catch (Exception ex)
            {

            }

            // Create your application here
            listview.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, displayedInfo);
            listview.ItemClick += (s, e) => {
                var t = displayedInfo[e.Position];
                Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Short).Show();
                //var selected = displayedInfo[e.Position];
                //// Intent intent = new Intent(this, typeof(ScanCasesActivity));
                //Intent intent = new Intent(this, typeof(BOLDetailsActivity));


                //StartActivity(intent);

            };

        }
    }
}