using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;


public class ARPlacement : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public GameObject terrainPrefab;

    private ARPlane selectedPlane;
    private GameObject spawnedTerrain;
    private bool isPlaced = false;
    public GameObject resetButton;


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
        if (isPlaced || terrainPrefab == null)
            return;

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
            // Instancie temporairement à la position (0, 0, 0)
            spawnedTerrain = Instantiate(terrainPrefab, Vector3.zero, Quaternion.identity);

            ProceduralTerrain terrainScript = spawnedTerrain.GetComponent<ProceduralTerrain>();
            if (terrainScript != null)
            {
                Vector3 planePos = selectedPlane.transform.position;
                terrainScript.offset = new Vector2(planePos.x, planePos.z);
                terrainScript.GenerateBaseTerrain();


                // Recentre après génération
                float halfWidth = terrainScript.width / 2f;
                float halfHeight = terrainScript.height / 2f;
                spawnedTerrain.transform.position = planePos - new Vector3(halfWidth, 0, halfHeight);

                terrainScript.AnimateUp();
            }

            foreach (var plane in allPlanes)
            {
                if (plane != selectedPlane)
                    plane.gameObject.SetActive(false);
            }

            isPlaced = true;

            if (resetButton != null)
                resetButton.SetActive(true);

        }
    }

    public void ResetScene()
    {
        if (spawnedTerrain != null)
        {
            var controller = spawnedTerrain.GetComponent<TerrainHeightController>();
            if (controller != null)
                controller.enabled = false;
        }

        if (resetButton != null)
            StartCoroutine(HideResetButtonWithDelay());
    }

    private IEnumerator HideResetButtonWithDelay()
    {
        // Facultatif : changer texte si tu utilises Text
        var text = resetButton.GetComponentInChildren<TMPro.TMP_Text>();
        if (text != null)
            text.text = "Interactions bloquées";

        yield return new WaitForSeconds(1.5f);

        resetButton.SetActive(false);
    }






}
