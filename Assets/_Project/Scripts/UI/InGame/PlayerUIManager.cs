using System.Collections.Generic;
using Colosseum.Core;
using Colosseum.Slot;

namespace Colosseum.UI.InGame
{
    public class PlayerUIManager: Singleton<PlayerUIManager>
    {
        public TextFillBar[] hpBars;
        public TextFillBar[] staminaBars;
        public SkillSlot[] keyboardSkillSlots;
        public SkillSlot[] padSkillSlots;
        // TODO : 지금 임시용으로 스킬슬롯 2개만 넣어놨음 나중에 어떻게해야할지 정하고 변경 
    }
}