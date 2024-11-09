using UnityEngine;
using UnityEngine.SceneManagement;
namespace Rokid.UXR.Demo
{
    public class MainSceneClickEvent : MonoBehaviour
    {
        public void OnLoadScene(int index)
        {
            switch (index)
            {
                case 0:
                    SceneManager.LoadScene("uxr000-HelloRokid");
                    break;
                case 1:
                    SceneManager.LoadScene("uxr001-HeadTracking");
                    break;
                case 2:
                    SceneManager.LoadScene("uxr002-GestureRecognize");
                    break;
                case 3:
                    SceneManager.LoadScene("uxr003-VoiceRecognize");
                    break;
                case 4:
                    SceneManager.LoadScene("uxr004-NormalController");
                    break;
                case 5:
                    SceneManager.LoadScene("uxr005-GamePadController");
                    break;
                case 6:
                    SceneManager.LoadScene("uxr006-Phone3DofController");
                    break;
                case 7:
                    SceneManager.LoadScene("uxr007-GlassScreenRecorder");
                    break;
                case 8:
                    SceneManager.LoadScene("uxr008-GlassCamera");
                    break;
                case 9:
                    SceneManager.LoadScene("uxr009-WebViewController");
                    break;
                case 10:
                    SceneManager.LoadScene("uxr010-GestureForRay");
                    break;
            }
        }
    }
}