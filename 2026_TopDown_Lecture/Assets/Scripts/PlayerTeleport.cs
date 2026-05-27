using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    public Transform teleportPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("들어왔다 trigger");
        if(collision.CompareTag("Player"))
        {
            collision.gameObject.transform.position = teleportPosition.position; 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("들어왔다 collider");
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = teleportPosition.position;
        }
    }
}
