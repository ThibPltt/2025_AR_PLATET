using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnSoldier : MonoBehaviour
{
    [Header("Param�tres de placement")]
    public GameObject soldierPrefab;
    public Camera mainCamera;
    public LayerMask terrainLayer;

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
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, terrainLayer))
            {
                GameObject soldier = Instantiate(soldierPrefab, hit.point, Quaternion.identity);
                soldier.transform.localScale = soldier.transform.localScale * 0.07f;  // R�duction taille � 30%
                isPlacing = false; // D�sactive mode placement apr�s clic
            }
        }
    }

    // M�thode � appeler via bouton UI
    public void EnablePlacement()
    {
        isPlacing = true;
        Debug.Log("Mode placement de soldat activ� !");
    }
}
