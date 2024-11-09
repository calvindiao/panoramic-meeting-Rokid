using System.Collections.Generic;
using UnityEngine;
using Rokid.UXR;
using GestureType = Rokid.UXR.GestureType;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rokid.UXR.UI;


/// <summary>
/// 记录手势识别的数量
/// </summary>

public class GestureTypeCount
{
    public int NoneCount;
    public int LeftCloseHandCount;
    public int LeftOpenHandCount;
    public int LeftClosePinchCount;
    public int LeftOpenPinchCount;
    public int LeftPalmCount;
    public int LeftBackCount;
    public int RightCloseHandCount;
    public int RightOpenHandCount;
    public int RightClosePinchCount;
    public int RightOpenPinchCount;
    public int RightPalmCount;
    public int RightBackCount;
}

public class GesForRayDemo : MonoBehaviour
{
    [SerializeField]
    private TextAsset testData;
    [SerializeField]
    private Text logText;
    [SerializeField]
    private Text logGesData;
    [SerializeField]
    private Button quitButton;
    [SerializeField]
    private Text leftGesInfoText;
    [SerializeField]
    private Text rightGesInfoText;
    [SerializeField]
    private RectTransform scrollView;
    [SerializeField]
    private GesRotate gesLaserBeam;

    private GestureResults results;

    private Vector2 viewPortPos;

    private GesRayStatus status;

    private GestureTypeCount gesTypeCount = new GestureTypeCount();

    private Dictionary<int, RectTransform> nodePointDict = new Dictionary<int, RectTransform>();

    void Start()
    {
        AutoInjectComponent.AutoInject(transform, this);
        // GesEventController.Instance.OnShowHandPoints += ShowHandPoints;
        GesEventController.OnTrackFailed += () =>
        {
            foreach (var node in nodePointDict.Values)
            {
                node.gameObject.SetActive(false);
            }
        };

        GesEventController.OnHandLost += (type, count) =>
        {
            int startIndex = HandType.LeftHand == type ? 0 : 21;
            for (int i = startIndex; i < startIndex + 21; i++)
            {
                if (nodePointDict.ContainsKey(i))
                {
                    nodePointDict[i].gameObject.SetActive(false);
                }
            }
        };

#if UNITY_EDITOR
        // cameraRig.GetComponent<HMDPoseTracker>().enabled = false;
        GesEventController.Instance.gestureCallback(testData.ToString());
#else
        // cameraRig.GetComponent<HMDPoseTracker>().enabled = true;
        // phoneScreenUI.gameObject.SetActive(true);
#endif
        // Debug.Log("Create UIBounds");
        // GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("UIBounds"));
        RKPointerLisener.OnPointerNothingUp += () =>
        {
            Debug.Log("====RayDemo==== Get Pointer Nothing");
        };

        RKPointerLisener.OnPointerDown += OnPointerDown;
        RKPointerLisener.OnPointerUp += OnPointerUp;
        RKPointerLisener.OnPointerDownFullData += OnPointerDownFullData;
        RKPointerLisener.OnPointerUpFullData += OnPointerUpFullData;

        quitButton.onClick.AddListener(() =>
        {
            // UsbDeviceHelper.SetGlass2DMode();
            Application.Quit();
        });

        UIManager.Instance().CreatePanel<CheckFuncTipPanel>(true);
    }

    private void AnalyzeGestureData(string gesData)
    {
        // usage of package "com.unity.nuget.newtonsoft-json"
        GestureResults results = JsonUtility.FromJson<GestureResults>(gesData);
        for (int i = 0; i < results.gestureResults.Count; i++)
        {
            GestureBean bean = results.gestureResults[i];
            HandType handType = (HandType)bean.hand_type;
            GestureType gesType = (GestureType)bean.gesture_type;
            int palmInt = bean.hand_orientation;
            switch (handType)
            {
                case HandType.RightHand:
                    switch (gesType)
                    {
                        case GestureType.CloseHand:
                            gesTypeCount.RightCloseHandCount++;
                            break;
                        case GestureType.OpenHand:
                            gesTypeCount.RightOpenHandCount++;
                            break;
                        case GestureType.ClosePinch:
                            gesTypeCount.RightClosePinchCount++;
                            break;
                        case GestureType.OpenPinch:
                            gesTypeCount.RightOpenPinchCount++;
                            break;
                        case GestureType.None:
                            gesTypeCount.NoneCount++;
                            break;
                    }
                    if (palmInt == 3)
                    {
                        gesTypeCount.RightPalmCount++;
                    }
                    else if (palmInt == 4)
                    {
                        gesTypeCount.RightBackCount++;
                    }
                    break;
                case HandType.LeftHand:
                    switch (gesType)
                    {
                        case GestureType.CloseHand:
                            gesTypeCount.LeftCloseHandCount++;
                            break;
                        case GestureType.OpenHand:
                            gesTypeCount.LeftOpenHandCount++;
                            break;
                        case GestureType.ClosePinch:
                            gesTypeCount.LeftClosePinchCount++;
                            break;
                        case GestureType.OpenPinch:
                            gesTypeCount.LeftOpenPinchCount++;
                            break;
                        case GestureType.None:
                            gesTypeCount.NoneCount++;
                            break;
                    }
                    if (palmInt == 3)
                    {
                        gesTypeCount.LeftPalmCount++;
                    }
                    else if (palmInt == 4)
                    {
                        gesTypeCount.LeftBackCount++;
                    }
                    break;
                case HandType.None:
                    gesTypeCount.NoneCount++;
                    break;
            }
        }
    }

    private void OnPointerUpFullData(PointerEventData obj)
    {
        // GameObject.Find("LogText (3)").GetComponent<Text>().text = $"OnPointerUp:{obj}";
    }

    private void OnPointerDownFullData(PointerEventData obj)
    {
        // GameObject.Find("LogText (3)").GetComponent<Text>().text = $"OnPointerDown:{obj}";
    }

    private void OnPointerUp(PointerEventData pointerData)
    {
        Text text = GameObject.Find("LogText (1)")?.GetComponent<Text>();
        if (text != null)
        {
            text.text = $" OnPointerUp interactiveObj:{pointerData.pointerCurrentRaycast.gameObject.name}";
        }
    }

    private void OnPointerDown(PointerEventData pointerData)
    {
        Text text = GameObject.Find("LogText (2)")?.GetComponent<Text>();
        if (text != null)
        {
            text.text = $" OnPointerDown interactiveObj:{pointerData.pointerCurrentRaycast.gameObject.name}";
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        results = JsonUtility.FromJson<GestureResults>(testData.ToString());
        viewPortPos = GesEventController.Instance.GetEventCamera().ScreenToViewportPoint(Input.mousePosition);
        status = GesEventController.Instance.GetRayStatus();
        SetDataGesType(results.gestureResults, HandType.LeftHand, GestureType.None);
        SetDataGesType(results.gestureResults, HandType.RightHand, GestureType.None);
        SetGesWirstPos(results.gestureResults, viewPortPos);

        #region  KeyCodeEvent

        if (Input.GetKeyDown(KeyCode.A))
        {
            GesEventController.Instance.SetRayStatus(GesRayStatus.LeftHandHold);
            gesLaserBeam.SetLeftHandRay();
        }

        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     GesEventController.Instance.SetRayStatus(GesRayStatus.RightHandHold);
        //     gesLaserBeam.SetRightHandRay();
        // }

        //移除右手信息
        if (Input.GetKey(KeyCode.L))
        {
            results.gestureResults.RemoveAt(1);
        }

        //移除左手信息
        if (Input.GetKey(KeyCode.R))
        {
            results.gestureResults.RemoveAt(0);
        }

        //移除所有信息
        if (Input.GetKey(KeyCode.Space))
        {
            results.gestureResults = null;
            results.gestureStatus = 0;
        }
        #endregion

        #region MouseEvent
        GestureType gesType = GestureType.None;


        if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.V) || Input.GetKey(KeyCode.X))
        {
            if (Input.GetMouseButtonDown(0))
            {
                gesType = GestureType.CloseHand;
            }
            else if (Input.GetMouseButton(0))
            {
                gesType = GestureType.CloseHand;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                gesType = GestureType.OpenHand;
            }
            else
            {
                gesType = GestureType.OpenHand;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                gesType = GestureType.ClosePinch;
            }
            else if (Input.GetMouseButton(0))
            {
                gesType = GestureType.ClosePinch;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                gesType = GestureType.OpenPinch;
            }
            else
            {
                gesType = GestureType.OpenPinch;
            }
        }

        //增加随机的手势
        if (Input.GetKey(KeyCode.Q))
        {
            gesType = GestureType.CloseHand;
        }
        if (Input.GetKey(KeyCode.W))
        {
            gesType = GestureType.ClosePinch;
        }
        if (Input.GetKey(KeyCode.E))
        {
            gesType = GestureType.OpenPinch;
        }
        if (Input.GetKey(KeyCode.A))
        {
            gesType = GestureType.CloseHand;
        }
        // if (Input.GetKey(KeyCode.S))
        // {
        //     gesType = GestureType.None;
        // }

        if (status == GesRayStatus.RightHandHold)
        {
            SetDataGesType(results.gestureResults, HandType.RightHand, gesType);
        }
        else if (status == GesRayStatus.LeftHandHold)
        {
            SetDataGesType(results.gestureResults, HandType.LeftHand, gesType);
        }
        #endregion
        // Debug.Log("====RayDemo====:" + gesType);
        GesEventController.Instance.gestureCallback(JsonUtility.ToJson(results));
        #region  Debug Event
        #endregion
#endif
        leftGesInfoText.text = GesEventController.Instance.GetGesture(HandType.LeftHand).ToString();
        rightGesInfoText.text = GesEventController.Instance.GetGesture(HandType.RightHand).ToString();
    }

    /// <summary>
    /// 设置数据类型
    /// </summary>
    /// <param name="gestures"></param>
    private void SetDataGesType(List<GestureBean> gestures, HandType hand, GestureType ges)
    {
        if (gestures == null)
        {
            return;
        }
        foreach (var gesture in gestures)
        {
            if (gesture.hand_type == (int)hand)
            {
                // Debug.Log("====RayDemo==== " + hand + "," + ges);
                gesture.gesture_type = (int)ges;
            }
        }
    }

    private void SetGesWirstPos(List<GestureBean> gestures, Vector2 viewPortPos)
    {
        if (gestures == null)
        {
            return;
        }
        foreach (var gesture in gestures)
        {
            gesture.hand_points[0].x = viewPortPos.x;
            gesture.hand_points[0].y = 1 - viewPortPos.y;
        }
    }

    #region  InfoTextLog
    public void OnEnter(string name)
    {
        logText.text = name + " is OnEnter";
    }

    public void OnExit(string name)
    {
        logText.text = name + " is OnExit";
    }

    public void OnClick(string name)
    {
        logText.text = name + " is OnClick";
    }

    public void OnDrag(string name)
    {
        logText.text = name + " is OnDrag";
    }

    public void OnDrop(string name)
    {
        logText.text = name + " is OnDrop";
    }
    #endregion
}
