using System;
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

    public static class JsonArrayHelper
    {
        [Serializable] private class Wrapper<T> { public List<T> Items; }

        public static List<T> FromJson<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new List<T>();
            var wrapper = JsonUtility.FromJson<Wrapper<T>>($"{{\"Items\":{json}}}");
            return wrapper?.Items ?? new List<T>();
        }

        public static string ToJson<T>(List<T> list, bool prettyPrint = false)
        {
            var wrapperJson = JsonUtility.ToJson(new Wrapper<T> { Items = list }, prettyPrint);
            int start = wrapperJson.IndexOf('[');
            int end = wrapperJson.LastIndexOf(']');
            return (start >= 0 && end > start) ? wrapperJson.Substring(start, end - start + 1) : "[]";
        }
    }
    public class ContactManager : MonoBehaviour
    {
        public static ContactManager Instance { get; private set; }
        public List<Contact> Contacts { get; private set; } = new();

        public event Action ContactsUpdated;
        private string persistentFolder;
        private string persistentContactsPath;

        private void Awake()
        {
            if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
            else { Destroy(gameObject); return; }
            Debug.Log("Run contactmanager");

            persistentFolder = Path.Combine(Application.persistentDataPath, "Contacts");
            persistentContactsPath = Path.Combine(persistentFolder, "contacts.json");

            StartCoroutine(EnsureContactsFileThenLoad());
        }


        private IEnumerator EnsureContactsFileThenLoad()
        {
            Directory.CreateDirectory(persistentFolder);

            if (!File.Exists(persistentContactsPath))
            {
                string sourcePath = Path.Combine(Application.streamingAssetsPath, "Contacts", "contacts.json");
                Debug.Log("Copying contacts from streaming assets: " + sourcePath);

                UnityWebRequest www = UnityWebRequest.Get(sourcePath);
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllText(persistentContactsPath, www.downloadHandler.text);
                    Debug.Log("Contacts copied to: " + persistentContactsPath);
                }
                else
                {
                    Debug.LogError("Failed to copy contacts file: " + www.error);
                    yield break;
                }
            }

            LoadContactsFromFile(persistentContactsPath);
        }


        private void LoadContactsFromFile(string path)
        {
            try
            {
                string json = File.ReadAllText(path);
                Contacts = JsonArrayHelper.FromJson<Contact>(json);
                Debug.Log("Contacts loaded, count = " + Contacts.Count);
                ContactsUpdated?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load contacts: " + e.Message);
            }
        }

        public bool TryAddContact(string name, string url)
        {
            name = name?.Trim();
            url = url?.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(url))
            {
                Debug.LogWarning("Name or URL is empty; contact not added.");
                return false;
            }

            Contacts.Add(new Contact { name = name, streamUrl = url });
            SaveContactsToFile();
            ContactsUpdated?.Invoke();
            Debug.Log($"Contact added: {name} -> {url}");
            return true;
        }

        private void SaveContactsToFile()
        {
            try
            {
                string json = JsonArrayHelper.ToJson(Contacts, true);
                File.WriteAllText(persistentContactsPath, json);
                Debug.Log("Contacts saved to: " + persistentContactsPath);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save contacts: " + e.Message);
            }
        }


        // private IEnumerator LoadContactsFromStreamingAssets()
        // {

        //     string sourcePath = Path.Combine(Application.streamingAssetsPath, "Contacts", "contacts.json");
        //     Debug.Log("Try to read the contacts file path:" + sourcePath);

        //     UnityWebRequest www = UnityWebRequest.Get(sourcePath);
        //     yield return www.SendWebRequest();

        //     if (www.result == UnityWebRequest.Result.Success)
        //     {
        //         string json = www.downloadHandler.text;
        //         Debug.Log("Successfully read JSON content:" + json);

        //         try
        //         {
        //             string wrapped = $"{{\"contacts\":{json}}}";
        //             ContactListWrapper wrapper = JsonUtility.FromJson<ContactListWrapper>(wrapped);
        //             Contacts = wrapper.contacts;

        //             Debug.Log("Number of contacts loaded:" + Contacts.Count);

        //             foreach (var contact in Contacts)
        //             {
        //                 Debug.Log($"Contact:{contact.name}, Stream address:{contact.streamUrl}");
        //             }
        //         }
        //         catch (System.Exception e)
        //         {
        //             Debug.LogError("Failed to parse contact JSON:" + e.Message);
        //         }
        //     }
        //     else
        //     {
        //         Debug.LogError("Failed to read contacts file:" + www.error);
        //         Debug.LogError("URL: " + sourcePath);
        //     }
        // }
    }
}
