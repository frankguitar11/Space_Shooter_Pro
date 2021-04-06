using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField] private float _rotationSpeed = 20f;

    [SerializeField] private GameObject _explosionVFX;

    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        if(_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL on Asteroid");
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

            Destroy(this.gameObject, 0.3f); ;
        }
    }
}
