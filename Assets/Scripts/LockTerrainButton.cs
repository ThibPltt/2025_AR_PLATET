using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.UI;  // Importer pour utiliser les boutons UI

public class LockTerrainButton : MonoBehaviour
{
    public GameObject cubePrefab;
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public ARAnchorManager anchorManager;

    public Button confirmButton; // Bouton de confirmation
    private bool hasPlacedCube = false;
    private GameObject placedCube = null;  // Référence pour le cube déjà placé
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        // Lier le bouton à la fonction de verrouillage du cube
        confirmButton.onClick.AddListener(LockCube);
    }

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

            // Crée une ancre sur le plan pour fixer le cube
            ARAnchor anchor = anchorManager.AttachAnchor(hitPlane, hitPose);
            if (anchor == null)
            {
                Debug.Log("Impossible de créer une ancre.");
                return;
            }

            // 🧱 Instancie le cube à l'ancre
            placedCube = Instantiate(cubePrefab, hitPose.position, hitPose.rotation);
            placedCube.transform.SetParent(anchor.transform);  // Fixe le cube à l'ancre

            // Le cube peut maintenant être verrouillé
            hasPlacedCube = true;

            // Désactiver la détection et l'affichage des plans après la pose
            planeManager.enabled = false;
            foreach (var plane in planeManager.trackables)
                plane.gameObject.SetActive(false);

            Debug.Log("Cube posé. Appuie sur 'OK' pour le verrouiller.");
        }
    }

    // Fonction pour verrouiller le cube (après avoir appuyé sur le bouton)
    void LockCube()
    {
        if (placedCube != null)
        {
            // Désactive la possibilité de le déplacer ou de le modifier
            placedCube.GetComponent<Collider>().enabled = false;
            placedCube.transform.SetParent(null); // Détache le cube de l'ancre
            Debug.Log("Cube verrouillé et ne peut plus être déplacé.");
        }
    }
}
