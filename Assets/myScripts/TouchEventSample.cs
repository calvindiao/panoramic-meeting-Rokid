using Rokid.UXR;
using System;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using RenderHeads.Media.AVProVideo.Demos; // 添加引用以访问LookAround360
using RenderHeads.Media.AVProVideo; // 添加引用以访问MediaPlayer

namespace Rokid.UXR.Demo
{
    public class TouchEventSample : MonoBehaviour
    {
        public Camera recoderCamera;
        public Text recordTxt;
        public Button recordButton;
        private bool isRecording = false;
        private float recordingTime;
        private bool active = true;
        [SerializeField]
        private LaserBeam laser;
        private string recordFilePath;

        public GameObject worldCanvas;
        
        // 添加控制WorldCanvas的按钮引用
        public Button displayButton;
        
        // 添加一个变量跟踪WorldCanvas的显示状态
        private bool isWorldCanvasVisible = true;

        // 添加媒体播放器相关引用
        public LookAround360 lookAround360Controller;

        public GameObject mediaSphere; // 视频播放的球体

        [SerializeField] private InputField nameInputField;
        [SerializeField] private InputField urlInputField;

        // 跟踪媒体播放状态
        private bool isMediaPlaying = false;
        private bool isMediaPreloaded = false;

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
                Debug.Log("LogManagerObj: " + logManagerObj);
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

            // RKVirtualController.Instance.Change(ControllerType.NORMAL);
            // RKVirtualController.Instance.ConfigMenuView(true, true, true, true);
            Application.targetFrameRate = 60;
            if (recoderCamera == null)
            {
                recoderCamera = Camera.main;
            }
            Recenter();

            recordTxt.text = "SceneName:" + SceneManager.GetActiveScene().name;


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
            worldCanvas.SetActive(true);
            displayButton.onClick.AddListener(ToggleWorldCanvas);


            // 如果没有在Inspector中设置引用，尝试查找
            if (lookAround360Controller == null)
            {
                lookAround360Controller = FindObjectOfType<LookAround360>();
                if (lookAround360Controller == null)
                {
                    Debug.LogError("LookAround360 controller not found in the scene!");
                }
            }
            

            
            // 设置媒体播放按钮事件
            // if (toggleMediaButton != null)
            // {
            //     toggleMediaButton.onClick.AddListener(ToggleMediaPlayback);
                
            //     // 更新按钮文本初始状态
            //     Text buttonText = toggleMediaButton.GetComponentInChildren<Text>();
            //     if (buttonText != null)
            //     {
            //         buttonText.text = "Play Media";
            //     }
            // }
            
            // 初始状态 - 媒体球体隐藏
            if (mediaSphere != null)
            {
                mediaSphere.SetActive(false);
            }

            // 预加载媒体但不开始播放
            if (lookAround360Controller != null && lookAround360Controller.mediaPlayer != null)
            {
                // PreloadMedia();
            }

        }
        public void ToggleWorldCanvas()
        {
            if (worldCanvas != null)
            {
                isWorldCanvasVisible = !isWorldCanvasVisible;
                worldCanvas.SetActive(isWorldCanvasVisible);
                
                Debug.Log("WorldCanvas visibility: " + (isWorldCanvasVisible ? "Visible" : "Hidden"));
            }
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


        private void Recenter()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Google.XR.Cardboard.Api.Recenter();
            }
        }


        private void OnDestroy()
        {
            OfflineVoiceModule.Instance.RemoveInstruct(LANGUAGE.CHINESE, "回到桌面");
            OfflineVoiceModule.Instance.RemoveInstruct(LANGUAGE.CHINESE, "退出应用");
            OfflineVoiceModule.Instance.Commit();
            StopRecording();
        }

        // 添加切换媒体播放/暂停的方法
        // public void ToggleMediaPlayback()
        // {
        //     if (lookAround360Controller == null || lookAround360Controller.mediaPlayer == null)
        //     {
        //         Debug.LogError("Media player reference not set!");
        //         return;
        //     }
            
        //     isMediaPlaying = !isMediaPlaying;
            
        //     if (isMediaPlaying)
        //     {
        //         // 显示媒体球体
        //         if (mediaSphere != null)
        //         {
        //             mediaSphere.SetActive(true);
        //         }
                
        //         // 开始播放媒体
        //         if (isMediaPreloaded)
        //         {
        //             lookAround360Controller.ResumeMedia();
        //         }
        //         else
        //         {
        //             lookAround360Controller.PlayMedia();
        //             isMediaPreloaded = true;
        //         }
                
        //         // 更新按钮文本
        //         if (toggleMediaButton != null)
        //         {
        //             Text buttonText = toggleMediaButton.GetComponentInChildren<Text>();
        //             if (buttonText != null)
        //             {
        //                 buttonText.text = "Stop Media";
        //             }
        //         }
                
        //         Debug.Log("Media playback started");
        //     }
        //     else
        //     {
        //         // 停止播放媒体，但保持连接
        //         lookAround360Controller.PauseMedia();
                
        //         //隐藏媒体球体
        //         if (mediaSphere != null)
        //         {
        //             mediaSphere.SetActive(false);
        //         }
                
        //         // 更新按钮文本
        //         if (toggleMediaButton != null)
        //         {
        //             Text buttonText = toggleMediaButton.GetComponentInChildren<Text>();
        //             if (buttonText != null)
        //             {
        //                 buttonText.text = "Play Media";
        //             }
        //         }
                
        //         Debug.Log("Media playback stopped");
        //     }
        // }


        // 供联系人按钮直接调用：设置 URL → 播放 → 更新可见性与 UI
        public void PlayMediaFromUrl(string name, string url)
        {
            if (lookAround360Controller == null || lookAround360Controller.mediaPlayer == null)
            {
                Debug.LogError("PlayMediaFromUrl::Media player reference not set!");
                return;
            }
            

            if (nameInputField != null&&urlInputField != null)
            {
                nameInputField.text = name;
                urlInputField.text = url;
            }
            recordTxt.text = "Calling:"+"name: " + name + " url: " + url;
            // 1. 设置地址并播放
            lookAround360Controller.SetUrl(url);

            lookAround360Controller.PlayMedia();

            // 2. 确保 360 球体可见
            if (mediaSphere != null)
            {
                mediaSphere.SetActive(true);
            }

            ToggleWorldCanvas();

            isMediaPlaying   = true;
            // isMediaPreloaded = true;

            Debug.Log("▶ PlayMediaFromUrl: " + url);
        }



        // 添加预加载媒体的方法
        private void PreloadMedia()
        {
            if (lookAround360Controller == null || lookAround360Controller.mediaPlayer == null)
            {
                Debug.LogError("Media player reference not set!");
                return;
            }
            
            Debug.Log("Preloading media connection...");
            lookAround360Controller.PreloadMedia();
            isMediaPreloaded = true;
        }

        void OnReceive(string msg)
        {
            if (string.Equals(msg, "回到桌面")) Application.Quit();
        }
    }
}


