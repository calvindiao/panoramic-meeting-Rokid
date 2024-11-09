using Rokid.UXR;
using UnityEngine;
using UnityEngine.UI;

namespace Rokid.UXR.Demo
{
    public class TouchEventSample : MonoBehaviour
    {
        public bool UsePhone3Dof;//是否使用手机3Dof射线控制器

        void Start()
        {
            if (UsePhone3Dof)
            {
                RKVirtualController.Instance.Change(ControllerType.PHONE3DOF);
                Recenter();
            }
            else
            {
                RKVirtualController.Instance.Change(ControllerType.NORMAL);
                RKVirtualController.Instance.ConfigMenuView(true, true, true, true);
            }
        }

        private void Update()
        {
            if (RKInput.Instance.GetKeyDown(RKKeyEvent.KEY_RESET)) //长按虚拟面板 HOME键事件
            {
                Debug.Log("UXR-UNITY::KEY_RESET");
                Recenter();
            }
        }

        private void Recenter()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Google.XR.Cardboard.Api.Recenter();
            }
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus && UsePhone3Dof)
            {
                ThreeDofEventController.Instance.ResetImuAxisY();
            }
        }
    }
}


