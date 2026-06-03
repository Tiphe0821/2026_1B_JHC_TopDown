using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EnemyMoveAndAttack : MonoBehaviour
{
    public int behaviorMode = 0;    // 0 - 피격(살짝 밀려나기 & 공격 취소), 1 - 순찰 (루트 X, 랜덤 이동), 2 - 대기 (플레이어 공격 전 대기),3 - 공격 (플레이어 타게팅) 

    // 적 공격 설정
    public float attackCooldown = 4.5f;        // 공격 쿨타임
    public float waitTime = 1.0f;              // 공격 전 기다리는 시간 (2 대기)
    public float lastAttackTime;               // 마지막 공격했던 시간
    public bool isAttacking = false;           // 지금 공격중인가?

    public Transform playerTransform;       // 플레이어 위치 받아오기
    public GameObject attackRangeDisplay;   // Sprite Renderer를 통한 공격 방향 보여주기 - 다른 방법이 있다면 좋을 것 같다
    public GameObject attackCollider;       // trigger타입 콜리더를 껐다키는 방식 사용하기


    public float attackDistance = 0.4f;     // 공격 가능 거리
    public Vector2 playerDir;               // 플레이어가 있는 위치 (플레이어를 바라보고 거길 향해서 공격할 예정)

    public List<Sprite> idleSprite;               // 리소스에 한쪽 방향만 있는 관계로 스케일값 조정하기
    public List<Sprite> attackSprite;             // 공격 리소스
    public List<Sprite> waitSprite;

    // 랜덤으로 이동하다 5초에 한번씩 플레이어가 있는지 확인하자. 

    private void Awake()
    {
        playerTransform = FindAnyObjectByType<PlayerController>().gameObject.transform;     // 플레이어 컨트롤러가 있는걸 찾아서 가져오기
    
        if(playerTransform == null)
        {
            Debug.Log("플레이어 없는데요?");
        }
    }

    void Start()
    {
        
    }

    public void AttackFinished()
    {   

    }

    // Update is called once per frame
    void Update()
    {

        // 플레이어와 자신의 위치 차이 벡터 계산
        playerDir = new Vector2(playerTransform.position.x - this.transform.position.x, playerTransform.position.y - this.transform.position.y);

        if(playerDir.magnitude < attackDistance)    // 거리가 설정한 거리값보다 적을 경우
        {
            if(!isAttacking)
            {
                //  그럼 공격 시작해야겠지?
                behaviorMode = 2; // 공격 대기로 전환
            }
        }
            // 플레이어와 자신의 위치를 계산해 바라볼 방향의 단위벡터 구하기


            // 비헤이비어 작성한거 사용하기
            switch (behaviorMode)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }

    }
}
