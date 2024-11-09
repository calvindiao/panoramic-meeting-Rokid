using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rokid.UXR.UI;
using Rokid.UXR;


public class CheckFuncTipPanel : BasePanel, IDialog
{
    [SerializeField]
    private Text tipText;
    private float delayTime = 6;
    private string funcName = "Camera";

    /// <summary>
    /// 是否支持该设备
    /// </summary>
    private bool supportDevice;

    public override void InitOnAwake()
    {
    }
    public override void InitOnStart()
    {
        supportDevice = UsbDeviceHelper.IsSupportCamera();
        if (supportDevice)
        {
            Destroy(this.gameObject);
        }
    }

    /// <param name="funcName">相机特有的功能</param>
    /// <param name="delayTime">延迟时间</param>
    public void Init(string funcName = "Camera", float delayTime = 6)
    {
        this.funcName = funcName;
        this.delayTime = delayTime;
    }

    private void Update()
    {
        if (supportDevice)
        {
            return;
        }
        delayTime -= Time.deltaTime;
        tipText.text = $"This glass not support {funcName}, back to main scene after {(int)delayTime} seconds.";
        if (delayTime < 0)
        {
            SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        }
    }
}