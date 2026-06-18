using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string playerName = "Chara";

    public int playerGameLevel = 0;         // 플레이어의 레벨 (투자 시스템)
    public int currentGameXP = 0;           // 플레이어가 가지고있던 현재 XP

    public int currentWeaponID = 0;         // 플레이어가 들고있던 무기 저장 (검 & 다른거 하나) - 개인적으로는 투사체 구현하고 싶습니다

    public int tryNum = 0;                  // 플레이어가 도전했던 시도 수 (같은 세이브 내에서 몇회차를 플레이했는지 저장)

    public int totalPoint = 0;              // 레벨업으로 얻은 총 포인트 (레벨 -1)
    public int pointLeft = 0;               // 남은 포인트

    public int atkPoint = 0;
    public int defPoint = 0;
    public int hpPoint = 0;                 // 투자한 포인트 (3가지 구현 예정)
}

[System.Serializable]

public class SaveDataList               // 총 3개를 저장할 예정이기 때문에 몇번 세이브 파일이냐에 따라 세이브 파일 덮어씌우기 할 예정 (구현이 어렵다면 수정 예정)
{
    public List<SaveData> Datas = new List<SaveData>();
}

public static class DataSaver
{
    private const string FILE                   = "SaveDatas_Player.json";                                  // 데이터를 저장할 파일 위치 설정

    private const string PLAYER_NAME            = "PlayerName";                                             // playerPrefs 다시 사용하기

    private static string filePath              = Path.Combine(Application.persistentDataPath, FILE);       // 저장할 위치 지정

    
}
