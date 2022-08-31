using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class WalkPath
{
    public Transform _target;
    public bool _active = false;
}
public class PathSystem : MonoBehaviour
{
    public List<WalkPath> _possiblePaths = new List<WalkPath>();
}
