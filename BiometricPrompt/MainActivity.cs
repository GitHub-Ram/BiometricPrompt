using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Runtime;
using System;
using Android.Views;

namespace BiometricPrompt
{
    [Activity(Label = "BiometricPrompt", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        private const int LOCK_REQUEST_CODE = 221;
        private const int SECURITY_SETTING_REQUEST_CODE = 233;
        Button button;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate {

                AuthenticateApp();
            };
        }

        private void AuthenticateApp()
        {
            //Get the instance of KeyGuardManager
            KeyguardManager keyguardManager = (KeyguardManager)GetSystemService(KeyguardService);

            //Check if the device version is greater than or equal to Lollipop(21)
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                //Create an intent to open device screen lock screen to authenticate
                //Pass the Screen Lock screen Title and Description
                Intent i = keyguardManager.CreateConfirmDeviceCredentialIntent("Unlock to Login.", "");
                try
                {
                    //Start activity for result
                    StartActivityForResult(i, LOCK_REQUEST_CODE);
                }
                catch (Exception e)
                {
                    //If some exception occurs means Screen lock is not set up please set screen lock
                    //Open Security screen directly to enable patter lock
                    Intent intent = new Intent(Android.Provider.Settings.ActionSecuritySettings);
                    try
                    {
                        //Start activity for result
                        StartActivityForResult(intent, SECURITY_SETTING_REQUEST_CODE);
                    }
                    catch (Exception ex)
                    {
                        //no security available
                    }
                }
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            switch (requestCode)
            {
                case LOCK_REQUEST_CODE:
                    if (resultCode == Result.Ok)
                    {
                        //If screen lock authentication is success update text
                        button.Text = "authentication is success";
                    }
                    else
                    {
                        //If screen lock authentication is failed update text
                        button.Text = "authentication is failed";
                    }
                    break;
                case SECURITY_SETTING_REQUEST_CODE:
                    //When user is enabled Security settings then we don't get any kind of RESULT_OK
                    //So we need to check whether device has enabled screen lock or not
                    if (IsDeviceSecure())
                    {
                        //If screen lock enabled show toast and start intent to authenticate user
                        Toast.MakeText(this, "Device is secure.", ToastLength.Long).Show();
                        AuthenticateApp();
                    }
                    else
                    {
                        //If screen lock is not enabled just update text
                        button.Text = "authentication is called";
                    }

                    break;
            }
        }

        private bool IsDeviceSecure()
        {
            KeyguardManager keyguardManager = (KeyguardManager)GetSystemService(KeyguardService);

            //this method only work whose api level is greater than or equal to Jelly_Bean (16)
            return Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean && keyguardManager.IsKeyguardSecure;

            //You can also use keyguardManager.isDeviceSecure(); but it requires API Level 23

        }

        //On Click of button do authentication again
        public void AuthenticateAgain(View view)
        {
            AuthenticateApp();
        }
    }
}

