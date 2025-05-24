using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnBuilding : MonoBehaviour
{
    [Header("Paramètres de placement")]
    public GameObject buildingPrefab;        // Prefab du bâtiment
    public Camera mainCamera;                // Caméra principale
    public LayerMask terrainLayer;           // Layer du terrain
    public Vector3 buildingScale = new Vector3(0.1f, 0.1f, 0.1f); // Taille du bâtiment

    private bool isPlacing = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (isPlacing && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, terrainLayer))
            {
                GameObject building = Instantiate(buildingPrefab, hit.point, Quaternion.identity);
                building.transform.localScale = buildingScale;
                isPlacing = false; // Désactive le mode placement après un seul bâtiment placé
            }
        }
    }

    // Méthode à appeler via bouton UI
    public void EnablePlacement()
    {
        isPlacing = true;
        Debug.Log("Mode placement de bâtiment activé !");
    }
}
