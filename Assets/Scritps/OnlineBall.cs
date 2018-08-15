using System;

using UnityEngine;

using DG.Tweening;

using Photon;

using Random = UnityEngine.Random;

public class OnlineBall : Ball {

    void OnEnable() {
        if (PhotonNetwork.isMasterClient)
            LaunchBall();
    }

    void OnBecameInvisible() {
        if (PhotonNetwork.isMasterClient && gameObject.activeSelf)
            photonView.RPC("HideBallRPC", PhotonTargets.All);
    }

    void LaunchBall() {
        photonView.RPC("SetupBallPropertiesRPC", PhotonTargets.All, Random.Range(0, int.MaxValue));

        var seq = DOTween.Sequence();
        seq.Append(transform.DOShakePosition(1f, Vector3.one * 0.2f).SetEase(Ease.OutElastic));
        seq.AppendCallback(() => {
                               var forceVector = new Vector2(Random.Range(-1f, 1), Random.Range(-1f, 1)).normalized;
                               _rb.AddForce(forceVector * _ballSpeed, ForceMode2D.Impulse);
                           });
    }

    [PunRPC]
    void SetupBallPropertiesRPC(int seed) {
        Random.InitState(seed);
        SetupBallProperties();
    }

    [PunRPC]
    protected void HideBallRPC() {
        HideBall();
    }
}