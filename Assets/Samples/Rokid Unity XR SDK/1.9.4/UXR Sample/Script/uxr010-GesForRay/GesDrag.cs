using System.Runtime.CompilerServices;
using System;
using System.Net.NetworkInformation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rokid.UXR;

public class GesDrag : MonoBehaviour, IGesDrag, IGesBeginDrag
{
    private float smoothSpeed = 8;
    private Vector3 allDelta;
    private Vector3 oriPos;
    private bool lookAtCamera = true;

    private void Start()
    {
        if (RKConst.cameraRig == null)
        {
            RKConst.cameraRig = GameObject.Find("CameraRig").transform;
        }
    }
    public void OnGesBeginDrag()
    {
        Debug.Log("Begin Drag ...");
        allDelta = Vector3.zero;
        oriPos = transform.position;
    }

    public void OnGesDrag(Vector3 delta)
    {
        preFilter(ref delta);
        allDelta += delta * 1.3f;
        Vector3 targetPos = oriPos + allDelta;
        smoothSpeed = 10;
        filter01(ref targetPos, ref allDelta, ref oriPos);
        filter02(ref targetPos, ref allDelta, ref oriPos);
        filter03(ref targetPos, ref allDelta, ref oriPos);

        if (lookAtCamera)
        {
            if (GetComponent<CanvasRenderer>())
            {
                //UI
                Vector3 director = targetPos - RKConst.cameraRig.position;
                transform.localRotation = Quaternion.LookRotation(director);
            }
            else
            {
                //模型
                Vector3 director = RKConst.cameraRig.position - targetPos;
                director.y = 0;
                transform.localRotation = Quaternion.LookRotation(director);
            }
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
    }

    private void preFilter(ref Vector3 delta)
    {
        // 0.判断delta在相机空间中的分量
        Vector3 cameraSpaceDelta = RKConst.cameraRig.transform.InverseTransformDirection(delta);
        if (cameraSpaceDelta.z * cameraSpaceDelta.z > 100 * (cameraSpaceDelta.x * cameraSpaceDelta.x + cameraSpaceDelta.y * cameraSpaceDelta.y))
        {
            //将x,y的分量归0
            cameraSpaceDelta.x = 0;
            cameraSpaceDelta.y = 0;
        }
        delta = RKConst.cameraRig.transform.rotation * cameraSpaceDelta;
    }

    private void filter01(ref Vector3 targetPos, ref Vector3 allDelta, ref Vector3 oriPos)
    {
        //1.处理手的距离相机较近的情况
        float handDistance = GesEventController.Instance.GetDistance();
        Vector3 director = Vector3.Normalize(targetPos - RKConst.cameraRig.transform.position);
        float result = Vector3.Dot(director, RKConst.cameraRig.transform.forward);
        if ((result < 1 && handDistance < 0.28f) || (result < 0.5f && handDistance < 0.5f))
        {
            targetPos = RKConst.cameraRig.transform.position + RKConst.cameraRig.transform.forward * 0.5f;
            allDelta = Vector3.zero;
            oriPos = transform.position;
            smoothSpeed = 5;
        }
    }

    private void filter02(ref Vector3 targetPos, ref Vector3 allDelta, ref Vector3 oriPos)
    {
        //2.处理物体距离相机较近的情况
        float distance = Vector3.Distance(RKConst.cameraRig.transform.position, targetPos);
        if (distance < 0.5f)
        {
            targetPos = RKConst.cameraRig.transform.position + RKConst.cameraRig.transform.forward * 0.5f;
            allDelta = Vector3.zero;
            oriPos = transform.position;
            smoothSpeed = 5;
        }
    }

    private void filter03(ref Vector3 targetPos, ref Vector3 allDelta, ref Vector3 oriPos)
    {
        //3.处理物体拖拽到相机后面的情况,导致拖拽抖动的问题
        Vector3 director = Vector3.Normalize(targetPos - RKConst.cameraRig.transform.position);
        float result = Vector3.Dot(director, RKConst.cameraRig.transform.forward);
        if (result < 0)
        {
            targetPos = RKConst.cameraRig.transform.position + RKConst.cameraRig.transform.forward * 0.5f;
            allDelta = Vector3.zero;
            oriPos = transform.position;
            smoothSpeed = 5;
        }
    }
}
