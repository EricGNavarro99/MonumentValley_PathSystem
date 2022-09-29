using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    #region Variables:

    [Space, Header("Speed:")]
    [Range(0f, 5f)] public float _speed = 1f;
    [Range(0f, 5f)] public float _stairSpeed = 1.5f;

    [Space]
    public Ease _playerAnimation;

    [Space, SerializeField] private List<Transform> _path = new List<Transform>();

    [HideInInspector] public bool _isWalking = false;

    [HideInInspector] public Transform _currentPosition;
    private Transform _clickedPosition;

    #endregion

    private void Start()
    {
        StartCoroutine(GetNextPositon());
        StartCoroutine(GetCurrentPosition());
    }

    private IEnumerator GetNextPositon()
    {
        while (true)
        {
            bool clickedButton = Input.GetMouseButtonDown(0);
            SetNextClickedPosition(clickedButton);
            clickedButton = false;
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }
    }

    private IEnumerator GetCurrentPosition()
    {
        while (true)
        {
            SetRaycastDown();
            yield return new WaitUntil(() => _isWalking);
        }
    }

    private void SetRaycastDown()
    {
        Ray playerRay;
        RaycastHit playerHit;

        if (CheckChild()) playerRay = new Ray(transform.GetChild(0).position, -transform.up);
        else return;

        if (Physics.Raycast(playerRay, out playerHit))
            if (playerHit.transform.GetComponent<PathableBlock>() != null)
                _currentPosition = playerHit.transform;

        transform.parent = CheckGroundMotion();
    }

    private void SetNextClickedPosition(bool clickedButton = false)
    {
        if (!_isWalking && clickedButton)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;

            if (Physics.Raycast(mouseRay, out mouseHit))
                if (mouseHit.transform.GetComponent<PathableBlock>() != null)
                {
                    _clickedPosition = mouseHit.transform;
                    _path.Clear();
                    FindPath();
                }
        }
        else return;
    }

    private void FindPath()
    {
        List<Transform> nextPositions = new List<Transform>();
        List<Transform> pastPositions = new List<Transform>();

        foreach (WalkPath path in _currentPosition.GetComponent<PathableBlock>()._possiblePaths)
        {
            if (path._active)
            {
                nextPositions.Add(path._target);
                path._target.GetComponent<PathableBlock>()._previousPosition = _currentPosition;
            }
        }

        pastPositions.Add(_currentPosition);

        ExplorePositions(nextPositions, pastPositions);
        BuildPath();
    }

    private void BuildPath()
    {
        Transform position = _clickedPosition;

        while (position != _currentPosition)
        {
            _path.Add(position);
            if (position.GetComponent<PathableBlock>()._previousPosition != null)
                position = position.GetComponent<PathableBlock>()._previousPosition;
            else return;
        }

        FollowPath();
    }

    private void FollowPath()
    {
        Sequence sq = DOTween.Sequence();

        _isWalking = true;

        for (int a = _path.Count - 1; a > 0; a--)
        {
            sq.Append(transform.DOMove(_path[a].GetComponent<PathableBlock>().GetWalkPoint() + transform.up / 2,
                .2f * SetSpeed(_path[a].GetComponent<PathableBlock>()._isStair)).SetEase(_playerAnimation));
        }

        sq.Append(transform.DOMove(_clickedPosition.GetComponent<PathableBlock>().GetWalkPoint() + transform.up / 2,
            .2f * SetSpeed(_clickedPosition.GetComponent<PathableBlock>()._isStair)).SetEase(_playerAnimation));

        sq.AppendCallback(() => ClearPath());
    }

    private void ClearPath()
    {
        foreach (Transform t in _path) t.GetComponent<PathableBlock>()._previousPosition = null;

        _path.Clear();
        _isWalking = false;
    }

    private void ExplorePositions(List<Transform> nextPositions = null, List<Transform> pastPositions = null)
    {
        Transform currentPosition = nextPositions.First();
        nextPositions.Remove(currentPosition);

        if (currentPosition == _clickedPosition) return;

        foreach (WalkPath wp in currentPosition.GetComponent<PathableBlock>()._possiblePaths)
        {
            if (!pastPositions.Contains(wp._target) && wp._active)
            {
                nextPositions.Add(wp._target);
                wp._target.GetComponent<PathableBlock>()._previousPosition = currentPosition;
            }
        }

        pastPositions.Add(currentPosition);

        if (nextPositions.Any()) ExplorePositions(nextPositions, pastPositions);
    }

    private bool CheckChild()
    { 
        return transform.childCount > 0; 
    }

    private Transform CheckGroundMotion() 
    { 
        return _currentPosition.GetComponent<PathableBlock>()._movableBlock ? _currentPosition.parent : null;
    }

    private float SetSpeed(bool isStair = false)
    {
        return (!isStair) ? _speed : _stairSpeed;
    }
}