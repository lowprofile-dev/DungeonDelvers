using System.Threading.Tasks;
using UnityEngine;

namespace DD.Skill.Animation
{
    public class SingleAnimation : IPlayerSkillAnimation
    {
        public BattlerAnimationInfo Animation;

        public Task Play(CharacterBattler battler)
        {
            return battler.AsyncPlayAndWait(Animation);
        }
    }
}