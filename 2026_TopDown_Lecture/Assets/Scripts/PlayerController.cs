using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //플레이어 움직임 구현

    public float moveSpeed = 0.8f;
    public float sprintSpeed = 1.5f;
    public Sprite[] spriteRightUp;
    public Sprite[] spriteRightDown;
    public Sprite[] spriteLeftUp;
    public Sprite[] spriteLeftDown;
    public float frameTime = 0.2f;
    public float runFrameTime = 0.1f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveInput;
    private Vector2 worldMouseInput;
    private Vector2 velocity;
    private Sprite[] currentSprites;
    private int frameIndex = 0;
    private float timer = 0f;

    private float isSprint;

    private Camera mainCamera;

    // 플레이어 캐릭터가 보는 위치 단위벡터
    private Vector2 lookDir;

    [Header("달리기 & 대시 설정")]
    // 달리기 & 대시 구현

    private Vector2 dashVelocity;
    private bool isDashing = false;
    public bool isInCombat = false;
    public bool isDashed = false;
    public float dashSpeed = 2.0f;
    private float lastDash = 0f;
    private float dashCooldown = 1.5f;

    [Header("공격 설정")]

    public float isAttacking;        // 공격을 하고있는지 확인
    public float attackMoveRange;           // 공격 시 미세하게 앞으로 움직이는 속도 (콤보 구현 시 개선 예정)
    public int currentCombo = 0;            // 현재 콤보 (몇번째 연속 공격인지)
    public float attackDuration;            // 공격 지속시간

    [Header("무기 설정")]
    // 무기 구현
    public GameObject weapon;       // 무기의 부모로 할당되어 있는 게임 오브젝트
    // 무기가 보는 방향 (왼쪽 오른쪽 정하기)
    public bool isLookRight = true;
    public bool isLookUp = true;
    public float myWeaponAngle;

    public MyWeapon myWeapon;       // 무기에 직접적으로 연결된 스크립트 받아오기

    private void Awake()
    {
        // 스크립트 상에서 자식의 자식에 있는 컴포넌트 가져오기는 조금 어려울 것 같으니 이건 프리팹으로 처리해보자

        mainCamera = Camera.main;

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        currentSprites = spriteRightDown;
        sr.sprite = currentSprites[0];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        velocity = moveInput.normalized * moveSpeed;

        /*
        if (moveInput.magnitude > 0.01f)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                if (moveInput.x > 0)
                    ChangeSprites(spriteLeftDown);
                else
                    ChangeSprites(spriteLeftUp);
            }
            else
            {
                if (moveInput.y > 0)
                    ChangeSprites(spritRightUp);
                else
                    ChangeSprites(spriteRightDown);
            }
        }
        */

    }

    private void OnLook(InputValue value)   // 마우스가 움직일 때 호출됨 -> 사용해보자. (Delta값 받아오는걸 position 값 받아오게 바꾼 뒤 월드포지션으로 변환)
    {
        worldMouseInput = mainCamera.ScreenToWorldPoint(value.Get<Vector2>());

        // 여기서 플레이어 보는 방향 정해주기
        // 세피리아 따라할거라면 제대로.
        // 세피리아에서 보는 방향 : 
        // 좌-상 / 우-상
        // 좌-하 / 우-하 (상하좌우 대신 무조건 왼쪽 또는 오른쪽을 본다)
        // 월드 포지션을 받아오도록 했으니 플레이어 포지션도 받아와야겠지?
        // 이게 플레이어 컨트롤 스크립트니까 그냥 transform.position 받아오자.

        if(worldMouseInput.x > this.transform.position.x)       // 위랑 다르게 예외처리 따로 안하는 이유. -> 굳이? 마우스 포지션 + 플레이어 캐릭터 포지션 기반이라 예외처리가 필요 없을 것 같다
        {
            isLookRight = true;

            if(worldMouseInput.y > this.transform.position.y)
            {
                isLookUp = true;
                ChangeSprites(spriteRightUp);
            }
            else
            {
                isLookUp = false;
                ChangeSprites(spriteRightDown);
            }
        }
        else
        {
            isLookRight = false;

            if (worldMouseInput.y > this.transform.position.y)
            {
                isLookUp = true;
                ChangeSprites(spriteLeftUp);
            }
            else
            {
                isLookUp = false;
                ChangeSprites(spriteLeftDown);
            }
        }

        // 플레이어가 움직일 때 무기도 같이 플레이어가 보는 방향을 바라봐야 한다
        // 위 값에서 플레이어로부터 마우스를 보는 단위벡터를 구하자
        // 둘 모두 원점에서 시작한 벡터값 기준이다. 그러니 마우스의 위치에서 플레이어의 위치 값을 빼면 플레이어 캐릭터가 바라보는 벡터값이 나온다

        lookDir = (worldMouseInput - new Vector2(transform.position.x, transform.position.y)).normalized;   // 플레이어가 보는 방향
        CalculateWeaponAngle();
    }

    private void OnSprint(InputValue value)
    {
        isSprint = value.Get<float>();
        
        if(isSprint > 0.01f && !isDashed)
        {
            // 대시할 방향 받기
            dashVelocity = velocity.normalized * dashSpeed;
            isDashing = true;
            isDashed = true;                                        // 왜 bool 값을 넣었지? 쿨타임 구현 + 연속 대시 기능을 넣을 예정이기에
            lastDash = Time.time;
        }
    }

    private void OnAttack(InputValue value)     // 마우스 좌클릭 (공격)
    {
        if (isDashing) return;

        isAttacking = value.Get<float>();       // 입력받는 버튼 값이 float 값.  그대로 받아온다




    }
    private void OnSkill(InputValue value)      // 마우스 우클릭 (스킬)
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(moveInput.sqrMagnitude <= 0.01f)
        {
            frameIndex = 0;
            sr.sprite = currentSprites[frameIndex];
            return;
        }

        timer += Time.deltaTime;

        if(timer >= frameTime)
        {
            timer = 0f;
            frameIndex++;

            if (frameIndex >= currentSprites.Length)
                frameIndex = 0;

            sr.sprite = currentSprites[frameIndex];
        }

        if (lastDash + dashCooldown <= Time.time)
            isDashed = false;

        
    }

    private void FixedUpdate()
    {
        if(isDashing)
        {
            rb.MovePosition(rb.position + dashVelocity * Time.fixedDeltaTime);      //  대시 사용
            if (lastDash + 0.1f <= Time.time)
                isDashing = false;
        }
        else
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);      //  달리지 않을 경우
        }

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

    private void CalculateWeaponAngle()
    {

        // 플레이어 자체를 돌아가도록 만드는 임시 함수
        // 단위벡터 2개 (하나는 마우스가 가리키는 방향의 단위벡터, 다른 하나는 lookdir.x, 0 가 된다)
        /*
        myWeaponAngle = Mathf.Acos((lookDir.x * lookDir.x) / (Mathf.Abs(lookDir.magnitude) * Mathf.Abs(lookDir.x)));
        this.transform.rotation = Quaternion.Euler(0f, 0f, ((isLookUp) ? 1.0f : -1.0f) * Mathf.Rad2Deg * myWeaponAngle);

        if (!isLookRight)
        {
            this.transform.rotation = Quaternion.Euler(0f, 0f, 180f -((isLookUp) ? 1.0f : -1.0f) * Mathf.Rad2Deg * myWeaponAngle);
        }
        */
        // 이제 이걸 무기에 적용시켜보자

        myWeaponAngle = Mathf.Acos((lookDir.x * lookDir.x) / (Mathf.Abs(lookDir.magnitude) * Mathf.Abs(lookDir.x)));

        if (!isLookRight)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            weapon.transform.rotation = Quaternion.Euler(0f, 0f, - ((isLookUp) ? 1.0f : -1.0f) * Mathf.Rad2Deg * myWeaponAngle);
        }
        else
        {
            transform.localScale = Vector3.one;
            weapon.transform.rotation = Quaternion.Euler(0f, 0f, ((isLookUp) ? 1.0f : -1.0f) * Mathf.Rad2Deg * myWeaponAngle);
        }

    }
}
