using UnityEngine;

public class DialogueNPC : MonoBehaviour
{
    public DialogueDataSO myDialogue;
    private DialogueManager dialogueManager;

    public GameObject interactionOB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueManager = FindAnyObjectByType<DialogueManager>();    

        if(dialogueManager != null )
        {
            Debug.LogError("다이얼 로그 매니저가 없습니다");
        }
    }

    // Update is called once per frame
    /*
    private void OnMouseDown()
    {
        if (dialogueManager == null) return;
        if (dialogueManager.IsDialogueActive()) return;
        if (myDialogue == null) return;

        dialogueManager.StartDialogue(myDialogue);
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            interactionOB.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            interactionOB.SetActive(false);
        }
    }

    private void Update()
    {
        if (interactionOB.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (dialogueManager == null) return;
                if (dialogueManager.IsDialogueActive()) return;
                if (myDialogue == null) return;
                dialogueManager.StartDialogue(myDialogue);
            }
        }
    }
}
