using Photon.Pun;

namespace Colosseum.Core
{
    public abstract class PhotonSingleton<T>: MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
    {
        public static T Instance { get; private set; } = null;
        
        
        protected virtual void Awake()
        {
            if(Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                DestroyImmediate(this);
            }
        }
        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}