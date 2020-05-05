using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using E7.Introloop;
using TMPro;
using UnityEngine;

namespace SkredUtils
{
    public static class SkredUtils
    {
        public static void Include<T>(this List<T> list, T obj)
        {
            if (obj != null && !list.Contains(obj))
                list.Add(obj);
        }

        public static void IfNotNull<T>(this T obj, Action<T> action)
        {
            if (obj != null)
                action(obj);
        }

        public static List<TResult> EachDo<T, TResult>(this IEnumerable<T> collection, Func<T, TResult> function)
        {
            var list = new List<TResult>();

            foreach (var instance in collection)
            {
                list.Add(function(instance));
            }

            return list;
        }

        public static void SwapRef<T>(ref T obj1, ref T obj2)
        {
            var temp = obj1;
            obj1 = obj2;
            obj2 = temp;
        }

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static float Min(this float value, float min)
        {
            return value < min ? min : value;
        }

        public static float Max(this float value, float max)
        {
            return value > max ? max : value;
        }

        public static Ref<T> CreateRef<T>(this T @ref) => new Ref<T>(@ref);

        public static IEnumerator TextWriter(TMP_Text textObject, string text, int updateFrequency = 1, Action<string> writeEvent = null)
        {
            //Update frequency must be at least 1 (frames/character)
            if (updateFrequency < 1)
                throw new ArgumentException();
            
            using (var textEnumerator = text.GetEnumerator())
            {
                //While there are still characters to be printed out
                while (textEnumerator.MoveNext())
                {
                    var currentChar = textEnumerator.Current;
                    
                    //If current character is a tag start, print until tag closes (or string is over)
                    if (currentChar == '<')
                    {
                        var stringToAppend = "<";
                        while (textEnumerator.MoveNext() && textEnumerator.Current != '>')
                        {
                            stringToAppend += textEnumerator.Current;
                        }
                        
                        //Close it
                        stringToAppend += ">";

                        textObject.text += stringToAppend;
                    }
                    else
                    {
                        textObject.text += currentChar;
                    }
                    
                    writeEvent.Invoke(textObject.text);
                    
                    //Wait for X frames, where X is updateFrequency
                    for (var i = 0; i < updateFrequency; i++)
                        yield return new WaitForFixedUpdate();
                }
            }
        }

        public static string ToHex(this Color color)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(color)}";
        }

        public static async Task PlayOneShotAsync(this AudioSource source, AudioClip clip, float volume = 1f)
        {
            IEnumerator PlayCoroutine()
            {
                source.PlayOneShot(clip,volume);
                yield return new WaitForSeconds(clip.length);
            }
            await GameController.Instance.PlayCoroutine(PlayCoroutine());
        }
        
        public static async Task PlayOneShotAsync(this AudioSource source, AsyncMonoBehaviour runner, AudioClip clip, float volume = 1f)
        {
            IEnumerator PlayCoroutine()
            {
                source.PlayOneShot(clip,volume);
                yield return new WaitForSeconds(clip.length);
            }
            await runner.PlayCoroutine(PlayCoroutine());
        }

        public static void PlayOneShot(this AudioSource source, SoundInfo info)
        {
            if (info != null && info.AudioClip != null) source.PlayOneShot(info.AudioClip, info.Volume);
        }

        public static void Ensure<T>(this MonoBehaviour behaviour, ref T field) where T : Component
        {
            if (field == null && !behaviour.TryGetComponent(out field)) field = behaviour.gameObject.AddComponent<T>();
        }

        public static T Ensure<T>(this GameObject obj) where T : Component
        {
            if (!obj.TryGetComponent(out T t)) t = obj.AddComponent<T>();
            return t;
        }

        public static T Random<T>(this IEnumerable<T> collection)
        {
            if (!collection.Any()) return default;
            var index = GameController.Instance.Random.Next(collection.Count());
            return collection.ElementAt(index);
        }

        public static T WeightedRandom<T>(this T[] collection, Func<T, int> weightFunction)
        {
            var dict = new Dictionary<T, int>();
            int totalWeight = 0;
            foreach (var obj in collection)
            {
                var weight = weightFunction(obj);
                totalWeight += weight;
                dict[obj] = weight;
            }

            var index = UnityEngine.Random.Range(0, totalWeight);
            foreach (var obj in collection)
            {
                index -= dict[obj];
                if (index < 0)
                    return obj;
            }
            return default;
        }
    }

    public class Ref<T>
    {
        public T Instance;
        public bool HasValue => Instance != null;

        public Ref()
        {

        }

        public Ref(T t)
        {
            Instance = t;
        }

        public static implicit operator T(Ref<T> @ref) => @ref.Instance;
    }
    
}

