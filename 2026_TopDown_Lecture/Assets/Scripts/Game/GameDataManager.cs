using UnityEngine;
using System.IO;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;
    //public GameSettingData gameSettingData;
    public SaveData saveData;
    public int isTutorialFinished;

    private string savePath;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Application.persistentDataPath + "/saveData.json";

            //LoadJsonData();
            //LoadPlayerPrefs();
        }
        else
            Destroy(gameObject);
    }
    
    // 나중에 추가할 거 : 플레이어 json 데이터 저장한걸 불러오기
    // 해당하는 사항이 없다면 기본값으로 대체하는 것도

    public void LoadPlayerPrefs()
    {
        isTutorialFinished = PlayerPrefs.GetInt("TUTORIAL", 0);
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetInt("TUTORIAL", isTutorialFinished);
        PlayerPrefs.Save();

        Debug.Log("PlayerPrefs 저장 완료");
    }

    public void DeletePlayerPrefs()     // 플레이어프레프 데이터 초기화
    {
        PlayerPrefs.DeleteKey("TUTORIAL");
        LoadPlayerPrefs();
    }

    public void SaveGameResult()
    {

    }

    public void SaveJsonData()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Json 저장 완료 : " + savePath);
    }

    public void LoadJsonData()
    {
        if(File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            saveData = new SaveData();
            SaveJsonData();
        }
    }

    public void DeleteJsonData()
    {
        if(File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        saveData = new SaveData();
        SaveJsonData();

        Debug.Log("JSON 저장 데이터 삭제");
    }
}
