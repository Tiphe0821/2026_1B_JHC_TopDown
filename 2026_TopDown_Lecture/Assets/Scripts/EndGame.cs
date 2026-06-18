using UnityEngine;

public class EndGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        GameDataManager mydatamanager = FindAnyObjectByType<GameDataManager>();
            
        mydatamanager.saveData.tryNum++;
        mydatamanager.saveData.currentGameXP += 30;
        if(mydatamanager.saveData.currentGameXP >= mydatamanager.saveData.playerGameLevel * 5 + 20)
        {
            mydatamanager.saveData.playerGameLevel++;
        }

        mydatamanager.SaveJsonData();

        FindAnyObjectByType<GameManager>().GoTitle();
    }
}
