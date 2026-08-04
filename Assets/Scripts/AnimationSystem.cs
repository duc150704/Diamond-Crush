using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public struct SpawnData
{
    public GridObject GridObject;
    public Vector3 Position;

    public SpawnData(GridObject gridObject, Vector3 position)
    {
        GridObject = gridObject;
        Position = position;
    }
}

public class AnimationSystem : MonoBehaviour
{
    [SerializeField] private float _fallDownTime;
    [SerializeField] private float _swapTime;
    [SerializeField] private float _removeTime;

    [SerializeField] private GameObject _pref;

    private Dictionary<GridObject, DiamondVisual> _visualMap = new();
    private Dictionary<int, DiamondSO> _dataMap = new();

    private CustomizeGrid<GridObject> _grid;

    public void SetUp(CustomizeGrid<GridObject> grid, List<DiamondSO> datas)
    {
        _grid = grid;

        DataMapping(datas);
        VisualMapping(grid);
    }

    public async UniTask Remove(List<GridObject> gridObjects)
    {
        List<UniTask> tasks = new List<UniTask>();
        foreach (var item in gridObjects)
        {
            tasks.Add( _visualMap[item].Scale(Vector3.zero, _removeTime, Ease.OutBack).ToUniTask());
        }

        await UniTask.WhenAll(tasks);
        foreach (var item in gridObjects) 
        {
            _visualMap[item] = null;
        }
    }

    public async UniTask Swap(GridObject objA, GridObject objB)
    {
        if (objA == null || objB == null)
            return;

        await UniTask.WhenAll(
            _visualMap[objA].Move(_grid.GridToWorld(objB.PreviousGrid), _swapTime).ToUniTask(),
            _visualMap[objB].Move(_grid.GridToWorld(objA.PreviousGrid), _swapTime).ToUniTask()
            );
    }

    public async UniTask Spawn(List<SpawnData> spawnDatas)
    {
        foreach (var item in spawnDatas)
        {
            if (_visualMap[item.GridObject] == null)
            {
                GameObject go = Instantiate(_pref, new Vector3(15f, 15f), Quaternion.identity);
                _visualMap[item.GridObject] = go.GetComponent<DiamondVisual>();
                _visualMap[item.GridObject].SetData(_dataMap[item.GridObject.ItemID]);
            }
            _visualMap[item.GridObject].Spawn(item.Position);
        }
        await UniTask.Yield();
    }

    public async UniTask Fall(List<GridObject> objs, Ease ease = Ease.Linear)
    {
        List<UniTask> tasks = new List<UniTask>();
        foreach (var obj in objs) 
        {
            tasks.Add(_visualMap[obj].Move(_grid.GridToWorld(obj.GridPosition), _fallDownTime, ease).ToUniTask());
        }

        await UniTask.WhenAll(tasks);
    }

    private void VisualMapping(CustomizeGrid<GridObject> grid)
    {
        List<GridObject> list = grid.GetAllValue();
        foreach (GridObject obj in list) 
        {
            GameObject go = Instantiate(_pref, new Vector3(15f, 15f), Quaternion.identity);
            _visualMap[obj] = go.GetComponent<DiamondVisual>();
            _visualMap[obj].SetData(_dataMap[obj.ItemID]);
        }
    }

    private void DataMapping(List<DiamondSO> datas)
    {
        for(int i = 1; i < datas.Count; i++)
        {
            _dataMap[i] = datas[i];
        }
    }

}
