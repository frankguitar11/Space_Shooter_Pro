using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{

    private Player _player;

    [SerializeField] bool _isAlive = true;

    [SerializeField] float _bossMaxHealth = 10;
    [SerializeField] float _bossCurrentHealth = 10;

    [SerializeField] float _asteroidRadius = 3f;
    [SerializeField] float _radiusYOffset = 1f;

    [SerializeField] Transform[] _bossWaypoints;
    [SerializeField] int _randomWaypoint;

    [SerializeField] float _bossSpeed = 3.5f;

    [SerializeField] Animator _asteroidBeltAnim;

    [SerializeField] GameObject _bossFireball;
    private float _canFire = 4f;
    [SerializeField] float _fireRate;
    [SerializeField] int _numberOfFireballs = 5;

    [SerializeField] GameObject _freezingAsteroid;
    private float _canFireAsteroid;
    private float _fireRateAsteroid;

    UIManager _uiManager;

    [SerializeField] GameObject _explosionVFX;

    SpawnManager _spawnManager;


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL");
        }

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
        {
            Debug.Log("Spawn Manager is NULL");
        }

        InitializeBossHealthUI();

        //Start by going towards waypoint C
        _randomWaypoint = 2;

        StartCoroutine(DropFreezeAsteroidsRoutine());
        StartCoroutine(AsteroidBeltExpansionRoutine());
        StartCoroutine(RainFireballsRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Time.time > _canFire && _isAlive == true)
        {
            _fireRate = Random.Range(1f, 3f);
            _canFire = Time.time + _fireRate;

            ShootFireBall();
        }

        CheckWhenDead();
    }

    private void InitializeBossHealthUI()
    {
        _uiManager.DisplayBossHealth(true);
        _uiManager.UpdateBossHealth(_bossCurrentHealth, _bossMaxHealth);
    }

    private void Movement()
    {
        if (transform.position == _bossWaypoints[0].position ||
    transform.position == _bossWaypoints[1].position ||
    transform.position == _bossWaypoints[2].position)
        {
            _randomWaypoint = Random.Range(0, 3);
        }

        transform.position = Vector3.MoveTowards(transform.position,
                                                _bossWaypoints[_randomWaypoint].position,
                                                _bossSpeed * Time.deltaTime);
    }

    IEnumerator AsteroidBeltExpansionRoutine()
    {
        while (_isAlive)
        {
            yield return new WaitForSeconds(7.5f);

            _asteroidBeltAnim.SetTrigger("AsteroidBeltExpansion");
        }
    }

    private void ShootFireBall()
    {
        Instantiate(_bossFireball, transform.position, Quaternion.identity);
    }

    IEnumerator RainFireballsRoutine()
    {
        while (_isAlive)
        {
            yield return new WaitForSeconds(5f);

            for (int i = 0; i < _numberOfFireballs; i++)
            {
                if (Time.time > _canFire && _isAlive == true)
                {
                    _fireRate = Random.Range(0.5f, 1f);
                    _canFire = Time.time + _fireRate;

                    ShootFireBall();
                }
            }
        }
    }

    IEnumerator DropFreezeAsteroidsRoutine()
    {
        while (_isAlive)
        {
            float randomDropTime = Random.Range(1f, 5f);

            if (Time.time > _canFireAsteroid && _isAlive == true)
            {
                _fireRateAsteroid = Random.Range(0.5f, 1f);
                _canFireAsteroid = Time.time + _fireRateAsteroid;

                Instantiate(_freezingAsteroid, transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(randomDropTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _player.DamagePlayer();

            _bossCurrentHealth--;
            _uiManager.UpdateBossHealth(_bossCurrentHealth, _bossMaxHealth);

            _player.CameraShake();
        }
        else if(collision.CompareTag("Laser"))
        {
            _bossCurrentHealth--;
            _uiManager.UpdateBossHealth(_bossCurrentHealth, _bossMaxHealth);

            Laser laser = collision.transform.GetComponent<Laser>();
            if (laser._isLaserSword == false)
            {
                _player.playerLasers.Remove(collision.gameObject);
                Destroy(collision.gameObject);
            }

            _player.CameraShake();
        }
    }

    private void CheckWhenDead()
    {
        if (_bossCurrentHealth == 0)
        {
            ExplosionSequece();
            _uiManager.DisplayBossHealth(false);

            _spawnManager.EnemyKilled();
            _spawnManager.EnableNextWaveSpawning();
            _uiManager.SpawnNextWave();
            _spawnManager.StartEnemySpawning();

            Destroy(this.gameObject);
        }
    }

    private void ExplosionSequece()
    {
        GameObject explosion1 = Instantiate(_explosionVFX, transform.position, Quaternion.identity);
        explosion1.transform.localScale *= 2;

        GameObject explosion2 = Instantiate(_explosionVFX, transform.position, Quaternion.identity);
        explosion2.transform.localScale *= 1.5f;
        explosion2.transform.position += new Vector3(-2, 1, 0);
        explosion2.GetComponent<SpriteRenderer>().color = Color.blue;

        GameObject explosion3 = Instantiate(_explosionVFX, transform.position, Quaternion.identity);
        explosion3.transform.localScale *= 1.2f;
        explosion3.transform.position += new Vector3(1, -2, 0);
        explosion3.GetComponent<SpriteRenderer>().color = Color.red;
    }


    //To set up asteroid belt
    private void OnDrawGizmos()
    {
        Vector3 radiusOffset = new Vector3(0, _radiusYOffset, 0);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + radiusOffset , _asteroidRadius);
    }

}
