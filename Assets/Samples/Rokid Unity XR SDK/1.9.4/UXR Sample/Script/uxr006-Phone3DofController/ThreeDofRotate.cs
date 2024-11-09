
using Rokid.UXR;
using UnityEngine;
using System;

namespace Rokid.UXR.Demo
{
    public class ThreeDofRotate : MonoBehaviour
    {
        protected LaserBeam laser;

#if UNITY_EDITOR
        /// <summary>
        /// Min俯仰角
        /// </summary>
        [SerializeField]
        [Range(-60, 0)]
        private float minPitch = -30;
        [SerializeField]

        /// <summary>
        /// Max俯仰角
        /// </summary>
        [Range(0, 60)]
        private float maxPitch = 30;

        /// <summary>
        /// Min偏航角
        /// </summary>
        [SerializeField]
        [Range(-60, 0)]
        private float minYaw = -30;
        /// <summary>
        /// Max偏航角
        /// </summary>
        [SerializeField]
        [Range(0, 60)]
        private float maxYaw = 30;
#endif

        private void Awake()
        {
            laser = this.GetComponent<LaserBeam>();
        }

        void Start()
        {
            ThreeDofEventController.Instance.OnPhoneRayForward += OnPhoneRayForward;
            ThreeDofEventController.Instance.OnPress += OnPress;
            ThreeDofEventController.Instance.OnRelease += OnRelease;
#if UNITY_EDITOR
            ThreeDofEventController.Instance.OnMoveByNormalizePos += OnMoveByNormalizePos;
#endif
        }

#if UNITY_EDITOR
        private void OnMoveByNormalizePos(Vector2 normailizePosition)
        {
            RotateAngleByNormalizePos(normailizePosition);
        }

        /// <summary>
        /// 通过屏幕上归一化的点，来计算屏幕中的角度
        /// </summary>
        /// <param name="normailizePosition"></param>
        private void RotateAngleByNormalizePos(Vector2 normailizePosition)
        {
            float picth = minPitch + (maxPitch - minPitch) * normailizePosition.y;
            float yaw = minYaw + (maxYaw - minYaw) * normailizePosition.x;
            this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.Euler(new Vector3(picth, yaw, 0)), 1);
        }
#endif

        private void OnRelease()
        {
            laser.SetRelease();
        }

        private void OnPress()
        {
            laser.SetPress();
        }

        private void OnDestroy()
        {
            ThreeDofEventController.Instance.OnPhoneRayForward -= OnPhoneRayForward;
            ThreeDofEventController.Instance.OnPress -= OnPress;
            ThreeDofEventController.Instance.OnRelease -= OnRelease;
#if UNITY_EDITOR
            ThreeDofEventController.Instance.OnMoveByNormalizePos -= OnMoveByNormalizePos;
#endif
        }

        private void OnPhoneRayForward(Vector3 forward)
        {
            this.transform.eulerAngles = forward;
        }

    }
}
