using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3 {
    public class BoardController : MonoBehaviour {
        public static BoardController Instance { get; private set; }

        [SerializeField] int width = 5;
        [SerializeField] int height = 5;
        [SerializeField] float cellSize = 1f;
        [SerializeField] Vector2 originPosition = Vector2.zero;
        [SerializeField] bool debug = true;
        [SerializeField] WeaponHandle blockPrefab;
        public Weapon[] weaponType = new Weapon[4];
        [SerializeField] Ease ease = Ease.InQuad;
        [SerializeField] GameObject explosion;
        [SerializeField] GameObject selectedFX;
        [SerializeField] float selectionRange = 1f;
        [SerializeField] GameObject selectionRangePrefab;
        [SerializeField] GameObject boardPrefab;
        public bool _isSwapping = false;

        Board<Cells<WeaponHandle>> grid;

        InputReader inputReader;

        [SerializeField] Vector2Int selectedBlock = Vector2Int.one * -1;
        private GameObject currentSelectionEffect;
        private GameObject currentSelectionRangeEffect;
        

        void Awake() {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            inputReader = GetComponent<InputReader>();
            InventoryManager.Instance.SetEquipedWeapon();
        }


        void Start() {
            InitializeGrid();
        }

        void OnDestroy() {
        }
        public bool DoneSwapingThing() {
            return selectedBlock == Vector2Int.one * -1;
        }

        public void BeginSwap() {
            inputReader.Fire += OnSelectBlock;
        }
        public void StopSwap() {
            inputReader.Fire -= OnSelectBlock;
        }

        public void OnSelectBlock() {
            if (_isSwapping) return;
            
            Vector2 screenPosition = inputReader.Selected;
            var gridPos = grid.GetXY(Camera.main.ScreenToWorldPoint(screenPosition));
            
            if (selectedBlock == gridPos || !IsValidPosition(gridPos)) {
                DeselectBlock();
            } else if (selectedBlock == Vector2Int.one * -1) {
                SelectBlock(gridPos);
            } else if (IsWithinSelectionRange(selectedBlock, gridPos)){

                // end selected animation
                if (currentSelectionEffect != null) {
                    Destroy(currentSelectionEffect);
                    currentSelectionEffect = null;
                }
                if (currentSelectionRangeEffect != null) {
                    Destroy(currentSelectionRangeEffect);
                    currentSelectionRangeEffect = null;
                }
                
                StartCoroutine(RunGameLoop(selectedBlock, gridPos));
            }
        }

        IEnumerator RunGameLoop(Vector2Int gridPosA, Vector2Int gridPosB) {
            yield return StartCoroutine(SwapBlock(gridPosA, gridPosB));
            
            yield return StartCoroutine(MatchAndCount());
            EventBus.TriggerBlockSwap();
        }

        IEnumerator MatchAndCount()
        {
            Dictionary<Weapon, int> accumulatedWeaponMatches = new Dictionary<Weapon, int>();


            List<Vector2Int> matches = FindMatches();
            
            if (matches.Count > 0) {
                var weaponMatchCounts = CountWeaponMatches(matches);
                // Accumulate matches across all cycles
                foreach (var kvp in weaponMatchCounts)
                {
                    if (accumulatedWeaponMatches.ContainsKey(kvp.Key))
                    {
                        accumulatedWeaponMatches[kvp.Key] += kvp.Value; // Add to existing count
                    }
                    else
                    {
                        accumulatedWeaponMatches[kvp.Key] = kvp.Value; // First time seeing this weapon
                    }
                }
                // Accumulate matches across all cycles
                foreach (var kvp in weaponMatchCounts)
                {
                    Debug.Log("Matched: " + kvp.Key + " x" + kvp.Value);
                }

                // Visual effects and board updates
                yield return StartCoroutine(ExplodeBlock(matches));
                yield return StartCoroutine(MakeBlockFall());
                yield return StartCoroutine(FillEmptySpots());
            } 
            DeselectBlock();
        }

        private Dictionary<Weapon, int> CountWeaponMatches(List<Vector2Int> matches) {
            Dictionary<Weapon, int> weaponMatchCounts = new Dictionary<Weapon, int>();

            foreach (var match in matches) {
                var block = grid.GetValue(match.x, match.y)?.GetValue();

                if (block != null) {
                    Weapon weaponType = block.GetTypes();

                    if (weaponMatchCounts.ContainsKey(weaponType)) {
                        weaponMatchCounts[weaponType]++;
                    } else {
                        weaponMatchCounts[weaponType] = 1;
                    }
                }
            }

            return weaponMatchCounts;
        }

        IEnumerator FillEmptySpots() {
            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height; y++) {
                    if (grid.GetValue(x, y) == null) {
                        yield return StartCoroutine(CreateBlock(x, y));
                    }
                }
            }
            yield return StartCoroutine(MatchAndCount());
        }

        IEnumerator MakeBlockFall() {
            // TODO: Make this more efficient
            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height; y++) {
                    if (grid.GetValue(x, y) == null) {
                        for (var i = y + 1; i < height; i++) {
                            if (grid.GetValue(x, i) != null) {
                                var block = grid.GetValue(x, i).GetValue();
                                grid.SetValue(x, y, grid.GetValue(x, i));
                                grid.SetValue(x, i, null);
                                block.transform
                                    .DOLocalMove(grid.GetWorldPositionCenter(x, y), 0.5f)
                                    .SetEase(ease);
                                yield return new WaitForSeconds(0.1f);
                                break;
                            }
                        }
                    }
                }
            }
        }

        IEnumerator ExplodeBlock(List<Vector2Int> matches) {

            foreach (var match in matches) {
                var block = grid.GetValue(match.x, match.y).GetValue();
                grid.SetValue(match.x, match.y, null);

                ExplodeVFX(match);
                
                block.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 0.5f);
                
                yield return new WaitForSeconds(0.1f);
                WeaponHandle weapon = block.GetComponent<WeaponHandle>();
                weapon.DestroyBlock();
                Destroy(block.gameObject, 0.1f);
            }
        }

        void ExplodeVFX(Vector2Int match) {
            // TODO: Pool
            var fx = Instantiate(explosion, transform);
            fx.transform.position = grid.GetWorldPositionCenter(match.x, match.y);
            Destroy(fx, 1f);
        }

        List<Vector2Int> FindMatches() {
            HashSet<Vector2Int> matches = new();
            
            // Horizontal
            for (var y = 0; y < height; y++) {
                for (var x = 0; x < width - 2; x++) {
                    var blockA = grid.GetValue(x, y);
                    var blockB = grid.GetValue(x + 1, y);
                    var blockC = grid.GetValue(x + 2, y);
                    
                    if (blockA == null || blockB == null || blockC == null) continue;
                    
                    if (blockA.GetValue().GetTypes() == blockB.GetValue().GetTypes() 
                        && blockB.GetValue().GetTypes() == blockC.GetValue().GetTypes()) {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x + 1, y));
                        matches.Add(new Vector2Int(x + 2, y));
                    }
                }
            }
            
            // Vertical
            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height - 2; y++) {
                    var blockA = grid.GetValue(x, y);
                    var blockB = grid.GetValue(x, y + 1);
                    var blockC = grid.GetValue(x, y + 2);
                    
                    if (blockA == null || blockB == null || blockC == null) continue;
                    
                    if (blockA.GetValue().GetTypes() == blockB.GetValue().GetTypes() 
                        && blockB.GetValue().GetTypes() == blockC.GetValue().GetTypes()) {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x, y + 1));
                        matches.Add(new Vector2Int(x, y + 2));
                    }
                }
            }
            return new List<Vector2Int>(matches);
        }

        bool HasMatch() => FindMatches().Any(); // return if have or not.

        IEnumerator SwapBlock(Vector2Int gridPosA, Vector2Int gridPosB) {
            _isSwapping = true;

            var gridObjectA = grid.GetValue(gridPosA.x, gridPosA.y);
            var gridObjectB = grid.GetValue(gridPosB.x, gridPosB.y);
            
            gridObjectA.GetValue().transform
                .DOLocalMove(grid.GetWorldPositionCenter(gridPosB.x, gridPosB.y), 0.5f)
                .SetEase(ease);
            gridObjectB.GetValue().transform
                .DOLocalMove(grid.GetWorldPositionCenter(gridPosA.x, gridPosA.y), 0.5f)
                .SetEase(ease);
            
            grid.SetValue(gridPosA.x, gridPosA.y, gridObjectB);
            grid.SetValue(gridPosB.x, gridPosB.y, gridObjectA);
            
            yield return new WaitForSeconds(0.5f);
            _isSwapping = false;
        }


        void InitializeGrid() {
            originPosition = CalculateBoardOriginPosition();

            grid = Board<Cells<WeaponHandle>>.NormalBoard(width, height, cellSize, originPosition, debug);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    StartCoroutine(CreateBlock(x, y));
                }
            }

            StartCoroutine(MatchAndCount());
        }

        Vector2 CalculateBoardOriginPosition() {
            // Get the camera's bounds
            Camera mainCamera = Camera.main;
            float cameraHeight = mainCamera.orthographicSize;
            float aspectRatio = mainCamera.aspect;
            float cameraWidth = cameraHeight * aspectRatio * 2;
            float boardWidth = width * cellSize; // Width of the board
            float boardHeight = height * cellSize; // Height of the board

            // Calculate the origin position to place the board at the center bottom
            float originX = 0; // Center horizontally
            float originY = mainCamera.transform.position.y - cameraHeight * 19/20;// Bottom of the camera view

            return new Vector2(originX, originY);
        }

        IEnumerator CreateBlock(int x, int y)
        {
            WeaponHandle block;
            GameObject boardCell;
            #nullable enable
            // Get the left and top block types (if they exist)
            Weapon? leftWeapon = (x > 0) ? grid.GetValue(x - 1, y)?.GetValue()?.GetTypes() : null;
            Weapon? topWeapon = (y > 0) ? grid.GetValue(x, y - 1)?.GetValue()?.GetTypes() : null;

            // Pick a random weapon type, but avoid matching left or top
            Weapon weapon;
            do
            {
                weapon = weaponType[Random.Range(0, weaponType.Length)];
            } while (weapon == leftWeapon || weapon == topWeapon); // Avoid matches

            // ✅ Create Block & Board Cell
            block = Instantiate(blockPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
            block.transform.localScale = new Vector3(cellSize * 0.6f, cellSize * 0.6f, 0);

            boardCell = Instantiate(boardPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
            boardCell.transform.localScale = new Vector3(cellSize, cellSize, 0);

            block.SetType(weapon); // ✅ Set the final weapon type

            // ✅ Set into grid
            var gridObject = new Cells<WeaponHandle>(grid, x, y);
            gridObject.SetValue(block);
            grid.SetValue(x, y, gridObject);

            yield return null; // Slight delay for coroutine
        }

        void DeselectBlock()
        {
            selectedBlock = new Vector2Int(-1, -1);

            // end selected animation
            if (currentSelectionEffect != null) {
                Destroy(currentSelectionEffect);
                currentSelectionEffect = null;
            }
            if (currentSelectionRangeEffect != null) {
                Destroy(currentSelectionRangeEffect);
                currentSelectionRangeEffect = null;
            }

        }

        void SelectBlock(Vector2Int gridPos)
        {
            selectedBlock = gridPos;

            // start selected animation
            if (currentSelectionEffect != null) {
                Destroy(currentSelectionEffect); // Destroy previous selection effect if it exists
            }
            Vector3 worldPosition = grid.GetWorldPositionCenter(gridPos.x, gridPos.y);
            currentSelectionEffect = Instantiate(selectedFX, worldPosition, Quaternion.identity, transform);
            currentSelectionEffect.transform.localScale = new Vector3(cellSize, cellSize, 1);

            // Instantiate the selection range effect
            if (currentSelectionRangeEffect != null) {
                Destroy(currentSelectionRangeEffect);
            }
            // fix how??
            // make the range
            currentSelectionRangeEffect = Instantiate(selectionRangePrefab, worldPosition, Quaternion.identity, transform);
            currentSelectionRangeEffect.transform.localPosition = new Vector3(worldPosition.x, worldPosition.y, 0);
            float scale = (1 + selectionRange * 2) * cellSize;
            currentSelectionRangeEffect.transform.localScale = new Vector3(scale, scale, 1); // Adjust to the appropriate axis
        }

        bool IsEmptyPosition(Vector2Int gridPosition) => grid.GetValue(gridPosition.x, gridPosition.y) == null;

        bool IsValidPosition(Vector2Int gridPosition) {
            return gridPosition.x >= 0 && gridPosition.x < width && gridPosition.y >= 0 && gridPosition.y < height;
        }

        private bool IsWithinSelectionRange(Vector2Int previousPosition, Vector2Int newPosition) {
            return Mathf.Abs(previousPosition.x - newPosition.x) <= selectionRange &&
                   Mathf.Abs(previousPosition.y - newPosition.y) <= selectionRange;
        }

    }
}