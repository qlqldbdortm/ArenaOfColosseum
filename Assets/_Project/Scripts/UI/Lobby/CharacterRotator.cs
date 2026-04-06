using System;
using UnityEngine;

namespace Colosseum.UI.Lobby
{
    public class CharacterRotator: MonoBehaviour
    {
        public static Action<Vector2> OnRotate { get; private set; } = null;


        [SerializeField] private Transform character;
        
        
        private Vector2 rotateSpeed = Vector2.zero;


        void OnEnable()
        {
            OnRotate += Rotate;
            character.rotation = Quaternion.Euler(0, 180, 0);
        }
        void OnDisable()
        {
            OnRotate -= Rotate;
            rotateSpeed = Vector2.zero;
        }

        void Update()
        {
            character.Rotate(0, -rotateSpeed.x * 90f * Time.deltaTime, 0, Space.Self);
        }


        public void Rotate(Vector2 dir)
        {
            rotateSpeed = dir;
        }
    }
}