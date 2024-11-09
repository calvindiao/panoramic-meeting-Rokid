using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using Rokid.UXR.UI;

namespace Rokid.UXR.Demo
{
    public class UVCCameraDemo : MonoBehaviour
    {
        public Button takePictureButton;
        public Button startRecordButton;
        public Text infoTxt;
        public Text fpsTxt;
        private Texture2D texture = null;
        public RawImage viewDisplay;
        private int previewWidth = 640;
        private int previewHeight = 480;
        /// <summary>
        /// 纹理数据
        /// </summary>
        private byte[] texData;
        private float time;

        /// <summary>
        /// 开始录像
        /// </summary>
        private bool beginRecord;

        private float elapsedTime;

        /// <summary>
        /// 计算fps使用
        /// </summary>
        private float fpsElapsedTime;

        /// <summary>
        /// 相机渲染帧率
        /// </summary>
        private int fps;

        private bool init = false;

        void Start()
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                Google.XR.Cardboard.Api.UpdateScreenParams();
            }


            takePictureButton.onClick.AddListener(() =>
            {
                UVCCameraBridge.Instance.TakePicture(null, path =>
                {
                    infoTxt.text = string.Format("照片保存路径是：" + path);
                });
            });

            startRecordButton.onClick.AddListener(() =>
            {
                if (!beginRecord)
                {
                    UVCCameraBridge.Instance.StartRecord(null, () =>
                    {
                        beginRecord = true;
                    });
                    startRecordButton.GetComponentInChildren<Text>().text = "Stop Record";
                }
                else
                {
                    UVCCameraBridge.Instance.StopRecord();
                    infoTxt.text = "结束录像";
                    beginRecord = false;
                    startRecordButton.GetComponentInChildren<Text>().text = "Start Record";
                }
            });
            UIManager.Instance().CreatePanel<CheckFuncTipPanel>(true);

            Debug.Log("StartCameraPreview!!!");
            //开启相机预览
            UVCCameraBridge.Instance.StartPreview(previewWidth, previewHeight);
        }


        void Update()
        {
            fpsElapsedTime += Time.deltaTime;
            if (UVCCameraBridge.Instance.IsPreviewing())
            {
                if (init == false)
                {
                    init = true;
                    Debug.Log("UVCCameraDemo:" + "create texture");
                    int width = UVCCameraBridge.Instance.GetWidth();
                    int height = UVCCameraBridge.Instance.GetHeight();
                    Debug.Log($"width:{width},height:{height}");
                    texData = new byte[width * height * 4];
                    texture = new Texture2D(width, height, TextureFormat.BGRA32, false);
                }
                if (NativeInterface.NativeAPI.haveDataInBufferC())
                {
                    Debug.Log("UVCCameraDemo:" + "update texture");
                    NativeInterface.NativeAPI.readDataFromBufferC(texData);
                    Debug.Log("UVCCameraDemo:" + "data.length :" + texData.Length);
                    texture.LoadRawTextureData(texData);
                    texture.Apply();
                    viewDisplay.texture = texture;
                    NativeInterface.NativeAPI.recycleBufferC();
                    //刷新fps
                    if (fpsElapsedTime <= 1.0f)
                    {
                        fps++;
                    }
                    else
                    {
                        fpsElapsedTime = 0;
                        //刷新帧率显示
                        fpsTxt.text = "Camera FPS:" + fps.ToString();
                        fps = 0;
                    }
                }
                else
                {
                    Debug.Log("UVCCameraDemo: no data");
                }
            }
            else
            {
                Debug.Log("UVCCameraDemo: no previewing");
            }
            if (beginRecord)
            {
                time += Time.deltaTime;
                infoTxt.text = "开始录像:" + (int)time;
            }
            else
            {
                time = 0;
            }
        }
        private void OnDestroy()
        {
            // Debug.Log("停止预览!!!");
            UVCCameraBridge.Instance.StopPreview();
            UVCCameraBridge.Instance.deInit();
        }
    }
}
