

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scgFullBodyController
{
    public class CameraControlledIK : MonoBehaviour
    {
        public Transform spineToOrientate;

        
        void LateUpdate()
        {
            spineToOrientate.rotation = transform.rotation;
        }
    }
}
