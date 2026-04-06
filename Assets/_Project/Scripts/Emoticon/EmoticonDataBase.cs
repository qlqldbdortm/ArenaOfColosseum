using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colosseum
{
    [CreateAssetMenu(fileName = "EmoticonDatabase", menuName = "Emoticon/EmoticonDatabase")]
    public class EmoticonDatabase : ScriptableObject
    {
        public List<EmoticonData> emoticons;
    }
}
