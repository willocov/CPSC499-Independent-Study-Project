using System;
using System.Data;
using System.Data.SqlClient;

using Android.App;
using Android.OS;
using Android.Support.V7.App;
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
                    //Validate Fields
                    if (!ValidateTextFields()) {
                        //If Field Validation Fails
                        Toast.MakeText(ApplicationContext, "Error: Invalid Field Entry. Fields can not be partially filled and fields can not exceed barcode length.", ToastLength.Long).Show();
                        return;
                    }

                    //Add or Update Rule
                    if (parsingID == -1)
                    {
                        try
                        {
                            AddParsingRule();
                            Toast.MakeText(ApplicationContext, "Success: Barcode Rule Added", ToastLength.Long).Show();

                        }
                        catch
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
                        catch
                        {

                        }
                    }
                }
                else
                    flipButtonState();
            };
        }

        void AddParsingRule() {
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString)) {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("AddParsingRule", connection)) {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Company", SqlDbType.NVarChar).Value = etCompany.Text;
                        command.Parameters.Add("@ParsingRuleName", SqlDbType.NVarChar).Value = etRuleName.Text;
                        command.Parameters.Add("@BarcodeLength", SqlDbType.NVarChar).Value = etBarcodeLength.Text;
                        command.Parameters.Add("@ItemNbrStartIndex", SqlDbType.NVarChar).Value = etItemNbrSP.Text;
                        command.Parameters.Add("@ItemNbrLen", SqlDbType.NVarChar).Value = etItemNbrLen.Text;
                        command.Parameters.Add("@DateStartIndex", SqlDbType.NVarChar).Value = etDateSP.Text;
                        command.Parameters.Add("@DateLen", SqlDbType.NVarChar).Value = etDateLen.Text;
                        command.Parameters.Add("@DateFormat", SqlDbType.NVarChar).Value = etDateFormat.Text;
                        command.Parameters.Add("@LotNbrStartIndex", SqlDbType.NVarChar).Value = etLotNbrSP.Text;
                        command.Parameters.Add("@LotNbrLen", SqlDbType.NVarChar).Value = etLotNbrLen.Text;
                        command.Parameters.Add("@WeightStartIndex", SqlDbType.NVarChar).Value = etWgtSP.Text;
                        command.Parameters.Add("@WeightLen", SqlDbType.NVarChar).Value = etWgtLen.Text;
                        command.Parameters.Add("@WeightDecimalPlace", SqlDbType.NVarChar).Value = etWgtDP.Text;
                        command.ExecuteNonQuery();

                    }

                    connection.Close();
                }
            }
            catch (Exception ex) {
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Specify Action");
                alert.SetMessage(ex.Message);

                Dialog dialog = alert.Create();
                dialog.Show();

            }
        }

        void UpdateParsingRule(int selectedParsingRule)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UpdateParsingRule", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@ParsingID", SqlDbType.Int).Value = selectedParsingRule;
                        command.Parameters.Add("@Company", SqlDbType.NVarChar).Value = etCompany.Text;
                        command.Parameters.Add("@ParsingRuleName", SqlDbType.NVarChar).Value = etRuleName.Text;
                        command.Parameters.Add("@BarcodeLength", SqlDbType.NVarChar).Value = etBarcodeLength.Text;
                        command.Parameters.Add("@ItemNbrStartIndex", SqlDbType.NVarChar).Value = etItemNbrSP.Text;
                        command.Parameters.Add("@ItemNbrLen", SqlDbType.NVarChar).Value = etItemNbrLen.Text;
                        command.Parameters.Add("@DateStartIndex", SqlDbType.NVarChar).Value = etDateSP.Text;
                        command.Parameters.Add("@DateLen", SqlDbType.NVarChar).Value = etDateLen.Text;
                        command.Parameters.Add("@DateFormat", SqlDbType.NVarChar).Value = etDateFormat.Text;
                        command.Parameters.Add("@LotNbrStartIndex", SqlDbType.NVarChar).Value = etLotNbrSP.Text;
                        command.Parameters.Add("@LotNbrLen", SqlDbType.NVarChar).Value = etLotNbrLen.Text;
                        command.Parameters.Add("@WeightStartIndex", SqlDbType.NVarChar).Value = etWgtSP.Text;
                        command.Parameters.Add("@WeightLen", SqlDbType.NVarChar).Value = etWgtLen.Text;
                        command.Parameters.Add("@WeightDecimalPlace", SqlDbType.NVarChar).Value = etWgtDP.Text;

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

        bool ValidateTextFields() {
            //1. Check for invalid empty fields
            //2. Field does not exceed parsing length
            //3. Additional Validation

            //Check for invalid empty fields
            //LotNbr
            if ((etLotNbrSP.Text == "" && etLotNbrLen.Text != "") || (etLotNbrSP.Text != "" && etLotNbrLen.Text == ""))
                return false;
            //ItemNbr
            if ((etItemNbrSP.Text == "" && etItemNbrLen.Text != "") || (etItemNbrSP.Text != "" && etItemNbrLen.Text == ""))
                return false;
            //Weight
            if ((etWgtSP.Text == "" && etWgtLen.Text != "") || (etWgtSP.Text != "" && etWgtLen.Text == ""))
                return false;
            //Date
            if ((etDateSP.Text == "" && etDateLen.Text != "") || (etDateSP.Text != "" && etDateLen.Text == ""))
                return false;

            //Check that fields do not exceed parsing length
            int maxLength = Int32.Parse(etBarcodeLength.Text);

            if(etItemNbrSP.Text != "" && etItemNbrLen.Text != "")
                if (Int32.Parse(etItemNbrSP.Text) + Int32.Parse(etItemNbrLen.Text) - 1 > maxLength)
                    return false;

            if (etLotNbrSP.Text != "" && etLotNbrLen.Text != "")
                if (Int32.Parse(etLotNbrSP.Text) + Int32.Parse(etLotNbrLen.Text) - 1 > maxLength)
                    return false;

            if (etWgtSP.Text != "" && etWgtLen.Text != "")
                if (Int32.Parse(etWgtSP.Text) + Int32.Parse(etWgtLen.Text) - 1 > maxLength)
                    return false;

            if (etDateSP.Text != "" && etDateLen.Text != "")
                if (Int32.Parse(etDateSP.Text) + Int32.Parse(etDateLen.Text) - 1 > maxLength)
                    return false;

            //Check Weight Decimal Places
            if (etWgtDP.Text != "" && etWgtLen.Text != "")
                if (Int32.Parse(etWgtDP.Text) > Int32.Parse(etWgtLen.Text))
                    return false;

            //All Validation Passed, Return True
            return true;
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
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
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