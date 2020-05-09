using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace CPSC499
{
    [Activity(Label = "Add Order Item")]
    public class BOLAddEditActivity : AppCompatActivity
    {
        //Variables to pass in
        //ItemNbr
        //BOL Nbr
        //Add or Edit
        EditText BOLNbr, CustomerName, CustomerCode, Quantity;
        Spinner Item;
        Button EnterBtn;

        List<string> displayedInfo = new List<string>();
        List<string> itemNumbers = new List<string>();
        string selectedBOLNbr = BOLEditDetailsActivity.BOLNbr;
        string selectedItemNbr = BOLEditDetailsActivity.ItemNbr;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BOLAddEditItem);

            //Assign screen objects
            Item = FindViewById<Spinner>(Resource.Id.BOLAddEditSpinner1);
            BOLNbr = FindViewById<EditText>(Resource.Id.BOLAddeditText1);
            CustomerName = FindViewById<EditText>(Resource.Id.BOLAddeditText2);
            CustomerCode = FindViewById<EditText>(Resource.Id.BOLAddeditText3);
            Quantity = FindViewById<EditText>(Resource.Id.BOLAddEditEditText1);
            EnterBtn = FindViewById<Button>(Resource.Id.BOLAddEditBtn);
          

            //Make Edit Texts readonly
            BOLNbr.SetFocusable(ViewFocusability.NotFocusable);
            CustomerName.SetFocusable(ViewFocusability.NotFocusable);
            CustomerCode.SetFocusable(ViewFocusability.NotFocusable);

            EnterBtn.Click += btnEnter_Click;
            void btnEnter_Click(object sender, EventArgs e) {
                try
                {
                    //Validate quantity
                    int parsedQuantity;
                    bool isNumeric = int.TryParse(Quantity.Text.ToString(), out parsedQuantity);
                    if (isNumeric)
                    {
                        string sql = @"
                        Insert into BOLDetails(
                            BOLNumber, 
                            ItemNumber, 
                            ItemQuantity, 
                            ItemsScanned)
                        Values(
                            '" + BOLNbr.Text + "'," +
                                "'" + selectedItemNbr + "', " +
                                 parsedQuantity.ToString() + ", " +
                                 0.ToString() + ")";
                        //Run SQL Query
                        using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                        {

                            using (SqlCommand cmd = new SqlCommand(sql, connection))
                            {
                                connection.Open();
                                int rowCount = cmd.ExecuteNonQuery();
                                if (rowCount == 0)
                                {
                                    //If no rows are affected, 
                                    Android.Widget.Toast.MakeText(this, "Error: Failed to Add Item", Android.Widget.ToastLength.Short).Show();
                                }
                                else
                                {
                                    //Item successfully added
                                    var duration = TimeSpan.FromMilliseconds(250);
                                    Vibration.Vibrate(duration);
                                    Android.Widget.Toast.MakeText(this, "Item Added To BOL", Android.Widget.ToastLength.Short).Show();

                                    //Reset Screen Fields
                                    Item.SetSelection(0);
                                    Quantity.Text = "";

                                }
                            }
                        }

                        Finish();
                    }
                    else
                    {
                        Android.Widget.Toast.MakeText(this, "Invalid Quantity", Android.Widget.ToastLength.Short).Show();
                    }
                }
                catch {
                    Android.Widget.Toast.MakeText(this, "Error: Failed to Add Item", Android.Widget.ToastLength.Short).Show();

                }
            }
            
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    //Get General BOL information
                    using (SqlCommand cmd = new SqlCommand(@"
                        Select CustomerName, CustomerNbr from BOLS Where BOLNumber = '" + selectedBOLNbr + "'"
                        , connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            BOLNbr.Text = selectedBOLNbr;
                            
                            while (reader.Read())
                            {  
                                CustomerName.Text = reader[0].ToString();
                                CustomerCode.Text = reader[1].ToString();
                            }
                        }
                        connection.Close();
                    }
                    //Load Items into Spinner
                    using (SqlCommand cmd = new SqlCommand(@"
                        Select 
                            ItemNumber,
                            ItemName
                        From Items"
                       , connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                displayedInfo.Add(String.Format("{0} - {1}", reader[0], reader[1]));
                                itemNumbers.Add(String.Format("{0}", reader[0]));
                            }
                        }
                        connection.Close();
                    }
                }
                ArrayAdapter adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, displayedInfo);
                Item.Adapter = adapter;
                int position = 0;
                Item.ItemSelected += Spinner1_ItemSelected;
                if (selectedItemNbr != null)
                {
                    for (int i = 0; i < itemNumbers.Count; i++) {
                        if (itemNumbers[i] == selectedItemNbr) {
                            position = i;
                        }
                    }
                    Item.SetSelection(position);
                }

            }
            catch { 
            
            }
            
        }
        private void Spinner1_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
            {
                selectedItemNbr = itemNumbers[e.Position];
                string s1 = displayedInfo[e.Position].ToString();
            }
    }
}