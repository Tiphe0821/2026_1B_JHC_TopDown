using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{

    // 무기를 휘두를 때마다 해당 스크립트가 붙은 콜리더 오브젝트를 껐다 켰다를 반복할 예정
    // 이유: 이렇게 해도 트리거 함수가 계속 작동하기 때문 
    // (차라리 적 콜리더에 트리거를 넣을지도 고민중. 그게 나을지도?)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("무기 들어왔어요");
    }
}
