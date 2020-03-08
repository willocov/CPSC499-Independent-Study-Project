using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace CPSC499
{
    [Activity(Label = "Scan Cases")]
    public class scanCases2Activity : AppCompatActivity 
    {
        //Declare Screen Objects
        Button btnBOL, btnBarcode, btnEnter, btnCancel, btnUndo;
        EditText txtBOL, txtCustomer, txtBarcode, txtTotalScans, txtItemNbr, txtItemDate, txtItemLot, txtItemWeight;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.scanCases2);

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

            //Make neccessary fields read only
           // txtCustomer.SetFocusable(false);


            //Put Object events here



        }
    }
}