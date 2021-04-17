using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed = 10f;
    [SerializeField] private float _thrusterSpeedMultiplier = 1.2f;
    public float maxThrusterFill = 50f;
    public float currentThrusterLevel = 25f;
    [SerializeField] private float _thrusterUsageAmount = 10f;
    private bool _canUseThrusters = true;
    private int _thrusterCooldown = 5;

    [SerializeField] private GameObject _playerLaser;
    [SerializeField] private float _laserYOffset = 1.1f;
    public float fireRate = 0.3f;
    private float _canFire = -1f;
    [SerializeField] private int _ammoCount = 15;
    private int _maxAmmo = 30;

    [SerializeField] private int _lives = 3;

    private SpawnManager _spawnManager;

    [SerializeField] private bool _tripleShotActivated = false;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private int _tripleShotCooldown = 5;

    private float _speedCooldown = 5.0f;
    [SerializeField] private float _speedMultiplier = 2.0f;

    [SerializeField] private bool _isShieldActive = false;
    [SerializeField] private GameObject _playerShieldVFX;
    [SerializeField] private int _shieldHealth = 3;
    private SpriteRenderer _playerShieldColor;

    [SerializeField] GameObject _explosionVFX;

    [SerializeField] private GameObject _leftEngineFailure, _rightEngineFailure;

    [SerializeField] private int _totalScore = 0;
    private UIManager _uiManager;

    [SerializeField] AudioClip _laserSFX;

    [SerializeField] private GameObject _laserSwordVFX;
    private int _laserSwordCooldown = 5;

    [SerializeField] private CameraShake _cameraShake;

    [SerializeField] private GameObject _freezeVFX;
    [SerializeField] private GameObject _thrusterVFX;
    [SerializeField] private Animator _freezeVFXAnimator;
    [SerializeField] private float _freezeCooldown = 3f;
    private float _ogSpeed;
    private bool _isFrozen = false;

    public List<GameObject> playerLasers = new List<GameObject>();

    [SerializeField] private bool _homingMissleActive;
    [SerializeField] private float _homingMissleCooldown = 3f;
    [SerializeField] GameObject _homingMisslePrefab;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        if(_spawnManager == null)
        {
            Debug.LogError("Spawn manager is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if(_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL");
        }

        _playerShieldColor = _playerShieldVFX.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        YAxisClamp();
        XWrap();

        if(Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammoCount > 0)
        {
            FireLaser();
        }

        if (currentThrusterLevel < maxThrusterFill)
        {
            currentThrusterLevel += (_thrusterUsageAmount / 3) * Time.deltaTime;
            _uiManager.UpdateThrusterFill();
        }
    }

    private void PlayerMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 directionInput = new Vector3(horizontalInput, verticalInput, 0);

        Thursters(directionInput);
    }

    private void Thursters(Vector3 directionInput)
    {
        //Thruster boost
        if (Input.GetKey(KeyCode.LeftShift) && currentThrusterLevel > 0)
        {
            if (_canUseThrusters == true)
            {
                transform.Translate(directionInput * (speed * _thrusterSpeedMultiplier) * Time.deltaTime);
                currentThrusterLevel -= _thrusterUsageAmount * Time.deltaTime;
                _uiManager.UpdateThrusterFill();
            }
            else
            {
                transform.Translate(directionInput * speed * Time.deltaTime);
                return;
            }

        }
        else if (Input.GetKey(KeyCode.LeftShift) && currentThrusterLevel <= 0)
        {
            StartCoroutine(PlayerThrusterCooldownRoutine());
        }
        else
        {
            transform.Translate(directionInput * speed * Time.deltaTime);
        }
    }

    IEnumerator PlayerThrusterCooldownRoutine()
    {
        _canUseThrusters = false;
        yield return new WaitForSeconds(_thrusterCooldown);
        _canUseThrusters = true;
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

        _ammoCount--;
        UpdateAmmoCount();

        if(_tripleShotActivated == true)
        {
            GameObject tripleLaser = Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            playerLasers.Add(tripleLaser);
        }
        if (_homingMissleActive == true)
        {
            GameObject homingMissle = Instantiate(_homingMisslePrefab, transform.position, Quaternion.identity);
            playerLasers.Add(homingMissle);
        }
        if(_tripleShotActivated == true && _homingMissleActive == true)
        {
            GameObject homingMissle = Instantiate(_homingMisslePrefab, transform.position, Quaternion.identity);
            playerLasers.Add(homingMissle);
        }
        else if (_tripleShotActivated == false && _homingMissleActive == false)
        {
            GameObject singleLaser = Instantiate(_playerLaser, transform.position + laserOffset, Quaternion.identity);
            playerLasers.Add(singleLaser);
        }



        AudioSource.PlayClipAtPoint(_laserSFX, Camera.main.transform.position, 1f);
    }

    public void DamagePlayer()
    {
        if (_isShieldActive == true)
        {
            _shieldHealth--;

            switch (_shieldHealth)
            {
                case 2:                  
                    _playerShieldColor.color = new Color(1, 0.92f, 0.63f, 0.5f);
                    break;
                case 1:
                    _playerShieldColor.color = new Color(1, 0, 0, 0.3f);
                    break;
                case 0:
                    DeactivateShield();
                    break;
            }

            return;
        }
        else
        {
            _lives--;
            CameraShake();

            if (_lives == 2)
            {
                _leftEngineFailure.SetActive(true);
            }
            else if (_lives == 1)
            {
                _rightEngineFailure.SetActive(true);
            }
            else if (_lives <= 0)
            {
                _lives = 0;
                _spawnManager.OnPlayerDeath();

                Instantiate(_explosionVFX, transform.position, Quaternion.identity);

                Destroy(this.gameObject);
            }

            _uiManager.UpdateLives(_lives);

        }
    }

    public void CameraShake()
    {
        _cameraShake.MainCameraShake();
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

    public void ActivateSpeedBoost()
    {
        StartCoroutine(SpeedPowerupCoroutine());
    }

    IEnumerator SpeedPowerupCoroutine()
    {
        speed *= _speedMultiplier;
        yield return new WaitForSeconds(_speedCooldown);
        speed /= _speedMultiplier;
    }

    public void ActivateShield()
    {
        _playerShieldVFX.SetActive(true);
        _playerShieldColor.color = Color.white;
        _shieldHealth = 3;
        _isShieldActive = true;
    }

    public void DeactivateShield()
    {
        _playerShieldVFX.SetActive(false);
        _isShieldActive = false;
    }

    public void AddScore(int pointsToAdd)
    {
        _totalScore += pointsToAdd;
        _uiManager.UpdateScore(_totalScore);
    }

    public void UpdateAmmoCount()
    {
        _uiManager.UpdateAmmo(_ammoCount);
    }

    public void AddAmmo()
    {
        _ammoCount += 15;
        if(_ammoCount > _maxAmmo)
        {
            _ammoCount = _maxAmmo;
        }

        UpdateAmmoCount();
    }

    public void Plus1Health()
    {
        if (_lives == 3)
        {
            Debug.Log("Lives at MAX");
            return;
        }

        _lives++;
        _uiManager.UpdateLives(_lives);

        if (_lives == 3)
        {
            _leftEngineFailure.SetActive(false);
        }
        else if (_lives == 2)
        {
            _rightEngineFailure.SetActive(false);
        }
    }

    public void ActivateLaserSword()
    {
        _laserSwordVFX.gameObject.SetActive(true);
        StartCoroutine(LaserSwordCooldownRoutine());
        
        Laser[] laserSwordChildren = _laserSwordVFX.GetComponentsInChildren<Laser>();
        for (int i = 0; i < laserSwordChildren.Length; i++)
        {
            laserSwordChildren[i].AssignLaserSword();
        }
    }

    IEnumerator LaserSwordCooldownRoutine()
    {
        yield return new WaitForSeconds(_laserSwordCooldown);
        _laserSwordVFX.gameObject.SetActive(false);
    }

    public void FreezeVFXActivate()
    {
        if (_isFrozen == false)
        {
            _ogSpeed = speed;
            speed /= speed;
            StartCoroutine(FreezeCooldownRoutine());
        }
        _isFrozen = true;
        _freezeVFX.SetActive(true);
        _thrusterVFX.SetActive(false);
        _freezeVFXAnimator.SetBool("Player_Frozen", true);
    }

    IEnumerator FreezeCooldownRoutine()
    {
        yield return new WaitForSeconds(_freezeCooldown);
        _isFrozen = false;
        speed *= _ogSpeed;
        _freezeVFXAnimator.SetBool("Player_Frozen", false);
        _thrusterVFX.SetActive(true);
        _freezeVFX.SetActive(false);
    }

    public void ActivateHomingMissle()
    {
        _homingMissleActive = true;
        StartCoroutine(HomingMissleCooldownRoutine());
    }

    IEnumerator HomingMissleCooldownRoutine()
    {
        yield return new WaitForSeconds(_homingMissleCooldown);

        _homingMissleActive = false;
    }
}
