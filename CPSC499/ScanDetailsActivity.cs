using System;
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
    [Activity(Label = "Scan Details")]
    public class ScanDetailsActivity : AppCompatActivity
    {
        const string connectionString = @"Server=192.168.1.102;Database=CPSC499;User Id=cpsc499;Password=test;";
        EditText editTextBarcode;
        EditText editTextRuleID;
        EditText editTextBOLNbr;
        EditText editTextItemNbr;
        EditText editTextDate;
        EditText editTextLotNbr;
        EditText editTextWgt;
        EditText editTextScanTime;
        Button btnDelete;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ScanDetails);

            // Create your application here
            editTextBarcode = FindViewById<EditText>(Resource.Id.editTextScanDetailsBarcode);
            editTextRuleID = FindViewById<EditText>(Resource.Id.editTextScanDetailsRuleID);
            editTextBOLNbr = FindViewById<EditText>(Resource.Id.editTextScanDetailsBOLNbr);
            editTextItemNbr = FindViewById<EditText>(Resource.Id.editTextScanDetailsItemNbr);
            editTextDate = FindViewById<EditText>(Resource.Id.editTextScanDetailsDate);
            editTextLotNbr = FindViewById<EditText>(Resource.Id.editTextScanDetailsLotNbr);
            editTextWgt = FindViewById<EditText>(Resource.Id.editTextScanDetailsWgt);
            editTextScanTime = FindViewById<EditText>(Resource.Id.editTextScanDetailsScanTime);
            btnDelete = FindViewById<Button>(Resource.Id.btnScanDetailsDelete);

            editTextBarcode.SetFocusable(ViewFocusability.NotFocusable);
            editTextRuleID.SetFocusable(ViewFocusability.NotFocusable);
            editTextBOLNbr.SetFocusable(ViewFocusability.NotFocusable);
            editTextItemNbr.SetFocusable(ViewFocusability.NotFocusable);
            editTextDate.SetFocusable(ViewFocusability.NotFocusable);
            editTextLotNbr.SetFocusable(ViewFocusability.NotFocusable);
            editTextWgt.SetFocusable(ViewFocusability.NotFocusable);
            editTextScanTime.SetFocusable(ViewFocusability.NotFocusable);

            //Set ScanID
            int selectedScanID = ViewScansActivity.SelectedScanID;

            //Load Data Fields
            LoadTextFields();

            btnDelete.Click += (s, e) =>
            {
                DeleteScan();
            };
        }

        private void DeleteScan() {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    using (SqlCommand command = new SqlCommand("DeleteScan", connection)) {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ScanID", SqlDbType.Int).Value = ViewScansActivity.SelectedScanID;
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                DisplayMessage("Success", "Scan has been deleted.");
            }
            catch (Exception ex) {
                DisplayError(ex.Message);
            }
        }

        private void LoadTextFields() {
            try {
                ClearTextFields();

                using (SqlConnection connection = new SqlConnection(connectionString)) {
                    using (SqlCommand command = new SqlCommand("ViewScanDetails", connection)) {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@ScanID", System.Data.SqlDbType.Int).Value = ViewScansActivity.SelectedScanID;
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                editTextBarcode.Text = reader[0].ToString();
                                editTextRuleID.Text = reader[1].ToString();
                                editTextBOLNbr.Text = reader[2].ToString();
                                editTextItemNbr.Text = reader[3].ToString();
                                editTextDate.Text = reader[4].ToString();
                                editTextLotNbr.Text = reader[5].ToString();
                                editTextWgt.Text = reader[6].ToString();
                                editTextScanTime.Text = reader[7].ToString();
                            }
                        }
                        connection.Close();
                    }
                }

            }
            catch (Exception ex){
                DisplayError(ex.Message);
            }
        }

        private void DisplayError(string error) {
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle("Error");
            alert.SetMessage(error);

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void DisplayMessage(string title, string message) {
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle(title);
            alert.SetMessage(message);

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void ClearTextFields() {
            editTextBarcode.Text = "";
            editTextRuleID.Text = "";
            editTextBOLNbr.Text = "";
            editTextItemNbr.Text = "";
            editTextDate.Text = "";
            editTextLotNbr.Text = "";
            editTextWgt.Text = "";
            editTextScanTime.Text = "";
        }
    }
}