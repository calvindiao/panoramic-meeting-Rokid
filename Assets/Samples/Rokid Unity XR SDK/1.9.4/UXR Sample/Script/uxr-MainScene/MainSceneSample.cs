using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

namespace Rokid.UXR.Demo
{
    public class MainSceneSample : MonoBehaviour
    {
        int minVersionForAirAppRequired = 190;//190: Air app v1.9.0
        void Start()
        {
            /*if (UsbDeviceHelper.GetGlassPID() == 5677) // camera permission is needed for 5677 glass
            {
                if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
                {
                    Permission.RequestUserPermission(Permission.Camera);
                }
            }*/

            if (Application.platform == RuntimePlatform.Android)
            {
                Google.XR.Cardboard.Api.UpdateScreenParams();
            }

            KeycodeInputModule.Instance.Initialized();

            OfflineVoiceModule.Instance.AddInstruct(LANGUAGE.CHINESE, "回到桌面", "hui dao zhuo mian", gameObject.name, "OnReceive");
            OfflineVoiceModule.Instance.AddInstruct(LANGUAGE.CHINESE, "退出应用", "tui chu yin yong", gameObject.name, "OnReceive");
            OfflineVoiceModule.Instance.Commit();

            RKVirtualController.Instance.Change(ControllerType.NORMAL);

            RKVirtualController.Instance.ConfigMenuView(true, true, true, true);

            UsbDeviceHelper.registerGlassSensorEvent();
            UsbDeviceHelper.setUSBStatusCallback();
            UsbDeviceHelper.OnPSensorUpdate += connect =>
            {
                Debug.Log("PSensor:" + connect);
            };

            UsbDeviceHelper.OnUSBConnect += () =>
            {
                Debug.Log("USBConnect !!!");
            };

            UsbDeviceHelper.OnUSBDisConnect += () =>
            {
                Debug.Log("USB Disconnect !!!");
            };

            //获取USB连接状态
            bool connect = UsbDeviceHelper.IsUSBConnect();
        }



        private void OnDestroy()
        {
            OfflineVoiceModule.Instance.RemoveInstruct(LANGUAGE.CHINESE, "回到桌面");
            OfflineVoiceModule.Instance.RemoveInstruct(LANGUAGE.CHINESE, "退出应用");
            OfflineVoiceModule.Instance.Commit();

        }

        void OnReceive(string msg)
        {
            if (string.Equals(msg, "回到桌面")) Application.Quit();
        }
    }
}

