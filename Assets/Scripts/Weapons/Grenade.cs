using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    private void Start()
    {
        StartCoroutine(Explosion());
    }

    // 폭발 코루틴
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;  // 물리적 속도를 모두 Vector3.zero로 초기화
        rigid.angularVelocity = Vector3.zero;
        // 메쉬는 비활성화, 폭발 효과는 활성화한다
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        // 구체모양의 레이캐스팅, 시작 위치, 반지름, 쏘는 방향, 레이를 쏘는 길이 
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }
        Destroy(gameObject, 5);
    }
}