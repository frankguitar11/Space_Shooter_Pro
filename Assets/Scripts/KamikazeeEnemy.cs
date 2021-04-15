using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeeEnemy : MonoBehaviour
{

    public float enemySpeed = 4.5f;

    private float _enemyYBounds = -6.3f;
    private float _enemyXBounds = 9.2f;
    private int _enemyYRespawn = 8;

    private Player _player;

    [SerializeField] private int _enemyPointValue = 20;

    private Animator _anim;

    [SerializeField] AudioClip _explosionSFX;

    [SerializeField] private GameObject _kamikazeeBombPrefab;
    private float _canFire = 2f;
    private float _fireRate = 3f;
    private bool _isAlive = true;
    [SerializeField] GameObject _enemyDamageVFX;

    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        _anim = GetComponent<Animator>();

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _canFire && _isAlive == true)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;

            Vector3 bombOffset = new Vector3(0, 1.4f, 0);

            if (_spawnManager.IsGameActive() == true)
            {
                GameObject enemyBomb = Instantiate(_kamikazeeBombPrefab, transform.position + bombOffset, Quaternion.identity);
            }
        }

        Movement();
    }

    private void Movement()
    {

        transform.Translate(new Vector3(0, -1, 0) * enemySpeed * Time.deltaTime, Space.World);

        if (transform.position.y < _enemyYBounds)
        {
            float randomX = Random.Range(-_enemyXBounds, _enemyXBounds);

            transform.position = new Vector3(randomX, _enemyYRespawn, 0);
        }

        if (_isAlive != false)
        {
            if (_player != null)
            {
                //Follow player
                if (Vector3.Distance(transform.position, _player.transform.position) != 0)
                {
                    //move towards
                    transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, enemySpeed / 2 * Time.deltaTime);

                    //Rotate towards
                    Vector2 direction = (_player.transform.position - transform.position).normalized;
                    var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    var offset = 90f;

                    transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isAlive = false;

            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.DamagePlayer();
            }

            _anim.SetTrigger("OnEnemyDeath");
            enemySpeed = 0.2f;

            AudioSource.PlayClipAtPoint(_explosionSFX, Camera.main.transform.position, 1f);

            _spawnManager.EnemyKilled();
            Destroy(GetComponent<Collider2D>());

            _enemyDamageVFX.SetActive(false);

            Destroy(this.gameObject, 2.7f);
        }
        else if (other.CompareTag("Laser"))
        {
            _isAlive = false;

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

            _anim.SetTrigger("OnEnemyDeath");
            enemySpeed = 0.2f;

            AudioSource.PlayClipAtPoint(_explosionSFX, Camera.main.transform.position, 1f);

            Destroy(GetComponent<Collider2D>());
            _spawnManager.EnemyKilled();

            _enemyDamageVFX.SetActive(false);

            Destroy(this.gameObject, 2.7f);
        }
    }

}
