using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [SerializeField] private GameObject ProceduralTerrain;

    private ProceduralTerrain currentTerrain;
    private TerrainHeightController currentTHC;

    // Instancie un clone proprement (appel� par exemple au d�marrage ou via UI)
    public void SpawnTerrainClone()
    {
        if (currentTerrain != null)
        {
            Debug.LogWarning("Un clone existe d�j� !");
            return;
        }

        GameObject cloneGO = Instantiate(ProceduralTerrain);
        cloneGO.name = ProceduralTerrain.name + " Clone";

        currentTerrain = cloneGO.GetComponent<ProceduralTerrain>();
        currentTHC = cloneGO.GetComponent<TerrainHeightController>();

        if (currentTHC == null)
            Debug.LogWarning("TerrainHeightController non trouv� sur le clone !");
        else
            currentTHC.enabled = true; // S�assurer qu�il est activ� au d�part

        Debug.Log("Clone de terrain cr�� et THC activ�.");
    }

    // Verrouille le terrain (d�sactive ProceduralTerrain + TerrainHeightController)
    public void LockTerrain()
    {
        if (currentTerrain == null)
        {
            // Tente de r�cup�rer un clone actif dans la sc�ne
            ProceduralTerrain pt = FindObjectOfType<ProceduralTerrain>();
            if (pt != null && pt.name.Contains("Clone"))
            {
                currentTerrain = pt;
                currentTHC = pt.GetComponent<TerrainHeightController>();
                Debug.Log("Clone trouv� dynamiquement pour verrouillage.");
            }
            else
            {
                Debug.LogWarning("Aucun clone � verrouiller.");
                return;
            }
        }

        currentTerrain.enabled = false;
        Debug.Log("ProceduralTerrain d�sactiv�.");

        if (currentTHC != null && currentTHC.enabled)
        {
            currentTHC.enabled = false;
            Debug.Log("TerrainHeightController d�sactiv�.");
        }
        else if (currentTHC != null)
        {
            Debug.Log("TerrainHeightController d�j� d�sactiv�.");
        }
    }


}
