using DG.Tweening;
using UnityEngine;

/// <summary>
/// Handles offline ball logic
/// </summary>
public class OfflineBall : Ball {
    void OnEnable() {
        LaunchBall();
    }

    void OnBecameInvisible() {
        HideBall();
    }

    void LaunchBall() {
        SetupBallProperties();

        var seq = DOTween.Sequence();
        seq.Append(transform.DOShakePosition(1f, Vector3.one * 0.2f).SetEase(Ease.OutElastic));
        seq.AppendCallback(() => {
                               var forceVector = new Vector2(Random.Range(-1f, 1), Random.Range(-1f, 1)).normalized;
                               _rb.AddForce(forceVector * _ballSpeed, ForceMode2D.Impulse);
                           });
    }
}