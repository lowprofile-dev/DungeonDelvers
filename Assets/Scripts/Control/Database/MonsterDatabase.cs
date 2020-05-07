using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Monster Database")]
public class MonsterDatabase : Database<Monster>
{
    //Monstros já vistos são salvos fora do save.
    public void BuildBestiary()
    {
        throw new NotImplementedException();
    }
}
