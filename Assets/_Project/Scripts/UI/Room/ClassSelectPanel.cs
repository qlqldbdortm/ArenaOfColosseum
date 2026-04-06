using Colosseum.Data;
using Colosseum.Unit;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.UI.Room
{
    public class ClassSelectPanel: MonoBehaviour
    {
        [SerializeField] private ClassSelectItem selectItemPrefab;
        [SerializeField] private Transform itemGroup;
        
        
        void Awake()
        {
            CreateClassData();
        }


        private void CreateClassData()
        {
            foreach (var classType in ClassDataManager.Classes)
            {
                ClassData data = ClassDataManager.GetData(classType);
                ClassSelectItem item = Instantiate(selectItemPrefab, itemGroup);
                item.Init(data);
            }
        }
    }
}