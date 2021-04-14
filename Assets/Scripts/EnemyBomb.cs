using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomb : MonoBehaviour
{

    [SerializeField] private float _bombSpeed = 2f;
    [SerializeField] private float _rotatationSpeed = 35f;

    private Player _player;

    [SerializeField] private GameObject _bombExplosionVFX;
    [SerializeField] private float _explodeInSeconds = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogError("Player is NULL on EnemyBomb");
        }

        Explode();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _bombSpeed * Time.deltaTime, Space.World);

        transform.Rotate(Vector3.forward * _rotatationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _player.DamagePlayer();
            Destroy(this.gameObject);
        }     
    }

    public void BombExplosion()
    {
        Instantiate(_bombExplosionVFX, transform.position, Quaternion.identity);
    }

    public void Explode()
    {
        StartCoroutine(BombExplodeRoutine());
    }

    IEnumerator BombExplodeRoutine()
    {
        yield return new WaitForSeconds(_explodeInSeconds);

        BombExplosion();
        Destroy(this.gameObject);
    }
}
