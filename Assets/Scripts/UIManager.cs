using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Text _scoreText;

    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private Image _livesIMG;

    [SerializeField] private Text _ammoText;

    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _restartText;

    private GameManager _gameManager;

    [SerializeField] private Image _thrusterFill;

    private Player _player;

    [SerializeField] private Text _waveText;

    private SpawnManager _spawnManager;

    [SerializeField] Image _bossHealthFill;
    [SerializeField] GameObject _bossHealthInfo;

    // Start is called before the first frame update
    void Start()
    {
        //initialize score
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 15 + "/30";

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if(_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL in UI Manager");
        }

        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogError("Player is NULL in UI Manager");
        }

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL in UI Manager");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(int totalScore)
    {
        _scoreText.text = "Score: " + totalScore;
    }

    public void UpdateLives(int currentLives)
    {
        _livesIMG.sprite = _livesSprites[currentLives];

        if (currentLives <= 0)
        {
            GameOverScreen();
        }
    }

    public void UpdateAmmo(int totalAmmo)
    {
        _ammoText.text = "Ammo: " + totalAmmo + "/30";
    }

    public void GameOverScreen()
    {
        _gameManager.GameOver();

        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void UpdateThrusterFill()
    {
        _thrusterFill.fillAmount = Mathf.Clamp(_player.currentThrusterLevel / _player.maxThrusterFill, 0, 1f);
    }

    public void SpawnNextWave()
    {
        StartCoroutine(WaveTextEnableRoutine());
    }

    IEnumerator WaveTextEnableRoutine()
    {
        _waveText.text = "Wave " + _spawnManager.GetWaveNumber();
        _waveText.gameObject.SetActive(true);
        _spawnManager.EnableNextWaveSpawning();

        yield return new WaitForSeconds(3f);

        _waveText.gameObject.SetActive(false);

    }

    public void UpdateBossHealth(float currentHealth, float maxHealth)
    {
        _bossHealthFill.fillAmount = Mathf.Clamp(currentHealth / maxHealth, 0, 1f);
    }

    public void DisplayBossHealth(bool toggle)
    {
        _bossHealthInfo.SetActive(toggle);
    }
}
