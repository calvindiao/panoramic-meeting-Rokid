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
            Debug.Log("Run contactmanager");
            StartCoroutine(LoadContactsFromStreamingAssets());
        }

        private IEnumerator LoadContactsFromStreamingAssets()
        {

            string sourcePath = Path.Combine(Application.streamingAssetsPath, "Contacts", "contacts.json");
            Debug.Log("Try to read the contacts file path:" + sourcePath);
            
            UnityWebRequest www = UnityWebRequest.Get(sourcePath);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log("Successfully read JSON content:" + json);
                
                try
                {
                    string wrapped = $"{{\"contacts\":{json}}}";
                    ContactListWrapper wrapper = JsonUtility.FromJson<ContactListWrapper>(wrapped);
                    Contacts = wrapper.contacts;
                    
                    Debug.Log("Number of contacts loaded:" + Contacts.Count);

                    foreach (var contact in Contacts)
                    {
                        Debug.Log($"Contact:{contact.name}, Stream address:{contact.streamUrl}");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Failed to parse contact JSON:" + e.Message);
                }
            }
            else
            {
                Debug.LogError("Failed to read contacts file:" + www.error);
                Debug.LogError("URL: " + sourcePath);
            }
        }
    }
}
