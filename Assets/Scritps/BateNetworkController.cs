using Photon;
using UnityEngine;

public class BateNetworkController : PunBehaviour {
    [SerializeField] float _bateSpeed;
    float _networkLerpSpeed = 9;
    float _bateWidth;
    float _sideLimit;

    Camera _camera;

    Vector2 _movingPosition;
    Quaternion _bateRotation;

    void Awake() {
        _camera = Camera.main;
        _movingPosition.y = transform.position.y;
    }

    void Start() {
        SetupPlayerInfo();
    }

    void Update() {
        MoveBates();
    }

    void SetupPlayerInfo() {
        var renderer = transform.GetComponentInChildren<SpriteRenderer>();
        _bateWidth = renderer.bounds.size.magnitude;
        _sideLimit = GameController.Instance.HorizontalBounds.Right - _bateWidth / 2;
    }

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