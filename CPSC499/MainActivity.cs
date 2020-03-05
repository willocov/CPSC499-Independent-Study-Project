using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using System.Data;
using System.Data.SqlClient;
using Android.Content;
using Android.Support.Design.Widget;
using Xamarin.Essentials;

namespace CPSC499
{
    [Activity(Label = "CPSC 499 Login Screen", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button loginButton;
        EditText usernameTextBox;
        EditText passwordTextBox;
        //Connection String When At Home
        string connectionString = @"Server=192.168.1.102;Database=CPSC499;User Id=cpsc499;Password=test;";
        //string connectionString = @"Server=10.67.87.20;Database=CPSC499;User Id=cpsc499;Password=test;";


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            loginButton = (Button)FindViewById(Resource.Id.ButtonLogin);
            usernameTextBox = (EditText)FindViewById(Resource.Id.EditTextUserName);
            passwordTextBox = (EditText)FindViewById(Resource.Id.EditTextPassword);
            loginButton.Click += (sender, e) =>
            {
                /*
                string userName = usernameTextBox.Text;
                string password = passwordTextBox.Text;

                bool loginResult = false;
                loginResult = Login(userName, password);
                
                if (loginResult)
                {
                    //Launch New Page to Main Menu
                    Intent intent = new Intent(this, typeof(MainMenuActivity));
                    StartActivity(intent);
                }
                else
                {
                    //Display error Message.
                    Toast.MakeText(ApplicationContext, "Login Failed.", ToastLength.Long).Show();
                }
                */

                //Skips user validation to speed up testing.
                Vibration.Vibrate();
                Intent intent = new Intent(this, typeof(MainMenuActivity));
                StartActivity(intent);

            };
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        private bool Login(string username, string password)
        {
            //Launch SQL Connection in a New Thread.
            int loginSuccess = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UserLogin", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password;
                        cmd.Parameters.Add("@isSuccess", SqlDbType.Bit).Direction = ParameterDirection.Output; ;
                        cmd.Parameters["@isSuccess"].Value = 0;
                        connection.Open();
                        cmd.ExecuteNonQuery();

                        loginSuccess = Convert.ToInt32(cmd.Parameters["@isSuccess"].Value);
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            if (loginSuccess == 1)
            {
                return true;
            }
            return false;
        }
    }
}