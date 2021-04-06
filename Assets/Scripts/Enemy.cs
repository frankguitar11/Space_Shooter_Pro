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


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        _anim = GetComponent<Animator>();
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

        Movement();
    }

    private void Movement()
    {
        transform.Translate(Vector3.down * enemySpeed * Time.deltaTime);

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
            _isAlive = false;
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.DamagePlayer();
            }

            _anim.SetTrigger("OnEnemyDeath");
            enemySpeed = 0.2f;

            AudioSource.PlayClipAtPoint(_explosionSFX, Camera.main.transform.position, 1f);

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.7f);
        }
        else if(other.CompareTag("Laser"))
        {
            _isAlive = false;
            Destroy(other.gameObject);

            if(_player != null)
            {
                // Adds 10 points
                _player.AddScore(_enemyPointValue);
            }

            _anim.SetTrigger("OnEnemyDeath");
            enemySpeed = 0.2f;

            AudioSource.PlayClipAtPoint(_explosionSFX, Camera.main.transform.position, 1f);

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.7f);
        }
    }
}
