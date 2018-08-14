using System;

using UnityEngine;

using DG.Tweening;

using Photon;

using Random = UnityEngine.Random;

public class Ball : PunBehaviour {
    public event Action BallDissapared;

    [Header("Speed")] [SerializeField] float _minSpeed;
    [SerializeField] float _maxSpeed;
    [Header("Size")] [SerializeField] float _minSize;
    [SerializeField] float _maxSize;
    [Header("Color")] [SerializeField] float _minHue;
    [SerializeField] float _maxHue, _minSaturation, _maxSaturation, _minValue, _maxValue;

    SpriteRenderer _renderer;
    Rigidbody2D _rb;

    Color _ballColor;
    float _ballSpeed;
    float _ballSize;

    void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable() {
        if (PhotonNetwork.isMasterClient)
            LaunchBall();
    }

    void OnBecameInvisible() {
        if (PhotonNetwork.isMasterClient)
            photonView.RPC("OnBallDissapeared", PhotonTargets.All);
    }

    void LaunchBall() {
        photonView.RPC("SetupBallProperties", PhotonTargets.All, Random.Range(0, int.MaxValue));

        var seq = DOTween.Sequence();
        seq.Append(transform.DOShakePosition(1f, Vector3.one * 0.2f).SetEase(Ease.OutElastic));
        seq.AppendCallback(() => {
                               var forceVector = new Vector2(Random.Range(-1f, 1), Random.Range(-1f, 1)).normalized;
                               _rb.AddForce(forceVector * _ballSpeed, ForceMode2D.Impulse);
                               photonView.RPC("SetPredictionSpeed", PhotonTargets.All, _rb.velocity.magnitude);
                           });
    }

    [PunRPC]
    void SetupBallProperties(int seed) {
        Random.InitState(seed);
        _rb.velocity = Vector2.zero;

        _ballSpeed = Random.Range(_minSpeed, _maxSpeed);
        _ballSize = Random.Range(_minSize, _maxSize);
        _ballColor = Random.ColorHSV(_minHue, _maxHue, _minSaturation, _maxSaturation, _minValue, _maxValue);

        transform.localScale = Vector3.one * _ballSize;
        _renderer.color = _ballColor;
    }

    [PunRPC]
    void SetPredictionSpeed(float speed) {
        GetComponent<PhotonTransformView>().m_PositionModel.InterpolateLerpSpeed = speed;
    }

    [PunRPC]
    void OnBallDissapeared() {
        gameObject.SetActive(false);
        BallDissapared?.Invoke();
    }
}