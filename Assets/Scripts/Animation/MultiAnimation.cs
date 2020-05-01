using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DD.Skill.Animation
{
    public class MultiAnimation : IPlayerSkillAnimation
    {
        public List<CharacterBattler.CharacterBattlerAnimation> Animations = new List<CharacterBattler.CharacterBattlerAnimation>();
        public float speedModifier = 1f;
        public float delay;
        
        public async Task Play(CharacterBattler battler)
        {
            foreach (var animation in Animations)
            {
                await battler.AsyncPlayAndWait(animation, speedModifier);
                await Task.Delay((int) (delay * 1000));
            }
        }
    }
}