using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySmall : MonoBehaviour
{
    public float enemySpeed = 3.5f;

    private float _enemyYBounds = -6.3f;
    private float _enemyXBounds = 9.2f;
    private int _enemyYRespawn = 8;

    private Player _player;

    [SerializeField] private int _enemyPointValue = 10;

    [SerializeField] AudioClip _explosionSFX;
    [SerializeField] GameObject _explosionVFX;

    [SerializeField] private GameObject _enemyLaserPrefab;
    private float _canFire = -1f;
    private float _fireRate = 3f;
    private bool _isAlive = true;

    [SerializeField] private EnemySineMovement _sineMovement;

    private SpawnManager _spawnManager;

    [SerializeField] private float _safetyDistance = 2f;
    [SerializeField] private float _dodgeSpeed = 2f;

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

            GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
            enemyLaser.GetComponent<Laser>().AssignEnemyLaser();
        }
        Movement();
        DodgeLasers();
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
            _isAlive = false;

            _sineMovement.GetComponent<EnemySineMovement>().enabled = false;

            _player.DamagePlayer();

            enemySpeed = 0.2f;

            AudioSource.PlayClipAtPoint(_explosionSFX, Camera.main.transform.position, 1f);
            
            GameObject explosion = Instantiate(_explosionVFX, transform.position, Quaternion.identity);
            explosion.transform.localScale /= 3;

            _spawnManager.EnemyKilled();
            Destroy(GetComponent<Collider2D>());

            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Laser"))
        {
            _isAlive = false;

            _sineMovement.GetComponent<EnemySineMovement>().enabled = false;

            Laser laser = other.transform.GetComponent<Laser>();
            if (laser._isLaserSword == false)
            {
                _player.playerLasers.Remove(other.gameObject);
                Destroy(other.gameObject);
            }

            if (_player != null)
            {
                // Adds 10 points
                _player.AddScore(_enemyPointValue);
            }

            enemySpeed = 0.2f;

            AudioSource.PlayClipAtPoint(_explosionSFX, Camera.main.transform.position, 1f);
            
            GameObject explosion = Instantiate(_explosionVFX, transform.position, Quaternion.identity);
            explosion.transform.localScale /= 3;

            Destroy(GetComponent<Collider2D>());
            _spawnManager.EnemyKilled();

            Destroy(this.gameObject);
        }
    }


    private void DodgeLasers()
    {
        Vector3 laserPosition = new Vector3(0,0,0);

        foreach(GameObject laser in _player.playerLasers)
        {
            laserPosition = laser.transform.position;
            if(_player.playerLasers == null)
            {
                return;
            }
        }

        if(Vector3.Distance(transform.position, laserPosition) <= _safetyDistance)
        {
            //laser approaches from the right
            if(laserPosition.x > transform.position.x)
            {
                transform.Translate(Vector3.left * _dodgeSpeed * Time.deltaTime);
            }
            //laser approaches from left
            else if(laserPosition.x < transform.position.x)
            {
                transform.Translate(Vector3.right * _dodgeSpeed * Time.deltaTime);
            }
        }
        //if player laser < safe distance
        //try to dodge
        //  - move to the left, or right
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _safetyDistance);
    }
}
