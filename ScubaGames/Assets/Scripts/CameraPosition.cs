using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CameraPosition : MonoBehaviour
{
    public float _speed = .2f;

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

    private void FindObjects()
    {
        _camera ??= Camera.main;
        _cameraPath ??= FindObjectOfType<CameraPath>();
        _player ??= GameObject.Find("Player");
        _playerController ??= _player != null ? _player.GetComponent<PlayerController>() : FindObjectOfType<PlayerController>();
    }

    private IEnumerator SetCameraPosition()
    {
        if (_playerController != null && _cameraPath != null)
            while(true)
            {
                //LookPlayer();
                MoveCamera();
                yield return new WaitUntil(() => _playerController._isWalking);
            }
    }

    private void LookPlayer()
    {
        if (_player != null && _playerController != null)
            _camera.transform.LookAt(_player.transform);
    }

    private void MoveCamera()
    {
        if (_playerController == null && _cameraPath == null) return;

        for (byte a = 0; a < _cameraPath._cameraPaths.Count; a++)
        {
            for (byte b = 0; b < _cameraPath._cameraPaths[a]._referenceBlocks.Count; b++)
            {
                if (_playerController._currentPosition == _cameraPath._cameraPaths[a]._referenceBlocks[b])
                    FollowPath(_cameraPath._cameraPaths[a]._sphereGizmosPosition, _speed);
            }
        }
    }

    private void FollowPath(Vector3 destination, float speed)
    {
        Sequence sq = DOTween.Sequence();

        sq.Append(_camera.transform.DOMove(destination, .2f * speed).SetEase(Ease.Linear));
    } // Mejorar
}
