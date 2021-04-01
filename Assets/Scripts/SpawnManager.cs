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

    //[SerializeField] private GameObject _tripleShotPowerup;

    [SerializeField] GameObject[] powerupsArray;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGameSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);

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
        yield return new WaitForSeconds(3f);

        while (_isGameActive)
        {
            yield return new WaitForSeconds(Random.Range(3, 8));

            Vector3 spawnPos = new Vector3(Random.Range(-_xSpawnRange, _xSpawnRange), _ySpawn, 0);

            int randomPowerup = Random.Range(0, 3);

            Instantiate(powerupsArray[randomPowerup], spawnPos, Quaternion.identity);     
        }
    }
}
