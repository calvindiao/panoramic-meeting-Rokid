using UnityEngine;
using UnityEngine.UI;
namespace Rokid.UXR.Demo
{
    public class HelloRokid : MonoBehaviour
    {
        public Text snText;
        public Text brightnessText;

        public Text deviceNameText;
        public Text changingNumberText;


        private float changingNumber = 0; // 变化的数字
        private float timer = 0f; // 计时器
        private float updateInterval = 1f; // 更新间隔（1秒更新一次）


        void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Google.XR.Cardboard.Api.UpdateScreenParams();
            }

            RKVirtualController.Instance.Change(ControllerType.NORMAL);
            snText.text = "Glass SN:dch " + UsbDeviceHelper.GetGlassSN();
            deviceNameText.text = "Glass Name: " + UsbDeviceHelper.GetGlassName();
        }

        void Update()
        {
            //only for debug, do not call these get methods in update.
            //snText.text = "Glass SN: " + UsbDeviceHelper.GetGlassSN();

            brightnessText.text = "Glass Brightness: " + changingNumber;

            timer += Time.deltaTime;
            if (timer >= updateInterval)
            {
                timer = 0f;
                changingNumber += Random.Range(1, 10); // 随机增加 1-10 之间的数
                changingNumberText.text = "Changing Number: " + changingNumber;
            }


        }
    }

}
