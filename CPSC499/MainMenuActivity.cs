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
    [Activity(Label = "Main Menu / DB Insert Test", Theme = "@style/AppTheme")]
    public class MainMenuActivity : ListActivity
    {
        //Screen Object Variables

        string connectionString = @"Server=192.168.1.102;Database=CPSC499;User Id=cpsc499;Password=test;";

        string[] mainMenuItems = {"Scan Cases", "Active BOL", "Manage Barcodes" };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ListAdapter = new ArrayAdapter<string>(this, Resource.Layout.MainMenu, mainMenuItems);  

            ListView.TextFilterEnabled = true;

            ListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                //Toast.MakeText(Application, ((TextView)args.View).Text, ToastLength.Short).Show();
                if (((TextView)args.View).Text == "Scan Cases") {
                   // Intent intent = new Intent(this, typeof(ScanCasesActivity));
                    Intent intent = new Intent(this, typeof(scanCases2Activity));

                    StartActivity(intent);
                }
            };
        }
    }
}