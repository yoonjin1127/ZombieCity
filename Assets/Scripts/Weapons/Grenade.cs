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

    // ���� �ڷ�ƾ
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;  // ������ �ӵ��� ��� Vector3.zero�� �ʱ�ȭ
        rigid.angularVelocity = Vector3.zero;
        // �޽��� ��Ȱ��ȭ, ���� ȿ���� Ȱ��ȭ�Ѵ�
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        // ��ü����� ����ĳ����, ���� ��ġ, ������, ��� ����, ���̸� ��� ���� 
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }
        Destroy(gameObject, 5);
    }
}