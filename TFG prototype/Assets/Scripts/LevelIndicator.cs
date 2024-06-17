using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelIndicator : MonoBehaviour
{
    public static LevelIndicator instance;

    private TMP_Text levelText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        levelText = GetComponent<TMP_Text>();
        UpdateLevelText();
    }

    public void UpdateLevelText()
    {
        int currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        levelText.text = "Level: " + currentLevel;
    }
}

