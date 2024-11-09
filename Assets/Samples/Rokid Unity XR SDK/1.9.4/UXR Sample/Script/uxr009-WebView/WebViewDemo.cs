using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rokid.UXR;
namespace Rokid.UXR.Demo
{
    public class WebViewDemo : MonoBehaviour
    {
        public Button openRokid;

        private void Start()
        {
            openRokid.onClick.AddListener(() =>
            {
                RKVirtualController.Instance.AutoLoadWebView("https://www.rokid.com");
            });
        }
    }

}
