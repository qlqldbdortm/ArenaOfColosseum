using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colosseum
{
    public class CameraFIxed : MonoBehaviour
    {
        private Camera mainCamera;

        void Start()
        {
            mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            transform.forward = mainCamera.transform.forward;
        }
    }
}
