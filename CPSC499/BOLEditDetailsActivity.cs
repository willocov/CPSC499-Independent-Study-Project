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
    [Activity(Label = "Edit BOL")]
    public class BOLEditDetailsActivity : AppCompatActivity
    {
        private ListView listview;
        public static string BOLNbr { get; set; }
        
        EditText editTextBOL;
        EditText editTextCustomerName;
        EditText editTextCustomerNbr;
        Button btnAdd;
        Button btnEdit;
        Button btnDelete;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BOLDetailsEdit);
            
            //Assign Screen Objects
            listview = FindViewById<ListView>(Resource.Id.BOLEditDetailsListview);
            editTextBOL = FindViewById<EditText>(Resource.Id.BOLEditDetailsEditText1);
            editTextCustomerName = FindViewById<EditText>(Resource.Id.BOLEditDetailsEditText2);
            editTextCustomerNbr = FindViewById<EditText>(Resource.Id.BOLEditDetailsEditText3);
            btnAdd = FindViewById<Button>(Resource.Id.BOLEditDetailsBtnAdd);
            btnEdit = FindViewById<Button>(Resource.Id.BOLEditDetailsBtnEdit);
            btnDelete = FindViewById <Button>(Resource.Id.BOLEditDetailsBtnDelete);

            //Disable Screen Text Fields
            editTextBOL.SetFocusable(ViewFocusability.NotFocusable);
            editTextCustomerName.SetFocusable(ViewFocusability.NotFocusable);
            editTextCustomerNbr.SetFocusable(ViewFocusability.NotFocusable);
            
            string selectedBOLNbr = ViewBOLActivity.BOLNbr;
            List<string> displayedInfo = new List<string>();
            List<string> itemNumbers = new List<string>();
            string connectionString = @"Server=192.168.1.102;Database=CPSC499;User Id=cpsc499;Password=test;";

            // Create your application here
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
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
            //listview.
            listview.ChoiceMode = ChoiceMode.Single;
            // Create your application here
            listview.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItemActivated1, displayedInfo);
            listview.ItemClick += (s, e) => {
                var t = displayedInfo[e.Position];
                //Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Short).Show();
                //listview.SetBackgroundColor(Android.Graphics.Color.Red);
                Android.Widget.Toast.MakeText(this, listview.CheckedItemCount.ToString(), Android.Widget.ToastLength.Short).Show();

            };
        }
    }
}