using UnityEngine;

public class TerrainHeightController : MonoBehaviour
{
    public ProceduralTerrain terrain;
    public float sensitivity = 0.005f;
    public float minHeight = 0.5f;
    public float maxHeight = 5f;

    private Vector2 startTouchPosition;
    private float currentHeight;

    void Start()
    {
        if (terrain != null)
            currentHeight = terrain.heightMultiplier;
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                startTouchPosition = (touch0.position + touch1.position) / 2f;
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                Vector2 currentTouchPosition = (touch0.position + touch1.position) / 2f;
                float deltaY = currentTouchPosition.y - startTouchPosition.y;

                float newHeight = Mathf.Clamp(currentHeight + deltaY * sensitivity, minHeight, maxHeight);

                terrain.UpdateTerrain(newHeight);
                currentHeight = newHeight;

                startTouchPosition = currentTouchPosition;
            }
        }
    }
}
