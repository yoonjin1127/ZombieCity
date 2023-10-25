using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
    // UI요소들을 저장하는 딕셔너리들
    protected Dictionary<string, RectTransform> transforms;
    protected Dictionary<string, Button> buttons;
    protected Dictionary<string, TMP_Text> texts;
    // TODO : add ui component

    protected virtual void Awake()
    {
        // 하위 오브젝트들을 바인딩
        BindChildren();
    }

    // 하위 오브젝트들을 찾아 딕셔너리에 저장하는 메서드
    protected virtual void BindChildren()
    {
        transforms = new Dictionary<string, RectTransform>();
        buttons = new Dictionary<string, Button>();
        texts = new Dictionary<string, TMP_Text>();

        // 해당 스크립트가 붙은 게임 오브젝트의 모든 하위 오브젝트를 찾음
        RectTransform[] children = GetComponentsInChildren<RectTransform>();
        foreach (RectTransform child in children)
        {
            string key = child.gameObject.name; // 오브젝트의 이름을 key로 사용

            if (transforms.ContainsKey(key))
                continue;   // 이미 같은 이름의 오브젝트가 있는 경우 건너뛰기

            transforms.Add(key, child);

            Button button = child.GetComponent<Button>();
            if (button != null)
                buttons.Add(key, button);

            TMP_Text text = child.GetComponent<TMP_Text>();
            if (text != null)
                texts.Add(key, text);
        }
    }

    // UI를 닫는 메서드, 가상 메서드로 하위 클래스에서 재정의 가능
    public virtual void CloseUI()
    {
    }
}