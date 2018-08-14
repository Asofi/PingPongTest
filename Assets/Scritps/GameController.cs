using DG.Tweening;

using Photon;

using UnityEngine;

public class GameController : PunBehaviour{

    public static GameController Instance;

    [Header("Game Setup")]
    [SerializeField] GameModes _gameMode;
    [SerializeField] Transform _offlinePlayer;
    [SerializeField] Transform _onlinePlayer;
    
    [Header("Ball Setup")] 
    [SerializeField] Transform _ballPrefab;
    [SerializeField] Transform _ballSpawnPoint;
    Transform _activeBall;

    [Header("Board Setup")] 
    [SerializeField] Transform _leftCollider;
    [SerializeField] Transform _rightCollider;
    [SerializeField] float _onlineBoardOffset = 5;
    Vector2 _leftBoardLimit;
    Vector2 _rightBoardLimit;
    // x - left border, y - right border
    Vector2 _horizontalBounds;
    
    public struct BoardBounds{
        public readonly float Left, Right, Up, Bot;

        public BoardBounds(float left, float right, float up, float bot){
            Left = left;
            Right = right;
            Up = up;
            Bot = bot;
        }
    }

    public BoardBounds HorizontalBounds{ get; private set; }

    void Awake(){
        if(Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;

        UIController.GameStarted += OnGameStarted;
        UIController.GameEnded += OnGameEnded;
        LobbyController.SessionStarted += OnSessionStarted;
    }

    void OnGameEnded() {
        Destroy(_activeBall);
        _activeBall = null;
    }

    void OnSessionStarted() {
        SetupOnlineGame();
    }

    void OnGameStarted(GameModes mode){
        _gameMode = mode;   
        SetupBorders();
        if(_gameMode!= GameModes.Offline)
            return;
        
        SetupOfflineGame();
    }

    void SetupOfflineGame(){
        Instantiate(_offlinePlayer);
        SetupBall();
    }

    void SetupOnlineGame() {
        if(!PhotonNetwork.isMasterClient)
            return;
        PhotonNetwork.Instantiate(_onlinePlayer.name, _onlinePlayer.position, Quaternion.identity, 0);
        SetupBall();
    }

    /// <summary>
    /// Offline setup allow us to play on any wild screen
    /// Online setup is placing borders at fixed positions 
    /// </summary>
    void SetupBorders(){
        if (_gameMode == GameModes.Offline) {
            var cam = Camera.main;
            var leftBound = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            var rightBound = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        
            HorizontalBounds = new BoardBounds(leftBound, rightBound, 3, -3);

            _leftCollider.DOMoveX(HorizontalBounds.Left, 0.25f);
            _rightCollider.DOMoveX(HorizontalBounds.Right, 0.25f);
        } else {
            _leftCollider.position = new Vector2(-_onlineBoardOffset, 0);
            _rightCollider.position = new Vector2(_onlineBoardOffset, 0);
        }

    }

    [PunRPC]
    void SetupBall(){
        
        if (_activeBall == null) {

            if (_gameMode == GameModes.Offline)
                _activeBall = Instantiate(_ballPrefab, _ballSpawnPoint.position, Quaternion.identity);
            else if (PhotonNetwork.isMasterClient) {
                _activeBall =
                    PhotonNetwork.Instantiate(_ballPrefab.name, _ballSpawnPoint.position, Quaternion.identity, 0).transform;
                photonView.RPC("Init", PhotonTargets.Others, _activeBall.gameObject.GetPhotonView().viewID);
            }    
            _activeBall.GetComponent<Ball>().BallDissapared += OnBallDissapared;
        } else{
            _activeBall.position = _ballSpawnPoint.position;
            _activeBall.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    void Init(int activeBallViewId) {
        _activeBall = PhotonView.Find(activeBallViewId).transform;
    }

    void OnBallDissapared(){
        if(_gameMode == GameModes.Offline)
            SetupBall();
        else {
            photonView.RPC("SetupBall", PhotonTargets.All);
        }
    }
}