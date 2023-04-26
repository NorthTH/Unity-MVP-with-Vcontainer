using UnityEngine;

public class AdjustPanel : MonoBehaviour
{
    RectTransform rect => GetComponent<RectTransform>();

    Vector2 longSize = new Vector2(1790, 900);
    Vector2 longOffSize = new Vector2(0, 0);
    Vector2 largeSize = new Vector2(1600, 1200);
    Vector2 normalSize = new Vector2(1600, 900);

    void Awake()
    {
        Adjust();
    }
#if UNITY_EDITOR
    void Update()
    {
        Adjust();
    }
#endif

    void Adjust()
    {
        if (Util.IsLongDisplayResolution)
        {
            rect.sizeDelta = longSize;
            rect.localPosition = new Vector2(longOffSize.x, longOffSize.y);
        }
        else if (Util.IsLongDisplayResolution)
        {
            rect.sizeDelta = largeSize;
            rect.localPosition = Vector2.zero;
        }
        else
        {
            rect.sizeDelta = normalSize;
            rect.localPosition = Vector2.zero;
        }
    }
}
