using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float enemySpeed = 4.5f;

    private float _enemyYBounds = -6.3f;
    private float _enemyXBounds = 9.2f;
    private int _enemyYRespawn = 8;

    private Player _player;

    [SerializeField] private int _enemyPointValue = 10;

    private Animator _anim;

    [SerializeField] AudioClip _explosionSFX;

    [SerializeField] private GameObject _enemyLaserPrefab;
    private float _canFire = -1f;
    private float _fireRate = 3f;
    private bool _isAlive = true;

    [SerializeField] GameObject _enemyExplosionVFX;

    [SerializeField] private EnemySineMovement _sineMovement;

    private SpawnManager _spawnManager;

    [SerializeField] private GameObject _enemyShield;
    [SerializeField] private bool _hasShield;

    [SerializeField] private float _rammingDistance = 1.5f;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _rammingSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        _anim = GetComponent<Animator>();

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        _spriteRenderer = GetComponent<SpriteRenderer>();

        RandomShieldEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > _canFire && _isAlive == true)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;         
           
            GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
            enemyLaser.GetComponent<Laser>().AssignEnemyLaser();
        }

        EnemyAggression();
    }

    private void Movement()
    {
        transform.Translate(new Vector3(0, -1, 0) * enemySpeed * Time.deltaTime);

        _sineMovement.GetComponent<EnemySineMovement>().enabled = true;

        if (transform.position.y < _enemyYBounds)
        {
            float randomX = Random.Range(-_enemyXBounds, _enemyXBounds);

            transform.position = new Vector3(randomX, _enemyYRespawn, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
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

            _sineMovement.GetComponent<EnemySineMovement>().enabled = false;
            
            _player.DamagePlayer();

            _anim.SetTrigger("OnEnemyDeath");
            enemySpeed = 0.2f;

            AudioSource.PlayClipAtPoint(_explosionSFX, Camera.main.transform.position, 1f);

            _spawnManager.EnemyKilled();
            Destroy(GetComponent<Collider2D>());

            Destroy(this.gameObject, 2.7f);
        }
        else if(other.CompareTag("Laser"))
        {
            //check for shields
            if (_hasShield == true)
            {
                _enemyShield.gameObject.SetActive(false);
                Laser laserSwordCheck = other.GetComponent<Laser>();
                if (laserSwordCheck._isLaserSword == false)
                {
                    _player.playerLasers.Remove(other.gameObject);
                    Destroy(other.gameObject);
                }
                _hasShield = false;
                return;
            }

            _isAlive = false;

            _sineMovement.GetComponent<EnemySineMovement>().enabled = false;

            Laser laser = other.transform.GetComponent<Laser>();
            if (laser._isLaserSword == false)
            {
                _player.playerLasers.Remove(other.gameObject);
                Destroy(other.gameObject);
            }

            if(_player != null)
            {
                // Adds 10 points
                _player.AddScore(_enemyPointValue);
            }

            _anim.SetTrigger("OnEnemyDeath");
            enemySpeed = 0.2f;

            AudioSource.PlayClipAtPoint(_explosionSFX, Camera.main.transform.position, 1f);

            Destroy(GetComponent<Collider2D>());
            _spawnManager.EnemyKilled();

            Destroy(this.gameObject, 2.7f);
        }
        else if(other.CompareTag("Missle"))
        {
            _isAlive = false;

            _sineMovement.GetComponent<EnemySineMovement>().enabled = false;

            if(_player != null)
            {
                _player.AddScore(_enemyPointValue);
            }

            Instantiate(_enemyExplosionVFX, transform.position, Quaternion.identity);
            enemySpeed = 0.2f;

            Destroy(GetComponent<Collider2D>());
            _spawnManager.EnemyKilled();

            Destroy(other.gameObject);

            Destroy(this.gameObject);

        }
    }

    private void RandomShieldEnemy()
    {
        int randomShieldNumber = Random.Range(0, 2);

        if(randomShieldNumber == 0)
        {
            _hasShield = true;
            _enemyShield.gameObject.SetActive(true);
        }
        else if(randomShieldNumber == 1)
        {
            _hasShield = false;
            _enemyShield.gameObject.SetActive(false);
        }
    }

    private void EnemyAggression()
    {
        if (_player != null && _isAlive)
        {
            if (Vector3.Distance(transform.position, _player.transform.position) <= _rammingDistance)
            {
                if (transform.position.y > _player.transform.position.y)
                {
                    _spriteRenderer.color = Color.red;

                    _sineMovement.GetComponent<EnemySineMovement>().enabled = false;

                    transform.position = Vector2.MoveTowards(transform.position, _player.transform.position,
                                                              _rammingSpeed * Time.deltaTime);
                }
                else
                {
                    Movement();
                }
            }

            else if (Vector3.Distance(transform.position, _player.transform.position) > _rammingDistance)
            {
                _spriteRenderer.color = Color.white;

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
