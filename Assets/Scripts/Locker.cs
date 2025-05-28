using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TerrainManager : MonoBehaviour
{
    [SerializeField] private GameObject ProceduralTerrain;
    [SerializeField] private Button toggleButton;
    [SerializeField] private TextMeshProUGUI toggleButtonText;

    private ProceduralTerrain currentTerrain;
    private TerrainHeightController currentTHC;
    private bool isLocked = false;

    void Start()
    {
        UpdateButtonText(); // Appliquer couleur rouge dès le début
    }

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
        {
            Debug.LogWarning("TerrainHeightController non trouvé sur le clone !");
        }
        else
        {
            currentTHC.enabled = true;
            isLocked = false;
            UpdateButtonText();
        }

        Debug.Log("Clone de terrain créé et THC activé.");
    }

    public void ToggleTerrainLock()
    {
        if (currentTerrain == null)
        {
            ProceduralTerrain pt = FindObjectOfType<ProceduralTerrain>();
            if (pt != null && pt.name.Contains("Clone"))
            {
                currentTerrain = pt;
                currentTHC = pt.GetComponent<TerrainHeightController>();
            }
            else
            {
                Debug.LogWarning("Aucun clone trouvé.");
                return;
            }
        }

        if (!isLocked)
        {
            currentTerrain.enabled = false;
            if (currentTHC != null) currentTHC.enabled = false;
            isLocked = true;
            Debug.Log("Terrain verrouillé.");
        }
        else
        {
            currentTerrain.enabled = true;
            if (currentTHC != null) currentTHC.enabled = true;
            isLocked = false;
            Debug.Log("Terrain déverrouillé.");
        }

        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        if (toggleButtonText != null)
        {
            toggleButtonText.text = isLocked ? "Unlock" : "Lock";
            toggleButtonText.color = isLocked ? Color.green : Color.red;
        }

        if (toggleButton != null && toggleButton.image != null)
        {
            toggleButton.image.color = isLocked ? new Color(0.7f, 1f, 0.7f) : new Color(1f, 0.7f, 0.7f);
        }
    }
}
