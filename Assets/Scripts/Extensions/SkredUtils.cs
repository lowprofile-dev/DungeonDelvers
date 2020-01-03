using System;
using System.Collections;
using System.Collections.Generic;

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
        
        public static Ref<T> CreateRef<T>(this T @ref) => new Ref<T>(@ref);
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
}

