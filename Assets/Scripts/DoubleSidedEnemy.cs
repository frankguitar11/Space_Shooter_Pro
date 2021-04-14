using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleSidedEnemy : MonoBehaviour
{

    public float enemySpeed = 3f;

    private float _enemyYBounds = -6.3f;
    private float _enemyXBounds = 9.2f;
    private int _enemyYRespawn = 8;

    private Player _player;

    [SerializeField] private int _enemyPointValue = 20;

    [SerializeField] AudioClip _explosionSFX;
    [SerializeField] GameObject _explosionVFX;

    [SerializeField] private GameObject _enemyLaserPrefab;
    private float _canFire = -1f;
    private float _fireRate = 3f;
    private bool _isAlive = true;

    private SpawnManager _spawnManager;

    [SerializeField] private GameObject _enemyShield;
    [SerializeField] private bool _hasShield = true;

    [SerializeField] private float _rammingDistance = 3f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _canFire && _isAlive == true)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;

            if (_player != null)
            {
                if (transform.position.y > _player.transform.position.y)
                {
                    Vector3 laserOffset = new Vector3(0, 0.45f, 0);

                    GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position + laserOffset, Quaternion.identity);
                    enemyLaser.GetComponent<Laser>().AssignEnemyLaser();
                }
                else if (transform.position.y < _player.transform.position.y)
                {
                    Vector3 laserOffset = new Vector3(0, 4.37f, 0);
                    GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position + laserOffset, Quaternion.identity);
                    enemyLaser.GetComponent<Laser>().AssignEnemyLaser();
                    enemyLaser.GetComponent<Laser>().AssignDoubleSidedLaser();
                }
            }
        }

        EnemyAggression();
    }

    private void Movement()
    {
        transform.Translate(new Vector3(0, -1, 0) * enemySpeed * Time.deltaTime);

        if (transform.position.y < _enemyYBounds)
        {
            float randomX = Random.Range(-_enemyXBounds, _enemyXBounds);

            transform.position = new Vector3(randomX, _enemyYRespawn, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //check for shields
            if (_hasShield == true)
            {
                _enemyShield.gameObject.SetActive(false);
                _player.DamagePlayer();
                _hasShield = false;
                return;
            }

            _isAlive = false;

            _player.DamagePlayer();

            Instantiate(_explosionVFX, transform.position, Quaternion.identity);

            enemySpeed = 0.2f;

            AudioSource.PlayClipAtPoint(_explosionSFX, Camera.main.transform.position, 1f);

            _spawnManager.EnemyKilled();
            Destroy(GetComponent<Collider2D>());

            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Laser"))
        {
            //check for shields
            if (_hasShield == true)
            {
                _enemyShield.gameObject.SetActive(false);
                Laser laserSwordCheck = other.GetComponent<Laser>();
                if (laserSwordCheck._isLaserSword == false)
                {
                    Destroy(other.gameObject);
                }
                _hasShield = false;
                return;
            }

            _isAlive = false;

            Laser laser = other.transform.GetComponent<Laser>();
            if (laser._isLaserSword == false)
            {
                Destroy(other.gameObject);
            }

            if (_player != null)
            {
                // Adds 10 points
                _player.AddScore(_enemyPointValue);
            }

            enemySpeed = 0.2f;

            Instantiate(_explosionVFX, transform.position, Quaternion.identity);

            AudioSource.PlayClipAtPoint(_explosionSFX, Camera.main.transform.position, 1f);

            Destroy(GetComponent<Collider2D>());
            _spawnManager.EnemyKilled();

            Destroy(this.gameObject);
        }
    }

    private void EnemyAggression()
    {
        if (_player != null)
        {
            if (Vector3.Distance(transform.position, _player.transform.position) <= _rammingDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, _player.transform.position,
                                                          (enemySpeed / 2) * Time.deltaTime);
            }

            else if (Vector3.Distance(transform.position, _player.transform.position) > _rammingDistance)
            {
                Movement();
            }
        }
        else if (_player == null)
        {
            Movement();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _rammingDistance);
    }
}
