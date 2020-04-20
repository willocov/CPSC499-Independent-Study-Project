﻿using System;
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

namespace CPSC499
{
    [Activity(Label = "Barcode Rule Details")]
    public class BarcodeAddEditActivity : AppCompatActivity
    {
        //Layout Objects
        //Text Boxes
        EditText etCompany,
            etRuleName,
            etBarcodeLength,
            etItemNbrSP,
            etItemNbrLen,
            etLotNbrSP,
            etLotNbrLen,
            etWgtSP,
            etWgtLen,
            etWgtDP,
            etDateSP,
            etDateLen,
            etDateFormat;
        Button btnSave;

        bool isNewRule;
        bool isEditVisible;
        int parsingID;
        string connectionString = @"Server=192.168.1.102;Database=CPSC499;User Id=cpsc499;Password=test;";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BarcodeDetails);

            // Assign Layout Objects to Variables
            etCompany = FindViewById<EditText>(Resource.Id.editTextCompany);
            etRuleName = FindViewById<EditText>(Resource.Id.editTextName);
            etBarcodeLength = FindViewById<EditText>(Resource.Id.editTextBarcodeLength);
            etItemNbrSP = FindViewById<EditText>(Resource.Id.editTextItemNbrPS);
            etItemNbrLen = FindViewById<EditText>(Resource.Id.editTextItemNbrLen);
            etLotNbrSP = FindViewById<EditText>(Resource.Id.editTextLotNbrSP);
            etLotNbrLen = FindViewById<EditText>(Resource.Id.editTextLotNbrLen);
            etWgtSP = FindViewById<EditText>(Resource.Id.editTextWgtSP);
            etWgtLen = FindViewById<EditText>(Resource.Id.editTextWgtLen);
            etWgtDP = FindViewById<EditText>(Resource.Id.editTextWgtDP);
            etDateSP = FindViewById<EditText>(Resource.Id.editTextDateSP);
            etDateLen = FindViewById<EditText>(Resource.Id.editTextDateLen);
            etDateFormat = FindViewById<EditText>(Resource.Id.editTextDateFormat);
            btnSave = FindViewById<Button>(Resource.Id.btnSaveBarcodeDetails);

            //Detemine whether we are creating a new rule or editing existing rule
            int inputParsingID = ViewBarcodeActivity.ParsingID;
            parsingID = inputParsingID;
            if (inputParsingID == -1)
            {
                //If New Rule
                isNewRule = true;
                isEditVisible = false;
            }
            else {
                isNewRule = false;
                isEditVisible = true;
            }

            //Set fields readonly status and button text
            setButtonState(isEditVisible);

            //Populate Text Fields if viewing existing rule
            if (!isNewRule) {
                getRuleInfo(inputParsingID);
            }

            btnSave.Click += (s, e) =>
            {
                if (!isEditVisible) {
                    //Add or Update Rule
                    if (parsingID == -1)
                    {
                        try
                        {
                            AddParsingRule();
                            Toast.MakeText(ApplicationContext, "Success: Barcode Rule Added", ToastLength.Long).Show();

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            UpdateParsingRule(parsingID);
                            Toast.MakeText(ApplicationContext, "Success: Barcode Rule Updated", ToastLength.Long).Show();

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                flipButtonState();
                
            };

        }

        void AddParsingRule() {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("AddParsingRule", connection)) {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Company", SqlDbType.NVarChar).Value = etCompany.Text;
                        command.Parameters.Add("@ParsingRuleName", SqlDbType.NVarChar).Value = etRuleName.Text;
                        command.Parameters.Add("@BarcodeLength", SqlDbType.Int).Value = Int32.Parse(etBarcodeLength.Text);
                        command.Parameters.Add("@ItemNbrStartIndex", SqlDbType.Int).Value = Int32.Parse(etItemNbrSP.Text);
                        command.Parameters.Add("@ItemNbrLen", SqlDbType.Int).Value = Int32.Parse(etItemNbrLen.Text);
                        command.Parameters.Add("@DateStartIndex", SqlDbType.Int).Value = Int32.Parse(etDateSP.Text);
                        command.Parameters.Add("@DateLen", SqlDbType.Int).Value = Int32.Parse(etDateLen.Text);
                        command.Parameters.Add("@DateFormat", SqlDbType.NVarChar).Value = etDateFormat.Text;
                        command.Parameters.Add("@LotNbrStartIndex", SqlDbType.Int).Value = Int32.Parse(etLotNbrSP.Text);
                        command.Parameters.Add("@LotNbrLen", SqlDbType.Int).Value = Int32.Parse(etLotNbrLen.Text);
                        command.Parameters.Add("@WeightStartIndex", SqlDbType.Int).Value = Int32.Parse(etWgtSP.Text);
                        command.Parameters.Add("@WeightLen", SqlDbType.Int).Value = Int32.Parse(etWgtLen.Text);
                        command.Parameters.Add("@WeightDecimalPlace", SqlDbType.Int).Value = Int32.Parse(etWgtDP.Text);

                        command.ExecuteNonQuery();

                    }

                    connection.Close();
                }
            }
            catch (Exception ex) { 
                Toast.MakeText(ApplicationContext, "Failed to Add Barcode Rule: " + ex.Message, ToastLength.Long).Show();

            }
        }

        void UpdateParsingRule(int selectedParsingRule)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UpdateParsingRule", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@ParsingID", SqlDbType.Int).Value = selectedParsingRule;
                        command.Parameters.Add("@Company", SqlDbType.NVarChar).Value = etCompany.Text;
                        command.Parameters.Add("@ParsingRuleName", SqlDbType.NVarChar).Value = etRuleName.Text;
                        command.Parameters.Add("@BarcodeLength", SqlDbType.Int).Value = Int32.Parse(etBarcodeLength.Text);
                        command.Parameters.Add("@ItemNbrStartIndex", SqlDbType.Int).Value = Int32.Parse(etItemNbrSP.Text);
                        command.Parameters.Add("@ItemNbrLen", SqlDbType.Int).Value = Int32.Parse(etItemNbrLen.Text);
                        command.Parameters.Add("@DateStartIndex", SqlDbType.Int).Value = Int32.Parse(etDateSP.Text);
                        command.Parameters.Add("@DateLen", SqlDbType.Int).Value = Int32.Parse(etDateLen.Text);
                        command.Parameters.Add("@DateFormat", SqlDbType.NVarChar).Value = etDateFormat.Text;
                        command.Parameters.Add("@LotNbrStartIndex", SqlDbType.Int).Value = Int32.Parse(etLotNbrSP.Text);
                        command.Parameters.Add("@LotNbrLen", SqlDbType.Int).Value = Int32.Parse(etLotNbrLen.Text);
                        command.Parameters.Add("@WeightStartIndex", SqlDbType.Int).Value = Int32.Parse(etWgtSP.Text);
                        command.Parameters.Add("@WeightLen", SqlDbType.Int).Value = Int32.Parse(etWgtLen.Text);
                        command.Parameters.Add("@WeightDecimalPlace", SqlDbType.Int).Value = Int32.Parse(etWgtDP.Text);

                        command.ExecuteNonQuery();

                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(ApplicationContext, "Failed to update Barcode Rule: " + ex.Message, ToastLength.Long).Show();

            }
        }

        void setButtonState(bool state) {
            if (state)
            {
                //If Edit is visible, all fields will be readonly
                etCompany.FocusableInTouchMode = false;
                etRuleName.FocusableInTouchMode = false;
                etBarcodeLength.FocusableInTouchMode = false;
                etItemNbrSP.FocusableInTouchMode = false;
                etItemNbrLen.FocusableInTouchMode = false;
                etLotNbrSP.FocusableInTouchMode = false;
                etLotNbrLen.FocusableInTouchMode = false;
                etWgtSP.FocusableInTouchMode = false;
                etWgtLen.FocusableInTouchMode = false;
                etWgtDP.FocusableInTouchMode = false;
                etDateSP.FocusableInTouchMode = false;
                etDateLen.FocusableInTouchMode = false;
                etDateFormat.FocusableInTouchMode = false;

                btnSave.Text = "Edit";
            }
            else {
                //If Edit is not visible, then Save is visible. All fields will be editable 
                etCompany.FocusableInTouchMode = true;
                etRuleName.FocusableInTouchMode = true;
                etBarcodeLength.FocusableInTouchMode = true;
                etItemNbrSP.FocusableInTouchMode = true;
                etItemNbrLen.FocusableInTouchMode = true;
                etLotNbrSP.FocusableInTouchMode = true;
                etLotNbrLen.FocusableInTouchMode = true;
                etWgtSP.FocusableInTouchMode = true;
                etWgtLen.FocusableInTouchMode = true;
                etWgtDP.FocusableInTouchMode = true;
                etDateSP.FocusableInTouchMode = true;
                etDateLen.FocusableInTouchMode = true;
                etDateFormat.FocusableInTouchMode = true;
                
                btnSave.Text = "Save";
            }
        }

        void flipButtonState() {
            isEditVisible = !isEditVisible;
            setButtonState(isEditVisible);
        }

        void getRuleInfo(int parsingID) {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GetParsingRuleInfo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@ParsingRuleID", SqlDbType.Int).Value = parsingID;
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            etCompany.Text = reader[0].ToString();
                            etRuleName.Text = reader[1].ToString();
                            etBarcodeLength.Text = reader[2].ToString();
                            etItemNbrSP.Text = reader[3].ToString();
                            etItemNbrLen.Text = reader[4].ToString();
                            etLotNbrSP.Text = reader[5].ToString();
                            etLotNbrLen.Text = reader[6].ToString();
                            etWgtSP.Text = reader[7].ToString();
                            etWgtLen.Text = reader[8].ToString();
                            etWgtDP.Text = reader[9].ToString();
                            etDateSP.Text = reader[10].ToString();
                            etDateLen.Text = reader[11].ToString();
                            etDateFormat.Text = reader[12].ToString();

                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex) {
                Toast.MakeText(ApplicationContext, "Failed to Get Rule Info: " + ex.Message, ToastLength.Long).Show();
            }
        }


    }
}