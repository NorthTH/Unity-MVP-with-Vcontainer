using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MVP;

[RequireComponent(typeof(Button))]
public class ReturnButton : MonoBehaviour
{
    Button returnBtn;

    void Awake()
    {
        returnBtn = this.GetComponent<Button>();
        returnBtn.onClick.AddListener(ReturnBtn_OnClick);
    }

    void ReturnBtn_OnClick()
    {
        _ = HistoryManager.ReturnScene();
    }
}
