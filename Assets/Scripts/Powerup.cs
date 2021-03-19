using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    [SerializeField] private float _speed = 3f;
    private float offScreenY = -5.7f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //fall downwards
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        //when off-screen, destroy
        if(transform.position.y < offScreenY)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player player = collision.transform.GetComponent<Player>();

            if(player != null)
            {
                //activate triple shot
                player.ActivateTripleShot();
            }

            //destroy this
            Destroy(this.gameObject);
        }
    }
}
