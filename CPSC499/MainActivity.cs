using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using System.Data;
using System.Data.SqlClient;
using Android.Content;

namespace CPSC499
{
    [Activity(Label = "CPSC 499 Login Screen", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button loginButton;
        EditText usernameTextBox;
        EditText passwordTextBox;
        //Connection String When At Home
        enum USERLEVEL { NONE = 0, BASIC = 1, SUPERVISOR = 2, ADMIN = 3};

        public static int UserLevel { get; set; }
        public class LoginResults {
            private bool isSuccess;
            private int userLevel;

            public void setIsSuccess(bool input) {
                try
                { 
                    isSuccess = input;
                }
                catch (Exception ex) {
                    throw new Exception("Error Occured While Setting Success Level: " + ex);
                }
            }

            public bool getIsSuccess() {
                return isSuccess;
            }

            public void setUserLevel(int input) {
                try
                {
                    userLevel = input;
                }
                catch (Exception ex) {
                    throw new Exception("Error Occured While Setting User Level: " + ex);
                }
            }

            public int getUserLevel() {
                return userLevel;
            }
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.LoginScreen);

            loginButton = (Button)FindViewById(Resource.Id.ButtonLogin);
            usernameTextBox = (EditText)FindViewById(Resource.Id.EditTextUserName);
            passwordTextBox = (EditText)FindViewById(Resource.Id.EditTextPassword);
            loginButton.Click += (sender, e) =>
            {
                
                string userName = usernameTextBox.Text;
                string password = passwordTextBox.Text;

                /*
                LoginResults results = new LoginResults();
                results = Login(userName, password);
                
                if (results.getIsSuccess())
                {
                    //Launch New Page to Main Menu
                    Vibration.Vibrate();
                    Intent intent = new Intent(this, typeof(MainMenuActivity));
                    UserLevel = results.getUserLevel();
                    intent.PutExtra("Myitem", UserLevel);
                    StartActivity(intent);

                    //Intent intent = new Intent(this, typeof(ListViewTest));
                    //UserLevel = results.getUserLevel();
                    //intent.PutExtra("Myitem", UserLevel);
                    //StartActivity(intent);

                }
                else
                {
                    //Display error Message.
                    Toast.MakeText(ApplicationContext, "Login Failed.", ToastLength.Long).Show();
                }
                */

                //Skips user validation to speed up testing.
                //Vibration.Vibrate();
                Intent intent = new Intent(this, typeof(MainMenuActivity));
                UserLevel = 3;
                intent.PutExtra("Myitem", UserLevel);
                StartActivity(intent);
                StartActivity(intent);

            };
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        private LoginResults Login(string username, string password)
        {
            //Launch SQL Connection in a New Thread.
            LoginResults res = new LoginResults();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UserLogin", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password;
                        cmd.Parameters.Add("@UserType", SqlDbType.Int).Direction = ParameterDirection.Output; ;
                        cmd.Parameters["@UserType"].Value = 0;
                        cmd.Parameters.Add("@isSuccess", SqlDbType.Bit).Direction = ParameterDirection.Output; ;
                        cmd.Parameters["@isSuccess"].Value = 0;
                        connection.Open();
                        cmd.ExecuteNonQuery();

                        res.setIsSuccess(Convert.ToBoolean(cmd.Parameters["@isSuccess"].Value));
                        res.setUserLevel(Convert.ToInt32(cmd.Parameters["@UserType"].Value));
                        connection.Close();
                    }
                }
            }
            catch
            {
                res.setIsSuccess(false);
                res.setUserLevel(0);
                return res;
            }
           
            return res;
        }
    }
}