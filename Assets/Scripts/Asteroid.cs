using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField] private float _rotationSpeed = 20f;

    [SerializeField] private GameObject _explosionVFX;

    private SpawnManager _spawnManager;
    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        if(_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL on Asteroid");
        }

        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogError("Player is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Laser"))
        {
            Destroy(collision.gameObject);

            //explosion vfx
            Instantiate(_explosionVFX, transform.position, Quaternion.identity);

            _spawnManager.StartGameSpawning();
            _player.playerLasers.Remove(collision.gameObject);

            Destroy(this.gameObject, 0.3f); ;
        }
    }
}
