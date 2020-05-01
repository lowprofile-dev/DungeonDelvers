using System.Threading.Tasks;

namespace DD.Skill.Animation
{
    public interface IPlayerSkillAnimation
    {
        Task Play(CharacterBattler battler);
    }
}

