using System;

[Obsolete] //Não ter condições já faz o mesmo efeito.
public class AlwaysUseSkillCondition : ISkillCondition
{
    public bool Evalute(MonsterBattler source, Skill skill) => true;
}