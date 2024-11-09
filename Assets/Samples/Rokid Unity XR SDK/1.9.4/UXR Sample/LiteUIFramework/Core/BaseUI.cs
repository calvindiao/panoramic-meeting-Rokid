using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Rokid.UXR.UI
{
    public abstract class BaseUI : MonoBehaviour, IUI
    {
        private Transform uiParent;
        private Transform dialogParent;

        public abstract void InitOnAwake();


        public abstract void InitOnStart();


        /// <summary>
        /// 数据池缓存
        /// </summary>
        protected DataCache data;


        private void Awake()
        {
            AutoInjectComponent.AutoInject(transform, this);
            data = DataCache.Instance();
            InitOnAwake();
        }

        private void Start()
        {
            InitOnStart();
        }

        public string GetPanelPath()
        {
            return "UI/Panel";
        }

        public string GetItemPath()
        {
            return "UI/Item";
        }


        /// <summary>
        /// 创建UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dialog">是否是对话框,如果是对话框则不会隐藏上层界面</param>
        /// <returns></returns>
        public T CreatePanel<T>(bool dialog = false, string prefabName = null, bool findExitUI = true) where T : BasePanel
        {
            return CreatePanel<T>(dialog ? GetDialogParent() : GetUIParent(), dialog, prefabName, findExitUI);
        }

        public bool ExistDialog<T>()
        {
            return GetUIFromParent<T>(GetDialogParent()) != null;
        }

        public bool ExistPanel<T>()
        {
            return GetUIFromParent<T>(GetUIParent()) != null;
        }

        public T CreatePanel<T>(Transform parent, bool dialog = false, string prefabName = null, bool findExitUI = true) where T : BasePanel
        {
            if (parent.childCount >= 1 && dialog == false)
            {
                Transform tsf = parent.GetChild(parent.childCount - 1);
                if (tsf != null)
                {
                    tsf.gameObject.SetActive(false);
                }
            }
            Transform uiTransform = null;
            if (findExitUI)
            {
                uiTransform = GetUIFromParent<T>(parent);
            }
            if (uiTransform != null)
            {
                uiTransform.SetAsLastSibling();
                uiTransform.gameObject.SetActive(true);
                return uiTransform.GetComponent<T>();
            }
            GameObject go = null;
            if (!string.IsNullOrEmpty(prefabName))
            {
                go = Resources.Load<GameObject>(GetPanelPath() + "/" + prefabName);
            }
            else
            {
                go = Resources.Load<GameObject>(GetPanelPath() + "/" + typeof(T).Name);
            }
            if (go != null)
            {
                go.SetActive(true);
                Transform tsf = Instantiate(go, parent).transform;
                tsf.name = typeof(T).Name;
                Debug.Log("创建一个Panel:" + tsf.gameObject.name);
                return tsf.GetComponent<T>();
            }
            else
            {
                Debug.LogWarning("找不到加载的路径：" + GetPanelPath() + "/" + typeof(T).Name);
                return default;
            }
        }

        public T CreateItem<T>(Transform parent, bool active = true) where T : BaseItem
        {
            GameObject go;
            go = Resources.Load<GameObject>(GetItemPath() + "/" + typeof(T).Name);
            if (go != null)
            {
                go.SetActive(active);
                Transform tsf = Instantiate(go, parent).transform;
                tsf.localScale = Vector3.one;
                tsf.name = typeof(T).Name;
                return tsf.GetComponent<T>();
            }
            else
            {
                Debug.LogWarning("找不到加载的路径：" + GetItemPath() + "/" + typeof(T).Name);
                return default;
            }
        }


        public T CreateItem<T>(string prefabName, Transform parent, bool active = true) where T : BaseItem
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                return CreateItem<T>(parent, active);
            }
            GameObject go;
            go = Resources.Load<GameObject>(GetItemPath() + "/" + prefabName);
            if (go != null)
            {
                go.SetActive(active);
                Transform tsf = Instantiate(go, parent).transform;
                tsf.localScale = Vector3.one;
                tsf.name = prefabName;
                return tsf.GetComponent<T>();
            }
            else
            {
                Debug.LogWarning("找不到加载的路径：" + GetItemPath() + "/" + typeof(T).Name);
                return default;
            }
        }


        public Transform CreateItem(string prefabName, Transform parent, bool active = true)
        {
            GameObject go;
            go = Resources.Load<GameObject>(GetItemPath() + "/" + prefabName);
            if (go != null)
            {
                go.SetActive(active);
                Transform tsf = Instantiate(go, parent).transform;
                tsf.name = prefabName;
                tsf.localScale = Vector3.one;
                return tsf;
            }
            return null;
        }

        /// <summary>
        /// 退出Panel
        /// </summary>
        public void ExitPanel()
        {
            if (GetUIParent().childCount >= 2)
            {
                GetUIParent().GetChild(GetUIParent().childCount - 2).gameObject.SetActive(true);
                Destroy(GetUIParent().GetChild(GetUIParent().childCount - 1).gameObject);
            }
        }

        /// <summary>
        /// 退出panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ExitPanel<T>() where T : BasePanel
        {
            Transform tsf = GetUIParent()?.Find(typeof(T).Name);
            if (tsf != null)
            {
                DestroyImmediate(tsf.gameObject);
            }
            //将上一个界面的UI设置为true
            Transform ui = FindTopChild(GetUIParent());
            if (ui != null)
            {
                ui.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 退出panel,不销毁
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ExitPanelNotDestroy<T>() where T : BasePanel
        {
            Transform tsf = GetUIParent()?.Find(typeof(T).Name);
            if (tsf != null)
            {
                tsf.SetAsFirstSibling();
                tsf.gameObject.SetActive(false);
            }
            //将上一个界面的UI设置为true
            Transform ui = FindTopChild(GetUIParent());
            if (ui != null)
            {
                ui.gameObject.SetActive(true);
            }
        }


        /// <summary>
        /// 退出dialog
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ExitDialog<T>() where T : IDialog
        {
            Transform tsf = GetDialogParent()?.Find(typeof(T).Name);
            if (tsf != null)
            {
                Destroy(tsf.gameObject);
            }
        }

        /// <summary>
        /// 找到位于栈顶的UI
        /// </summary>
        /// <param name="tsf"></param>
        /// <returns></returns>
        private Transform FindTopChild(Transform tsf)
        {
            if (tsf.childCount > 0)
            {
                return tsf.GetChild(tsf.childCount - 1);
            }
            return null;
        }

        public void ExitItem<T>() where T : BaseItem
        {
            Destroy(this.gameObject);
        }

        /// <summary>
        /// 退出所有
        /// </summary>
        public void ExitAll()
        {
            foreach (Transform tsf in GetUIParent())
            {
                Destroy(tsf.gameObject);
            }
        }

        /// <summary>
        /// 退出所有对话框
        /// </summary>
        public void ExitAllDialog()
        {
            foreach (Transform tsf in GetDialogParent())
            {
                Destroy(tsf.gameObject);
            }
        }

        /// <summary>
        /// 从Parent中获得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private Transform GetUIFromParent<T>(Transform parent)
        {
            Transform tsf = parent.Find(typeof(T).Name);
            return tsf;
        }

        public Transform GetUIParent()
        {
            if (uiParent == null)
            {
                uiParent = GameObject.FindWithTag("UIParent")?.transform;
            }
            if (uiParent == null)
            {
                Debug.LogError("未指定Tag==UIParent的物体!!!");
            }
            return uiParent;
        }

        public Transform GetDialogParent()
        {
            if (dialogParent == null)
            {
                dialogParent = GameObject.FindWithTag("DialogParent")?.transform;
            }
            if (dialogParent == null)
            {
                Debug.LogError("未指定Tag==DialogParent的物体!!!");
            }
            return dialogParent;
        }

    }

}

