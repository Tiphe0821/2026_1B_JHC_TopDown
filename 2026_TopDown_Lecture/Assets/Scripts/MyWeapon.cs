using UnityEngine;

public class MyWeapon : MonoBehaviour
{

    // 플레이어가 검을 휘두르도록 하면 애니메이션에서 이벤트를 가져오자

    public GameObject weaponTriggerBox;

    public void AttakStart()
    {
        weaponTriggerBox.SetActive(true);
    }

    public void AttackEnd()
    {
        weaponTriggerBox.SetActive(false);  
    }
}
