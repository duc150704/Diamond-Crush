using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum BoardState
{
    Idle,
    Swap,
    SwapBack,
    Fall,
    Refill
}

public class Board : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private float _cellSize;
    [SerializeField] private Vector3 _centerPosition;

    [SerializeField] private AnimationSystem _animationSystem;
    [SerializeField] private List<DiamondSO> _datas;

    private CustomizeGrid<GridObject> _grid;
    private TextMeshPro[,] _debugTexts;
    private Vector3 _clickedPosition;
    private Vector3 _dragDirection;

    private BoardState _state = BoardState.Idle;

    private void Awake()
    {
        _grid = new CustomizeGrid<GridObject>(_width, _height, _cellSize, _centerPosition);
        _debugTexts = new TextMeshPro[_width, _height];
    }

    private void OnEnable()
    {
        _grid.OnGridValueChanged += HandleValueChanged;
    }

    private void OnDisable()
    {
        _grid.OnGridValueChanged -= HandleValueChanged;
    }

    void Start()
    {
        for (int i = 0; i < _width; i++)
        {
            for(int j = 0; j < _height; j++)
            {
                _grid.SetValue(new GridPosition(i, j), new GridObject(NETUltilities.GetRandomInt(1, 4)));
            }
        }

        _animationSystem.SetUp(_grid, _datas);

        ShowDebugLine();
        ShowDebugText();

        Init().Forget();
    }

    async void Update()
    {
        if (_state == BoardState.Idle)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _clickedPosition = UnityUltilities.GetMousePosition();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            DragDirection direction = DragDirectionCalculate();
            GridObject clickedObj = _grid.GetValue(_clickedPosition);

            if (_dragDirection.magnitude >= _cellSize / 2 && clickedObj != null)
            {
                
                GridObject targetObj = _grid.GetValue(clickedObj.GridPosition.GetNeighbor(direction));

                _grid.Swap(clickedObj.GridPosition, targetObj.GridPosition);
                await _animationSystem.Swap(clickedObj, targetObj);

                List<MatchResult> res = MatchChecker.FindMatches(_grid);

                if(res.Count <= 0)
                {
                    _grid.Swap(clickedObj.GridPosition, targetObj.GridPosition);
                    await _animationSystem.Swap(clickedObj, targetObj);
                    return;
                }

                List<GridObject> matched = new List<GridObject>();


                foreach (var item in res)
                {
                    foreach (var item1 in item.MatchedGridPosition)
                    {
                        matched.Add(_grid.GetValue(item1));
                        _grid.SetValue(item1, null);
                    }
                }

                await _animationSystem.Remove(matched);

                //GravitySystem.Apply(_grid);

                //List<GridPosition> a = RefillSystem.FindRefillPosition(_grid);

                //RefillSystem.Fill(_grid, a, 0, 5);
            }
        }
    }

    private void MatchResolves()
    {

    }

    private DragDirection DragDirectionCalculate()
    {
        _dragDirection = UnityUltilities.GetMousePosition() - _clickedPosition;
        DragDirection dragDir = DragDirection.Zero;

        if (_dragDirection.magnitude >= _cellSize)
        {
            dragDir = DirectionNormalization.Normalize(_dragDirection);
        }

        return dragDir;
    }

    private async UniTask Init()
    {
        List<GridObject> objs = _grid.GetAllValue();
        List<SpawnData> spawnData = new List<SpawnData>();

        await UniTask.Delay(2000);

        foreach (var obj in objs)
        {
            spawnData.Add(new SpawnData(obj, _grid.GridToWorld(obj.GridPosition) + new Vector3(0, _height)));
        }
        await _animationSystem.Spawn(spawnData);
        await _animationSystem.Fall(objs, DG.Tweening.Ease.OutSine);
    }

    private void ShowDebugLine()
    {
        if (_width <= 0 || _height <= 0 || _cellSize <= 0)
            return;

        Vector3 offset = new Vector3(_cellSize / 2, _cellSize / 2);

        for(int i = 0; i <= _width; i++)
        {
            Debug.DrawLine(
                _grid.GridToWorld(new GridPosition(i, 0)) - offset
                , _grid.GridToWorld(new GridPosition(i, _height)) - offset
                , Color.white, 100f
                );
        }

        for(int i = 0; i <= _height; i++)
        {
            Debug.DrawLine(
                _grid.GridToWorld(new GridPosition(0, i)) - offset
                , _grid.GridToWorld(new GridPosition(_width, i)) - offset
                , Color.white, 100f
                );
        }
    }

    private void ShowDebugText()
    {
        if (_width <= 0 || _height <= 0 || _cellSize <= 0)
            return;

        for(int i = 0; i < _width; i++)
        {
            for(int j = 0; j < _height; j++)
            {
                GridObject obj = _grid.GetValue(new GridPosition(i, j));
                if (obj == null)
                    continue;
                Vector3 position = _grid.GridToWorld(obj.GridPosition);
                _debugTexts[i, j] = UnityUltilities.CreateWorldText(obj.GetDebugText(), position, 6, Color.white);
            }
        }
    }

    private void HandleValueChanged(GridPosition gridPosition)
    {
        if (_debugTexts[gridPosition.x, gridPosition.y] == null)
            return;

        if (_grid.GetValue(gridPosition) == null)
        {
            _debugTexts[gridPosition.x, gridPosition.y].text = "null";
            return;
        }

        _debugTexts[gridPosition.x, gridPosition.y].text = _grid.GetValue(gridPosition).GetDebugText();
    }
}
