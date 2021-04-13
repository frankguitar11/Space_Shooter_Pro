using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField] private bool _isGameActive = true;
    [SerializeField] private bool _spawnEnemyWave = true;

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;

    private float _xSpawnRange = 9.2f;
    private float _ySpawn = 8f;
    public float spawnRate = 2f;

    [SerializeField] GameObject[] powerupsArray;

    [SerializeField] private int _currentEnemies = 0;
    [SerializeField] private int _enemiesInCurrentWave = 15;
    [SerializeField] private int _waveNumber = 1;

    private UIManager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL in Spawn Manager");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentEnemies <= 0 && _spawnEnemyWave == false)
        {
            //EnableNextWaveSpawning();
            _uiManager.SpawnNextWave();
            StartEnemySpawning();
        }
    }

    public void StartGameSpawning()
    {
        _uiManager.SpawnNextWave();
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (_isGameActive && _spawnEnemyWave)
        {
                for (int i = 0; i < _enemiesInCurrentWave; i++)
                {
                    GameObject newEnemy = Instantiate(_enemyPrefab,
                                new Vector3(Random.Range(-_xSpawnRange, _xSpawnRange), _ySpawn, 0), Quaternion.identity);

                    newEnemy.transform.parent = _enemyContainer.transform;

                    _currentEnemies++;

                    yield return new WaitForSeconds(spawnRate);

                    if(_isGameActive == false)
                     {
                        break;
                     }
                }

                _enemiesInCurrentWave += 15;
                _waveNumber++;

                _spawnEnemyWave = false;
        }
    }

    public void StartEnemySpawning()
    {
        if (_waveNumber % 2 == 0)
        {
            spawnRate -= 0.2f;
            if(spawnRate <= 0.4f)
            {
                spawnRate = 0.4f;
            }
        }
        StartCoroutine(SpawnEnemyRoutine());
    }

    public void EnemyKilled()
    {
        _currentEnemies--;
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

            int randomPowerup = Random.Range(0, powerupsArray.Length);

            Instantiate(powerupsArray[randomPowerup], spawnPos, Quaternion.identity);     
        }
    }

    public int GetWaveNumber()
    {
        return _waveNumber;
    }

    public void EnableNextWaveSpawning()
    {
        _spawnEnemyWave = true;
    }
}
