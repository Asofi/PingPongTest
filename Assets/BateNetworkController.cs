using System.Collections;
using System.Collections.Generic;

using Photon;

using UnityEngine;

public class BateNetworkController : PunBehaviour {
    [SerializeField] float _bateSpeed;
    float _bateWidth;
    float _sideLimit;

    Camera _camera;

    Vector2 _movingPosition;

    void Awake() {
        _camera = Camera.main;
        _movingPosition.y = transform.position.y;
    }

    void Start() {
        SetupPlayerInfo();
    }

    void Update() {
        if (photonView.isMine)
            GetInput();
        
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
        transform.position = Vector2.Lerp(transform.position, _movingPosition, _bateSpeed * Time.deltaTime);
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(_movingPosition);
        } else {
            _movingPosition = (Vector2) stream.ReceiveNext();
        }
    }
}