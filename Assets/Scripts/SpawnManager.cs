using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<SpawnPoint> _spawnPoints;
    [SerializeField] private GameObject _charecterPrefab;

    private void Start()
    {
        SpawnCharecters(_charecterPrefab, _charecterPrefab);
    }

    public void SpawnCharecters(params GameObject[] gameObjects)
    {
        //check if there are enough spawn points for all players
        if (gameObjects.Length > _spawnPoints.Count)
        {
            Debug.LogError("Not enough spawn points");
            return;
        }

        int awaiter = 0;

        for (int i = 0; i < gameObjects.Length;)
        {
            awaiter++;
            if (GetRandomSpawnPoint().TrySpawn(gameObjects[i]))
            {
                i++;
                awaiter = 0;
            }
            //if the randomization is too long
            if (awaiter > 100)
            {
                GetFreeSpawnPoint()?.TrySpawn(gameObjects[i]);
                awaiter = 0;
            }
        }
    }

    private SpawnPoint GetRandomSpawnPoint()
    {
        return _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)];
    }

    private SpawnPoint GetFreeSpawnPoint()
    {
        foreach (var spawnPoint in _spawnPoints)
        {
            if (!spawnPoint.IsTaken)
                return spawnPoint;
        }

        Debug.LogError("No empty spawn points");
        return null;
    }
}