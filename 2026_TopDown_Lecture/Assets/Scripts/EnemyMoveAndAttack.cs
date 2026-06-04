using UnityEngine;

public class EnemyMoveAndAttack : MonoBehaviour
{
    // 플레이어랑 충돌 처리는 피하고 벽이랑은 충돌시키고 싶은데 어떻게 처리해야할지 모르겠다

    // public int behaviorMode = 0;    // 0 - 피격(살짝 밀려나기 & 공격 취소), 1 - 순찰 (루트 X, 랜덤 이동), 2 - 대기 (플레이어 공격 전 대기),3 - 공격 (플레이어 타게팅) (안써)
    // 사용 안해도 될것 같다

    // 적 공격 설정
    public float attackCooldown = 4.5f;        // 공격 쿨타임
    public float waitTime = 1.0f;              // 공격 전 기다리는 시간 (2 대기)
    public float currentWaitTime = 0f;
    public bool isAttacking = false;           // 지금 공격중인가?
    public float lastAttackTime;

    public Transform playerTransform;       // 플레이어 위치 받아오기
    public GameObject attackRangeDisplay;   // Sprite Renderer를 통한 공격 방향 보여주기 - 다른 방법이 있다면 좋을 것 같다
    public GameObject attackCollider;       // trigger 타입 콜리더를 껐다키는 방식 사용하기

    public Rigidbody2D rb;
    public SpriteRenderer sr;

    public float attackDistance = 0.4f;     // 공격 가능 거리
    public Vector2 playerDir;               // 플레이어가 있는 위치 (플레이어를 바라보고 거길 향해서 공격할 예정)
    public Vector2 attackVelocity;
    public Vector2 randomMoveVelocity;           // 랜덤으로 움직일 방향
    public float moveSpeed = 0.3f;
    public float attackMoveSpeed = 1.2f;
    public float moveCooldown = 4.0f;
    public float moveCooldownTimer = 0f;

    public Sprite[] currentSprites;

    public Sprite[] idleSprite;                 // 리소스에 한쪽 방향만 있는 관계로 스케일값 조정하기
    public Sprite[] attackSprite;               // 공격
    public Sprite[] waitSprite;                 // 공격 대기

    private int frameIndex = 0;
    public float frameTime;
    private float timer = 0f;

    // 랜덤으로 이동하다 5초에 한번씩 플레이어가 있는지 확인하자. 

    private void Awake()
    {
        playerTransform = FindAnyObjectByType<PlayerController>().gameObject.transform;     // 플레이어 컨트롤러가 있는걸 찾아서 가져오기
        rb = GetComponent<Rigidbody2D>();

        ChangeSprites(idleSprite);

        if(playerTransform == null)
        {
            Debug.Log("플레이어 없는데요?");
        }
    }

    void Start()
    {
        
    }


    private void ChangeSprites(Sprite[] newSprites)
    {
        if (currentSprites == newSprites)
            return;

        currentSprites = newSprites;
        frameIndex = 0;
        timer = 0f;
        sr.sprite = currentSprites[frameIndex];

    }

    public void ReadyForAttack()
    {
        currentWaitTime = 0f;
        ChangeSprites(waitSprite);
        Debug.Log("공격 대기 스프라이트");

        float lookAngle = Mathf.Acos((playerDir.normalized.x * playerDir.normalized.x) / (Mathf.Abs(playerDir.normalized.magnitude) * Mathf.Abs(playerDir.normalized.x)));
        bool isLookUp = false;

        if (playerDir.y > 0)
            isLookUp = true;
        
        if(playerDir.x > 0)
        {
            attackRangeDisplay.transform.rotation = Quaternion.Euler(0f, 0f, ((isLookUp) ? 1.0f : -1.0f) * Mathf.Rad2Deg * lookAngle);
        }
        else
        {
            attackRangeDisplay.transform.rotation = Quaternion.Euler(0f, 0f, -((isLookUp) ? 1.0f : -1.0f) * Mathf.Rad2Deg * lookAngle);
        }

        attackRangeDisplay.SetActive(true);
    }

    public void Attack()
    {
        ChangeSprites(attackSprite);
        Debug.Log("공격 스프라이트");
        lastAttackTime = Time.time;

        attackCollider.SetActive(true);
        attackRangeDisplay.SetActive(false);
    }

    public void AttackFinished()
    {   
        isAttacking = false;
        attackCollider.SetActive(false);
        lastAttackTime = Time.time + attackCooldown; 

        ChangeSprites(idleSprite);
    }

    public void RandomMove()
    {
        randomMoveVelocity = new Vector2((Random.Range(0, 10) - 5), (Random.Range(0, 10) - 5)).normalized * moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        // 플레이어와 자신의 위치 차이 벡터 계산


        playerDir = new Vector2(playerTransform.position.x - this.transform.position.x, playerTransform.position.y - this.transform.position.y);

        if(playerDir.x >0 && !isAttacking)      // 플레이어 보는거 쫒아서 보기
        {
            this.transform.localScale = Vector3.one;
        }
        else if(playerDir.x < 0 && !isAttacking) 
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }

        if (!isAttacking && lastAttackTime < Time.time && playerDir.magnitude < attackDistance)       // 공격하지 않을 경우
        {
            //  그럼 공격 시작해야겠지?
            currentWaitTime = 0.0f;
            isAttacking = true;
            attackVelocity = playerDir.normalized * attackMoveSpeed;    // 공격 방향 & 속도 설정
            ReadyForAttack();
        }

        if(!isAttacking)
        {

        }

        timer += Time.deltaTime;

        if (timer >= frameTime)
        {
            timer = 0f;
            frameIndex++;

            if (frameIndex >= currentSprites.Length)
                frameIndex = 0;

            sr.sprite = currentSprites[frameIndex];
        }

    }

    private void FixedUpdate()
    {

        if (isAttacking && waitTime < currentWaitTime)
        {
            rb.MovePosition(rb.position + attackVelocity * Time.fixedDeltaTime);      //  공격
            if (lastAttackTime + 0.5f <= Time.time)
                AttackFinished();
        }
        else if(isAttacking && waitTime > currentWaitTime)
        {
            currentWaitTime += Time.fixedDeltaTime;
            if (waitTime < currentWaitTime)
                Attack();
            // 기다리는 동안 아무것도 하지 말기
        }
        else
        {
            rb.MovePosition(rb.position + randomMoveVelocity * Time.fixedDeltaTime);
            moveCooldownTimer += Time.fixedDeltaTime;

            if (moveCooldownTimer > moveCooldown * 2)
            {
                moveCooldownTimer = 0f;
                RandomMove();
            }
            else if (moveCooldownTimer > moveCooldown)
                randomMoveVelocity = Vector2.zero;
        }
    }
}
