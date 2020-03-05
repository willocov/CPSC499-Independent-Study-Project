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

namespace CPSC499
{
    [Activity(Label = "Scan Cases", Theme = "@style/AppTheme")]
    public class ScanCasesActivity : AppCompatActivity, IPermissionListener
    {
        //Screen Object Variables
        Button InsertButton, ScanQR;
        ZXingScannerView scannerView;
        EditText Textbox1, TextBox2, TextBox3;
        string connectionString = @"Server=192.168.1.102;Database=CPSC499;User Id=cpsc499;Password=test;";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.ScanCases);

            //request permission
            Dexter.WithActivity(this)
                .WithPermission(Manifest.Permission.Camera)
                .WithListener(this)
                .Check();

            //Assign Variables here
            InsertButton = (Button)FindViewById(Resource.Id.button1);
            Textbox1 = (EditText)FindViewById(Resource.Id.editText1);
            TextBox2 = (EditText)FindViewById(Resource.Id.editText2);
            TextBox3 = (EditText)FindViewById(Resource.Id.editText3);
            ScanQR = (Button)FindViewById(Resource.Id.qr);
            scannerView = FindViewById<ZXingScannerView>(Resource.Id.zxscan);

            ScanQR.Click += (Sender, e) =>
            {
                scannerView.SetResultHandler(new MyResultHandler(this));
                scannerView.StartCamera();
                scannerView.Visibility = Android.Views.ViewStates.Invisible;

            };

            InsertButton.Click += (Sender, e) =>
            {
                string val1, val2, val3;
                val1 = Textbox1.Text;
                val2 = TextBox2.Text;
                val3 = TextBox3.Text;

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand("InsertIntoTestTable", conn))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("@val1", SqlDbType.NVarChar).Value = val1;
                            command.Parameters.Add("@val2", SqlDbType.NVarChar).Value = val2;
                            command.Parameters.Add("@val3", SqlDbType.NVarChar).Value = val3;
                            conn.Open();
                            command.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to insert to database: " + ex);
                }
            };
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
            scannerView.StopCamera();
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

            public MyResultHandler(ScanCasesActivity mainActivity)
            {
                this.scanCases = mainActivity;
            }

            public void HandleResult(ZXing.Result rawResult)
            {
                scanCases.Textbox1.Text = rawResult.Text;
                scanCases.scannerView.StopCamera();
                scanCases.scannerView.Visibility = Android.Views.ViewStates.Gone;
            }
        }
    }

}