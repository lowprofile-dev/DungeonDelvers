using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using EncounterLayout = EncounterSet.EncounterLayout;


public class MonsterLayoutManager : MonoBehaviour
{
    public List<RectTransform> Monsters = new List<RectTransform>();
    public RectTransform MonsterLayoutRect;
    public EncounterLayout EncounterLayout;

    public void SetMonsters(List<MonsterBattler> battlers)
    {
        Monsters = battlers.Select(battler => battler.RectTransform).ToList();
    }

    public void SetLayout(EncounterLayout layout)
    {
        EncounterLayout = layout;
    }

    [Button] public void OrderRects()
    {
        Monsters.ForEach(Adjust);
        switch (EncounterLayout)
        {
            case EncounterLayout.ZigZag:
            {
                OrderZigZag();
                break;
            }
            case EncounterLayout.Line:
            {
                OrderLine();
                break;
            }
        }
    }

    private void OrderLine()
    {
        if (!Monsters.Any())
            return;

        var verticalLayoutGroup = MonsterLayoutRect.gameObject.AddComponent<VerticalLayoutGroup>();
        verticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
    }
    
    private void OrderZigZag()
    {
        if (!Monsters.Any())
            return;

        var gridLayoutGroup = MonsterLayoutRect.gameObject.AddComponent<GridLayoutGroup>();
        
        gridLayoutGroup.spacing = new Vector2(50,50);
        gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = 2;

        var objectOrder = new List<GameObject>();

        void CreateSpacing()
        {
            var obj = new GameObject("Spacing");
            obj.AddComponent<RectTransform>();
            obj.transform.SetParent(MonsterLayoutRect,false);
            objectOrder.Add(obj);
        }
        
        CreateSpacing();
        
        for (int i = 0; i < Monsters.Count; i++)
        {
            objectOrder.Add(Monsters[i].gameObject);
            if ((i + 1) % 2 == 0)
            {
                CreateSpacing(); CreateSpacing();
            }
        }

        objectOrder.ForEach(obj => obj.transform.SetAsLastSibling());
    }

    private void Adjust(RectTransform rect)
    {
        rect.localRotation = Quaternion.Euler(-55,0,0);
        rect.localPosition = new Vector3(0,0,0);
    }
}