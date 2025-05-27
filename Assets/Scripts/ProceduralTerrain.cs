using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTerrain : MonoBehaviour
{
    [Header("Dimensions")]
    public int width = 20;
    public int height = 20;

    [Header("Relief")]
    public float scale = 0.1f;
    public float heightMultiplier = 15f;

    [Header("Apparition")]
    public float animationDuration = 1f;

    [Header("Matériau")]
    public Material terrainMaterial;

    [Header("Décorations")]
    public GameObject[] treePrefabs;
    public GameObject[] rockPrefabs;
    public GameObject[] flowerPrefabs;
    public float decorationScaleMultiplier = 0.1f;

    [Header("Distribution")]
    public int maxTrees = 30;
    public int maxRocks = 15;
    public int maxFlowers = 40;
    public float decorationAreaMargin = 1f;

    [HideInInspector]
    public Vector2 offset;

    [HideInInspector]
    public bool isLocked = false;

    private Mesh mesh;
    private Vector3[] baseVertices;
    private Vector3[] animatedVertices;
    private List<GameObject> spawnedDecorations = new List<GameObject>();

    private bool decorationsSpawned = false; // pour éviter de les recréer en boucle

    void Start()
    {
        isLocked = false;
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

                baseVertices[i] = new Vector3(x, y, z);
                animatedVertices[i] = new Vector3(x, 0, z);
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

        mesh.RecalculateBounds();

        // Regénérer le mesh à chaque modification de la hauteur du terrain

        MeshCollider mc = GetComponent<MeshCollider>();
        if (mc == null)
        {
            mc = gameObject.AddComponent<MeshCollider>();
            mc.sharedMesh = mesh;
            mc.convex = true;
        }

        mc.sharedMesh = mesh;
        mc.enabled = true;

        ApplyMaterial();
        ClearDecorations();
        decorationsSpawned = false;
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

        if (!decorationsSpawned)
        {
            SpawnDecorations();
            decorationsSpawned = true;
        }
    }

    public void UpdateTerrain(float newHeightMultiplier)
    {
        if (isLocked) return;

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

        mesh.vertices = baseVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshCollider mc = GetComponent<MeshCollider>();
        if (mc == null)
        {
            mc = gameObject.AddComponent<MeshCollider>();
            mc.sharedMesh = mesh;
            mc.convex = true;
        }

        mc.sharedMesh = mesh;
        mc.enabled = true;

        RepositionDecorations();


    }

    void ApplyMaterial()
    {
        var renderer = GetComponent<MeshRenderer>();
        if (renderer != null && terrainMaterial != null)
            renderer.material = terrainMaterial;
    }

    public void SpawnDecorations()
    {
        ClearDecorations();

        float minX = decorationAreaMargin;
        float maxX = width - decorationAreaMargin;
        float minZ = decorationAreaMargin;
        float maxZ = height - decorationAreaMargin;

        for (int i = 0; i < maxTrees; i++)
        {
            Vector3 pos = GetRandomPointOnTerrain(minX, maxX, minZ, maxZ);
            if (treePrefabs.Length > 0)
                SpawnDecoration(treePrefabs[Random.Range(0, treePrefabs.Length)], pos);
        }

        for (int i = 0; i < maxRocks; i++)
        {
            Vector3 pos = GetRandomPointOnTerrain(minX, maxX, minZ, maxZ);
            if (rockPrefabs.Length > 0)
                SpawnDecoration(rockPrefabs[Random.Range(0, rockPrefabs.Length)], pos);
        }

        for (int i = 0; i < maxFlowers; i++)
        {
            Vector3 pos = GetRandomPointOnTerrain(minX, maxX, minZ, maxZ);
            if (flowerPrefabs.Length > 0)
                SpawnDecoration(flowerPrefabs[Random.Range(0, flowerPrefabs.Length)], pos);
        }
    }

    private Vector3 GetRandomPointOnTerrain(float minX, float maxX, float minZ, float maxZ)
    {
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);
        float y = GetHeightAtPosition(x, z);
        return new Vector3(x, y, z);
    }

    private float GetHeightAtPosition(float x, float z)
    {
        // Clamp pour éviter l'accès hors limites
        x = Mathf.Clamp(x, 0f, width - 0.001f);
        z = Mathf.Clamp(z, 0f, height - 0.001f);

        int x0 = Mathf.FloorToInt(x);
        int z0 = Mathf.FloorToInt(z);
        int x1 = x0 + 1;
        int z1 = z0 + 1;

        if (x1 > width) x1 = width;
        if (z1 > height) z1 = height;

        float sx = x - x0;
        float sz = z - z0;

        Vector3 v00 = baseVertices[z0 * (width + 1) + x0];
        Vector3 v10 = baseVertices[z0 * (width + 1) + x1];
        Vector3 v01 = baseVertices[z1 * (width + 1) + x0];
        Vector3 v11 = baseVertices[z1 * (width + 1) + x1];

        float y0 = Mathf.Lerp(v00.y, v10.y, sx);
        float y1 = Mathf.Lerp(v01.y, v11.y, sx);
        return Mathf.Lerp(y0, y1, sz);
    }


    private void SpawnDecoration(GameObject prefab, Vector3 position)
    {
        GameObject go = Instantiate(prefab, transform);
        go.transform.localPosition = position;
        go.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        float scale;

        if (System.Array.IndexOf(rockPrefabs, prefab) >= 0)
        {
            // Si c'est un rocher, échelle plus petite
            scale = Random.Range(0.2f, 0.07f);
        }
        else
        {
            // Pour les autres (arbres, fleurs)
            scale = Random.Range(0.05f, 0.15f);
        }

        go.transform.localScale = new Vector3(scale, scale, scale);
        spawnedDecorations.Add(go);
    }


    private void ClearDecorations()
    {
        foreach (var obj in spawnedDecorations)
        {
            if (obj != null)
                Destroy(obj);
        }
        spawnedDecorations.Clear();
    }

    public void RepositionDecorations()
    {
        foreach (var decoration in spawnedDecorations)
        {
            if (decoration != null)
            {
                Vector3 localPos = decoration.transform.localPosition;
                float newY = GetHeightAtPosition(localPos.x, localPos.z);
                decoration.transform.localPosition = new Vector3(localPos.x, newY, localPos.z);
            }
        }
    }

}
