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
using Android.Support.V7.RecyclerView.Extensions;

namespace CPSC499
{
    [Activity(Label = "Main Menu", Theme = "@style/AppTheme")]
    public class MainMenuActivity : AppCompatActivity
    {
        private ListView listview;
        //string connectionString = @"Server=192.168.1.102;Database=CPSC499;User Id=cpsc499;Password=test;";

        static readonly string[] mainMenuItemsBasic = { "Scan Cases", "List BOL" };
        static readonly string[] mainMenuItemsSuper = { "Scan Cases", "List BOL", "Manage BOL" };
        static readonly string[] mainMenuItemsAdmin = { "Scan Cases", "List BOL", "Manage BOL", "Manage Barcodes", "Manage Scans" };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainMenu);
            listview = FindViewById<ListView>(Resource.Id.mainMenuListview);
            string[] mainMenuItems;
            switch (MainActivity.UserLevel) {
                case 1:
                    mainMenuItems = mainMenuItemsBasic;
                    break;
                case 2:
                    mainMenuItems = mainMenuItemsSuper;
                    break;
                case 3:
                    mainMenuItems = mainMenuItemsAdmin;
                    break;
                default:
                    mainMenuItems = mainMenuItemsBasic;
                    break;
            }

            listview.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, mainMenuItems);  

            Toast.MakeText(ApplicationContext, "User Level: " + MainActivity.UserLevel, ToastLength.Long).Show();


            listview.ItemClick += (s, e) => {
                var t = mainMenuItems[e.Position];
                Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Long).Show();
                
                //Toast.MakeText(Application, ((TextView)args.View).Text, ToastLength.Short).Show();
                if (mainMenuItems[e.Position].ToString() == "Scan Cases") {
                   // Intent intent = new Intent(this, typeof(ScanCasesActivity));
                    Intent intent = new Intent(this, typeof(scanCases2Activity));

                    StartActivity(intent);
                }
                else if (mainMenuItems[e.Position].ToString() == "List BOL")
                {
                    // Intent intent = new Intent(this, typeof(ScanCasesActivity));
                    Intent intent = new Intent(this, typeof(ViewBOLActivity));

                    StartActivity(intent);
                }
                
            };
        }
    }
}