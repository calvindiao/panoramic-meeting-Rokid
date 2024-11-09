using UnityEngine;
using UnityEngine.UI;
namespace Rokid.UXR.Demo
{
    public class HelloRokid : MonoBehaviour
    {
        public Text snText;
        public Text brightnessText;

        public Text deviceNameText;

        void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Google.XR.Cardboard.Api.UpdateScreenParams();
            }

            RKVirtualController.Instance.Change(ControllerType.NORMAL);
            snText.text = "Glass SN:1111 " + UsbDeviceHelper.GetGlassSN();
            deviceNameText.text = "Glass Name: " + UsbDeviceHelper.GetGlassName();
        }

        void Update()
        {
            //only for debug, do not call these get methods in update.
            //snText.text = "Glass SN: " + UsbDeviceHelper.GetGlassSN();

            brightnessText.text = "Glass Brightness: " + UsbDeviceHelper.GetGlassBrightness();
        }
    }

}
