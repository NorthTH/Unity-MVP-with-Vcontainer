using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using VContainer;

public sealed class ErrorManager : SingletonMonoBehaviour<ErrorManager>
{
    ///<summary>
    ///必ずあってはいけないエラー用、エラー表示してタイトルに戻ります
    ///</summary>
    public async UniTask<bool> ShowError(Exception ex)
    {
        int select = 0;

        switch (ex)
        {
            case Exception _ when ex is TimeoutException:

                break;
            case Exception _ when ex is System.Net.WebException:

                break;
            case Exception _ when ex is ArgumentException:
            case Exception _ when ex is ArgumentNullException:

                break;
            case Exception _ when ex is System.IO.FileNotFoundException:
            case Exception _ when ex is System.IO.PathTooLongException:
            case Exception _ when ex is System.IO.IOException:
            case Exception _ when ex is NotSupportedException:

                break;
            case Exception _ when ex is UnauthorizedAccessException:

                break;
            case Exception _ when ex is System.Security.SecurityException:

                break;
            case Exception _ when ex is System.Net.HttpListenerException:

                break;
            case Exception _ when ex is System.OutOfMemoryException:

                break;
            default:

                break;
        }

        await UniTask.WaitUntil(() => select != 0);
        if (select == 1) //タイトルへ
        {

        }

        return select == 2;
    }
}
