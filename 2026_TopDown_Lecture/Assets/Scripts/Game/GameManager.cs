using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string titleSceneName = "TitleScene";
    public string gameSceneName = "GameScene";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GetComponent<GameDataManager>().LoadJsonData();
        FindAnyObjectByType<playerLv>().gameObject.GetComponent<TextMeshProUGUI>().text = "플레이어 레벨 : " + GetComponent<GameDataManager>().saveData.playerGameLevel;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void GameOver()
    {
        
    }

    public void GoTitle()
    {
        SceneManager.LoadScene(titleSceneName);
        ChangeplayerLVBKGR();
    }

    public void ChangeplayerLVBKGR()
    {
        {
            FindAnyObjectByType<playerLv>().gameObject.GetComponent<TextMeshProUGUI>().text = "플레이어 레벨 : " + GetComponent<GameDataManager>().saveData.playerGameLevel;

        }
    }
}
