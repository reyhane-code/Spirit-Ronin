using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject armoredEnemyPrefab;
    public GameObject splitterEnemyPrefab;
    public GameObject friendlySpiritPrefab;

    [Header("Spawning Settings")]
    public float initialSpawnDelay = 2f;
    public float timeBetweenSpawns = 2.5f;
    public bool spawnEnemies = true;

    [Header("Spawn Area")]
    public Transform enemyContainer;
    public float spawnYOffset = -10f;

    [Header("Spawn Variations")]
    public float randomOffset = 3f;
    public float minDistanceBetweenEnemies = 2f;

    [Header("Unique X Control")]
    public float minXDifference = 1.5f;

    [Header("Scale")]
    public Vector2 scaleRange = new Vector2(1.1f, 1.3f);

    [Header("Enemy Weights (0-100)")]
    [Range(0, 100)] public float armoredWeight = 40f;
    [Range(0, 100)] public float splitterWeight = 40f;
    [Range(0, 100)] public float friendlyWeight = 20f;

    private List<Transform> spawnPoints = new List<Transform>();
    private HashSet<float> usedXPositions = new HashSet<float>();
    private Coroutine spawnRoutine;

    // ===================== UNITY =====================

    private void Start()
    {
        CacheSpawnPoints();
        EnsureContainer();
        StartSpawning();
    }

    // ===================== SETUP =====================

    void CacheSpawnPoints()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("SpawnPoint");

        spawnPoints.Clear();
        foreach (var p in points)
        {
            if (p != null)
                spawnPoints.Add(p.transform);
        }

        if (spawnPoints.Count == 0)
            Debug.LogError("SpawnManager: No SpawnPoints found! (Tag = SpawnPoint)");
    }

    void EnsureContainer()
    {
        if (enemyContainer != null) return;

        GameObject containerObj = GameObject.Find("_EnemyContainer");
        if (containerObj != null)
            enemyContainer = containerObj.transform;
    }

    // ===================== SPAWNING CONTROL =====================

    public void StartSpawning()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        spawnRoutine = null;
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(initialSpawnDelay);

        while (spawnEnemies)
        {
            SpawnRandomEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    // ===================== CORE SPAWN =====================

    void SpawnRandomEnemy()
    {
        if (spawnPoints.Count == 0) return;

        Transform randomSpawn = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Vector3 spawnPos = randomSpawn.position;
        spawnPos.y += spawnYOffset;

        // Final overlap safety
        spawnPos = AvoidOverlap(spawnPos);

        GameObject prefab = ChooseEnemyByWeight();
        if (prefab == null) return;

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity, enemyContainer);

        // 🔹 Bigger scale (controlled)
        float scale = Random.Range(scaleRange.x, scaleRange.y);
        enemy.transform.localScale = Vector3.one * scale;
        enemy.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        Debug.Log($"Spawned {enemy.name} at X={spawnPos.x}");
    }

    // ===================== HELPERS =====================

    Vector3 AvoidOverlap(Vector3 proposed)
    {
        if (enemyContainer == null) return proposed;

        foreach (Transform child in enemyContainer)
        {
            if (child == null) continue;

            float dist = Vector3.Distance(proposed, child.position);
            if (dist < minDistanceBetweenEnemies)
            {
                Vector3 dir = (proposed - child.position).normalized;
                proposed = child.position + dir * minDistanceBetweenEnemies;
                break;
            }
        }

        return proposed;
    }

    GameObject ChooseEnemyByWeight()
    {
        float total = armoredWeight + splitterWeight + friendlyWeight;
        if (total <= 0f) return armoredEnemyPrefab;

        float r = Random.Range(0f, total);

        if (r < armoredWeight)
            return armoredEnemyPrefab;

        if (r < armoredWeight + splitterWeight)
            return splitterEnemyPrefab;

        return friendlySpiritPrefab;
    }

    // ===================== CLEANUP =====================

    public void RemoveX(float x)
    {
        usedXPositions.RemoveWhere(v => Mathf.Abs(v - x) < 0.05f);
    }
}
