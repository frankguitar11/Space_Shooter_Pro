using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed = 10f;

    [SerializeField] private GameObject _playerLaser;
    [SerializeField] private float _laserYOffset = 1.1f;
    public float fireRate = 0.3f;
    private float _canFire = -1f;

    [SerializeField] private int _lives = 3;

    private SpawnManager _spawnManager;

    [SerializeField] private bool _tripleShotActivated = false;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private int _tripleShotCooldown = 5;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        if(_spawnManager == null)
        {
            Debug.LogError("Spawn manager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        YAxisClamp();
        XWrap();

        if(Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
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
        _canFire = Time.time + fireRate;

        //offset of laser
        Vector3 laserOffset = new Vector3(0, _laserYOffset, 0);

        if(_tripleShotActivated == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_playerLaser, transform.position + laserOffset, Quaternion.identity);
        }
    }

    public void DamagePlayer()
    {
        _lives--;

        if(_lives <= 0)
        {

            _spawnManager.OnPlayerDeath();

            Destroy(this.gameObject);
        }
    }

    public void ActivateTripleShot()
    {
        _tripleShotActivated = true;
        StartCoroutine(TripleShotCooldownRoutine());
    }

    IEnumerator TripleShotCooldownRoutine()
    {
        yield return new WaitForSeconds(_tripleShotCooldown);

        _tripleShotActivated = false;
    }
}
