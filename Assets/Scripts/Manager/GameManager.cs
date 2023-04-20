using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] GameObject Curtain;

    public void Init()
    {
        // 
    }

    public void SetShowLoadingCurtain(bool value)
    {
        Curtain.SetActive(value);
    }
}
