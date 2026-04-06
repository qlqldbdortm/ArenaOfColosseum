using UnityEngine;
using Photon.Pun;

namespace Colosseum
{
    public class EmoteController : MonoBehaviourPun
    {
        [Header("Emoticon Database")]
        [SerializeField] private EmoticonDatabase emoticonDatabase;
    }
}
