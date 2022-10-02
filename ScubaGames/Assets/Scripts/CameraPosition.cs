using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CameraPosition : MonoBehaviour
{
    public Ease _cameraAnimation = Ease.InSine;

    private float _speed = .1f;

    private Camera _camera;
    private CameraPath _cameraPath;
    private GameObject _player;
    private PlayerController _playerController;

    private void Awake()
    {
        FindObjects();
    }

    private void Start()
    {
        StartCoroutine(SetCameraPosition());
    }

    private IEnumerator SetCameraPosition()
    {
        if (_playerController != null && _cameraPath != null)
            while(true)
            {
                MoveCamera();
                yield return new WaitUntil(() => _playerController._isWalking);
            }
    }

    private void FindObjects()
    {
        _camera ??= Camera.main;
        _cameraPath ??= FindObjectOfType<CameraPath>();
        _player ??= GameObject.Find("Player");
        _playerController ??= _player != null ? _player.GetComponent<PlayerController>() : FindObjectOfType<PlayerController>();
    }

    private void MoveCamera()
    {
        if (_playerController == null && _cameraPath == null) return;

        for (byte a = 0; a < _cameraPath._cameraPaths.Count; a++)
        {
            for (byte b = 0; b < _cameraPath._cameraPaths[a]._referenceBlocks.Count; b++)
            {
                if (_playerController._clickedPosition == _cameraPath._cameraPaths[a]._referenceBlocks[b])
                    FollowPath(_cameraPath._cameraPaths[a]._sphereGizmosPosition);
            }
        }
    }

    private void FollowPath(Vector3 destination)
    {
        Sequence sq = DOTween.Sequence();
        sq.Append(_camera.transform.DOMove(destination, SetSpeed()).SetEase(_cameraAnimation));
        DOTween.SetTweensCapacity((int)SetSpeed(), 5000);
    }

    private float SetSpeed()
    {        
        return _playerController != null ? _speed * _playerController._path.Count : _speed;
    }
}
