using System.Collections.Generic;
using UnityEngine;

namespace Rokid.UXR.UI
{
    public class DataCache : Singleton<DataCache>
    {

        Dictionary<string, object> data = new Dictionary<string, object>();

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cover"></param>
        public void Add(string key, object value, bool cover = false)
        {
            // Debug.Log("add key:" + key);
            if (data.ContainsKey(key) && cover == false)
            {
                Debug.Log("您添加的数据已经存在");
                return;
            }
            else if (data.ContainsKey(key) && cover == true)
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cover"></param>
        public void Add(object value, bool cover = false)
        {
            string key = value.GetType().FullName;
            Add(key, value, cover);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void UpdateData(string key, object value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                Debug.Log("update key 不存在:" + key);
            }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            object obj = null;
            if (data.TryGetValue(key, out obj))
            {
                return (T)obj;
            }
            else
            {
                Debug.Log("get key 不存在:" + key);
            }
            return default;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>()
        {
            string key = typeof(T).FullName;
            return Get<T>(key);
        }

        public List<string> Keys()
        {
            return new List<string>(data.Keys);
        }

        /// <summary>
        /// 清理数据
        /// </summary>
        public void Clear()
        {
            data.Clear();
        }


        public string[] analysisContent(string msg)
        {
            return msg.Split(':')[1].Substring(1, msg.Split(':')[1].Length - 2).Split(',');
        }
    }
}

