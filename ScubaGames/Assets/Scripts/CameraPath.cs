using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FollowPath
{
    public bool _active = false;

    [Space, Header("Gizmos options:")]
    public Color _sphereGizmosColor = Color.black;
    public float _sphereGizmosRadius = .1f;

    [Space] public Vector3 _sphereGizmosPosition;
    [Space] public List<Transform> _referenceBlocks;
}

public class CameraPath : MonoBehaviour
{
    public List<FollowPath> _cameraPaths = new List<FollowPath>();
    
    private void OnDrawGizmos()
    {
        if (_cameraPaths == null) return;

        for (byte a = 0; a < _cameraPaths.Count; a++)
        {
            if (!_cameraPaths[a]._active) continue;

            Gizmos.color = _cameraPaths[a]._sphereGizmosColor;
            Gizmos.DrawSphere(_cameraPaths[a]._sphereGizmosPosition, _cameraPaths[a]._sphereGizmosRadius);
        }
    }
}
