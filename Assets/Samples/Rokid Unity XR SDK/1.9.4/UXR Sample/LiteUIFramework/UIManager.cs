using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rokid.UXR.UI
{
    public class UIManager : BaseUI
    {
        private static UIManager instance;
        public static UIManager Instance()
        {
            if (instance == null)
            {
                GameObject go = new GameObject("UIManager");
                instance = go.AddComponent<UIManager>();
            }
            return instance;
        }
        public override void InitOnAwake()
        {

        }

        public override void InitOnStart()
        {

        }
    }

}

