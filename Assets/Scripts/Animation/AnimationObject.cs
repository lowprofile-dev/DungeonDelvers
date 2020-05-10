using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SkredUtils;
using UnityEngine;

namespace DD.Skill.Animation
{
    public class AnimationObject : AsyncMonoBehaviour
    {
        public Animator animator;
        public AudioSource AudioSource;

        private void Awake()
        {
            this.Ensure(ref AudioSource);
            AudioSource.outputAudioMixerGroup = GameSettings.Instance.SFXChannel;
        }

        public async Task PlayAndAwait(string animation, SoundInfo soundInfo = null, float speedMultiplier = 1f)
        {
            await PlayCoroutine(AnimationCoroutine(animation, soundInfo, speedMultiplier));
        }

        private IEnumerator AnimationCoroutine(string animation, SoundInfo soundInfo, float speedMultiplier)
        {
            var hashHame = Animator.StringToHash(animation);
            if (!animator.HasState(0,hashHame))
                yield break;

            animator.speed = speedMultiplier;
            animator.Play(hashHame);
            AudioSource.PlayOneShot(soundInfo);
            
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            
            yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash == hashHame);
        }
    }
}

