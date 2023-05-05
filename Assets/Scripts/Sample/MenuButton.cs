using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MVP;

namespace Sample
{
    public class MenuButton : MonoBehaviour
    {
        [SerializeField]
        Button menu1Button, menu2Button, menu3Button, menu4Button;

        Action Action1, Action2, Action3, Action4;

        void Awake()
        {
            menu1Button.onClick.AddListener(() => MenuButton_OnClick(0));
            menu2Button.onClick.AddListener(() => MenuButton_OnClick(1));
            menu3Button.onClick.AddListener(() => MenuButton_OnClick(2));
            menu4Button.onClick.AddListener(() => MenuButton_OnClick(3));
        }

        public void SetMenuButtonAction(Action action1, Action action2, Action action3, Action action4)
        {
            Action1 = action1;
            Action2 = action2;
            Action3 = action3;
            Action4 = action4;
        }

        void MenuButton_OnClick(int index)
        {
            switch (index)
            {
                case 0:
                    Action1?.Invoke();
                    break;
                case 1:
                    Action2?.Invoke();
                    break;
                case 2:
                    Action3?.Invoke();
                    break;
                case 3:
                    Action4?.Invoke();
                    break;
            }
        }
    }
}
