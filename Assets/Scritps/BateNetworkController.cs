using System.Collections;
using DG.Tweening;
using Photon;
using UnityEngine;

/// <summary>
/// Handles bate controlling over network
/// </summary>
public class BateNetworkController : PunBehaviour {
    [SerializeField] float _bateSpeed;
    float _networkLerpSpeed = 9;
    float _bateWidth;
    float _sideLimit;

    Camera _camera;
    SpriteRenderer _renderer;

    Vector2 _movingPosition;
    Quaternion _bateRotation;

    void Awake() {
        _camera = Camera.main;
        _renderer = GetComponent<SpriteRenderer>();
        _movingPosition.y = transform.position.y;
    }

    void Start() {
        SetupPlayerInfo();
        StartCoroutine(IndicatePlayer());
    }

    void Update() {
        MoveBates();
    }

    /// <summary>
    /// Allows us to change bate x scale to any
    /// </summary>
    void SetupPlayerInfo() {
        _bateWidth = _renderer.bounds.size.magnitude;
        _sideLimit = GameController.Instance.HorizontalBounds.Right - _bateWidth / 2;
    }

    /// <summary>
    /// We can use GetMouseButton because game needs only one touch
    /// </summary>
    void GetInput() {
        if (Input.GetMouseButton(0)) {
            var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            var xPos = Mathf.Clamp(mousePosition.x, -_sideLimit, _sideLimit);
            _movingPosition = new Vector2(xPos, transform.position.y);
        }
    }

    void MoveBates() {
        if (photonView.isMine)
            GetInput();
        else {
            transform.rotation =
                Quaternion.Slerp(transform.rotation, _bateRotation, _networkLerpSpeed * Time.deltaTime);
        }

        transform.position = Vector2.Lerp(transform.position, _movingPosition, _bateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Waiting a second because wnership transfering takes some time
    /// </summary>
    /// <returns></returns>
    IEnumerator IndicatePlayer(){
        yield return new WaitForSeconds(1);
        if(!photonView.isMine)
            yield break;
        _renderer.DOColor(Color.white, 0.5f).SetLoops(6, LoopType.Yoyo);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(_movingPosition);
            stream.SendNext(transform.rotation);
        } else {
            if (!photonView.isMine) {
                _movingPosition = (Vector2) stream.ReceiveNext();
                _bateRotation = (Quaternion) stream.ReceiveNext();
            }
        }
    }
}