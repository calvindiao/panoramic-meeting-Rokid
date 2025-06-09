using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RenderHeads.Media.AVProVideo.Demos; // 添加引用以访问LookAround360
using RenderHeads.Media.AVProVideo; // 添加引用以访问MediaPlayer


namespace Rokid.UXR.Demo
{
    public class ContactListUI : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Transform contentParent;   // ScrollView/Content
        [SerializeField] private Button buttonPrefab;       // ContactButton prefab
        [SerializeField] private LookAround360 lookAround;  // 你的播放器控制脚本（可选）

        private IEnumerator Start()
        {
            Debug.Log("▶ ContactListUI Start running");
            // 等待 ContactManager 完成加载
            while (ContactManager.Instance == null || ContactManager.Instance.Contacts.Count == 0)
                yield return null;

            PopulateButtons();
        }

        private void PopulateButtons()
        {
            Debug.Log($"▶ PopulateButtons, contact count = {ContactManager.Instance.Contacts.Count}");
            // 先清空旧的（编辑器里留的、或刷新用）
            foreach (Transform child in contentParent) Destroy(child.gameObject);

            foreach (var c in ContactManager.Instance.Contacts)
            {
                // 1. 实例化按钮
                Button btn = Instantiate(buttonPrefab, contentParent);
                btn.transform.localScale = Vector3.one;   // 防止缩放错乱

                // 2. 设置按钮文字
                Text txt = btn.GetComponentInChildren<Text>();
                txt.text = c.name;

                // 3. 绑定点击事件
                btn.onClick.AddListener(() =>
                {
                    Debug.Log($"点击联系人：{c.name}，URL：{c.streamUrl}");

                    // 如果你想播放流：
                    if (lookAround != null)
                    {
                        // lookAround.SetUrl(c.streamUrl);  // 之前给 LookAround360 加过这个接口
                        // lookAround.PlayMedia();
                        Debug.Log("播放流："+c.streamUrl);
                    }
                });
            }
        }
    }
}
