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

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnRoutine()
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
}
