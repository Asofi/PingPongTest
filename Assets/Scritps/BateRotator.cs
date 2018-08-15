using UnityEngine;

/// <summary>
/// Rotates bate to position-dependent angle
/// </summary>
public class BateRotator : MonoBehaviour {
    [SerializeField] float _angleAmplitude = 15f;
    [SerializeField] bool _isUpperBate;

    void Update() {
        RotateBate();
    }

    /// <summary>
    /// Small rotation prevents ball stacking between bates. Also it looks pretty :)
    /// </summary>
    void RotateBate() {
        var halfScreenWidth = GameController.Instance.HorizontalBounds.Right;
        transform.localRotation =
            Quaternion.Euler(0, 0, transform.position.x / halfScreenWidth * _angleAmplitude * (_isUpperBate ? -1 : 1));
    }
}