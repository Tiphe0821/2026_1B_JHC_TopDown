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

    [Header("무기 설정")]
    // 무기 구현
    public GameObject weapon;


    private void Awake()
    {
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
            if(worldMouseInput.y > this.transform.position.y)
                ChangeSprites(spriteRightUp);
            else
                ChangeSprites(spriteRightDown);
        }
        else
        {
            if (worldMouseInput.y > this.transform.position.y)
                ChangeSprites(spriteLeftUp);
            else
                ChangeSprites(spriteLeftDown);
        }

        // 플레이어가 움직일 때 무기도 같이 플레이어가 보는 방향을 바라봐야 한다
        // 위 값에서 플레이어로부터 마우스를 보는 단위벡터를 구하자
        // 둘 모두 원점에서 시작한 벡터값 기준이다. 그러니 마우스의 위치에서 플레이어의 위치 값을 빼면 플레이어 캐릭터가 바라보는 벡터값이 나온다

        lookDir = (worldMouseInput - new Vector2(transform.position.x, transform.position.y)).normalized;   // 플레이어가 보는 방향
        
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
}
