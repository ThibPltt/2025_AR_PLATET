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
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                float deltaY = touch.position.y - startTouchPosition.y;

                float newHeight = Mathf.Clamp(currentHeight + deltaY * sensitivity, minHeight, maxHeight);

                terrain.UpdateTerrain(newHeight);
                currentHeight = newHeight;

                startTouchPosition = touch.position;
            }
        }
    }
}
