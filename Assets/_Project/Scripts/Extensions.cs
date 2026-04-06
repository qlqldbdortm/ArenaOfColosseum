using System;
using Colosseum.InputSystem;
using Colosseum.LifeCycle;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.InputSystem;

namespace Colosseum
{
    /// <summary>
    /// 확장 메소드를 정의하기 위한 static class 
    /// </summary>
    public static class Extensions
    {
        #region ◇ InputSystem ◇
        public static void AddAction(this InputActionMap actionMap, string actionName,
            Action<InputAction.CallbackContext> started, Action<InputAction.CallbackContext> performed, Action<InputAction.CallbackContext> canceled)
        {
            InputAction inputAction = actionMap.FindAction(actionName);
            if (started is not null)
            {
                inputAction.started += started;
                InputManager.OnSceneChanged += () => inputAction.started -= started;
            }
            if (performed is not null)
            {
                inputAction.performed += performed;
                InputManager.OnSceneChanged += () => inputAction.performed -= performed;
            }
            if (canceled is not null)
            {
                inputAction.canceled += canceled;
                InputManager.OnSceneChanged += () => inputAction.canceled -= canceled;
            }
        }
        public static void AddAction(this InputActionMap actionMap, string actionName,
            Action<InputAction.CallbackContext> action)
        {
            InputAction inputAction = actionMap.FindAction(actionName);
            inputAction.started += action;
            inputAction.performed += action;
            inputAction.canceled += action;
            InputManager.OnSceneChanged += () =>
            {
                inputAction.started -= action;
                inputAction.performed -= action;
                inputAction.canceled -= action;
            };
        }
        #endregion

        #region ◇ LifeCycle ◇
        public static Action<T> GetActions<T>(this IInit<T>[] inters)
        {
            Action<T> result = null;
            foreach (var inter in inters)
            {
                result += inter.OnInit;
            }

            return result;
        }
        public static Action<T> GetActions<T>(this ISpawn<T>[] inters)
        {
            Action<T> result = null;
            foreach (var inter in inters)
            {
                result += inter.OnSpawn;
            }

            return result;
        }
        public static Action<T> GetActions<T>(this IRelease<T>[] inters)
        {
            Action<T> result = null;
            foreach (var inter in inters)
            {
                result += inter.OnRelease;
            }

            return result;
        }
        #endregion

        public static T GetValueOrDefault<T>(this Hashtable table, string key, T defaultValue)
        {
            if (table.ContainsKey(key))
            {
                return (T)table[key];
            }
            return defaultValue;
        }

        public static void AddProperty(this Player player, params (string, object)[] values)
        {
            Hashtable table = new();
            foreach (var item in values)
            {
                string key = item.Item1;
                object value = item.Item2;
                table[key] = value;
            }
            player.SetCustomProperties(table);
        }
    }
}