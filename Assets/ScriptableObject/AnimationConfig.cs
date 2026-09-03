using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimConfig", menuName = "Animation Config")]
public class AnimationConfig : ScriptableObject
{
    [Header("Time")]
    public float DisapearTime;
    public float FallTime;
    public float HintTime;
    public float SwapTime;

    [Header("Scale")]
    public Vector3 NormalScale;
    public Vector3 HintScale;
    public Vector3 DisapearScale;

    [Header("Ease")]
    public Ease HintEase;
    public Ease FallEase;
    public Ease DisapearEase;
    public Ease SwapEase;
}
