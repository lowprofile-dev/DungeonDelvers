using Sirenix.OdinInspector;
using UnityEngine;


    public class testgetrectransform : SerializedMonoBehaviour
    {
        [Button("1")]
        public void Test()
        {
            var hasRect = TryGetComponent<RectTransform>(out var rect);
            Debug.Log(hasRect);
        }

        [Button("2")]
        public void Test2()
        {
            gameObject.AddComponent<RectTransform>();
        }
    }
