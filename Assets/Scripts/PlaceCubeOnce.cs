using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class PlaceCubeOnce : MonoBehaviour
{
    public GameObject cubePrefab;
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public ARAnchorManager anchorManager;

    private bool hasPlacedCube = false;
    private GameObject placedCube = null;  // Référence pour le cube déjà placé
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        // Ne rien faire si le cube a déjà été placé
        if (hasPlacedCube || Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began)
            return;

        // Raycast sur un plan détecté
        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            ARPlane hitPlane = hits[0].trackable as ARPlane;

            // Ne pas placer un autre cube si le premier a déjà été posé
            if (hasPlacedCube)
                return;

            // 🧷 Crée une ancre sur le plan pour fixer le cube
            ARAnchor anchor = anchorManager.AttachAnchor(hitPlane, hitPose);
            if (anchor == null)
            {
                Debug.Log("Impossible de créer une ancre.");
                return;
            }

            // 🧱 Instancie le cube à l'ancre
            placedCube = Instantiate(cubePrefab, hitPose.position, hitPose.rotation);
            placedCube.transform.SetParent(anchor.transform);  // Fixe le cube à l'ancre

            // 🔒 Une fois le cube posé, on ne permet plus de nouveaux placements
            hasPlacedCube = true;

            // ❌ Désactive la détection et l'affichage des plans après la pose
            planeManager.enabled = false;
            foreach (var plane in planeManager.trackables)
                plane.gameObject.SetActive(false);

            Debug.Log("Cube placé et verrouillé à la position.");
        }
    }
}
