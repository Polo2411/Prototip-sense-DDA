using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public static SceneManagerScript instance;

    private int totalEnemies;
    private int enemiesKilled;
    private float totaltimelevels;
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
        UpdateLevelIndicator();
    }

    public void RegisterEnemy()
    {
        totalEnemies++;
        Debug.Log($"Total Enemies: {totalEnemies}");
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        Debug.Log($"Enemies Killed: {enemiesKilled} / {totalEnemies}");
        CheckIfAllEnemiesKilled();
    }

    private void CheckIfAllEnemiesKilled()
    {
        Debug.Log($"Checking if all enemies are killed: {enemiesKilled} / {totalEnemies}");
        if (enemiesKilled >= totalEnemies)
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        Debug.Log("Loading next level...");
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            PlayerPrefs.SetInt("PlayerLives", player.GetCurrentLives());
            PlayerPrefs.SetInt("Deaths", player.deathCounter);
            PlayerPrefs.SetInt("Hits", player.hitsCounter);
            PlayerPrefs.SetInt("Dash", player.dashCounter);
            PlayerPrefs.SetInt("Charged", player.chargedCounter);
            PlayerPrefs.Save();
            totaltimelevels += player.timeLevel;
        }

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadNextLevelWithDelay(nextSceneIndex));
        }
        else
        {
            Debug.Log("All levels completed!");
            ReturnToMenu();
            BackgroundMusic.instance.StopBackgroundMusic();
        }
    }

    private IEnumerator LoadNextLevelWithDelay(int nextSceneIndex)
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene(nextSceneIndex);
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
        UpdateLevelIndicator();
    }

    private void UpdateLevelIndicator()
    {
        if (LevelIndicator.instance != null)
        {
            LevelIndicator.instance.UpdateLevelText();
        }
    }

    public void ResetGame()
    {
        StartCoroutine(ResetGameCoroutine());
    }

    private IEnumerator ResetGameCoroutine()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Player player = FindObjectOfType<Player>();
        PlayerPrefs.SetInt("Deaths", player.deathCounter);
        PlayerPrefs.SetInt("Hits", player.hitsCounter);
        PlayerPrefs.SetInt("Dash", player.dashCounter);
        PlayerPrefs.SetInt("Charged", player.chargedCounter);
        PlayerPrefs.Save();
        totaltimelevels += player.timeLevel;
        Time.timeScale = 0f;
        SceneManager.LoadScene(currentSceneIndex);
        yield return new WaitForSecondsRealtime(3f); // Esperar un momento para asegurarse de que la escena se haya cargado
        ResetPlayerLives();
        Time.timeScale = 1f;
        ResetEnemyCounters();
        RegisterAllEnemies();
    }

    private void ResetPlayerLives()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.SetCurrentLives(3); // Establecer vidas a 3
            PlayerPrefs.SetInt("PlayerLives", 3);
            player.UpdateHealthUI();
            PlayerPrefs.Save();
        }
    }

    private void ResetEnemyCounters()
    {
        totalEnemies = 0;
        enemiesKilled = 0;
    }

    private void RegisterAllEnemies()
    {
        Ghost[] ghost = FindObjectsOfType<Ghost>();
        foreach (var enemy in ghost)
        {
            RegisterEnemy();
        }

        EyeBall[] eyeball = FindObjectsOfType<EyeBall>();
        foreach (var enemy in eyeball)
        {
            RegisterEnemy();
        }

        FireMonster[] firemonster = FindObjectsOfType<FireMonster>();
        foreach (var enemy in firemonster)
        {
            RegisterEnemy();
        }
    }
    public void ReturnToMenu()
    {
        SavePlayerStatistics();
        SceneManager.LoadScene(0);
    }

    private void SavePlayerStatistics()
    {
        Player player = FindObjectOfType<Player>();
        if (player == null) return;

        // Obtener la ruta de la carpeta de la build
        string buildFolder = Directory.GetParent(Application.dataPath).FullName;
        string filePath = Path.Combine(buildFolder, "PlayerStatistics.txt");

        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.WriteLine("Player Statistics:");
            writer.WriteLine($"Last level: {SceneManager.GetActiveScene().name}");
            writer.WriteLine($"Deaths: {player.deathCounter}");
            writer.WriteLine($"Hits: {player.hitsCounter}");
            writer.WriteLine($"Dashes: {player.dashCounter}");
            writer.WriteLine($"Charged Projectiles: {player.chargedCounter}");
            writer.WriteLine($"Total time: {totaltimelevels} seconds");
        }

        Debug.Log($"Player statistics saved to {filePath}");
    }
}
