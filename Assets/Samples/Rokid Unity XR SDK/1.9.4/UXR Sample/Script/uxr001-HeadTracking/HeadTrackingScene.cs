using UnityEngine;
using UnityEngine.Android;
using Rokid.UXR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Rokid.UXR.Demo
{
    public class HeadTrackingScene : MonoBehaviour
    {
        [SerializeField]
        private Text infoTxt;

        [SerializeField]
        private Text engineTxt;
        public Camera mainCamera;
        void Start()
        {
            AutoInjectComponent.AutoInject(transform, this);

            // Configures the app to not shut down the screen
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            //When the phone activity is paused, keep the glass screen alive for a longer time by Ignoring battery optimazion, but causes more power consuming
            //UsbDeviceHelper.RequestIgnoreBattery();

            if (Application.platform == RuntimePlatform.Android)
            {
                Google.XR.Cardboard.Api.UpdateScreenParams();
            }

            Recenter(); // reset glass 3dof
            RKVirtualController.Instance.Change(ControllerType.NORMAL);

            if (infoTxt == null)
            {
                infoTxt = GameObject.Find("InfoTxt").GetComponent<Text>();
            }
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                //Google.XR.Cardboard.Api.UpdateScreenParams();
            }

            if (RKInput.Instance.GetKeyDown(RKKeyEvent.KEY_RESET)) //长按虚拟面板 HOME键事件
            {
                Debug.Log("UXR-UNITY::KEY_RESET");
                Recenter();
            }

            infoTxt.text = string.Format("Position:{0}\r\nEuler:{1}\r\nRotation:{2}", mainCamera.transform.position.ToString("f3"), mainCamera.transform.rotation.eulerAngles.ToString(), mainCamera.transform.rotation.ToString("f3"));

            engineTxt.text = $"DebugInfo:{Google.XR.Cardboard.Api.getSlamState()},{Google.XR.Cardboard.Api.getDebugInfo()}";
        }

        private void OnEnable()
        {
            Debug.Log("-UXR-  HeadTrackingScene OnEnable");
            RegisterVoiceCommand();
        }

        private void OnBecameVisible()
        {
            Debug.Log("-UXR-  HeadTrackingScene OnBecameVisible ");
        }

        private void OnDisable()
        {
            Debug.Log("-UXR-  HeadTrackingScene OnDisable ");
            UnRegisterVoiceCommand();
        }

        private void RegisterVoiceCommand()
        {
            OfflineVoiceModule.Instance.AddInstruct(LANGUAGE.CHINESE, "重置", "chong zhi", this.gameObject.name, "Recenter");
        }

        private void UnRegisterVoiceCommand()
        {
            OfflineVoiceModule.Instance.RemoveInstruct(LANGUAGE.CHINESE, "重置");
        }

        private void Recenter()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Google.XR.Cardboard.Api.Recenter();
            }
        }
    }
}
