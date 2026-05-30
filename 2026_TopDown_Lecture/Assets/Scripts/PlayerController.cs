using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public Sprite[] spriteRightUp;
    public Sprite[] spriteRightDown;
    public Sprite[] spriteLeftUp;
    public Sprite[] spriteLeftDown;
    public float frameTime = 0.15f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveInput;
    private Vector2 worldMouseInput;
    private Vector2 velocity;
    private Sprite[] currentSprites;
    private int frameIndex = 0;
    private float timer = 0f;

    private Camera mainCamera;

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
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
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
