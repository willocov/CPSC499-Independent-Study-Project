using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace CPSC499
{
    [Activity(Label = "Order Details")]
    public class BOLEditDetailsActivity : AppCompatActivity
    {
        private ListView listview;
        public static string BOLNbr { get; set; }
        public static string ItemNbr { get; set; }
        public static bool isAddItem { get; set; }

        EditText editTextBOL;
        EditText editTextCustomerName;
        EditText editTextCustomerNbr;
        Button btnNew;
        Button btnDelete;

        string selectedBOLNbr;
        List<string> displayedInfo;
        List<string> itemNumbers;
        int selectedPosition;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BOLDetailsEdit);

            BOLNbr = null;
            ItemNbr = null;
            isAddItem = true;

            //Assign Screen Objects
            listview = FindViewById<ListView>(Resource.Id.BOLEditDetailsListview);
            editTextBOL = FindViewById<EditText>(Resource.Id.BOLEditDetailsEditText1);
            editTextCustomerName = FindViewById<EditText>(Resource.Id.BOLEditDetailsEditText2);
            editTextCustomerNbr = FindViewById<EditText>(Resource.Id.BOLEditDetailsEditText3);
            btnNew= FindViewById<Button>(Resource.Id.BOLEditDetailsBtnAdd);
            btnDelete = FindViewById <Button>(Resource.Id.BOLEditDetailsBtnDelete);

            //Disable Screen Text Fields
            editTextBOL.SetFocusable(ViewFocusability.NotFocusable);
            editTextCustomerName.SetFocusable(ViewFocusability.NotFocusable);
            editTextCustomerNbr.SetFocusable(ViewFocusability.NotFocusable);
            BOLNbr = ViewBOLActivity.BOLNbr;

            selectedBOLNbr = ViewBOLActivity.BOLNbr;
            displayedInfo = new List<string>();
            itemNumbers = new List<string>();
            selectedPosition = -1;

            btnNew.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            // Create your application here
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
                            }
                        }
                        connection.Close();
                    }
                   
                }
                reloadListview();

            }
            catch
            {
              
            }
            //listview.
            listview.ChoiceMode = ChoiceMode.Single;
            // Create your application here
            listview.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItemActivated1, displayedInfo);
            listview.ItemClick += (s, e) => {
                var t = displayedInfo[e.Position];
                selectedPosition = e.Position;
            };


           
            
        }

        protected override void OnRestart()
        {
            base.OnResume(); // Always call the superclass first.
            reloadListview();

            listview.ChoiceMode = ChoiceMode.Single;
            // Create your application here
            listview.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItemActivated1, displayedInfo);
            listview.ItemClick += (s, e) => {
                var t = displayedInfo[e.Position];
                selectedPosition = e.Position;
            };
        }
        void reloadListview()
        {
            //Clear out item lists
            displayedInfo.Clear();
            itemNumbers.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
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
            catch
            {

            }
            listview.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, displayedInfo);

        }
        void btnDelete_Click(object sender, EventArgs e)
        {
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle("Delete Item?");
            alert.SetMessage("Are you sure you want to delete this item?");
            alert.SetButton("YES", (c, ev) =>
            {
                // Ok button click task  
                //Run sql query to delete item from bol
                try
                {
                    string sql =
                        @"Delete from BOLDetails Where BOLNumber = '" + BOLNbr + "' AND ItemNumber = '" + itemNumbers[selectedPosition] + "'";
                    using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                    {

                        using (SqlCommand cmd = new SqlCommand(sql, connection))
                        {
                            connection.Open();
                            int rowCount = cmd.ExecuteNonQuery();
                            if (rowCount == 0)
                            {
                                //If no rows are affected, 
                                Android.Widget.Toast.MakeText(this, "Error: Failed to Delete Item", Android.Widget.ToastLength.Short).Show();
                            }
                            else
                            {
                                //Item successfully added
                                var duration = TimeSpan.FromMilliseconds(250);
                                Vibration.Vibrate(duration);
                                Android.Widget.Toast.MakeText(this, "Item Deleted From BOL", Android.Widget.ToastLength.Short).Show();
                                deleteItemsFromScansTable(selectedBOLNbr, itemNumbers[selectedPosition]);
                                reloadListview();

                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            });
            alert.SetButton2("CANCEL", (c, ev) => { });
            alert.Show();
        }
        void btnAdd_Click(object sender, EventArgs e)
        {

            Intent intent = new Intent(this, typeof(BOLAddEditActivity));
            intent.PutExtra("MyItem", BOLNbr);
            isAddItem = true;
            intent.PutExtra("MyItem", isAddItem);
            if (selectedPosition != -1)
            {
                ItemNbr = itemNumbers[selectedPosition];
                intent.PutExtra("MyItem", ItemNbr);
            }
            else
            {
                ItemNbr = null;
                intent.PutExtra("MyItem", ItemNbr);
            }
            StartActivity(intent);
            ItemNbr = null;

        }

        void ErrorMessage(string title, string error)
        {

            Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(this);
            alertDiag.SetTitle("Error: " + title);
            alertDiag.SetMessage(error);
            alertDiag.SetPositiveButton("OK", (senderAlert, args) => {
                alertDiag.Dispose();
            });

            Dialog diag = alertDiag.Create();
            diag.Show();
        }
        void deleteItemsFromScansTable(string bolNbr, string itemNbr)
        {
            string sql = @"Update Scans Set isActive = 0 Where BOL = '" + bolNbr + "' " +
                "AND ItemNbr = '" + itemNbr + "'";
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            catch
            {

            }
        }
    }
}