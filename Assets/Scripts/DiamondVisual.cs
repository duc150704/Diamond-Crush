using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class DiamondVisual : MonoBehaviour
{
    private DiamondSO _data;
    private Transform _transform;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Spawn(Vector3 position)
    {
        _transform.position = position;
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SetData(DiamondSO data)
    {
        _data = data;
        SetSprite(data.Sprite);
    }

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public Tween Move(Vector3 position, float duration, Ease ease = Ease.Linear)
    {
        return _transform.DOMove(position, duration).SetEase(ease);
    }

    public Tween Scale(Vector3 endValue, float duration, Ease ease = Ease.Linear)
    {
        return _transform.DOScale(endValue, duration).SetEase(ease);    
    }

    public Tween Rotate(Vector3 endValue, float duration, Ease ease = Ease.Linear)
    {
        return _transform.DORotate(endValue,duration).SetEase(ease);
    }

}
