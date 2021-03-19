using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField] private bool _isGameActive = true;

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;

    private float _xSpawnRange = 9.2f;
    private float _ySpawn = 8f;
    public float spawnRate = 5f;

    [SerializeField] private GameObject _tripleShotPowerup;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (_isGameActive)
        {
            GameObject newEnemy = Instantiate(_enemyPrefab,
                        new Vector3(Random.Range(-_xSpawnRange, _xSpawnRange), _ySpawn, 0), Quaternion.identity);

            newEnemy.transform.parent = _enemyContainer.transform;
            
            yield return new WaitForSeconds(spawnRate);
        }
    }

    public void OnPlayerDeath()
    {
        _isGameActive = false;
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (_isGameActive)
        {
            yield return new WaitForSeconds(Random.Range(3, 8));

            Vector3 spawnPos = new Vector3(Random.Range(-_xSpawnRange, _xSpawnRange), _ySpawn, 0);

            Instantiate(_tripleShotPowerup, spawnPos, Quaternion.identity);     
        }
    }
}
