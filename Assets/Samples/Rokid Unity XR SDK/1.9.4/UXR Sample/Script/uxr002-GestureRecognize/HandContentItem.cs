using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rokid.UXR;
using static Rokid.UXR.RKGestureHelper;
namespace Rokid.UXR.Demo
{
    public class HandContentItem : MonoBehaviour
    {
        [SerializeField]
        public RectTransform targetRect;
        [SerializeField]
        public Text handType;
        [SerializeField]
        public Text gesType;
        [SerializeField]
        public Text gesStatus;
        [SerializeField]
        public Text orientation;
        [SerializeField]
        public Text fingerUp;

        private GameObject gestureNode;
        private static SelectStatusEnum logoSelectStatus = SelectStatusEnum.NONE;

        /// <summary>
        /// 节点颜色缓存
        /// </summary>
        /// <typeparam name="int"></typeparam>
        /// <typeparam name="Color"></typeparam>
        /// <returns></returns>
        public Dictionary<int, Color> fingerIndexColorDict = new Dictionary<int, Color>(){
        {0,Color.gray},
        {1,Color.red},
        {6,Color.red},
        {7,Color.red},
        {8,Color.red},
        {2,Color.green},
        {9,Color.green},
        {10,Color.green},
        {11,Color.green},
        {3,Color.yellow},
        {12,Color.yellow},
        {13,Color.yellow},
        {14,Color.yellow},
        {4,Color.blue},
        {15,Color.blue},
        {16,Color.blue},
        {17,Color.blue},
        {5,Color.white},
        {18,Color.white},
        {19,Color.white},
        {20,Color.white},
    };
        public void Initialized()
        {
            AutoInjectComponent.AutoInject(transform, this);
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="gestureBean"></param>
        /// <param name="gesNode"></param>
        public void UpdateInfo(GestureBean gestureBean, GameObject gesNode, Transform logo)
        {
            if (gestureBean.gesture_type == (int)GestureType.None)
            {
                this.gameObject.SetActive(false);
                return;
            }
            this.gameObject.SetActive(true);
            this.gestureNode = gesNode;
            gesType.text = GetGestureTypeText(gestureBean.gesture_type);
            gesStatus.text = GetGestureStatusText(gestureBean.gesture_status);
            handType.text = gestureBean.hand_type == 1 ? "RightHand" : "LeftHand";
            fingerUp.text = gestureBean.index_finger_up == 1 ? "FingerUp: True" : "FingerUp: False";
            orientation.text = gestureBean.hand_orientation == 3 ? "Orientaion: Palm" : "Orientaion: Back";
            float width = (float)(1920 * (gestureBean.x2 - gestureBean.x1)) > 540 ? (float)(1920 * (gestureBean.x2 - gestureBean.x1)) : 540;
            targetRect.sizeDelta = new Vector2(width, (float)(1080 * (gestureBean.y2 - gestureBean.y1)) + 120);
            float position_x = (float)(1920 * (gestureBean.x2 + gestureBean.x1 - 1) / 2);
            float position_y = (float)((1080 * (1 - gestureBean.y2 - gestureBean.y1) + 120) / 2);
            targetRect.localPosition = new Vector3(position_x, position_y);
            UpdateHandPointsPosition(gestureBean.hand_points, transform);

            //更新logo位置...
            if (gestureBean.hand_type == 1)
            {
                //right hand
                if (gestureBean.gesture_type == (int)GestureType.ClosePinch && logoSelectStatus != SelectStatusEnum.LEFT_HAND_SELECT)
                {
                    logoSelectStatus = SelectStatusEnum.RIGHT_HAND_SELECT;
                    float distance = GetRefScaleZ(gestureBean);
                    UpdateLogoPos(logo, position_x, position_y, distance * -1000);
                }
                else if (gestureBean.gesture_type != (int)GestureType.ClosePinch && logoSelectStatus == SelectStatusEnum.RIGHT_HAND_SELECT)
                {
                    logoSelectStatus = SelectStatusEnum.NONE;
                }
            }
            else
            {
                //left hand
                if (gestureBean.gesture_type == (int)GestureType.ClosePinch && logoSelectStatus != SelectStatusEnum.RIGHT_HAND_SELECT)
                {
                    logoSelectStatus = SelectStatusEnum.LEFT_HAND_SELECT;
                    float distance = GetRefScaleZ(gestureBean);
                    UpdateLogoPos(logo, position_x, position_y, distance * -1000);
                }
                else if (gestureBean.gesture_type != (int)GestureType.ClosePinch && logoSelectStatus == SelectStatusEnum.LEFT_HAND_SELECT)
                {
                    logoSelectStatus = SelectStatusEnum.NONE;
                }
            }
        }

        /// <summary>
        /// 更新Logo位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private void UpdateLogoPos(Transform logo, float x, float y, float z)
        {
            logo.transform.localPosition = new Vector3(x, y, z);
        }


        private string GetGestureTypeText(int gesType)
        {
            string result;
            switch (gesType)
            {
                case (int)GestureType.CloseHand:
                    result = "GestureType : CloseHand";
                    break;
                case (int)GestureType.OpenHand:
                    result = "GestureType : OpenHand";
                    break;
                case (int)GestureType.ClosePinch:
                    result = "GestureType : ClosedPinch";
                    break;
                case (int)GestureType.OpenPinch:
                    result = "GestureType : OpenPinch";
                    break;
                default:
                    result = "GestureType : None";
                    break;
            }
            return result;
        }

        /// <summary>
        /// 更新手势状态文本
        /// </summary>
        /// <param name="status"></param>
        private string GetGestureStatusText(int status)
        {
            string result;
            switch (status)
            {
                case 0:
                    //正在检测
                    result = "GestureStatus : DETECTING";
                    break;
                case 1:
                    //第一帧
                    result = "GestureStatus:INIT";
                    break;
                case 2:
                    result = "GestureStatus : TRACKING";
                    break;
                case 3:
                    result = "GestureStatus : LOST";
                    break;
                default:
                    result = "GestureStatus: UNKNOWN";
                    break;
            }
            return result;
        }

        private void UpdateHandPointsPosition(List<Hand_pointsItem> handPoints, Transform parent)
        {
            Debug.Log("update hand points ...");
            GameObject image;
            for (int i = 0; i < handPoints.Count; i++)
            {
                if (parent.Find("Node" + i))
                {
                    image = parent.Find("Node" + i).gameObject;
                }
                else
                {
                    image = Instantiate(gestureNode);
                    image.gameObject.SetActive(true);
                    image.transform.parent = parent;
                    image.name = "Node" + i;
                    image.transform.localScale = new Vector3(1, 1, 1);
                    Color color = Color.white;
                    if (fingerIndexColorDict.TryGetValue(i, out color))
                    {
                        image.GetComponent<Image>().color = fingerIndexColorDict[i];
                    }
                    else
                    {
                        Debug.Log("index color not find: " + i);
                    }
                }
                image.transform.localPosition = new Vector3((float)((handPoints[i].x - 0.5) * 1920), (float)((0.5 - handPoints[i].y) * 1080));
            }
        }
    }

}
