using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public GameObject armoredEnemyPrefab;
    public GameObject splitterEnemyPrefab;
    public GameObject friendlySpiritPrefab;
    public float initialSpawnDelay = 2f;
    public float timeBetweenSpawns = 2.5f;
    public bool spawnEnemies = true;
    public Transform enemyContainer;
    public float spawnYOffset = -10f;
    public float randomOffset = 3f;
    public float minDistanceBetweenEnemies = 2f;
    public float minXDifference = 1.5f;
    public Vector2 scaleRange = new Vector2(1.1f, 1.3f);
    [Range(0, 100)] public float armoredWeight = 40f;
    [Range(0, 100)] public float splitterWeight = 40f;
    [Range(0, 100)] public float friendlyWeight = 20f;
    private List<Transform> spawnPoints = new List<Transform>();
    private HashSet<float> usedXPositions = new HashSet<float>();
    private Coroutine spawnRoutine;


    private void Start()
    {
        CacheSpawnPoints();
        EnsureContainer();
        StartSpawning();
    }

    void CacheSpawnPoints()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("SpawnPoint");

        spawnPoints.Clear();
        foreach (var p in points)
        {
            if (p != null)
                spawnPoints.Add(p.transform);
        }

    }

    void EnsureContainer()
    {
        if (enemyContainer != null) return;

        GameObject containerObj = GameObject.Find("_EnemyContainer");
        if (containerObj != null)
            enemyContainer = containerObj.transform;
    }


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


    void SpawnRandomEnemy()
    {
        if (spawnPoints.Count == 0) return;

        Transform randomSpawn = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Vector3 spawnPos = randomSpawn.position;
        spawnPos.y += spawnYOffset;

        spawnPos = AvoidOverlap(spawnPos);

        GameObject prefab = ChooseEnemyByWeight();
        if (prefab == null) return;

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity, enemyContainer);

        float scale = Random.Range(scaleRange.x, scaleRange.y);
        enemy.transform.localScale = Vector3.one * scale;
        enemy.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

    }


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


    public void RemoveX(float x)
    {
        usedXPositions.RemoveWhere(v => Mathf.Abs(v - x) < 0.05f);
    }
}
