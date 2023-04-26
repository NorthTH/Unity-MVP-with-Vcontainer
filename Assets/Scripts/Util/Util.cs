using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Util
{
    public static bool IsLongDisplayResolution
    {
        get
        {
            float width = Screen.width;
            float height = Screen.height;
            return (width / height) >= 18.4f / 9f;
        }
    }

    public static bool IsLargeDisplayResolution
    {
        get
        {
            float width = Screen.width;
            float height = Screen.height;
            return (width / height) < 4f / 2.9f;
        }
    }

}
