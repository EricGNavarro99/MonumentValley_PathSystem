using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Space, Header("Speed:")]
    [Range(0f, 5f)] public float _speed = 1f;
    [Range(0f, 5f)] public float _stairSpeed = 1.5f;

    [Space]
    public Ease _playerAnimationMovement;

    [HideInInspector] public bool _isWalking = false;

    [Space]
    [SerializeField] private Transform _currentPosition;
    [SerializeField] private Transform _clickedPosition;

    [Space, SerializeField] private List<Transform> _finalPath = new List<Transform>();

    private void Start()
    {
        StartStopCoroutine(true, "PositionCoroutine");
        StartStopCoroutine(true, "RaycastCoroutine");
    }

    IEnumerator PositionCoroutine()
    {
        while (true)
        {
            bool clickedButton = Input.GetMouseButtonDown(0);
            SelectedPositionDetecter(clickedButton);
            clickedButton = false;
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }
    }

    IEnumerator RaycastCoroutine()
    {
        while (true)
        {
            RaycastDown();
            yield return new WaitUntil(() => _isWalking);
        }
    }

    public void StartStopCoroutine(bool start, string coroutineName)
    {
        if (start) StartCoroutine(coroutineName);
        else StopCoroutine(coroutineName);
    }

    private void RaycastDown()
    {
        Ray playerRay;
        RaycastHit playerHit;

        if (CheckChild()) playerRay = new Ray(transform.GetChild(0).position, -transform.up);
        else return;

        if (Physics.Raycast(playerRay, out playerHit))
            if (playerHit.transform.GetComponent<PathSystem>() != null) _currentPosition = playerHit.transform;

        transform.parent = CheckGroundMotion();
    }

    private void SelectedPositionDetecter(bool clickedButton)
    {
        if (_isWalking) return;

        if (clickedButton)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;

            if (Physics.Raycast(mouseRay, out mouseHit))
                if (mouseHit.transform.GetComponent<PathSystem>() != null)
                {
                    _clickedPosition = mouseHit.transform;
                    _finalPath.Clear();
                    FindPath();
                }
        }
    }

    private void FindPath()
    {
        List<Transform> nextPositions = new List<Transform>();
        List<Transform> pastPositions = new List<Transform>();

        foreach (WalkPath path in _currentPosition.GetComponent<PathSystem>()._possiblePaths)
        {
            if (path._active)
            {
                nextPositions.Add(path._target);
                path._target.GetComponent<PathSystem>()._previousPosition = _currentPosition;
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
            _finalPath.Add(position);
            if (position.GetComponent<PathSystem>()._previousPosition != null)
                position = position.GetComponent<PathSystem>()._previousPosition;
            else return;
        }
        FollowPath();
    }

    private void FollowPath()
    {
        Sequence sq = DOTween.Sequence();

        _isWalking = true;

        for (int i = _finalPath.Count - 1; i > 0; i--)
        {
            sq.Append(transform.DOMove(_finalPath[i].GetComponent<PathSystem>().GetWalkPoint() + transform.up / 2,
                .2f * SetPlayerSpeed(_finalPath[i].GetComponent<PathSystem>()._isStair)).SetEase(_playerAnimationMovement));
        }

        sq.Append(transform.DOMove(_clickedPosition.GetComponent<PathSystem>().GetWalkPoint() + transform.up / 2,
            .2f * SetPlayerSpeed(_clickedPosition.GetComponent<PathSystem>()._isStair)).SetEase(_playerAnimationMovement));

        sq.AppendCallback(() => Clear());
    }

    private void Clear()
    {
        foreach (Transform tr in _finalPath) tr.GetComponent<PathSystem>()._previousPosition = null;

        _finalPath.Clear();
        _isWalking = false;
    }

    private void ExplorePositions(List<Transform> nextPositions, List<Transform> pastPositions)
    {
        Transform current = nextPositions.First();
        nextPositions.Remove(current);

        if (current == _clickedPosition) return;

        foreach (WalkPath path in current.GetComponent<PathSystem>()._possiblePaths)
        {
            if (!pastPositions.Contains(path._target) && path._active)
            {
                nextPositions.Add(path._target);
                path._target.GetComponent<PathSystem>()._previousPosition = current;
            }
        }

        pastPositions.Add(current);

        if (nextPositions.Any()) ExplorePositions(nextPositions, pastPositions);
    }
    
    private bool CheckChild() => transform.childCount > 0;

    private Transform CheckGroundMotion() => _currentPosition.GetComponent<PathSystem>()._movableBlock ? _currentPosition.parent : null;

    private float SetPlayerSpeed(bool isStair) => (!isStair) ? _speed : _stairSpeed;
}