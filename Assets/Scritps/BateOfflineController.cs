using UnityEngine;

/// <summary>
/// Handles bate controlling in single game
/// </summary>
public class BateOfflineController : MonoBehaviour {
    [SerializeField] float _bateSpeed;
    Transform _playerBates;
    float _bateWidth;
    float _sideLimit;

    Camera _camera;
    Vector2 _movingPosition;

    void Awake() {
        _camera = Camera.main;
        _playerBates = transform;
    }

    void Start() {
        SetupPlayerInfo();
    }

    void Update() {
        MoveBates();
    }
    
    /// <summary>
    /// We can use GetMouseButton because game needs only one touch
    /// </summary>
    void GetInput() {
        if (Input.GetMouseButton(0)) {
            var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            var xPos = Mathf.Clamp(mousePosition.x, -_sideLimit, _sideLimit);
            _movingPosition = new Vector2(xPos, 0);
        }
    }

    /// <summary>
    /// Allows us to change bate x scale to any
    /// </summary>
    void SetupPlayerInfo() {
        var renderer = _playerBates.GetComponentInChildren<SpriteRenderer>();
        _bateWidth = renderer.bounds.size.magnitude;
        _sideLimit = GameController.Instance.HorizontalBounds.Right - _bateWidth / 2;
    }

    void MoveBates() {
        GetInput();
        _playerBates.position = Vector2.Lerp(_playerBates.position, _movingPosition, _bateSpeed * Time.deltaTime);
    }
}