using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class FollowPath
{
    public bool _active = false;

    [Space, Header("Gizmos Options:")]
    public Color _sphereGizmosColor = Color.black;
    public float _sphereGizmosRadius = .1f;
    public Color _linearGizmosColor = Color.black;

    [Space] public Vector3 _spherePosition = Vector3.zero;

    [Space] public List<Transform> _targets;

    [Space] public List<Transform> _referenceBlock = new List<Transform>();
}
public class CameraPath : MonoBehaviour
{
    [Space] public List<FollowPath> _cameraPaths = new List<FollowPath>();

    private void OnDrawGizmos()
    {
        if (_cameraPaths == null) return;

        for (int a = 0; a < _cameraPaths.Count; a++)
        {
            if (_cameraPaths[a]._referenceBlock == null) return;

            Gizmos.color = _cameraPaths[a]._active ? _cameraPaths[a]._linearGizmosColor : Color.clear;

            /* Encontrar manera de acceder a todas las posiciones de target de cada list.
            for (int b = 0; b < _cameraPaths[a]._targets.Count; b++)
                Gizmos.DrawLine(_cameraPaths[a]._spherePosition, _cameraPaths[a]._targets.ToArray().GetValue(b));
            */
            
        }
    }
}
