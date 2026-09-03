using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum BoardState
{
    Idle,
    Running,
}

public class Board : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private float _cellSize;
    [SerializeField] private Vector3 _centerPosition;

    [SerializeField] private List<DiamondSO> _datas;
    [SerializeField] private AnimationView _view;
    [SerializeField] private AnimationConfig _config;

    private AnimationModel _model;
    private AnimationPresenter _animationPresenter;
    private CustomizedGrid<GridObject> _grid;
    private TextMeshPro[,] _debugTexts;
    private Vector3 _clickedPosition;
    private Vector3 _dragDirection;

    private BoardState _state = BoardState.Idle;

    private void Awake()
    {
        _grid = new CustomizedGrid<GridObject>(_width, _height, _cellSize, _centerPosition);
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

    async void Start()
    {
        _state = BoardState.Running;

        List<GridData<GridObject>> data = Match3Logic.GenerateGridData(_width, _height, _datas);
        _grid.SetData(data);

        _model = new AnimationModel(_grid, _datas);
        _animationPresenter = new AnimationPresenter(_view, _model, _config);

        ShowDebugLine();
        ShowDebugText();

        await Init();

        await UniTask.Delay(1000);

        await FirstCheck();
        _state = BoardState.Idle;
    }

    void Update()
    {
        if (_state != BoardState.Idle)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _clickedPosition = UnityUltilities.GetMousePosition();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            ProcessInput().Forget();
        }
    }


    private async UniTask FirstCheck()
    {
        MatchFinalResult mfr = Match3Logic.FindMatches(_grid);
        await ResolveCascade(mfr);
    }

    private async UniTask ProcessInput()
    {
        _state = BoardState.Running;

        if(!TryGetSwapObjects(out GridObject clickedObj, out GridObject targetObj))
        {
            _state = BoardState.Idle;
            return;
        }
            
        await TrySwap(clickedObj, targetObj);
        await Hint();
        _state = BoardState.Idle;
    }

    private async UniTask Hint()
    {
        CustomizedGrid<GridObject> clone = _grid.Clone();
        BestMove move = Match3Logic.FindBestMove(clone);

        foreach (var item in move.Result)
        {
            move.Objects.Add(_grid.Get(item));    
        }
        await _animationPresenter.Hint(move.Objects);
    }

    private async UniTask ResolveCascade(MatchFinalResult mfr)
    {
        int cascade = 0;

        while (mfr.HasMatches)
        {
            await ResolveMatch(mfr);
            cascade++;

            mfr = Match3Logic.FindMatches(_grid);

            await UniTask.Delay(200);
        }
    }

    private async UniTask ResolveMatch(MatchFinalResult mfr)
    {
        Match3Logic.RemoveMatches(_grid, mfr);
        List<FallResult> fallResults = Match3Logic.ApplyGravity(_grid);
        List<RefillablePositionData> refillablePosition = Match3Logic.FindRefillablePosition_2(_grid);
        RefillResult refillResult = Match3Logic.Fill(_grid, refillablePosition, 0, _datas.Count);

        await _animationPresenter.Remove(mfr.MatchedObjs);
        _animationPresenter.Fall(fallResults).Forget();
        await _animationPresenter.Refill(refillResult);
    }

    private async UniTask TrySwap(GridObject clickedObj, GridObject targetObj)
    {
        SwapResult swapResult = Match3Logic.Swap(_grid, clickedObj.GridPosition, targetObj.GridPosition);

        if (!swapResult.IsSuccess)
            return;

        await _animationPresenter.Swap(swapResult.FirstObject, swapResult.SecondObject);
        MatchFinalResult matchFinalResult = Match3Logic.FindMatches(_grid);

        if (!matchFinalResult.HasMatches)
        {
            swapResult = Match3Logic.Swap(_grid, clickedObj.GridPosition, targetObj.GridPosition);
            await _animationPresenter.Swap(swapResult.FirstObject, swapResult.SecondObject);
            return;
        }

        await ResolveCascade(matchFinalResult);
    }

    private bool TryGetSwapObjects(out GridObject clickedObj, out GridObject targetObj)
    {
        targetObj = null;
        clickedObj = null;

        if (!_grid.TryGet(_grid.WorldToGrid(_clickedPosition), out clickedObj) || clickedObj == null)
            return false;

        _dragDirection = UnityUltilities.GetMousePosition() - _clickedPosition;
        GridPosition offset = GridOffset.GetFromVector(_dragDirection);

        if (_dragDirection.magnitude <= _cellSize / 2 || offset == GridOffset.Zero)
            return false;

        if (!_grid.TryGet(clickedObj.GridPosition + offset, out targetObj) || targetObj == null)
            return false;

        return true;
    }

    private async UniTask Init()
    {
        List<GridObject> objs = _grid.GetAllValue();
        List<SpawnData> spawnData = new List<SpawnData>();
        List<FallResult> fallDatas = new List<FallResult>();
        foreach (var obj in objs)
        {
            spawnData.Add(
                new SpawnData()
                {
                    GridObject = obj,
                    Position = obj.GridPosition + new GridPosition(0, _grid.Rows),
                    ObjectType = obj.ItemType,
                }
            );

            fallDatas.Add(
                new FallResult()
                {
                    GridObject = obj,
                    TargetPosition = obj.GridPosition,
                }    
            );
        }

        _animationPresenter.Spawn(spawnData);
        await _animationPresenter.Fall(fallDatas);
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
                GridObject obj = _grid.Get(new GridPosition(i, j));
                if (obj == null)
                    continue;
                Vector3 position = _grid.GridToWorld(obj.GridPosition);
                _debugTexts[i, j] = UnityUltilities.CreateWorldText(obj.GetDebugText(), position, 6, Color.white);
            }
        }
    }

    private void HandleValueChanged(GridPosition gridPosition)
    {
        if (_debugTexts[gridPosition.Column, gridPosition.Row] == null)
            return;

        if (_grid.Get(gridPosition) == null)
        {
            _debugTexts[gridPosition.Column, gridPosition.Row].text = "null";
            return;
        }

        _debugTexts[gridPosition.Column, gridPosition.Row].text = _grid.Get(gridPosition).GetDebugText();
    }
}
