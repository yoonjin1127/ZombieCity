using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ Ÿ�Ե��� �ݵ�� �����ؾ��ϴ� �������̽�
public interface IItem
{
    // �Է����� �޴� target�� ������ ȿ���� ����� ���
    void Use(GameObject target);
    
}
