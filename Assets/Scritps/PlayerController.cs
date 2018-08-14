using UnityEngine;

public class PlayerController : MonoBehaviour{
    [SerializeField] float _bateSpeed;
    Transform _playerBates;
    float _bateWidth;
    float _sideLimit;

    Camera _camera;

    void Awake(){
        _camera = Camera.main;
        _playerBates = transform;
        UIController.GameEnded += OnGameEnded;
    }

    void OnGameEnded() {
        Destroy(gameObject);
    }

    void Start(){
        SetupPlayerInfo();
    }

    void Update(){
        if (Input.GetMouseButton(0)){
            MoveBates();
        }
    }

    void SetupPlayerInfo(){
        var renderer = _playerBates.GetComponentInChildren<SpriteRenderer>();
        _bateWidth = renderer.bounds.size.magnitude;
        _sideLimit = GameController.Instance.HorizontalBounds.Right - _bateWidth/2;
    }

    void MoveBates(){
        var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        var xPos = Mathf.Clamp(mousePosition.x, -_sideLimit, _sideLimit);
        var movePosition = new Vector2(xPos, 0);
        _playerBates.position = Vector2.Lerp(_playerBates.position, movePosition, _bateSpeed * Time.deltaTime);
    }
}