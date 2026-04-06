using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Colosseum
{
    /// <summary>
    /// 모든 팝업의 기본 클래스
    /// </summary>
    public class PopupBase : MonoBehaviour
    {
        [Header("팝업 설정")]
        [Tooltip("패드로 조작 시 처음 선택될 UI")]
        public GameObject firstSelectableUI;
        
        [Tooltip("ESC/B버튼으로 닫을 수 있는가?")]
        public bool canBeCanceled = true;
        
        [Tooltip("입력을 받을 수 있는가? (로딩 중 일때 false)")]
        public bool canReceiveInput = true;
        
        [Header("다른 UI 차단 (마우스 용)")]
        [Tooltip("뒤쪽 UI 클릭 방지용 Image")]
        public Image blockingImage;

        [Header("UI 복원 설정")]
        [Tooltip("팝업이 닫힐 때 복원할 기본 UI (설정 안 하면 자동으로 찾음)")]
        public GameObject fallbackUI;
        
        [Tooltip("팝업 캔버스 이름 패턴 (이 이름을 포함한 캔버스는 복원 대상에서 제외)")]
        public string[] popupCanvasNamePatterns = new string[] { "Popup", "Dialog" };

        // 팝업이 열리기 전 마지막으로 선택된 UI 오브젝트
        protected GameObject lastSelectedObject;
        
        /// <summary>
        /// 팝업이 열렸을 때
        /// </summary>
        public virtual void OnPopupOpened()
        {
            lastSelectedObject = EventSystem.current.currentSelectedGameObject;
            
            if (blockingImage != null)
            {
                blockingImage.raycastTarget = true;
            }
        }
        
        /// <summary>
        /// 팝업이 닫혔을 때
        /// </summary>
        public virtual void OnPopupClosed()
        {
            if (blockingImage != null)
            {
                blockingImage.raycastTarget = false;
            }
            
            RestoreUISelection();
        }
        
        /// <summary>
        /// 다른 팝업이 위에 열렸을 때
        /// </summary>
        public virtual void OnPopupDeactivated()
        {
            canReceiveInput = false;
        }
        
        /// <summary>
        /// 위에 있던 팝업이 닫혀서 다시 최상위가 되었을 때
        /// </summary>
        public virtual void OnPopupReActivated()
        {
            canReceiveInput = true;
            
            if (firstSelectableUI != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectableUI);
            }
        }

        public void ClosePopupAndResetFocus()
        {
            // 1. EventSystem의 현재 선택을 null로 만들어 포커스를 강제로 잃게 합니다.
            EventSystem.current.SetSelectedGameObject(null);

            // 2. PopupManager를 통해 현재 팝업을 닫습니다.
            PopupManager.Instance.CloseCurrentPopup();
        }

        
        #region UI 선택 복원 메서드
        //팝업창을 열고 닫을때 UI 포인팅을 잃어버리지 않게해서 패드 조작을 원활하게 합니다.
        
        /// <summary>
        /// UI 선택 복원 (안전한 방식)
        /// </summary>
        protected virtual void RestoreUISelection()
        {
            // 1. lastSelectedObject가 유효하면 복원
            if (TryRestoreLastSelected())
            {
                return;
            }
            
            // 2. fallbackUI가 설정되어 있으면 사용
            if (TryRestoreFallback())
            {
                return;
            }
            
            // 3. 현재 활성화된 첫 번째 유효한 Selectable 찾기
            if (TryRestoreFirstValidSelectable())
            {
                return;
            }
            
            // 4. 모두 실패하면 null로 설정
            EventSystem.current.SetSelectedGameObject(null);
            lastSelectedObject = null;
        }
        
        /// <summary>
        /// 1단계: 저장된 lastSelectedObject 복원 시도
        /// </summary>
        private bool TryRestoreLastSelected()
        {
            if (lastSelectedObject != null && lastSelectedObject.activeInHierarchy)
            {
                Selectable selectable = lastSelectedObject.GetComponent<Selectable>();
                if (selectable != null && selectable.interactable)
                {
                    EventSystem.current.SetSelectedGameObject(lastSelectedObject);
                    lastSelectedObject = null;
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 2단계: fallbackUI 복원 시도
        /// </summary>
        private bool TryRestoreFallback()
        {
            if (fallbackUI != null && fallbackUI.activeInHierarchy)
            {
                Selectable fallbackSelectable = fallbackUI.GetComponent<Selectable>();
                if (fallbackSelectable != null && fallbackSelectable.interactable)
                {
                    EventSystem.current.SetSelectedGameObject(fallbackUI);
                    lastSelectedObject = null;
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 3단계: 첫 번째 유효한 Selectable 찾기
        /// </summary>
        private bool TryRestoreFirstValidSelectable()
        {
            Selectable[] selectables = FindObjectsOfType<Selectable>();
            foreach (var selectable in selectables)
            {
                // 활성화되어 있고, 상호작용 가능하며, 팝업이 아닌 UI인지 확인
                if (selectable.gameObject.activeInHierarchy && 
                    selectable.interactable && 
                    !IsInPopupCanvas(selectable.gameObject))
                {
                    EventSystem.current.SetSelectedGameObject(selectable.gameObject);
                    lastSelectedObject = null;
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 해당 GameObject가 팝업 캔버스 안에 있는지 확인
        /// </summary>
        private bool IsInPopupCanvas(GameObject obj)
        {
            Canvas[] canvases = obj.GetComponentsInParent<Canvas>();
            foreach (var canvas in canvases)
            {
                foreach (var pattern in popupCanvasNamePatterns)
                {
                    if (canvas.name.Contains(pattern))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion
        
    }
}