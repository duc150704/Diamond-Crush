using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSystem : MonoBehaviour
{
    [SerializeField] private float _fallDownTime;
    [SerializeField] private float _swapTime;
    [SerializeField] private float _removeTime;

    [SerializeField] private GameObject _pref;

    private Dictionary<GridObject, DiamondVisual> _visualMap = new();
    private Dictionary<int, DiamondSO> _dataMap = new();

    private CustomizedGrid<GridObject> _grid;

    public void SetUp(CustomizedGrid<GridObject> grid, List<DiamondSO> datas)
    {
        _grid = grid;
        DataMapping(datas);
    }

    public async UniTask Refill(RefillResult data)
    {
        await Spawn(data.SpawnData);
        await FallDown(data.FallData);
    }

    public async UniTask FallDown(List<FallResult> fallDatas)
    {
        List<UniTask> uniTasks = new List<UniTask>();

        foreach (var item in fallDatas)
        {
            uniTasks.Add(_visualMap[item.GridObject].Move(_grid.GridToWorld(item.TargetPosition), _fallDownTime, Ease.InSine).ToUniTask());

        }
        await UniTask.WhenAll(uniTasks);
    }

    public async UniTask Remove(MatchFinalResult matchFinalResult)
    {
        List<UniTask> tasks = new List<UniTask>();
        foreach (var item in matchFinalResult.MatchedObjs)
        {
            tasks.Add( _visualMap[item].Scale(Vector3.zero, _removeTime, Ease.InBack).ToUniTask());
        }

        await UniTask.WhenAll(tasks);
        foreach (var item in matchFinalResult.MatchedObjs) 
        {
            Destroy(_visualMap[item].gameObject);
            _visualMap.Remove(item);
        }
    }

    public async UniTask Swap(SwapResult swapResult)
    {
        await UniTask.WhenAll(
            _visualMap[swapResult.FirstObject].Move(_grid.GridToWorld(swapResult.FirstObject.GridPosition), _swapTime).ToUniTask(),
            _visualMap[swapResult.SecondObject].Move(_grid.GridToWorld(swapResult.SecondObject.GridPosition), _swapTime).ToUniTask()
            );
    }

    public async UniTask Spawn(List<SpawnData> spawnDatas)
    {
        foreach (var item in spawnDatas)
        {
            GameObject go = Instantiate(_pref, _grid.GridToWorld(item.Position), Quaternion.identity);
            _visualMap[item.GridObject] = go.GetComponent<DiamondVisual>();
            _visualMap[item.GridObject].SetData(_dataMap[item.GridObject.ItemType]);
            _visualMap[item.GridObject].Spawn(_grid.GridToWorld(item.Position));
        }
        await UniTask.Yield();
    }

    private void DataMapping(List<DiamondSO> datas)
    {
        for(int i = 0; i < datas.Count; i++)
        {
            _dataMap[i] = datas[i];
        }
    }

}
