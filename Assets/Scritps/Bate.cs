using UnityEngine;

public class Bate : MonoBehaviour {
    [SerializeField] float _angleAmplitude = 15f;
    [SerializeField] bool _isUpperBate;

    void Update() {
        RotateBate();
    }

    void RotateBate() {
        var halfScreenWidth = GameController.Instance.HorizontalBounds.Right;
        transform.localRotation =
            Quaternion.Euler(0, 0, transform.position.x / halfScreenWidth * _angleAmplitude * (_isUpperBate ? -1 : 1));
    }
}