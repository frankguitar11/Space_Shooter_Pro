using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed = 10f;

    [SerializeField] private GameObject playerLaser;
    [SerializeField] private float laserYOffset = 1.1f;
    public float fireRate = 0.3f;
    private float canFire = -1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        YAxisClamp();
        XWrap();

        if(Input.GetKeyDown(KeyCode.Space) && Time.time > canFire)
        {
            FireLaser();
        }
    }

    private void PlayerMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 directionInput = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(directionInput * speed * Time.deltaTime);
    }

    private void YAxisClamp()
    {
        //y axis clamp, with Mathf.Clamp
        float minY = -3.9f;
        float maxY = 5f;

        float yClamp = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(transform.position.x, yClamp, 0);
    }

    private void XWrap()
    {
        //x axis wrap
        float xWrapPoint = 11.2f;

        if (transform.position.x <= -xWrapPoint)
        {
            transform.position = new Vector3(xWrapPoint, transform.position.y, 0);
        }
        else if (transform.position.x >= xWrapPoint)
        {
            transform.position = new Vector3(-xWrapPoint, transform.position.y, 0);
        }
    }

    private void FireLaser()
    {
        //add cooldown
        canFire = Time.time + fireRate;

        //offset of laser
        Vector3 laserOffset = new Vector3(0, laserYOffset, 0);

        Instantiate(playerLaser, transform.position + laserOffset, Quaternion.identity);
    }
}
