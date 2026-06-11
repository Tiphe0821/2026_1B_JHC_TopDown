using JetBrains.Annotations;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    // 플레이어의 체력 공격력 등등 어쩌구

    public int playerGameLevel;         // 플레이어의 레벨 (투자 시스템 참고용)
    public int currentGameXP;           // 플레이어가 가지고있던 현재 XP

    public int currentWeaponID;         // 플레이어가 들고있던 무기 저장 (검 & 다른거 하나) - 개인적으로는 투사체 구현하고 싶습니다

    public int tryNum;                  // 플레이어가 도전했던 시도 수 (같은 세이브 내에서 몇회차를 플레이했는지 저장)

    public int totalPoint;              // 레벨업으로 얻은 총 포인트 (레벨 -1)
    public int pointLeft;               // 남은 포인트

    public int atkPoint;
    public int defPoint;
    public int hpPoint;                 // 투자한 포인트 (3가지 구현 예정)

    public int atk = 0;                     // 플레이어 공격력 (여기서 호출할 건 아니다. 공격에 맞은 적 쪽에서 가져가서 데미지를 받을 예정
    public int def = 0;

    public int defaultAtk = 10;              // 무기 따라서 다르게 그렇지만 지금은 공통 기본 공격력 놓기
    private int defaultDef = 0;
    private int defaultHp = 50;

    public int maxHp;                       // 플레이어 체력
    public int currentHp;                   // 현재 체력

    // 여기서 플레이어 충돌 처리 다 하자.

    // 리지드바디 어쩌구
    public Rigidbody2D rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();       // 리지드바디 받아오기
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
