using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlacement : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public GameObject terrainPrefab;

    private ARPlane selectedPlane;
    private GameObject spawnedTerrain;
    private bool isPlaced = false;

    void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (isPlaced) return;

        List<ARPlane> allPlanes = new List<ARPlane>();
        foreach (var plane in planeManager.trackables)
            allPlanes.Add(plane);

        if (allPlanes.Count == 0) return;

        selectedPlane = allPlanes[0];
        foreach (var plane in allPlanes)
        {
            if (plane.transform.position.y < selectedPlane.transform.position.y)
                selectedPlane = plane;
        }

        if (selectedPlane != null)
        {
            spawnedTerrain = Instantiate(terrainPrefab, selectedPlane.transform.position, Quaternion.identity);

            ProceduralTerrain terrainScript = spawnedTerrain.GetComponent<ProceduralTerrain>();
            if (terrainScript != null)
            {
                Vector3 planePos = selectedPlane.transform.position;
                terrainScript.offset = new Vector2(planePos.x, planePos.z);
                terrainScript.GenerateBaseTerrain();
                terrainScript.AnimateUp();
            }

            foreach (var plane in allPlanes)
            {
                if (plane != selectedPlane)
                    plane.gameObject.SetActive(false);
            }

            isPlaced = true;
        }
    }
}
