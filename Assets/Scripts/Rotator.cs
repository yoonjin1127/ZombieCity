using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 60f;   // �ʴ� ȸ�� �ӵ�

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);   // y������ 1�ʿ� 60���� ȸ��
    }
}
