using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 自动注入组件
/// Author:chenlin
/// </summary>
public class AutoInjectComponent {
    /// <summary>
    /// 查找
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="infos"></param>
    /// <param name="obj"></param>
    private static void Find (Transform transform, List<FieldInfo> infos, object obj) {
        foreach (Transform child in transform) {
            for (var i = 0; i < infos.Count; i++) {
                var fieldName = infos[i].ToString ().Split (' ') [1].ToLower ();
                if (child.name.ToLower () == fieldName.ToLower ()) {
                    string[] names = infos[i].FieldType.ToString ().Split ('.');
                    string typeName = names[names.Length - 1];
                    if (typeName.ToLower () == "gameobject") {
                        infos[i].SetValue (obj, child.gameObject);
                    } else {
                        infos[i].SetValue (obj, child.GetComponent (typeName));
                    }
                    infos.Remove (infos[i]);
                }
            }
            if (child.childCount > 0) {
                Find (child, infos, obj);
            }
        }
    }

    /// <summary>
    /// 自动注入组件(通过字段名和场景的中的物体名称匹配选择) 
    /// 注意名称查找不区分大小写
    /// </summary>
    /// <param name="tsf">查找组件父级</param>
    /// <param name="obj">类</param>
    public static void AutoInject (Transform tsf, object obj) {
        var type = obj.GetType ();
        List<FieldInfo> infos = new List<FieldInfo> ();
        List<FieldInfo> needInjectInfos = new List<FieldInfo> ();
        infos.AddRange (type.GetFields ().ToList ()); //找到类中的所有的公共字段
        infos.AddRange (type.GetFields (BindingFlags.Instance | BindingFlags.NonPublic).ToList ()); //找到类中的所有的私有字段
        for (int i = 0; i < infos.Count; i++) {
            //获取所有的特性
            object[] objects = infos[i].GetCustomAttributes (true);
            try {
                for (int j = 0; j < objects.Length; j++) {
                    if (objects[j].GetType () == typeof (SerializeField)) {
                        needInjectInfos.Add (infos[i]);
                        break;
                    }
                }
            } catch (System.Exception e) {
                Debug.Log (e.ToString ());
            }

        }
        Find (tsf, needInjectInfos, obj);
        for (int i = 0; i < needInjectInfos.Count; i++) {
            if (needInjectInfos[i].GetValue (obj) == null) {
                Debug.Log (string.Format ("{0},{1},没有找到目标组件", obj, needInjectInfos[i].ToString ().Split (' ') [1]));
            }
        }
    }
}