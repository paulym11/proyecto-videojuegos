

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace scgFullBodyController
{
    public class sceneController : MonoBehaviour
    {
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
