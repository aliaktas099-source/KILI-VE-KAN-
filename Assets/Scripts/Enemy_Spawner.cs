using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    public GameObject Samuray;
    public GameObject Archer;
    public GameObject Boss;
    public List<Transform> spawnPoints;
    private List<Transform> spawnedPoints;
    void Start()
    {
        spawnedPoints = spawnPoints;
        switch(PlayerPrefs.GetInt("Level"))
        {
            case 1:
                SpawnEnemies(Samuray, 3);
                break;
            case 2:
                SpawnEnemies(Archer, 3);
                break;
            case 3:
                SpawnEnemies(Samuray, 3);
                SpawnEnemies(Archer, 2);
                break;
            case 4:
                SpawnEnemies(Boss, 1);
                SpawnEnemies(Samuray, 2);
                SpawnEnemies(Archer, 1);
                break;
        }
    }

    private void SpawnEnemies(GameObject enemy, int count)
    {
        
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);
            Transform spawnPoint = spawnPoints[randomIndex];
            Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
            spawnedPoints.Remove(spawnPoint);
        }
        
    }
}