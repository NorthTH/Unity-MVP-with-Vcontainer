using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using MVP;

namespace Tetris
{
    public class TetrisMainSceneView : View, ITetrisMainSceneView
    {
        public Action<KeyCode> UserInputKey;

        [SerializeField]
        PopUpContainer popUpContainer;
        [SerializeField]
        Transform blockParent;
        [SerializeField]
        Image blockPrefab;
        [SerializeField]
        Button pauseBtn;

        const int FieldWidth = 10;
        const int FieldHeight = 20;
        const int Paddad = 105;
        const int OffSetX = -500;
        const int OffSetY = 800;

        Image[,] blockBroad;

        public override void Initialize()
        {
            pauseBtn.onClick.AddListener(PauseBtn_OnClick);

            blockBroad = new Image[FieldWidth, FieldHeight];
            for (int i = 0; i < FieldWidth; i++)
            {
                for (int j = 0; j < FieldHeight; j++)
                {
                    blockBroad[i, j] = Instantiate<Image>(blockPrefab, blockParent);
                    var block = blockBroad[i, j];
                    block.color = Color.black;
                    block.transform.localPosition = new Vector2(i * Paddad + OffSetX, -j * Paddad + OffSetY);
                    block.gameObject.SetActive(true);
                }
            }
        }

        public override void PostInitialize()
        {
            UserInputThread().Forget();
        }

        async UniTaskVoid UserInputThread()
        {
            var utcs = this.GetCancellationTokenOnDestroy();
            while (!utcs.IsCancellationRequested)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    UserInputKey.Invoke(KeyCode.LeftArrow);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    UserInputKey.Invoke(KeyCode.RightArrow);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    UserInputKey.Invoke(KeyCode.DownArrow);
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    UserInputKey.Invoke(KeyCode.UpArrow);
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    UserInputKey.Invoke(KeyCode.Space);
                }
                await UniTask.Yield();
            }
        }

        public void ShowField(PieceColor[,] fieldData, Tetromino currentTetromino)
        {
            for (int i = 0; i < FieldWidth; i++)
            {
                for (int j = 0; j < FieldHeight; j++)
                {
                    blockBroad[i, j].color = GetColor(fieldData[j, i]);
                }
            }

            for (int i = 0; i < currentTetromino.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < currentTetromino.Shape.GetLength(1); j++)
                {
                    if (currentTetromino.Shape[i, j] != 0)
                    {
                        int x = currentTetromino.X + j;
                        int y = currentTetromino.Y + i;

                        if (y >= 0 && y < FieldHeight && x >= 0 && x < FieldWidth)
                        {
                            blockBroad[x, y].color = GetColor(currentTetromino.Color);
                        }
                    }
                }
            }
        }

        Color GetColor(PieceColor color) => color switch
        {
            PieceColor.Cyan => Color.cyan,
            PieceColor.Blue => Color.blue,
            PieceColor.DarkYellow => Color.grey,
            PieceColor.Yellow => Color.yellow,
            PieceColor.Green => Color.green,
            PieceColor.Magenta => Color.magenta,
            PieceColor.Red => Color.red,
            _ => Color.black,
        };

        public void ShowGameOver()
        {
            Debug.Log("GameOver");
        }

        void PauseBtn_OnClick()
        {
            var pausePopUp = popUpContainer.CreatePopup<TetrisPasuePopUp>(10);
            pausePopUp.Open();
        }
    }

    public interface ITetrisMainSceneView { }
}
