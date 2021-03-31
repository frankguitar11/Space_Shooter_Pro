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
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.DamagePlayer();
            }

            _anim.SetTrigger("OnEnemyDeath");
            enemySpeed = 0.2f;

            Destroy(this.gameObject, 2.7f);
        }
        else if(other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);

            if(_player != null)
            {
                // Adds 10 points
                _player.AddScore(_enemyPointValue);
            }

            _anim.SetTrigger("OnEnemyDeath");
            enemySpeed = 0.2f;

            Destroy(this.gameObject, 2.7f);
        }
    }
}
