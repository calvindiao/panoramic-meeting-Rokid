using Rokid.UXR;
using System;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

namespace Rokid.UXR.Demo
{
    public class TouchEventSample : MonoBehaviour
    {
        public Camera recoderCamera;
        public Text recordTxt;
        public Text capTxt;
        public Button recordButton;
        private bool isRecording = false;
        private float recordingTime;
        private bool active = true;
        [SerializeField]
        private LaserBeam laser;




        void Start()
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }
            RKVirtualController.Instance.Change(ControllerType.NORMAL);
            RKVirtualController.Instance.ConfigMenuView(true, true, true, true);
            Application.targetFrameRate = 60;
            if (recoderCamera == null)
            {
                recoderCamera = Camera.main;
            }



            recordButton.onClick.AddListener(() =>
            {
                string filename = DateTime.Now.ToString("yyyyMMddhhmmss") + "_recordVideo.mp4";
                string path = ScreenRecorder.Instance().UtilPath() + filename;
                if (isRecording == false)
                {
                    ScreenRecorder.Instance().StartRecordVideo(recoderCamera, 1280, 720, 30, () =>
                    {
                        recordButton.GetComponentInChildren<Text>().text = "Stop Record";
                        isRecording = true;
                    }, faild =>
                    {
                        Debug.Log("failed:" + faild);
                    }, path, false);
                }
                else
                {
                    ScreenRecorder.Instance().StopRecordVideo(() =>
                    {
                        isRecording = false;
                        recordButton.GetComponentInChildren<Text>().text = "Start Record";
                        capTxt.text = "Screen recording saved path:" + path;
                    });
                }
            });


        }

        private void Update()
        {
            if (RKInput.Instance.GetKeyDown(RKKeyEvent.KEY_RESET)) //长按虚拟面板 HOME键事件
            {
                Debug.Log("UXR-UNITY::KEY_RESET");
                Recenter();
            }

            if (isRecording)
            {
                recordingTime += Time.unscaledDeltaTime;
                recordTxt.text = string.Format("Recording  :{0}", recordingTime.ToString("0.0"));
            }
            else
            {
                recordingTime = 0;
            }

            if (RKInput.Instance.GetKeyDown(RKKeyEvent.KEY_MENU_BTN2))
            {
                Debug.Log("KEY_MENU_BTN2: Down...");
                // captureButton.onClick.Invoke();
                active = !active;
                laser.gameObject.SetActive(active);
                laser.GetComponentInChildren<MeshRenderer>(true).enabled = active;
            }

            if (RKInput.Instance.GetKeyDown(RKKeyEvent.KEY_MENU_BTN3))
            {
                Debug.Log("KEY_MENU_BTN3: Down...");
                recordButton.onClick.Invoke();
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

        }
    }
}


