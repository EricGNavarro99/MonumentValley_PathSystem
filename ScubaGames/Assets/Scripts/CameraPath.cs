using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class FollowPath
{
    public bool _active = false;
    [Space] public float _sphereGizmosRadius = .1f;
    [Space] public Vector3 _sphereGizmosPosition;
    [Space] public List<int> _gizmosTargets;
    [Space] public List<Transform> _referenceBlocks;
}
public class CameraPath : MonoBehaviour
{
    [Space, Header("Gizmos options:")]
    public Color _sphereGizmosColor = Color.black;
    public float _sphereGizmosRadius = .1f;
    public Color _linearGizmosColor = Color.black;

    [Space] public List<FollowPath> _cameraPaths = new List<FollowPath>();
    
    private void OnDrawGizmos()
    {
        if (_cameraPaths == null) return;

        for (int a = 0; a < _cameraPaths.Count; a++)
        {
            if (!_cameraPaths[a]._active) continue;

            Gizmos.color = _sphereGizmosColor;
            Gizmos.DrawSphere(_cameraPaths[a]._sphereGizmosPosition, _cameraPaths[a]._sphereGizmosRadius);

            try
            {
                for (int b = 0; b < _cameraPaths[a]._gizmosTargets.Count; b++)
                {
                    Gizmos.color = _cameraPaths[a]._active ? _linearGizmosColor : Color.clear;
                    Gizmos.DrawLine(_cameraPaths[a]._sphereGizmosPosition, _cameraPaths[_cameraPaths[a]._gizmosTargets[b]]._sphereGizmosPosition);
                }
            }
            catch (System.ArgumentOutOfRangeException)
            { 
                Debug.LogError("Error: CameraPaths > CameraPath script > Targets in out of range.");
            }
        }
    }
}
