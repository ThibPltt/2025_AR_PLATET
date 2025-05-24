using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnBuilding : MonoBehaviour
{
    [Header("Param�tres de placement")]
    public GameObject buildingPrefab;        // Prefab du b�timent
    public Camera mainCamera;                // Cam�ra principale
    public LayerMask terrainLayer;           // Layer du terrain
    public Vector3 buildingScale = new Vector3(0.1f, 0.1f, 0.1f); // Taille du b�timent

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
                isPlacing = false; // D�sactive le mode placement apr�s un seul b�timent plac�
            }
        }
    }

    // M�thode � appeler via bouton UI
    public void EnablePlacement()
    {
        isPlacing = true;
        Debug.Log("Mode placement de b�timent activ� !");
    }
}
