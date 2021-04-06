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

    // Start is called before the first frame update
    void Start()
    {
        //initialize score
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 15;

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if(_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL");
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
        _ammoText.text = "Ammo: " + totalAmmo;
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
}
