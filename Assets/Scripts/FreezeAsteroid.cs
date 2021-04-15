using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeAsteroid : MonoBehaviour
{

    [SerializeField] float _movementSpeed = 4f;
    [SerializeField] float _rotationSpeed = 30f;

    private Player _player;

    [SerializeField] GameObject _explosionVFX;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogError("PLayer is NULL on Freeze Asteroid");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _movementSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.back * _rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Laser"))
        {
            GameObject explosion = Instantiate(_explosionVFX, transform.position, Quaternion.identity);
            explosion.transform.localScale /= 2;

            _player.playerLasers.Remove(collision.gameObject);
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
        else if(collision.CompareTag("Player"))
        {
            _player.FreezeVFXActivate();

            Destroy(this.gameObject);
        }
    }
}
