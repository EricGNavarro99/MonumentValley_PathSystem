using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class WalkPath
{
    public Transform _target;
    public bool _active = false;
}

public class PlayerPath : MonoBehaviour
{
    [Space] public List<WalkPath> _possiblePaths = new List<WalkPath>();

    [Space] public Transform _previousPosition;

    [Space]
    public bool _movableBlock = false;
    public bool _isStair = false;
    public bool _playerCanRotate = false;

    [Space] public float _walkPointOffset = .5f;

    [Space, Header("Gizmos Options:")]
    public Color _sphereGizmosColor = Color.black;
    public float _sphereGizmosRadius = .1f;
    public Color _linearGizmosColor = Color.black;

    private void OnDrawGizmos()
    {
        Gizmos.color = _sphereGizmosColor;
        Gizmos.DrawSphere(GetWalkPoint(), _sphereGizmosRadius);

        if (_possiblePaths == null) return;

        foreach (WalkPath paths in _possiblePaths)
        {
            if (paths._target == null) return;

            Gizmos.color = paths._active ? _linearGizmosColor : Color.clear;
            Gizmos.DrawLine(GetWalkPoint(), paths._target.GetComponent<PlayerPath>().GetWalkPoint());
        }
    }

    public Vector3 GetWalkPoint() => transform.position + transform.up * _walkPointOffset;
}