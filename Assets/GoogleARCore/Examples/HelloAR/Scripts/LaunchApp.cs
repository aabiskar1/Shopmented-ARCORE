using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GoogleARCore.Examples.HelloAR
{
    public class LaunchApp : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            LaunchAppMessage();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void LaunchAppMessage()
        {
            string bundleId = "com.example.sidenavtest";
            bool fail = false;
            string message = "Message from AR app";
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject launchIntent = null;

            try
            {
                launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
                launchIntent.Call<AndroidJavaObject>("putExtra", "arguments", message);
            }
            catch (System.Exception e)
            {
                fail = true;
            }

            if (fail)
            {
                Application.OpenURL("https://google.com");
            }
            else
            {
                ca.Call("startActivity", launchIntent);
            }
            up.Dispose();
            ca.Dispose();
            packageManager.Dispose();
            launchIntent.Dispose();
        }

    }
}