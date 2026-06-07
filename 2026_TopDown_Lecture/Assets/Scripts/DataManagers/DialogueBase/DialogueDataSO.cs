using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/DialogueData")]
public class DialogueDataSO : ScriptableObject
{
    // 에셋 메뉴 만드는 것

    [Header("캐릭터 정보")]
    public string characterName = "캐릭터";            //   대화 창에 표시될 캐릭터 이름
    public Sprite characterImage;                     //   캐릭터 얼굴 이미지

    [Header("대화 내용")]
    [TextArea(3, 10)]                                           // 인스펙터 창에서 여러줄 입력 가능하게 창 설정
    public List<string> dialogueLines = new List<string>();     // 대화 내용들 (순서대로 출력)
    


}
