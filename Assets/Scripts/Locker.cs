using UnityEngine;

public class TerrainLockWatcher : MonoBehaviour
{
    private ProceduralTerrain lockedTerrain;

    // Appelé chaque frame jusqu'à ce qu'on trouve et verrouille le clone
    private void Update()
    {
        if (lockedTerrain == null)
        {
            // Cherche un clone actif de ProceduralTerrain
            ProceduralTerrain pt = FindObjectOfType<ProceduralTerrain>();
            if (pt != null && pt.name.Contains("Clone"))
            {
                lockedTerrain = pt;
                LockTerrain(lockedTerrain);
            }
        }
    }

    private void LockTerrain(ProceduralTerrain terrain)
    {
        terrain.enabled = false;

        Debug.Log("Terrain clone verrouillé automatiquement.");
    }
}
