using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("UI 요소 - 인스펙터 창에서 연결")]
    public GameObject DialoguePannel;
    public Image characterImage;                // 대화창 전체 패널
    public TextMeshProUGUI characternameText;   // 캐릭터 이름 표시 텍스트
    public TextMeshProUGUI dialogueText;        // 대화 내용을 표시하는 텍스트
    public Button nextButton;                   // 다음 버튼

    [Header("기본 설정")]
    public Sprite defaultCharacterImage;

    [Header("타이핑 효과 설정")]
    public float typingSpeed = 0.05f;           // 글자 하나당 출력 속도
    public bool skipTypingOnClick = true;       // 클릭시 타이핑 즉시 완료할지 여부


    // 내부에서 사용할 변수

    private DialogueDataSO currentDialogue;     // 현재 진행중인 대화 데이터
    private int currentLineIndex = 0;           // 현재 몇번째 대화 중인지
    private bool isDialogueActive = false;      // 대화가 진행 중인지 확인하는 플래그
    private bool isTyping = false;              // 현재 타이핑 효과가 진행 중인지 확인
    private Coroutine typingCoroutine;          // 타이핑 효과 코루틴 등록
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DialoguePannel.SetActive(false);
        nextButton.onClick.AddListener(HandNextInput);
    }

    // Update is called once per frame
    void Update()
    {
        if(isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            HandNextInput();
        }
    }

    IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        dialogueText.text = "";

        // 텍스트를 한글자씩 추가
        for(int i = 0; i < textToType.Length; i++)
        {
            dialogueText.text += textToType[i];      // 한글자씩 추가
            yield return new WaitForSeconds(typingSpeed); // 대기 시간 설정
        }

        isTyping = false;
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);     // 코루틴 중지
        }

        isTyping = false;

        // 현재 줄의 전체 텍스트를 즉시 표시
        if (currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count)
        {
            dialogueText.text = currentDialogue.dialogueLines[currentLineIndex];
        }
    }

    void ShowCurrentLine()
    {
        if(currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count)
        {
            if(typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            // 현재 줄의 대화 내용으로 타이핑 효과 시작
            string currentText = currentDialogue.dialogueLines[currentLineIndex];
            typingCoroutine = StartCoroutine(TypeText(currentText));

        }
    }

    public void ShowNextLine()
    {
        currentLineIndex++;         // 다음줄로 인덱스 증가

        // 마지막 대화였는지 확인
        if(currentLineIndex >= currentDialogue.dialogueLines.Count)
        {
            EndDialogue();
        }
        else
        {
            ShowCurrentLine();
        }
    }

    void EndDialogue()                      // 대화를 완전히 종료하는 함수
    {
        if(typingCoroutine != null)         // 타이핑 효과 정리
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isDialogueActive = false;           // 대화 비활성화
        isTyping = false;                   // 타이밍 상태 해제
        DialoguePannel.SetActive(false);    // 대화창 숨기기
        currentLineIndex = 0;               // 인텍스 초기화
    }

    public void HandNextInput()             // 스페이스 or 버튼 클릭시 호출되는 입력 처리 함수
    {
        if(isTyping &&skipTypingOnClick)
        {
            CompleteTyping();               // 타이핑 중이라면 즉시 완료
        }
        else if(!isTyping)                  // 타이핑이 완료 상태면 다음 줄로
        {
            ShowNextLine();
        }
    }

    public void SkipDialogue()              // 대화 전체를 바로 스킵하는 함수
    {
        EndDialogue();
    }

    public bool IsDialogueActive()          // 대화가 진행중인지 확인
    {
        return isDialogueActive;
    }

    public void StartDialogue(DialogueDataSO dialogue)  // 새로운 대화를 시작하는 함수
    {
        if (dialogue == null || dialogue.dialogueLines.Count == 0) return;      // 대화 데이터가 없거나 내용이 비어있으면 실행하지 않음

        // 대화 시작 준비 
        currentDialogue = dialogue;             // 현재 대화 데이터 설정
        currentLineIndex = 0;                   // 첫번째 대화부터 시작
        isDialogueActive = true;                // 대화 활성화 플래그 On

        // UI 업데이트
        DialoguePannel.SetActive(true);
        characternameText.text = dialogue.characterName;    // 캐릭터 이름 표시
        if (characterImage != null)
        {

            if (dialogue.characterImage != null)
            {
                characterImage.sprite = dialogue.characterImage;        // 대화 데이터의 이미지 사용
            }
            else
            {
                characterImage.sprite = defaultCharacterImage;      // 기본 이미지 사용
            }
        }

        ShowCurrentLine();
    }
}
