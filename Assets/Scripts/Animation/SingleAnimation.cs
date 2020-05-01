using System.Threading.Tasks;
using UnityEngine;

namespace DD.Skill.Animation
{
    public class SingleAnimation : IPlayerSkillAnimation
    {
        public CharacterBattler.CharacterBattlerAnimation Animation;
        public float speed = 1f;

        public Task Play(CharacterBattler battler)
        {
            return battler.AsyncPlayAndWait(Animation,speed);
        }
    }
}