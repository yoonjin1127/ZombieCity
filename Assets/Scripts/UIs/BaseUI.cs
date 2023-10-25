using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    // UI��ҵ��� �����ϴ� ��ųʸ���
    protected Dictionary<string, RectTransform> transforms;
    protected Dictionary<string, Button> buttons;
    protected Dictionary<string, TMP_Text> texts;
    // TODO : add ui component

    protected virtual void Awake()
    {
        // ���� ������Ʈ���� ���ε�
        BindChildren();
    }

    // ���� ������Ʈ���� ã�� ��ųʸ��� �����ϴ� �޼���
    protected virtual void BindChildren()
    {
        transforms = new Dictionary<string, RectTransform>();
        buttons = new Dictionary<string, Button>();
        texts = new Dictionary<string, TMP_Text>();

        // �ش� ��ũ��Ʈ�� ���� ���� ������Ʈ�� ��� ���� ������Ʈ�� ã��
        RectTransform[] children = GetComponentsInChildren<RectTransform>();
        foreach (RectTransform child in children)
        {
            string key = child.gameObject.name; // ������Ʈ�� �̸��� key�� ���

            if (transforms.ContainsKey(key))
                continue;   // �̹� ���� �̸��� ������Ʈ�� �ִ� ��� �ǳʶٱ�

            transforms.Add(key, child);

            Button button = child.GetComponent<Button>();
            if (button != null)
                buttons.Add(key, button);

            TMP_Text text = child.GetComponent<TMP_Text>();
            if (text != null)
                texts.Add(key, text);
        }
    }

    // UI�� �ݴ� �޼���, ���� �޼���� ���� Ŭ�������� ������ ����
    public virtual void CloseUI()
    {
    }
}