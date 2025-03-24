using System;
using TMPro;
using UnityEngine;

namespace Match3 {
    public class Board<T>
    {
        public int width;
        public int height;
        public float cellSize;
        public Vector2 origin;
        public T[,] cellArray;

        public Generate generate;

        public event Action<int, int, T> OnValueChangeEvent;

        public static Board<T> NormalBoard(int width, int height, float cellSize, Vector2 origin, bool debug = false) {
            return new Board<T>(width, height, cellSize, origin, new NormalGenerate(), debug);
        }

        public Board (int width, int height, float cellSize, Vector2 origin, Generate generate, bool debug) {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.origin = origin;
            this.generate = generate ?? new NormalGenerate();

            cellArray = new T[width, height];

            if (debug) {
                DrawDebugLine();
            }
        }

        public void SetValue(Vector2 worldPosition, T value) {
            Vector2Int pos = generate.WorldToGrid(worldPosition, cellSize, origin);
            SetValue(pos.x, pos.y, value);
        }

        public void SetValue(int x, int y, T value) {
            if (IsValid(x, y)) {
                cellArray[x, y] = value;
                OnValueChangeEvent?.Invoke(x, y, value);
            }
        }

        public T GetValue(Vector2 worldPosition) {
            Vector2Int pos = GetXY(worldPosition);
            return GetValue(pos.x, pos.y);
        }

        public T GetValue(int x, int y) {
            return IsValid(x, y) ? cellArray[x, y] : default(T);
        }

        bool IsValid(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;

        public Vector2Int GetXY(Vector2 worldPosition) => generate.WorldToGrid(worldPosition, cellSize, origin);

        public Vector2 GetWorldPositionCenter(int x, int y) => generate.GridToWorldCenter(x, y, cellSize, origin);

        Vector2 GetWorldPosition(int x, int y) => generate.GridToWorld(x, y, cellSize, origin);

        void DrawDebugLine() {
            const float duration = 100f;
            var parent = new GameObject("Debugging");

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    // CreateWorldText(parent, x + "," + y, GetWorldPositionCenter(x, y), generate.Forward);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, duration);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, duration);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, duration);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, duration);
        }

        private TextMeshPro CreateWorldText(GameObject parent, string text, Vector2 position, Vector3 dir, 
        int fontSize = 2, Color color = default, TextAlignmentOptions textAnchor = TextAlignmentOptions.Center, int sortingOrder = 0) {
            GameObject gameObject = new GameObject("DebugText_" + text, typeof(TextMeshPro));
            gameObject.transform.SetParent(parent.transform);
            gameObject.transform.position = position;
            gameObject.transform.forward = dir;

            TextMeshPro textMeshPro = gameObject.GetComponent<TextMeshPro>();
            textMeshPro.text = text;
            textMeshPro.fontSize = fontSize;
            textMeshPro.color = color == default ? Color.white : color;
            textMeshPro.alignment = textAnchor;
            textMeshPro.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            return textMeshPro;
        }
        
        public abstract class Generate {
            public abstract Vector2 GridToWorld(int x, int y, float cellSize, Vector2 origin);
            public abstract Vector2 GridToWorldCenter(int x, int y, float cellSize, Vector2 origin);
            public abstract Vector2Int WorldToGrid(Vector2 worldPosition, float cellSize, Vector2 origin);
            public abstract Vector3 Forward { get; }
        }

        public class NormalGenerate : Generate {
            public override Vector2 GridToWorld(int x, int y, float cellSize, Vector2 origin)
            {
                return new Vector2(x, y) * cellSize + origin;
            }
            public override Vector2 GridToWorldCenter(int x, int y, float cellSize, Vector2 origin)
            {
                return new Vector2((x + 0.5f) * cellSize, (y + 0.5f) * cellSize) + origin;
            }
            public override Vector2Int WorldToGrid(Vector2 worldPosition, float cellSize, Vector2 origin)
            {
                Vector2 gridPosition = (worldPosition - origin) / cellSize;
                var x = Mathf.FloorToInt(gridPosition.x);
                var y = Mathf.FloorToInt(gridPosition.y);
                return new Vector2Int(x, y);
            }

            public override Vector3 Forward => Vector3.forward;
        }

    }
}