using System.Threading.Tasks;
using UnityEngine;

public class DragonbonesMonsterBattler : MonsterBattler
{
    public override void LoadMonsterBase(Monster monsterBase, int level)
    {
        //Fazer MonsterBattler<T>?
        if (monsterBase is DragonbonesMonster == false)
        {
            Debug.LogError("Invalid Monster Type");
            return;
        }
        
    }
}
