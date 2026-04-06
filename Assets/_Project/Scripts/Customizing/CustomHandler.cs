using System;
using System.Collections.Generic;
using Colosseum.Authentication;
using Colosseum.Data;
using Colosseum.Unit;
using UnityEngine;

namespace Colosseum.Customizing
{
    public class CustomHandler : MonoBehaviour
    {
        private CustomCharacter character;
        private Dictionary<CustomPart, GameObject> equipped = new();

        private void Awake()
        {
            character = GetComponent<CustomCharacter>();
            if (equipped == null)
            {
                equipped = new Dictionary<CustomPart, GameObject>();
            }
        }

        public void ApplyPart(CustomPart part, int index)
        {
            string partName = GetStringCustomizePart(part, index);
            Transform root = GetRootTransform(part, partName);
            Transform target = root.Find(partName);
            
            if (!root.gameObject.activeSelf)
                root.gameObject.SetActive(true);
            

            if (part == CustomPart.Hair || part == CustomPart.FacialHair)
            {
                if (target)
                    SwitchPart(part, target.gameObject);
            }
            else
            {
                foreach (Transform child in root)
                {
                    if (child.name.EndsWith(part.ToString()))
                    {
                        SwitchPart(part, child.gameObject);
                        return;
                    }
                }
            }
        }

        private string GetStringCustomizePart(CustomPart part, int index)
        {
            return part switch
            {
                CustomPart.Hair => $"Hair_{index + 1:D2}",
                CustomPart.FacialHair => $"FacialHair_{index:D2}",
                _ => index == 0 ? "Body" : $"Armor_{index:D3}"
            };
        }

        private Transform GetRootTransform(CustomPart part, string partName)
        {
            return part switch
            {
                CustomPart.Hair => character.hairMeshRoot,
                CustomPart.FacialHair => character.facialHairRoot,
                _ => character.isFemale
                    ? character.femaleMeshRoot.Find(partName)
                    : character.maleMeshRoot.Find(partName)
            };
        }

        private void SwitchPart(CustomPart part, GameObject newPart)
        {
            if (equipped.TryGetValue(part, out GameObject oldPart) && oldPart != null)
            {
                if (oldPart != newPart)
                {
                    ParentActiveCheck(oldPart, part);
                }
            }

            newPart.SetActive(true);
            equipped[part] = newPart;
        }

        public void Unequip(CustomPart part)
        {
            if (equipped.TryGetValue(part, out GameObject oldPart) && oldPart != null)
            {
                oldPart.SetActive(false);
                equipped[part] = null;
            }
        }
        
        /// <summary>
        /// 이전 파츠의 부모의 자식들의 활성화 상태를 확인한 다음 전부 비활성화 되어있으면 부모도 비활성화 하는 메서드 <br/>
        /// 이전 파츠는 비활성화 처리한 다음 판단
        /// </summary>
        /// <param name="oldPart">바꾸기 전 파츠</param>
        /// <param name="partToCheck">바꾸기 전 파츠 부위</param>
        private void ParentActiveCheck(GameObject oldPart, CustomPart partToCheck)
        {
            var oldPartParent = oldPart.transform.parent;
            int childCount = oldPartParent.transform.childCount;
            bool isActive = false;
            oldPart.SetActive(false);

            // 이전 파츠의 부모가 가지고있는 자식 중 하나라도 활성화 된 상태가 있다면 부모도 활성화 된 상태 유지 아니면 비활성화
            if (partToCheck != CustomPart.Hair && partToCheck != CustomPart.FacialHair)
            {
                for (int i = 0; i < childCount; i++)
                {
                    if (oldPartParent.GetChild(i).gameObject.activeSelf)
                    {
                        isActive = true;
                        break; // 하나라도 true라면 활성화 된게 있으니 반복문 탈출
                    }
                }
                oldPartParent.gameObject.SetActive(isActive);
            }
        }
        public void ApplyCustomization(int weaponIdx)
        {
            switch ((CharacterClass)weaponIdx)
            {
                case CharacterClass.Warrior:
                case CharacterClass.Knight:
                    character.weaponSwordRoot.gameObject.SetActive(true);
                    character.weaponShieldRoot.gameObject.SetActive(true);
                    break;
                case CharacterClass.Archer:
                case CharacterClass.Ranger:
                    character.weaponBowRoot.gameObject.SetActive(true);
                    break;
                case CharacterClass.WizardIce:
                case CharacterClass.WizardFire:
                case CharacterClass.Priest:
                    character.weaponStaffRoot.gameObject.SetActive(true);
                    break;
                // 아래 직업 미구현 
                case CharacterClass.Egoist:
                    break;
                case CharacterClass.ArmoredMagician:
                    break;
                
                case CharacterClass.Bard:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(weaponIdx), weaponIdx, null);
            }
            character.isFemale = DataManager.Instance.IsFemale;
            character.femaleMeshRoot.gameObject.SetActive(character.isFemale);
            character.maleMeshRoot.gameObject.SetActive(!character.isFemale);
            
            character.hairMeshRoot.GetChild(0).gameObject.SetActive(false);
            
            foreach (var custom in DataManager.Instance.CurrentIndex)
            {
                ApplyPart(custom.Key, custom.Value);
            }

            character.leftBust.transform.localScale = Vector3.one * DataManager.Instance.BustSize;
            character.rightBust.transform.localScale = Vector3.one * DataManager.Instance.BustSize;
        }
        
        public void ApplyCustomization(int weaponIdx, CustomizeData data)
        {
            switch ((CharacterClass)weaponIdx)
            {
                case CharacterClass.Warrior:
                case CharacterClass.Knight:
                    character.weaponSwordRoot.gameObject.SetActive(true);
                    character.weaponShieldRoot.gameObject.SetActive(true);
                    break;
                case CharacterClass.Archer:
                case CharacterClass.Ranger:
                    character.weaponBowRoot.gameObject.SetActive(true);
                    break;
                case CharacterClass.WizardIce:
                case CharacterClass.WizardFire:
                case CharacterClass.Priest:
                    character.weaponStaffRoot.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
            
            character.isFemale = data.isFemale == 1;
            character.femaleMeshRoot.gameObject.SetActive(character.isFemale);
            character.maleMeshRoot.gameObject.SetActive(!character.isFemale);
    
            character.hairMeshRoot.GetChild(0).gameObject.SetActive(false);

            ApplyPart(CustomPart.Hair, data.hair);
            ApplyPart(CustomPart.FacialHair, data.facialHair);
            ApplyPart(CustomPart.Chest, data.chest);
            ApplyPart(CustomPart.Arm, data.arm);
            ApplyPart(CustomPart.Waist, data.waist);
            ApplyPart(CustomPart.Leg, data.leg);

            character.leftBust.transform.localScale = Vector3.one * data.bustSize;
            character.rightBust.transform.localScale = Vector3.one * data.bustSize;
        }
    }
}