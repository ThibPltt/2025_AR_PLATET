using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [SerializeField] private GameObject ProceduralTerrain;

    private ProceduralTerrain currentTerrain;
    private TerrainHeightController currentTHC;

    // Instancie un clone proprement (appelé par exemple au démarrage ou via UI)
    public void SpawnTerrainClone()
    {
        if (currentTerrain != null)
        {
            Debug.LogWarning("Un clone existe déjà !");
            return;
        }

        GameObject cloneGO = Instantiate(ProceduralTerrain);
        cloneGO.name = ProceduralTerrain.name + " Clone";

        currentTerrain = cloneGO.GetComponent<ProceduralTerrain>();
        currentTHC = cloneGO.GetComponent<TerrainHeightController>();

        if (currentTHC == null)
            Debug.LogWarning("TerrainHeightController non trouvé sur le clone !");
        else
            currentTHC.enabled = true; // S’assurer qu’il est activé au départ

        Debug.Log("Clone de terrain créé et THC activé.");
    }

    // Verrouille le terrain (désactive ProceduralTerrain + TerrainHeightController)
    public void LockTerrain()
    {
        if (currentTerrain == null)
        {
            // Tente de récupérer un clone actif dans la scène
            ProceduralTerrain pt = FindObjectOfType<ProceduralTerrain>();
            if (pt != null && pt.name.Contains("Clone"))
            {
                currentTerrain = pt;
                currentTHC = pt.GetComponent<TerrainHeightController>();
                Debug.Log("Clone trouvé dynamiquement pour verrouillage.");
            }
            else
            {
                Debug.LogWarning("Aucun clone à verrouiller.");
                return;
            }
        }

        currentTerrain.enabled = false;
        Debug.Log("ProceduralTerrain désactivé.");

        if (currentTHC != null && currentTHC.enabled)
        {
            currentTHC.enabled = false;
            Debug.Log("TerrainHeightController désactivé.");
        }
        else if (currentTHC != null)
        {
            Debug.Log("TerrainHeightController déjà désactivé.");
        }
    }


}
