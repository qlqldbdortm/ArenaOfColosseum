using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Colosseum
{
    public class TESTCustomSceneChanger : MonoBehaviour
    {
        public void ChangeScene()
        {
            SceneManager.LoadScene("Demo");
        }
    }
}
