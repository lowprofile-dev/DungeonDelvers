using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DD.Skill.Animation
{
    public class MultiAnimation : IPlayerSkillAnimation
    {
        public List<BattlerAnimationInfo> Animations = new List<BattlerAnimationInfo>();
        public float animationDelay;
        
        public async Task Play(CharacterBattler battler)
        {
            foreach (var info in Animations)
            {
                await battler.AsyncPlayAndWait(info);
                await Task.Delay((int) (animationDelay * 1000));
            }
        }
    }
}