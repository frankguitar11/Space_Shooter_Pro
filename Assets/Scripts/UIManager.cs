using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Text _scoreText;

    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private Image _livesIMG;

    // Start is called before the first frame update
    void Start()
    {
        //initialize score
        _scoreText.text = "Score: " + 0;
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
    }
}
