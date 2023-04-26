using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectSetting : IStartable
{
    [Inject] GameManager gameManager;

    public void Start()
    {
        // Vcontainer Initializer
        Debug.Log("Start: Vcontainer");
        gameManager.Initialize();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        // Project Initializer
    }
}
