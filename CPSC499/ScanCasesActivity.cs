using System;
using System.Data;
using System.Data.SqlClient;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using EDMTDev.ZXingXamarinAndroid;
using Com.Karumi.Dexter;
using Android;
using Com.Karumi.Dexter.Listener.Single;
using Com.Karumi.Dexter.Listener;
using Xamarin.Essentials;
using System.Text.RegularExpressions;

namespace CPSC499
{
    [Activity(Label = "Scan Cases")]
    public class ScanCasesActivity : AppCompatActivity, IPermissionListener
    {
        //Declare Screen Objects
        Button btnBOL, btnBarcode, btnEnter, btnCancel, btnUndo;
        EditText txtBOL, txtCustomer, txtBarcode, txtTotalScans, txtItemNbr, txtItemDate, txtItemLot, txtItemWeight;
        ZXingScannerView BOLScanner;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.ScanCases);


            Dexter.WithActivity(this)
                .WithPermission(Manifest.Permission.Camera)
                .WithListener(this)
                .Check();

            //Assign Variables Here
            //Buttons
            btnBOL = (Button)FindViewById(Resource.Id.btnBOL);
            btnBarcode = (Button)FindViewById(Resource.Id.btnBarcode);
            btnEnter = (Button)FindViewById(Resource.Id.btnEnter);
            btnCancel = (Button)FindViewById(Resource.Id.btnCancel);
            btnUndo = (Button)FindViewById(Resource.Id.btnUndo);

            //Text Boxes
            txtBOL = (EditText)FindViewById(Resource.Id.txtBOL);
            txtCustomer = (EditText)FindViewById(Resource.Id.txtCustomer);
            txtBarcode = (EditText)FindViewById(Resource.Id.txtBarcode);
            txtTotalScans = (EditText)FindViewById(Resource.Id.txtTotalScans);
            txtItemNbr = (EditText)FindViewById(Resource.Id.txtItemNbr);
            txtItemDate = (EditText)FindViewById(Resource.Id.txtDate);
            txtItemLot = (EditText)FindViewById(Resource.Id.txtLot);
            txtItemWeight = (EditText)FindViewById(Resource.Id.txtWeight);

            //QR Code Scanners
            BOLScanner = FindViewById<ZXingScannerView>(Resource.Id.qrBOL);
            
            //Put Object events here
            btnBOL.Click += (Sender, e) =>
            {
                txtBOL.Text = "";
                txtCustomer.Text = "";

                BOLScanner.SetResultHandler(new MyResultHandler(this, 0));
                BOLScanner.StartCamera();
                BOLScanner.Visibility = Android.Views.ViewStates.Visible;
            };
            btnBarcode.Click += (Sender, e) =>
            {

                BOLScanner.SetResultHandler(new MyResultHandler(this, 1));
                BOLScanner.StartCamera();
                BOLScanner.Visibility = Android.Views.ViewStates.Visible;
            };
            btnEnter.Click += (Sender, e) =>
            {
                ClearBarcodeFields();

                bool success = ParseBarcode(txtBarcode.Text, txtBOL.Text);
                if (success == true)
                {
                    //Clear Barcode Text and Display Success Message
                    txtBarcode.Text = "";
                    Vibration.Vibrate(250);
                }
                else {
                    //Display error Message.
                    Toast.MakeText(ApplicationContext, "Failed to Enter Barode.", ToastLength.Long).Show();
                    Vibration.Vibrate(250);
                }

                success = CheckBOLCompletion(txtBOL.Text);
                if (success == true) {
                    //BOL is Complete, mark as finished
                    Android.Support.V7.App.AlertDialog.Builder alertDiag = new Android.Support.V7.App.AlertDialog.Builder(this);
                    alertDiag.SetTitle("Finish BOL");
                    alertDiag.SetMessage("All items in this order have been verified. Would you like to mark this order complete?");
                    alertDiag.SetPositiveButton("Yes", (senderAlert, cargs) => {
                        MarkBOLComplete(txtBOL.Text);

                    });
                    alertDiag.SetNegativeButton("No", (senderAlert, args) => {
                        alertDiag.Dispose();
                    });
                    Dialog diag = alertDiag.Create();
                    diag.Show();

                }
            };
            btnCancel.Click += (Sender, e) =>
            {
                ClearBarcodeFields();
            };
            btnUndo.Click += (Sender, e) =>
            {
                ClearBarcodeFields();

                Android.Support.V7.App.AlertDialog.Builder alertDiag = new Android.Support.V7.App.AlertDialog.Builder(this);
                alertDiag.SetTitle("Confirm delete");
                alertDiag.SetMessage("Would you like to delete the last scan?");
                alertDiag.SetPositiveButton("Yes", (senderAlert, cargs) => {
                    Toast.MakeText(this, "Undoing Last Scan", ToastLength.Short).Show();
                });
                alertDiag.SetNegativeButton("No", (senderAlert, args) => {
                    alertDiag.Dispose();
                });
                Dialog diag = alertDiag.Create();
                diag.Show();
            };
        }

        public override void OnBackPressed()
        {
            //Closes camera
            if (BOLScanner.Visibility == ViewStates.Visible)
            {
                BOLScanner.StopCamera();
                BOLScanner.Visibility = Android.Views.ViewStates.Gone;
            }
            //Closes Scan Cases Screen
            else {
                base.OnBackPressed();
            }
        }


        public void ClearBarcodeFields() {
            //Sets all text fields to blank
            //txtBarcode.Text = "";
            txtTotalScans.Text = "";
            txtItemNbr.Text = "";
            txtItemDate.Text = "";
            txtItemLot.Text = "";
            txtItemWeight.Text = "";
        }

        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            // scannerView.SetResultHandler(new MyResultHandler(this));
            //scannerView.StartCamera();
        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {
        }

        protected override void OnDestroy()
        {
            BOLScanner.StopCamera();
            base.OnDestroy();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private class MyResultHandler : IResultHandler
        {
            private ScanCasesActivity scanCases;
            int textBox;
            const int customerType = 0;
            const int barcodeType = 1;
            public MyResultHandler(ScanCasesActivity mainActivity, int txtbox)
            {
                this.scanCases = mainActivity;
                textBox = txtbox;
            }

            public void HandleResult(ZXing.Result rawResult)
            {
                string result = Regex.Replace(rawResult.Text, @"\t|\n|\r", "");
                if (textBox == customerType)
                {
                    scanCases.txtBOL.Text = result;
                    scanCases.txtCustomer.Text = scanCases.GetCustomerName(result);
                }
                else if (textBox == barcodeType)
                {
                    scanCases.txtBarcode.Text = rawResult.Text;
                }
                scanCases.BOLScanner.StopCamera();
                scanCases.BOLScanner.Visibility = Android.Views.ViewStates.Gone;
                Vibration.Vibrate(250);
            }
        }
        public string GetCustomerName(string bolNbr)
        {
            //This function runs a SQL query to get the customer name based off the BOL number.
            string customerName = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("GetCustomerName", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@BOLNbr", SqlDbType.NVarChar, 250).Value = bolNbr;
                        command.Parameters.Add("@CustomerName", SqlDbType.NVarChar, 250).Direction = ParameterDirection.Output;
                        command.Parameters["@CustomerName"].Value = "";
                        connection.Open();
                        command.ExecuteNonQuery();
                        customerName = Convert.ToString(command.Parameters["@CustomerName"].Value);
                        connection.Close();

                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Failed to insert to database: " + ex);
            }

            return customerName;
        }

        public bool ParseBarcode(string barcode, string bol)
        {
            //This function runs a SQL query to get the customer name based off the BOL number.
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("ParseBarcode", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Barcode", SqlDbType.NVarChar, 250).Value = barcode;
                        command.Parameters.Add("@BOLNbr", SqlDbType.NVarChar, 200).Value = bol;
                        command.Parameters.Add("@TotalScans", SqlDbType.NVarChar, 250).Direction = ParameterDirection.Output;
                        command.Parameters.Add("@ItemNbr", SqlDbType.NVarChar, 250).Direction = ParameterDirection.Output;
                        command.Parameters.Add("@ItemDate", SqlDbType.NVarChar, 250).Direction = ParameterDirection.Output;
                        command.Parameters.Add("@ItemLot", SqlDbType.NVarChar, 250).Direction = ParameterDirection.Output;
                        command.Parameters.Add("@ItemWgt", SqlDbType.NVarChar, 250).Direction = ParameterDirection.Output;
                        command.Parameters.Add("@Success", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        command.Parameters["@TotalScans"].Value = null;
                        command.Parameters["@ItemNbr"].Value = null;
                        command.Parameters["@ItemDate"].Value = null;
                        command.Parameters["@ItemLot"].Value = null;
                        command.Parameters["@ItemWgt"].Value = null;
                        command.Parameters["@Success"].Value = null;
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();

                        txtTotalScans.Text = Convert.ToString(command.Parameters["@TotalScans"].Value);
                        txtItemNbr.Text = Convert.ToString(command.Parameters["@ItemNbr"].Value);
                        txtItemDate.Text = Convert.ToString(command.Parameters["@ItemDate"].Value);
                        txtItemLot.Text = Convert.ToString(command.Parameters["@ItemLot"].Value);
                        txtItemWeight.Text = Convert.ToString(command.Parameters["@ItemWgt"].Value);
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
                throw new Exception("Failed to insert to database: " + ex.Message);
            }

            return true;

        }

        private bool CheckBOLCompletion(string BOLNbr) {
            bool success = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
                    using (SqlCommand command = new SqlCommand("isBOLComplete", connection)) {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@BOLNbr", SqlDbType.NVarChar).Value = BOLNbr;
                        command.Parameters.Add("@Result", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        command.Parameters["@Result"].Value = 0;

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();

                        //Check Return value
                        if (Convert.ToBoolean(command.Parameters["@Result"].Value) == true) {
                            success = true;
                        }
                    }
                }
            }
            catch (Exception ex) {
                Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(this);
                alertDiag.SetTitle("Error");
                alertDiag.SetMessage(ex.Message);
                alertDiag.SetPositiveButton("OK", (senderAlert, args) => {
                    alertDiag.Dispose();
                });
                Dialog diag = alertDiag.Create();
                diag.Show();
            }

            return success;
        }

        private bool MarkBOLComplete(string BOLNbr) {
            bool result = false;            
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
                    using (SqlCommand command = new SqlCommand("MarkBOLComplete", connection)) {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@BOLNbr", SqlDbType.NVarChar).Value = BOLNbr;
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex) {
                Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(this);
                alertDiag.SetTitle("Error");
                alertDiag.SetMessage("Error Marking BOL Complete: " + ex.Message);
                alertDiag.SetPositiveButton("OK", (senderAlert, args) => {
                    alertDiag.Dispose();
                });

                Dialog diag = alertDiag.Create();
                diag.Show();
            }

            return result;
        }
    }

}

