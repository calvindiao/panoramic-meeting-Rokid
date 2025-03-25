using Rokid.UXR;
using System;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Rokid.UXR.Demo
{
    public class TouchEventSample : MonoBehaviour
    {
        public Camera recoderCamera;
        public Text recordTxt;
        public Text capTxt;
        public Button recordButton;
        public Button go360Button; // Button for navigating to 360 scene
        private bool isRecording = false;
        private float recordingTime;
        private bool active = true;
        [SerializeField]
        private LaserBeam laser;
        private string recordFilePath;
        private static bool hasStartedRecording = false; // 静态变量跟踪是否已经开始录制
        // Path to the Demo_360Stereo scene
        private string demo360ScenePath = "Assets/AVProVideo/Demos/Scenes/Demo_360Stereo";


        void Start()
        {

            if (Application.platform == RuntimePlatform.Android)
            {
                Google.XR.Cardboard.Api.UpdateScreenParams();
            }

            KeycodeInputModule.Instance.Initialized();

            if (LogManager.Instance == null)
            {
                GameObject logManagerObj = new GameObject("LogManager");
                logManagerObj.AddComponent<LogManager>();
            }
            
            LogManager.Instance.Log("应用启动");





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



            //
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
            Recenter();
            // Add listener for the 360 demo button
            // if (go360Button != null)
            // {
            //     go360Button.onClick.AddListener(GoTo360DemoScene);
            // }
            recordTxt.text = "SceneName:" + SceneManager.GetActiveScene().name;
            if (SceneManager.GetActiveScene().name == "uxr004-NormalController" && !hasStartedRecording)
            {
                StartRecording();
                hasStartedRecording = true;
            }

            // 保留原来的按钮功能，但可以隐藏按钮或不使用
            if (recordButton != null)
            {
                recordButton.onClick.AddListener(() =>
                {
                    if (isRecording == false)
                    {
                        StartRecording();
                    }
                    else
                    {
                        StopRecording();
                    }
                });
            }

            // recordButton.onClick.AddListener(() =>
            // {
            //     string filename = DateTime.Now.ToString("yyyyMMddhhmmss") + "_recordVideo.mp4";
            //     string path = ScreenRecorder.Instance().UtilPath() + filename;
            //     if (isRecording == false)
            //     {
            //         ScreenRecorder.Instance().StartRecordVideo(recoderCamera, 1280, 720, 30, () =>
            //         {
            //             recordButton.GetComponentInChildren<Text>().text = "Stop Record";
            //             isRecording = true;
            //         }, faild =>
            //         {
            //             Debug.Log("failed:" + faild);
            //         }, path, false);
            //     }
            //     else
            //     {
            //         ScreenRecorder.Instance().StopRecordVideo(() =>
            //         {
            //             isRecording = false;
            //             recordButton.GetComponentInChildren<Text>().text = "Start Record";
            //             capTxt.text = "Screen recording saved path:" + path;
            //         });
            //     }
            // });


        }
        private void StartRecording()
        {
            string filename = DateTime.Now.ToString("yyyyMMddhhmmss") + "_recordVideo.mp4";
            recordFilePath = ScreenRecorder.Instance().UtilPath() + filename;
            
            ScreenRecorder.Instance().StartRecordVideo(recoderCamera, 1280, 720, 30, () =>
            {
                if (recordButton != null)
                    recordButton.GetComponentInChildren<Text>().text = "Stop Record";
                isRecording = true;
                Debug.Log("Recording started: " + recordFilePath);
            }, failed =>
            {
                Debug.Log("Recording failed: " + failed);
            }, recordFilePath, false);
        }

        // 新增方法：停止录制
        private void StopRecording()
        {
            if (isRecording)
            {
                ScreenRecorder.Instance().StopRecordVideo(() =>
                {
                    isRecording = false;
                    if (recordButton != null)
                        recordButton.GetComponentInChildren<Text>().text = "Start Record";
                    if (capTxt != null)
                        capTxt.text = "Screen recording saved path:" + recordFilePath;
                    Debug.Log("Recording stopped and saved: " + recordFilePath);
                });
            }
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
                if (recordButton != null)
                    recordButton.onClick.Invoke();
            }
        }

        // Method to navigate to the 360 demo scene
        // private void GoTo360DemoScene()
        // {
        //     Debug.Log("Navigating to Demo_360Stereo scene");
        //     SceneManager.LoadScene("Demo_360Stereo");
        // }

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

        private void OnDestroy()
        {
            OfflineVoiceModule.Instance.RemoveInstruct(LANGUAGE.CHINESE, "回到桌面");
            OfflineVoiceModule.Instance.RemoveInstruct(LANGUAGE.CHINESE, "退出应用");
            OfflineVoiceModule.Instance.Commit();
            StopRecording();
        }
        private void OnDisable()
        {
            //StopRecording();
        }


        void OnReceive(string msg)
        {
            if (string.Equals(msg, "回到桌面")) Application.Quit();
        }
    }
}


