using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    public float speed = 6f;
    private float outOfBounds = 8.2f;

    [SerializeField] private bool _isEnemyLaser = false;
    [SerializeField] private bool _isDoubleSidedLaser = false;

    public bool _isLaserSword = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_isEnemyLaser && _isDoubleSidedLaser)
        {
            MoveUp();
        }
        else if(_isEnemyLaser && _isDoubleSidedLaser == false)
        {
            MoveDown();
            speed = 7.5f;
        }
        else
        {
            MoveUp();
        }
    }

    private void MoveUp()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.position.y >= outOfBounds)
        {
            //if the laser has a parent, destroy the parent too
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y <= -outOfBounds)
        {
            //if the laser has a parent, destroy the parent too
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    public void AssignDoubleSidedLaser()
    {
        _isDoubleSidedLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && _isEnemyLaser)
        {
            collision.GetComponent<Player>().DamagePlayer();
            
            Destroy(this.gameObject);
        }
        
        if(collision.tag == "Enemy Bomb")
        {
            collision.GetComponent<EnemyBomb>().BombExplosion();
            Destroy(collision.gameObject);

            Destroy(this.gameObject);
        }
        else if(collision.tag == "Enemy Bomb" && _isLaserSword)
        {
            collision.GetComponent<EnemyBomb>().BombExplosion();
            Destroy(collision.gameObject);
        }
    }

    public void AssignLaserSword()
    {
        _isLaserSword = true;
    }
}
