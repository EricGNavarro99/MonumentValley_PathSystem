using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SelectedBlockColor : MonoBehaviour
{
    private Material _material;

    [Space]
    public Color _currentColor;
    public Color _selectedColor;

    [Space]
    public float _effectDuration = .2f;
    public Ease _colorAnimation;

    private void Awake()
    {
        _material ??= GetComponent<Renderer>().material;
    }

    public IEnumerator ChangeMaterialColor()
    {
        _material.DOColor(_selectedColor, _effectDuration).SetEase(_colorAnimation);
        yield return new WaitForSecondsRealtime(_effectDuration);
        _material.DOColor(_currentColor, _effectDuration).SetEase(_colorAnimation);
    }
}
