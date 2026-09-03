using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AnimationView : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;

    private Dictionary<GridObject, DiamondVisual> _visualMap = new();

    public void SetData(GridObject obj, DiamondSO data)
    {
        DiamondVisual visual = GetVisual(obj);
        if ( visual != null)
        {
            visual.SetData(data);
            return;
        }
        Debug.Log("Khong gan duoc du lieu cho : " + obj.GridPosition.ToString());
        return;
    }

    public DiamondVisual CreateAndMap(GridObject obj, Vector3 position)
    {
        DiamondVisual visual = Create(position);
        Map(obj, visual);
        return visual;
    }

    public DiamondVisual Create(Vector3 position)
    {
        GameObject go = Instantiate(_prefab, position, Quaternion.identity);
        DiamondVisual visual = go.GetComponent<DiamondVisual>();
        return visual;
    }

    public void Map(GridObject obj, DiamondVisual visual)
    {
        _visualMap[obj] = visual;
    }

    public void DestroyAndUnMap(List<GridObject> objs)
    {
        foreach (GridObject obj in objs) 
        { 
            DestroyAndUnMap(obj);
        }
    }

    public void DestroyAndUnMap(GridObject obj)
    {
        this.Destroy(obj);
        UnMap(obj);
    }

    public void Destroy(GridObject obj)
    {
        DiamondVisual visual = GetVisual(obj);
        if (visual == null)
        {
            Debug.Log("Khong huy duoc : " + obj.ItemType);
            return;
        }

        Destroy(visual.gameObject);
    }

    public void UnMap(GridObject obj)
    {
        _visualMap.Remove(obj);
    }

    public async UniTask Scale(List<GridObject> obj, Vector3 to, float duration, Ease ease)
    {
        List<UniTask> tasks = new List<UniTask>();
        foreach (var item in obj)
        {
            tasks.Add(Scale(item, to, duration, ease).ToUniTask());
        }

        await UniTask.WhenAll(tasks);
    }

    public Tween Scale(GridObject obj, Vector3 to, float duration, Ease ease)
    {
        DiamondVisual visual = GetVisual(obj);
        if (visual == null)
        {
            Debug.Log("Khong scale duoc : " + obj.ItemType);
            return null;
        }
        return visual.Scale(to, duration, ease);
    }

    public Tween Move(GridObject obj, Vector3 to, float duration, Ease ease)
    {
        DiamondVisual visual = GetVisual(obj);
        if (visual == null) 
        {
            Debug.Log("Khong di chuyenn duoc : " + obj.ItemType);
            return null;
        }
        return visual.Move(to, duration, ease);
    }

    // ==================================================//

    private DiamondVisual GetVisual(GridObject obj) 
    {
        if (_visualMap.TryGetValue(obj, out DiamondVisual visual))
            return visual;

        Debug.Log("Khong co visual cua : " + obj.GridPosition.ToString());
        return null;
    }
}
