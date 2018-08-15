using DG.Tweening;
using Photon;
using UnityEngine;

/// <summary>
/// Handles base game logic
/// </summary>
public class GameController : PunBehaviour {
    public static GameController Instance;

    [Header("Game Setup")] [SerializeField]
    GameModes _gameMode;
    [SerializeField] Transform _offlinePlayerPrefab;
    [SerializeField] Transform _onlinePlayerPrefab;
    Transform _offlinePlayer;
    Transform _onlinePlayer;

    [Header("Ball Setup")] [SerializeField]
    Transform _offlineBallPrefab;
    [SerializeField] Transform _onlineBallPrefab;
    [SerializeField] Transform _ballSpawnPoint;
    Transform _activeBall;

    [Header("Board Setup")] [SerializeField]
    Transform _leftCollider;
    [SerializeField] Transform _rightCollider;
    [SerializeField] float _onlineBoardOffset = 5;
    Vector2 _leftBoardLimit;
    Vector2 _rightBoardLimit;
    // x - left border, y - right border
    Vector2 _horizontalBounds;

    public struct BoardBounds {
        public readonly float Left, Right;

        public BoardBounds(float left, float right) {
            Left = left;
            Right = right;
        }
    }

    public BoardBounds HorizontalBounds { get; private set; }

    #region Unity Messages

    void Awake(){
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        UIController.GameStarted += OnGameStarted;
        UIController.GameEnded += OnGameEnded;
        LobbyController.SessionStarted += OnSessionStarted;
    }

    #endregion

    #region Event Handlers

    void OnGameEnded() {
        Ball.BallDissapeared -= OnBallDissapared;
        Destroy(_activeBall.gameObject);

        if (_gameMode == GameModes.Offline) {
            if (_offlinePlayer != null)
                Destroy(_offlinePlayer.gameObject);
        } else {
            if (_onlinePlayer != null)
                Destroy(_onlinePlayer.gameObject);
        }

        _gameMode = GameModes.Menu;
        _activeBall = null;

        if (PhotonNetwork.connected)
            PhotonNetwork.Disconnect();
    }

    void OnSessionStarted() {
        SetupOnlineGame();
    }

    void OnGameStarted(GameModes mode) {
        _gameMode = mode;
        SetupBorders();
        if (_gameMode != GameModes.Offline)
            return;

        SetupOfflineGame();
    }

    void OnBallDissapared() {
        if (_gameMode == GameModes.Offline)
            SetupBall();
        else {
            photonView.RPC("SetupBall", PhotonTargets.All);
        }
    }

    #endregion

    #region Project Methods

    void SetupOfflineGame() {
        _offlinePlayer = Instantiate(_offlinePlayerPrefab);
        SetupBall();
    }

    void SetupOnlineGame() {
        if (!PhotonNetwork.isMasterClient)
            return;

        _onlinePlayer = PhotonNetwork
            .Instantiate(_onlinePlayerPrefab.name, _onlinePlayerPrefab.position, Quaternion.identity, 0)
            .transform;

        SetupBall();
    }

    /// <summary>
    /// The Offline setup allows us to play on any wild screen
    /// The Online setup is placing borders at fixed positions 
    /// </summary>
    void SetupBorders() {
        if (_gameMode == GameModes.Offline) {
            var cam = Camera.main;
            var leftBound = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            var rightBound = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

            HorizontalBounds = new BoardBounds(leftBound, rightBound);

            _leftCollider.DOMoveX(HorizontalBounds.Left, 0.25f);
            _rightCollider.DOMoveX(HorizontalBounds.Right, 0.25f);
        } else {
            _leftCollider.position = new Vector2(-_onlineBoardOffset, 0);
            _rightCollider.position = new Vector2(_onlineBoardOffset, 0);
            HorizontalBounds = new BoardBounds(-_onlineBoardOffset, _onlineBoardOffset);
        }
    }

    #endregion

    #region RPCs

    /// <summary>
    /// Spawns ball depends on GameMode and then respawn it after dissapearing
    /// </summary>
    [PunRPC]
    void SetupBall() {
        if (_gameMode == GameModes.Menu)
            return;

        if (_activeBall == null) {
            if (_gameMode == GameModes.Offline)
                _activeBall = Instantiate(_offlineBallPrefab, _ballSpawnPoint.position, Quaternion.identity);
            else if (PhotonNetwork.isMasterClient) {
                _activeBall =
                    PhotonNetwork.Instantiate(_onlineBallPrefab.name, _ballSpawnPoint.position, Quaternion.identity, 0)
                        .transform;

                photonView.RPC("Init", PhotonTargets.Others, _activeBall.gameObject.GetPhotonView().viewID);
            }

            Ball.BallDissapeared += OnBallDissapared;
        } else {
            _activeBall.position = _ballSpawnPoint.position;
            _activeBall.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Sends _activeBall id to other players
    /// </summary>
    /// <param name="activeBallViewId">_activeBall's id</param>
    [PunRPC]
    void Init(int activeBallViewId) {
        _activeBall = PhotonView.Find(activeBallViewId).transform;
    }

    #endregion


}