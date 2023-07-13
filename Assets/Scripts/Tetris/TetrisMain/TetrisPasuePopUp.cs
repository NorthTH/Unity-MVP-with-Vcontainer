using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tetris
{
    public class TetrisPasuePopUp : PopUpBase
    {
        [SerializeField]
        Button closeBtn;

        public override void Initialize()
        {
            base.Initialize();

            closeBtn.onClick.AddListener(Close);
        }

        public void Open()
        {
            Time.timeScale = 0;
            WindowOpen();
        }

        protected override void CurtainCallBack()
        {
            Close();
        }

        async void Close()
        {
            await GetController().DeletePopup();
            Time.timeScale = 1;
        }
    }
}
