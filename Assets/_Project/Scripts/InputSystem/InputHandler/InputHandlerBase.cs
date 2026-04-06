using UnityEngine;

namespace Colosseum.InputSystem.InputHandler
{
    public abstract class InputHandlerBase: MonoBehaviour
    {
        void Start()
        {
            Init();
        }


        protected abstract void Init();
    }
}