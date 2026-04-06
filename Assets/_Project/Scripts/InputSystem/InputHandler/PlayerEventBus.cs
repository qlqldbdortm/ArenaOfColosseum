using System;
using UnityEngine;

namespace Colosseum.InputSystem.InputHandler
{
    public static class PlayerEventBus
    {
        public static Action<Vector2> OnMove { get; set; } = null;
        public static Action<Vector2> OnLookDir { get; set; } = null;
        public static Action<Vector3> OnLook { get; set; } = null;
        
        public static Action<bool> OnSprint { get; set; } = null;
        
        public static Action OnNormalAttack { get; set; } = null;
        public static Action OnHardAttack { get; set; } = null;
        public static Action OnSkill1 { get; set; } = null;
        public static Action OnSkill2 { get; set; } = null;
        public static Action OnMenu { get; set; } = null;
        public static Action<int> OnEmote { get; set; } = null;
        public static Action OnOpenSettingsUI { get; set; } = null;
        public static Action OnCloseSettingsUI { get; set; } = null;
        
        public static Action OnToggleSettingsUI { get; set; } = null;
        
        public static void Clear()
        {
            OnMove = null;
            OnLookDir = null;
            OnLook = null;
            OnSprint = null;
            OnNormalAttack = null;
            OnHardAttack = null;
            OnSkill1 = null;
            OnSkill2 = null;
            OnMenu = null;
            OnEmote = null;
            OnOpenSettingsUI = null;
            OnCloseSettingsUI = null;
        }
    }
}