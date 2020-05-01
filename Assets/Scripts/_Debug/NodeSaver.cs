#if UNITY_EDITOR
using System.Linq;
using UnityEditor.Experimental.SceneManagement;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using XNode;

public class NodeSaver : MonoBehaviour
{
    [Button] public void SaveNodes()
    {
        var prefabScene = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabScene == null) return;
        
        var sceneGraphs = FindObjectsOfType<SceneGraph>();
        var graphs = sceneGraphs.Select(sceneGraph => sceneGraph.graph);
        
        foreach (var graph in graphs)
        {
            AssetDatabase.AddObjectToAsset(graph,prefabScene.prefabAssetPath);
        }
    }
}
#endif
