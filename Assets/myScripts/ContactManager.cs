using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Rokid.UXR.Demo
{
    [System.Serializable]
    public class Contact
    {
        public string name;
        public string streamUrl;
    }

    [System.Serializable]
    public class ContactListWrapper
    {
        public List<Contact> contacts;
    }

    public class ContactManager : MonoBehaviour
    {
        public static ContactManager Instance { get; private set; }
        public List<Contact> Contacts { get; private set; } = new();

        private void Awake()
        {
            if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
            else { Destroy(gameObject); return; }
            Debug.Log("首次运行contactmanager");
            StartCoroutine(LoadContactsFromStreamingAssets());
        }

        private IEnumerator LoadContactsFromStreamingAssets()
        {
            // 使用正确的路径，包含AVProVideoSamples子文件夹
            string sourcePath = Path.Combine(Application.streamingAssetsPath, "Contacts", "contacts.json");
            Debug.Log("尝试读取联系人文件路径：" + sourcePath);
            
            UnityWebRequest www = UnityWebRequest.Get(sourcePath);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log("成功读取JSON内容：" + json);
                
                try
                {
                    // 由于JSON文件直接是数组格式，需要包装成对象
                    string wrapped = $"{{\"contacts\":{json}}}";
                    ContactListWrapper wrapper = JsonUtility.FromJson<ContactListWrapper>(wrapped);
                    Contacts = wrapper.contacts;
                    
                    Debug.Log("已加载联系人数量：" + Contacts.Count);

                    foreach (var contact in Contacts)
                    {
                        Debug.Log($"联系人：{contact.name}，流地址：{contact.streamUrl}");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("解析联系人JSON失败: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("读取联系人文件失败: " + www.error);
                Debug.LogError("请求的URL: " + sourcePath);
            }
        }
    }
}
