using UnityEngine;
using UnityEngine.SceneManagement;
using Rokid.UXR;
using System;

namespace Rokid.UXR.Demo
{

    public class KeycodeInputModule : MonoSingleton<KeycodeInputModule>
    {
        public void Initialized()
        {
            DontDestroyOnLoad(this.gameObject);
            this.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
    }

}
