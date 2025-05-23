using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTerrain : MonoBehaviour
{
    [Header("Dimensions")]
    public int width = 20;
    public int height = 20;

    [Header("Relief")]
    public float scale = 0.1f;            // Plus petit = relief plus serré, plus visible
    public float heightMultiplier = 15f; // Amplitude du relief

    [Header("Apparition")]
    public float animationDuration = 1f;

    [Header("Matériau")]
    public Material terrainMaterial;

    [HideInInspector]
    public Vector2 offset;

    private Mesh mesh;
    private Vector3[] baseVertices;      // hauteurs cibles
    private Vector3[] animatedVertices;  // vertices pour animation (ex : hauteur initiale 0)

    void Start()
    {
        // Génère et anime le terrain au démarrage (optionnel)
        GenerateBaseTerrain();
        AnimateUp();
    }

    public void GenerateBaseTerrain()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        baseVertices = new Vector3[(width + 1) * (height + 1)];
        animatedVertices = new Vector3[baseVertices.Length];
        int[] triangles = new int[width * height * 6];
        Vector2[] uv = new Vector2[baseVertices.Length];

        float centerX = width / 2f;
        float centerZ = height / 2f;

        int octaves = 5;
        float persistence = 0.6f;  // amortissement plus lent des octaves

        for (int i = 0, z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                // Distance normalisée au centre (colline)
                float distX = Mathf.Abs(x - centerX) / centerX;
                float distZ = Mathf.Abs(z - centerZ) / centerZ;
                float dist = Mathf.Max(distX, distZ);
                float hillFactor = Mathf.Clamp01(1f - dist);

                // Masque local bruité pour variabilité (avec remap 0.5-1)
                float mask = Mathf.PerlinNoise((x + 1000) * scale * 0.5f, (z + 1000) * scale * 0.5f);
                mask = Mathf.Lerp(0.5f, 1f, mask);

                float y = 0f;
                float frequency = 1f;
                float amplitude = heightMultiplier;

                for (int octave = 0; octave < octaves; octave++)
                {
                    float sampleXOct = (x + offset.x) * scale * frequency;
                    float sampleZOct = (z + offset.y) * scale * frequency;

                    y += Mathf.PerlinNoise(sampleXOct, sampleZOct) * amplitude;

                    frequency *= 2f;
                    amplitude *= persistence;
                }

                y *= hillFactor * mask;

                baseVertices[i] = new Vector3(x, y, z);
                animatedVertices[i] = new Vector3(x, 0, z);  // départ hauteur 0 pour animation
                uv[i] = new Vector2((float)x / width, (float)z / height);
            }
        }

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        mesh.Clear();
        mesh.vertices = animatedVertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        ApplyMaterial();
    }

    public void AnimateUp()
    {
        StartCoroutine(AnimateTerrainUp());
    }

    IEnumerator AnimateTerrainUp()
    {
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            for (int i = 0; i < baseVertices.Length; i++)
            {
                float newY = Mathf.Lerp(0, baseVertices[i].y, t);
                animatedVertices[i] = new Vector3(baseVertices[i].x, newY, baseVertices[i].z);
            }

            mesh.vertices = animatedVertices;
            mesh.RecalculateNormals();
            yield return null;
        }

        mesh.vertices = baseVertices;
        mesh.RecalculateNormals();
    }

    public void UpdateTerrain(float newHeightMultiplier)
    {
        heightMultiplier = newHeightMultiplier;

        float centerX = width / 2f;
        float centerZ = height / 2f;

        int octaves = 5;
        float persistence = 0.6f;

        for (int i = 0, z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                float distX = Mathf.Abs(x - centerX) / centerX;
                float distZ = Mathf.Abs(z - centerZ) / centerZ;
                float dist = Mathf.Max(distX, distZ);
                float hillFactor = Mathf.Clamp01(1f - dist);

                float mask = Mathf.PerlinNoise((x + 1000) * scale * 0.5f, (z + 1000) * scale * 0.5f);
                mask = Mathf.Lerp(0.5f, 1f, mask);

                float y = 0f;
                float frequency = 1f;
                float amplitude = heightMultiplier;

                for (int octave = 0; octave < octaves; octave++)
                {
                    float sampleXOct = (x + offset.x) * scale * frequency;
                    float sampleZOct = (z + offset.y) * scale * frequency;

                    y += Mathf.PerlinNoise(sampleXOct, sampleZOct) * amplitude;

                    frequency *= 2f;
                    amplitude *= persistence;
                }

                y *= hillFactor * mask;

                baseVertices[i].y = y;
                animatedVertices[i].y = y;
            }
        }

        mesh.vertices = animatedVertices;
        mesh.RecalculateNormals();
    }

    void ApplyMaterial()
    {
        var renderer = GetComponent<MeshRenderer>();
        if (renderer != null && terrainMaterial != null)
            renderer.material = terrainMaterial;
    }
}
