using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    [SerializeField] private float _speed = 3f;
    private float offScreenY = -5.7f;

    [SerializeField] private int _powerupID;
    // 0 = triple shot, 1 = speed, 2 = shields

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
                switch (_powerupID)
                {
                    case 0:
                        player.ActivateTripleShot();
                        break;
                    case 1:
                        player.ActivateSpeedBoost();
                        break;
                    case 2:
                        player.ActivateShield();
                        break;
                    default:
                        Debug.Log("Defaulted value of _powerupID");
                        break;
                }

            }

            //destroy this
            Destroy(this.gameObject);
        }
    }
}
