using Rokid.UXR;
using System.Collections.Generic;
using UnityEngine;
using Rokid.UXR.UI;

namespace Rokid.UXR.Demo
{
    /// <summary>
    /// 选中状态
    /// </summary>
    public enum SelectStatusEnum
    {
        NONE,
        LEFT_HAND_SELECT,
        RIGHT_HAND_SELECT
    }

    public class RKGestureView : MonoBehaviour
    {

        [SerializeField]
        private GameObject gestureNode;

        [SerializeField]
        private TextAsset testData;

        [SerializeField]
        private HandContentItem handContentItem;

        public List<HandContentItem> handContentItems = new List<HandContentItem>();

        private Transform logo;

        private GestureBean addBean = new GestureBean();

        private void Awake()
        {
            logo = GameObject.Find("LOGO").transform;
            AutoInjectComponent.AutoInject(transform, this);
            HandContentItem hand01 = GameObject.Instantiate(handContentItem.gameObject, handContentItem.transform.parent).GetComponent<HandContentItem>();
            HandContentItem hand02 = GameObject.Instantiate(handContentItem.gameObject, handContentItem.transform.parent).GetComponent<HandContentItem>();
            hand01.Initialized();
            hand02.Initialized();
            handContentItems.Add(hand01);
            handContentItems.Add(hand02);
        }

        private void Start()
        {
#if UNITY_EDITOR
            gestureCallback(JsonUtility.FromJson<GestureResults>(testData.ToString()));
#endif
            // RKGestureSample.gestureDelegate += gestureCallback;
            GesEventController.Instance.Initialized();
            GesEventController.OnProcessGesData += gestureCallback;

            UIManager.Instance().CreatePanel<CheckFuncTipPanel>(true);
        }

        private void OnDestroy()
        {
            // RKGestureSample.gestureDelegate -= gestureCallback;
            GesEventController.OnProcessGesData -= gestureCallback;
        }

        private void Update()
        {
            GesEventController.Instance.gesFrameLock = false;
        }

        void gestureCallback(GestureResults results)
        {
            List<GestureBean> gestureBeans = results.gestureResults;
            if (gestureBeans.Count == 0)
            {
                foreach (var handItem in handContentItems)
                {
                    handItem.gameObject.SetActive(false);
                }
            }
            else
            {
                if (gestureBeans.Count == 1)
                {
                    //补充数据
                    GestureBean bean = results.gestureResults[0];
                    addBean.hand_type = bean.hand_type == 1 ? 2 : 1;
                    addBean.gesture_type = (int)GestureType.None;
                    results.gestureResults.Add(addBean);
                }
                for (int i = 0; i < gestureBeans.Count; i++)
                {
                    Debug.Log("update info...:" + i);
                    gestureBeans[i].gesture_status = results.gestureStatus;
                    handContentItems[i].UpdateInfo(gestureBeans[i], gestureNode, logo);
                }
            }
        }
    }
}