using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading.Tasks;
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

        public static IEnumerator TextWriter(TMP_Text textObject, string text, int updateFrequency = 1)
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
            source.PlayOneShot(info.AudioClip,info.Volume);
        }
    }

    public class Ref<T>
    {
        public T Instance { get; set; }
        public bool HasValue => Instance != null;

        public Ref()
        {

        }

        public Ref(T t)
        {
            Instance = t;
        }

        public static implicit operator T(Ref<T> @ref) => @ref.Instance;
        //public static implicit operator Ref<T>(T inst) => new Ref<T>(inst);
    }
    
    //Credit Enigmativity@https://stackoverflow.com/questions/10966331/two-way-bidirectional-dictionary-in-c
    public class Map<T1, T2>
    {
        private Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        public Map()
        {
            this.Forward = new Indexer<T1, T2>(_forward);
            this.Reverse = new Indexer<T2, T1>(_reverse);
        }

        public class Indexer<T3, T4>
        {
            private Dictionary<T3, T4> _dictionary;
            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }
            public T4 this[T3 index]
            {
                get { return _dictionary[index]; }
                set { _dictionary[index] = value; }
            }
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public Indexer<T1, T2> Forward { get; private set; }
        public Indexer<T2, T1> Reverse { get; private set; }
    }
    
}

