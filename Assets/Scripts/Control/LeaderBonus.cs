using System;

public static class LeaderBonus 
{
    //Botar depois um Dictionary<Class, Description> pra mostrar na UI
    public static void ApplyLeaderBonus(Class leaderClass)
    {
        switch (leaderClass)
        {
            case Class.Knight:
                GameController.Instance.GlobalPriceModifier *= 0.8f;
                break;
            case Class.Fighter:
                //passiva que dá X% mais dano contra inimigos com <Y% de vida
                break;
            case Class.Rogue:
                //Loot melhorada de luta/baus (ver como vai ser e se vai ser)
                break;
            case Class.Assassin:
                //Sempre age primeiro (botar opção nos combates de ter como dar override)
                break;
            case Class.Priest:
                //passiva que recupera 10% da vida perdida no final de uma luta
                break;
            case Class.Magician:
                //Party toda ganha EP bonus por turno (?) ou EP bonus no inicio (?)
                break;
            case Class.Paladin:
                //X% redução de dano quando <Y% de vida
                break;
            case Class.Hunter:
                GameController.Instance.GlobalExperienceModifier *= 1.2f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(leaderClass), leaderClass, null);
        }
    }
}

public enum Class
{
    Knight,
    Fighter,
    Rogue,
    Assassin,
    Priest,
    Magician,
    Paladin,
    Hunter
}
