using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemy;

    private int enemyCount;
    public int maxEnemyCount;

    
    void Start()
    {
        enemyCount = 0;
        maxEnemyCount = 10;

        StartCoroutine("Spawner");
    }
    
    void Update()
    {
        
    }

    IEnumerator Spawner()
    {
        yield return new WaitForSeconds(2.0f);

        while (enemyCount < maxEnemyCount)
        {
            Instantiate(enemy);
            ++enemyCount;
            yield return new WaitForSeconds(2.0f);
        }
    }
}
