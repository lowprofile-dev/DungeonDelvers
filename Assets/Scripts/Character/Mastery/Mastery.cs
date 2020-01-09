using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public class Mastery
{
    public string MasteryName;
    [ValidateInput("_validateMaxLevel", "Max Level precisa ser maior ou igual a 1")]public int MasteryMaxLevel = 1;
    public List<MasteryCondition> Conditions = new List<MasteryCondition>();
    
    //efeitos

#if UNITY_EDITOR
    private bool _validateMaxLevel(int level) => level >= 1;
#endif
}