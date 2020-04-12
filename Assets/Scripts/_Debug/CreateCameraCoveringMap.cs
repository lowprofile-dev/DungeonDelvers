using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class CreateCameraCoveringMap : MonoBehaviour
{
    public TilemapManager TilemapManager;
    private bool active = false;

    [Button] public void CreateCameraAtMiddle()
    {
       var tilemaps = TilemapManager.Tilemaps;
       var centers = tilemaps.Select(tilemap => tilemap.localBounds.center);
       
       var center = new Vector3();
       centers.ForEach(c => center += c);
       center /= centers.Count();
       
       var cameraObj = new GameObject("Minimap Camera", typeof(Camera));
       cameraObj.transform.position = center + new Vector3(0,0,-10);
       var camera = cameraObj.GetComponent<Camera>();
       camera.orthographic = true;
    }
    
    //get bounds of all tilemaps
    
    [Button] public void ToggleMinimap()
    {
        //var cameraObj = new GameObject("Camera");
    }

    public void ToggleTilemap()
    {
        
    }
    
    public void ToggleMapTile(){}
}