using System;
using System.Collections;
using Photon;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Handles players connection
/// </summary>
public class LobbyController : PunBehaviour {
    public static event Action<int> PlayerConnected;
    public static event Action SessionStarted, PlayerDisconnected, RejectConnection;

    const float MaxConnectionTime = 30; 
    const byte MaxPlayersPerRoom = 2;
    const string GameVersion = "1";

    float _connnectionTime;

    #region Unity Messages

    void Awake() {
        SetupPhoton();
        UIController.GameStarted += OnGameStarted;
    }

    #endregion

    #region Photon Messages

    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
        PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = MaxPlayersPerRoom}, null);
    }

    public override void OnFailedToConnectToPhoton(DisconnectCause cause) {
        Debug.LogError("Cause: " + cause);
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
        PlayerConnected?.Invoke(PhotonNetwork.countOfPlayers);
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer player) {
        PlayerDisconnected?.Invoke();
    }

    public override void OnJoinedRoom() {
        StartCoroutine(WaitingForSecondPlayer());
    }

    #endregion

    #region Project Methods
    
    public void RejectConnectionManually(){
        PhotonNetwork.Disconnect();
        RejectConnection?.Invoke();
        StopAllCoroutines();
    }

    void OnGameStarted(GameModes obj) {
        if (obj != GameModes.Online)
            return;

        Connect();
    }

    void SetupPhoton(){
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.sendRate = 20;
        PhotonNetwork.sendRateOnSerialize = 20;
    }

    void Connect() {
        if (PhotonNetwork.connected) {
            PhotonNetwork.JoinRandomRoom();
        } else {
            PhotonNetwork.ConnectUsingSettings(GameVersion);
        }
    }

    void StartSession() {
        StopAllCoroutines();
        SessionStarted?.Invoke();
    }

    IEnumerator WaitingForSecondPlayer() {
        _connnectionTime = 0;
        while (PhotonNetwork.room.PlayerCount < 2) {
            _connnectionTime += Time.deltaTime;

            if (_connnectionTime > MaxConnectionTime) {
                RejectConnectionManually();
            }
            yield return null;
        }
        StartSession();
    }

    #endregion
}