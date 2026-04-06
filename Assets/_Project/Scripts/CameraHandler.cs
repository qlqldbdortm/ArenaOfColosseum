using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colosseum
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform player;                
        public Vector3 offset = new Vector3(0, 20, -15);  
        public float smoothSpeed = 5f;          
        
        public Vector3 fixedEulerAngles = new Vector3(45, 0, 0);

        private void LateUpdate()
        {
            if (player == null) return;
            
            Vector3 targetPos = player.position + offset;

            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(fixedEulerAngles);
        }
    }
}
