using Cinemachine;
using Photon.Pun;
using UnityEngine;

namespace Colosseum.Unit
{
    public class UnitCameraHandler : MonoBehaviourPun
    {
        public GameObject cameraPrefab; 

        private void Start()
        {
            if (photonView.IsMine)
            {
                SetupCamera();
            }
        }

        private void SetupCamera()
        {
            if (cameraPrefab == null) return;
            
            GameObject camObj = Instantiate(cameraPrefab);
            var camScript = camObj.GetComponent<CameraHandler>();
            if (camScript != null)
            {
                camScript.player = transform;
            }
        }
    }
}