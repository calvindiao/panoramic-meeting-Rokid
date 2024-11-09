using Rokid.UXR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rokid.UXR.Demo
{
    public class VoiceScene : MonoBehaviour
    {

        [SerializeField]
        private MeshRenderer m_Render;

        [SerializeField]
        private Color m_BlueColor;

        [SerializeField]
        private Color m_GreenColor;

        public Text tipText;

        public Text infoText;

        public SystemLanguage language;

        private void Awake()
        {
            language = Application.systemLanguage;
            OfflineVoiceModule.Instance.Initialized();
            RKVirtualController.Instance.Change(ControllerType.NORMAL);
            if (language == SystemLanguage.Chinese || language == SystemLanguage.ChineseSimplified || language == SystemLanguage.ChineseTraditional)
            {
                tipText.text = "请说出“变成蓝色”或者“变成绿色”来控制物体的颜色. 点击返回按键或者说”回到上一级“回到首页";
            }
            else if (language == SystemLanguage.English)
            {
                tipText.text = "Please say “show blue” or “show green” to set the color, click the back button or say”go back“ to return to the home page";
            }
            else
            {
                tipText.text = "系统语言不支持，请切换到中文或者英文";
            }
        }

        private void OnEnable()
        {
            RegisterVoiceCommand();
        }

        private void OnDisable()
        {
            UnRegisterVoiceCommand();
        }

        private void RegisterVoiceCommand()
        {
            if (language == SystemLanguage.Chinese || language == SystemLanguage.ChineseSimplified || language == SystemLanguage.ChineseTraditional)
            {
                OfflineVoiceModule.Instance.AddInstruct(LANGUAGE.CHINESE, "回到上一级", "hui dao shang yi ji", this.gameObject.name, "OnReceive");

                OfflineVoiceModule.Instance.AddInstruct(LANGUAGE.CHINESE, "变成蓝色", "bian cheng lan se", this.gameObject.name, "OnReceive");
                OfflineVoiceModule.Instance.AddInstruct(LANGUAGE.CHINESE, "变成绿色", "bian cheng lv se", this.gameObject.name, "OnReceive");

                OfflineVoiceModule.Instance.AddNumberInstruct(LANGUAGE.CHINESE, "第", "页", "", 0, 30, this.gameObject.name, "OnReceive");

            }
            else if (language == SystemLanguage.English)
            {
                //system language needs to be English for English instruct
                OfflineVoiceModule.Instance.AddInstruct(LANGUAGE.ENGLISH, "show blue", null, this.gameObject.name, "OnReceive");
                OfflineVoiceModule.Instance.AddInstruct(LANGUAGE.ENGLISH, "show green", null, this.gameObject.name, "OnReceive");
                OfflineVoiceModule.Instance.AddInstruct(LANGUAGE.ENGLISH, "go back", null, this.gameObject.name, "OnReceive");

                OfflineVoiceModule.Instance.AddNumberInstruct(LANGUAGE.ENGLISH, "page", "", "", 0, 30, this.gameObject.name, "OnReceive");//page one, page two...
            }
            OfflineVoiceModule.Instance.Commit();
        }

        private void UnRegisterVoiceCommand()
        {
            if (language == SystemLanguage.Chinese || language == SystemLanguage.ChineseSimplified || language == SystemLanguage.ChineseTraditional)
            {
                OfflineVoiceModule.Instance.RemoveInstruct(LANGUAGE.CHINESE, "变成蓝色");
                OfflineVoiceModule.Instance.RemoveInstruct(LANGUAGE.CHINESE, "变成绿色");
                OfflineVoiceModule.Instance.RemoveInstruct(LANGUAGE.CHINESE, "回到上一级");
            }
            else if (language == SystemLanguage.English)
            {
                //system language needs to be English for English instruct
                OfflineVoiceModule.Instance.RemoveInstruct(LANGUAGE.ENGLISH, "show blue");
                OfflineVoiceModule.Instance.RemoveInstruct(LANGUAGE.ENGLISH, "show green");
                OfflineVoiceModule.Instance.RemoveInstruct(LANGUAGE.ENGLISH, "go back");
            }

            OfflineVoiceModule.Instance.ClearNumberInstruct(); //清除当前全部数字类型指令

            OfflineVoiceModule.Instance.Commit();
        }

        public void OnReceive(string msg)
        {
            infoText.text = "OnReceive: " + msg;
            if (string.Equals("回到上一级", msg) || string.Equals("go back", msg))
            {
                back();
            }
            else if (string.Equals("变成蓝色", msg) || string.Equals("show blue", msg))
            {
                m_Render.material.color = m_BlueColor;
            }
            else if (string.Equals("变成绿色", msg) || string.Equals("show green", msg))
            {
                m_Render.material.color = m_GreenColor;
            }
        }

        private void back()
        {
            SceneManager.LoadScene(0);
        }
    }
}

