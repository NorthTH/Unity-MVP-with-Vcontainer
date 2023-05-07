using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MVP;

[RequireComponent(typeof(Button))]
public class ReturnButton : MonoBehaviour
{
    Button returnBtn;

    UnityEvent onClick;
    public UnityEvent OnClick
    {
        get
        {
            if (onClick == null)
                onClick = new UnityEvent();
            return onClick;
        }
    }

    void Awake()
    {
        returnBtn = this.GetComponent<Button>();
        returnBtn.onClick.AddListener(ReturnBtn_OnClick);
    }

    void ReturnBtn_OnClick()
    {
        if (onClick != null)
            onClick.Invoke();
        else
            _ = HistoryManager.ReturnScene();
    }
}
