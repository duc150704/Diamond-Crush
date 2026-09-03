using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class AnimationPresenter
{
    private AnimationView _view;
    private AnimationModel _model;
    private AnimationConfig _config;

    public AnimationPresenter(AnimationView view, AnimationModel model, AnimationConfig config)
    {
        _view = view;
        _model = model;
        _config = config;
    }

    public async UniTask Hint(List<GridObject> objs)
    {
        await _view.Scale(objs, _config.HintScale, _config.HintTime / 2, _config.HintEase);
        await _view.Scale(objs, _config.NormalScale, _config.HintTime / 2, _config.HintEase);
    }

    public async UniTask Refill(RefillResult data)
    {
        Spawn(data.SpawnData);
        await Fall(data.FallData);
    }

    public async UniTask Fall(List<FallResult> data)
    {
        List<UniTask> tasks = new();
        foreach (var item in data) 
        {
            tasks.Add(
                _view.Move(
                    item.GridObject, 
                    _model.Grid.GridToWorld(item.TargetPosition), 
                    _config.FallTime, 
                    _config.FallEase
                ).ToUniTask()
            );
        }

        await UniTask.WhenAll(tasks);
    }

    public async UniTask Remove(List<GridObject> objs)
    {
        await _view.Scale(objs, _config.DisapearScale, _config.DisapearTime, _config.DisapearEase);
        _view.DestroyAndUnMap(objs);
    }

    public async UniTask Swap(GridObject firstObj, GridObject secondObj)
    {
        await UniTask.WhenAll(
            _view.Move(firstObj, _model.Grid.GridToWorld(firstObj.GridPosition), _config.SwapTime, _config.SwapEase).ToUniTask(),
            _view.Move(secondObj, _model.Grid.GridToWorld(secondObj.GridPosition), _config.SwapTime, _config.SwapEase).ToUniTask()
        );

    }

    public void Spawn(List<SpawnData> data)
    {
        foreach (var item in data)
        {
            DiamondVisual visual = _view.CreateAndMap(item.GridObject, _model.Grid.GridToWorld(item.Position));
            _view.SetData(item.GridObject, _model.ObjectData[item.ObjectType]);       // tam
        }
    }

}
