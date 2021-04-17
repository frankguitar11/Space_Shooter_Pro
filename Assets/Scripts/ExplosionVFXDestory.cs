using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionVFXDestory : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(GetComponent<CircleCollider2D>(), 1.5f);
        Destroy(this.gameObject, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(this.CompareTag("Enemy Bomb Explosion") && collision.CompareTag("Player"))
        {
            Player _player = collision.GetComponent<Player>();

            _player.DamagePlayer();

            Destroy(GetComponent<CircleCollider2D>());
        }
    }
}
