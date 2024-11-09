using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using Rokid.UXR;
using Rokid.UXR.UI;
namespace Rokid.UXR.Demo
{
    public class ScreenRecorderDemo : MonoBehaviour
    {
        public Camera recoderCamera;
        public Text recordTxt;
        public Text capTxt;
        public Button captureButton;
        public Button recordButton;
        public GameObject logo;
        private bool isRecording = false;
        private float recordingTime;
        private bool active = true;
        [SerializeField]
        private LaserBeam laser;

        private void Start()
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

            captureButton.onClick.AddListener(() =>
            {
                string filename = DateTime.Now.ToString("yyyyMMddhhmmss") + "_shotImage.png";
                string path = ScreenRecorder.Instance().UtilPath() + filename;
                ScreenRecorder.Instance().StartCapture(recoderCamera, 1920, 1080, path);
                capTxt.text = "截图保存的路径是:" + path;
            });

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
                        Debug.Log("录屏失败:" + faild);
                    }, path, false);
                }
                else
                {
                    ScreenRecorder.Instance().StopRecordVideo(() =>
                    {
                        isRecording = false;
                        recordButton.GetComponentInChildren<Text>().text = "Start Record";
                        capTxt.text = "录屏保存的路径:" + path;
                    });
                }
            });
        }

        private void Capture(string msg)
        {
            Debug.Log("截图...");
            captureButton.onClick.Invoke();
        }

        private void Update()
        {
            logo.transform.RotateAround(logo.transform.position, logo.transform.up, -2.0f);
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
    }

}
