using UnityEngine;

public class TerrainLockController : MonoBehaviour
{
    public ProceduralTerrain proceduralTerrain;

    public void ToggleLock()
    {
        if (proceduralTerrain != null)
        {
            proceduralTerrain.isLocked = !proceduralTerrain.isLocked;
            Debug.Log("Terrain lock toggled: " + proceduralTerrain.isLocked);
        }
        else
        {
            Debug.LogWarning("ProceduralTerrain reference not set!");
        }
    }
}
