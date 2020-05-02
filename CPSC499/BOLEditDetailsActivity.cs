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
        Button btnEdit;
        Button btnDelete;

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
            btnEdit = FindViewById<Button>(Resource.Id.BOLEditDetailsBtnEdit);
            btnDelete = FindViewById <Button>(Resource.Id.BOLEditDetailsBtnDelete);

            //Disable Screen Text Fields
            editTextBOL.SetFocusable(ViewFocusability.NotFocusable);
            editTextCustomerName.SetFocusable(ViewFocusability.NotFocusable);
            editTextCustomerNbr.SetFocusable(ViewFocusability.NotFocusable);
            BOLNbr = ViewBOLActivity.BOLNbr;
            string selectedBOLNbr = ViewBOLActivity.BOLNbr;
            List<string> displayedInfo = new List<string>();
            List<string> itemNumbers = new List<string>();
            int selectedPosition = -1;

            btnNew.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
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
                Android.Widget.Toast.MakeText(this, listview.CheckedItemCount.ToString(), Android.Widget.ToastLength.Short).Show();

                selectedPosition = e.Position;
                Android.Widget.Toast.MakeText(this, listview.CheckedItemCount.ToString(), Android.Widget.ToastLength.Short).Show();

            };

            void reloadListview() {
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
            }
            void btnDelete_Click(object sender, EventArgs e) {
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
                    catch {
                        Android.Widget.Toast.MakeText(this, "Error: Failed to Delete Item", Android.Widget.ToastLength.Short).Show();
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
                else {
                    ItemNbr = null ;
                    intent.PutExtra("MyItem", ItemNbr);
                }
                StartActivity(intent);
                ItemNbr = null;

            }
            void btnEdit_Click(object sender, EventArgs e)
            {
                Android.Widget.Toast.MakeText(this, "Feature Not Implemented", Android.Widget.ToastLength.Short).Show();
                /*
                Intent intent = new Intent(this, typeof(BOLAddEditActivity));
                intent.PutExtra("MyItem", BOLNbr);
                isAddItem = false;
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
                */
            }
            void deleteItemsFromScansTable(string bolNbr, string itemNbr) { 
                string sql = @"Update Scans Set isActive = 0 Where BOL = '" + bolNbr + "' " +
                    "AND ItemNbr = '" + itemNbr + "'";
                try {
                    using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) 
                    {
                        using (SqlCommand cmd = new SqlCommand(sql, connection))
                        {
                            connection.Open();
                            int rowCount = cmd.ExecuteNonQuery();
                            if (rowCount == 0)
                            {
                                Android.Widget.Toast.MakeText(this, "Error: Delete Item From Scans Table", Android.Widget.ToastLength.Short).Show();
                            }
                            else {

                            }
                        }
                        connection.Close();
                    }
                }
                catch{ 
                
                }
            }
        }
    }
}