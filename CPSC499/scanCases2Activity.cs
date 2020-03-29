using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Android.App;
using Android.Content;
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

namespace CPSC499
{
    [Activity(Label = "Scan Cases")]
    public class scanCases2Activity : AppCompatActivity, IPermissionListener
    {
        //Declare Screen Objects
        Button btnBOL, btnBarcode, btnEnter, btnCancel, btnUndo;
        EditText txtBOL, txtCustomer, txtBarcode, txtTotalScans, txtItemNbr, txtItemDate, txtItemLot, txtItemWeight;
        ZXingScannerView BOLScanner;
        string connectionString = @"Server=192.168.1.102;Database=CPSC499;User Id=cpsc499;Password=test;";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.scanCases2);


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
            //request permission



            //Make neccessary fields read only



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
                alertDiag.SetNegativeButton("Yes", (senderAlert, args) => {
                    alertDiag.Dispose();
                });
                Dialog diag = alertDiag.Create();
                diag.Show();
            };


        }

       
   

        public void ClearBarcodeFields() {
            txtBarcode.Text = "";
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
            private scanCases2Activity scanCases;
            int textBox;
            const int customerType = 0;
            const int barcodeType = 1;
            public MyResultHandler(scanCases2Activity mainActivity, int txtbox)
            {
                this.scanCases = mainActivity;
                textBox = txtbox;
            }

            public void HandleResult(ZXing.Result rawResult)
            {
                if (textBox == customerType)
                {
                    scanCases.txtBOL.Text = rawResult.Text;
                    scanCases.txtCustomer.Text = scanCases.GetCustomerName(rawResult.Text);
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("GetCustomerName", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@BOLNbr", SqlDbType.NVarChar, 250).Value = bolNbr;
                        command.Parameters.Add("@CustomerName", SqlDbType.NVarChar, 250).Direction = ParameterDirection.Output;
                        command.Parameters["@CustomerName"].Value = null;
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();

                        customerName = Convert.ToString(command.Parameters["@CustomerName"].Value);
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
                using (SqlConnection connection = new SqlConnection(connectionString))
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
                throw new Exception("Failed to insert to database: " + ex);
            }

            return true;

        }
    }

}

