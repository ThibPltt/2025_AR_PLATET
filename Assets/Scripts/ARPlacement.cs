using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public GameObject terrainPrefab;

    private ARPlane selectedPlane;
    private bool isPlaced = false;
    private GameObject spawnedTerrain;
    public int minPlaneCountBeforeLock = 5;

    void Update()
    {
        if (isPlaced || selectedPlane != null) return;

        int planeCount = 0;
        ARPlane lowestPlane = null;

        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                planeCount++;
                if (lowestPlane == null || plane.transform.position.y < lowestPlane.transform.position.y)
                {
                    lowestPlane = plane;
                }
            }
        }

        if (lowestPlane != null && planeCount >= minPlaneCountBeforeLock)
        {
            selectedPlane = lowestPlane;

            foreach (var plane in planeManager.trackables)
            {
                if (plane != selectedPlane)
                    plane.gameObject.SetActive(false);
            }

            planeManager.enabled = false;

            // Instancier le terrain vallonné
            spawnedTerrain = Instantiate(terrainPrefab, selectedPlane.transform.position, Quaternion.identity);
            spawnedTerrain.transform.rotation = Quaternion.Euler(0, selectedPlane.transform.eulerAngles.y, 0);
        }
    }
}
