using UnityEngine;

public class NPCSayHello : MonoBehaviour
{
    public GameObject interactionIcon;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            interactionIcon.SetActive(false);
        }
    }
}
