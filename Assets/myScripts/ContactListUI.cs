using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RenderHeads.Media.AVProVideo.Demos; // 添加引用以访问LookAround360
using RenderHeads.Media.AVProVideo; // 添加引用以访问MediaPlayer


namespace Rokid.UXR.Demo
{
    public class ContactListUI : MonoBehaviour
    {
        [Header("UI References")]
        public Transform contentParent;          // ScrollView/Viewport/Content
        public GameObject buttonPrefab;          // ContactButton.prefab
        public LookAround360 lookAround;         // 场景里的播放器控制脚本

        private IEnumerator Start()
        {
            // 确保数据已加载完
            while (ContactManager.Instance == null || ContactManager.Instance.Contacts.Count == 0)
                yield return null;

            foreach (var c in ContactManager.Instance.Contacts)
            {
                GameObject btnObj = Instantiate(buttonPrefab, contentParent);
                btnObj.transform.localScale = Vector3.one;     // 防止缩放错乱

                Text label = btnObj.transform.Find("Label").GetComponent<Text>();
                label.text = c.name;

                Button btn = btnObj.GetComponent<Button>();
                // btn.onClick.AddListener(() =>
                // {
                //     lookAround.SetUrl(c.streamUrl);   // ↓ 第 4 步会加
                //     lookAround.PlayMedia();
                //     Debug.Log("点击联系人: " + c.name);
                // });
            }
        }
    }
}
