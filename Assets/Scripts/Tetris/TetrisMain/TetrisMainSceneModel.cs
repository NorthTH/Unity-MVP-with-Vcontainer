using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MVP;
using Random = System.Random;

namespace Tetris
{
    public enum PieceColor { Cyan = 1, Blue = 2, DarkYellow = 3, Yellow = 4, Green = 5, Magenta = 6, Red = 7 }

    public class Tetromino
    {
        // テトリミノの形状
        private int[,] shape;
        // テトリミノの位置
        private int x;
        private int y;
        // テトリミノの色
        private PieceColor color;

        public Tetromino(int[,] shape, PieceColor color)
        {
            this.shape = shape;
            this.color = color;
            x = 0;
            y = 0;
        }

        public int[,] Shape
        {
            get { return shape; }
            set { shape = value; }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public PieceColor Color
        {
            get { return color; }
        }
    }

    public sealed class TetrisMainSceneModel : Model, ITetrisMainSceneModel
    {
        public TetrisMainSceneModel() { }

        public Action<PieceColor[,], Tetromino> ShowField;
        public Action ShowGameOver;

        const int FieldWidth = 10;
        const int FieldHeight = 20;

        // テトリミノの定義
        private Tetromino[] tetrominos = {
            new Tetromino(new int[,] { { 1, 1, 1, 1 } }, PieceColor.Cyan),    // I
            new Tetromino(new int[,] { { 1, 0, 0 }, { 1, 1, 1 } }, PieceColor.Blue),    // J
            new Tetromino(new int[,] { { 0, 0, 1 }, { 1, 1, 1 } }, PieceColor.DarkYellow),    // L
            new Tetromino(new int[,] { { 1, 1 }, { 1, 1 } }, PieceColor.Yellow),    // O
            new Tetromino(new int[,] { { 0, 1, 1 }, { 1, 1, 0 } }, PieceColor.Green),    // S
            new Tetromino(new int[,] { { 0, 1, 0 }, { 1, 1, 1 } }, PieceColor.Magenta),    // T
            new Tetromino(new int[,] { { 1, 1, 0 }, { 0, 1, 1 } }, PieceColor.Red)    // Z
        };

        // テトリスのフィールド
        private PieceColor[,] field = new PieceColor[FieldHeight, FieldWidth];

        // 現在のテトリミノ
        private Tetromino currentTetromino;

        // ゲームオーバーフラグ
        private bool gameOver;

        public override void Initialize() { }

        public override void PostInitialize()
        {
            GameStart().Forget();
        }

        async UniTaskVoid GameStart()
        {
            while (!gameOver)
            {
                // テトリミノを生成
                GenerateNewTetromino();

                // // テトリミノを下方向に移動
                while (!CheckCollision(currentTetromino.Shape, currentTetromino.X, currentTetromino.Y + 1))
                {
                    // // フィールドを表示
                    ShowField.Invoke(field, currentTetromino);
                    currentTetromino.Y++;
                    await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                }

                // // テトリミノをフィールドに固定
                PlaceTetromino(currentTetromino.Shape, currentTetromino.X, currentTetromino.Y, currentTetromino.Color);

                // // 行が揃ったら消去
                ClearLines();

                // // フィールドを表示
                ShowField.Invoke(field, currentTetromino);

                // // ゲームオーバーチェック
                if (CheckGameOver())
                {
                    break;
                }

                // 一定の遅延を設ける
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            }
            ShowGameOver.Invoke();
        }

        // テトリミノを生成
        void GenerateNewTetromino()
        {
            Random random = new Random();
            int randomIndex = random.Next(0, tetrominos.Length);
            currentTetromino = tetrominos[randomIndex];
            currentTetromino.X = FieldWidth / 2 - currentTetromino.Shape.GetLength(1) / 2;
            currentTetromino.Y = 0;
        }

        // 衝突判定
        bool CheckCollision(int[,] shape, int x, int y)
        {
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] != 0)
                    {
                        int newX = x + j;
                        int newY = y + i;

                        if (newX < 0 || newX >= FieldWidth || newY >= FieldHeight || (newY >= 0 && field[newY, newX] != 0))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // テトリミノをフィールドに配置する
        void PlaceTetromino(int[,] shape, int x, int y, PieceColor color)
        {
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] != 0)
                    {
                        field[y + i, x + j] = color;
                    }
                }
            }
        }

        // ラインが揃ったら消去する
        void ClearLines()
        {
            for (int i = FieldHeight - 1; i >= 0; i--)
            {
                bool lineIsFull = true;
                for (int j = 0; j < FieldWidth; j++)
                {
                    if (field[i, j] == 0)
                    {
                        lineIsFull = false;
                        break;
                    }
                }

                if (lineIsFull)
                {
                    for (int k = i; k > 0; k--)
                    {
                        for (int j = 0; j < FieldWidth; j++)
                        {
                            field[k, j] = field[k - 1, j];
                        }
                    }
                    Array.Clear(field, 0, FieldWidth);
                }
            }
        }

        bool CheckGameOver()
        {
            for (int i = 0; i < FieldWidth; i++)
            {
                if (field[0, i] != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void UserInput(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.LeftArrow:
                    if (!CheckCollision(currentTetromino.Shape, currentTetromino.X - 1, currentTetromino.Y))
                    {
                        currentTetromino.X--;
                    }
                    break;
                case KeyCode.RightArrow:
                    if (!CheckCollision(currentTetromino.Shape, currentTetromino.X + 1, currentTetromino.Y))
                    {
                        currentTetromino.X++;
                    }
                    break;
                case KeyCode.DownArrow:
                    if (!CheckCollision(currentTetromino.Shape, currentTetromino.X, currentTetromino.Y + 1))
                    {
                        currentTetromino.Y++;
                    }
                    break;
                case KeyCode.UpArrow:
                    RotateTetromino();
                    break;
                case KeyCode.Space:
                    while (!CheckCollision(currentTetromino.Shape, currentTetromino.X, currentTetromino.Y + 1))
                    {
                        currentTetromino.Y++;
                    }
                    break;
                default:
                    break;
            }
        }

        void RotateTetromino()
        {
            int[,] rotatedShape = new int[currentTetromino.Shape.GetLength(1), currentTetromino.Shape.GetLength(0)];
            for (int i = 0; i < currentTetromino.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < currentTetromino.Shape.GetLength(1); j++)
                {
                    rotatedShape[j, currentTetromino.Shape.GetLength(0) - i - 1] = currentTetromino.Shape[i, j];
                }
            }

            if (!CheckCollision(rotatedShape, currentTetromino.X, currentTetromino.Y))
            {
                currentTetromino.Shape = rotatedShape;
            }
        }
    }

    public interface ITetrisMainSceneModel { }
}
