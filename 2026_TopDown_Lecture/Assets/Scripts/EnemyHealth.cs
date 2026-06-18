using UnityEngine;

public class EnemyHealth : MonoBehaviour        // 적 체력 구현 부분
{
    public int hp;
    public int def;
    public int defaultDamage;

    public int expAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerWeapon"))
        {
            hp--;
            if(hp <= 0 )
                gameObject.SetActive(false);
        }
    }
}
