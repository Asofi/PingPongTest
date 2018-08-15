using System;
using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class LobbyController : PunBehaviour{

	public static event Action<int> PlayerConnected;
	public static event Action SessionStarted, PlayerDisconnected;
	
	[SerializeField] byte MaxPlayersPerRoom = 2;
	const string _gameVersion = "1";

	void Awake()
	{
		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.automaticallySyncScene = true;
		PhotonNetwork.sendRate = 20;
		PhotonNetwork.sendRateOnSerialize = 20;
		UIController.GameStarted += OnGameStarted;
	}

	void OnGameStarted(GameModes obj){
		if(obj != GameModes.Online)
			return;
		Connect();
	}
	
	void Connect()
	{
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.JoinRandomRoom();
		}
		else
		{
			PhotonNetwork.ConnectUsingSettings(_gameVersion);
		}
	}
	
	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
		PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
	}

	public override void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		Debug.LogError("Cause: " + cause);
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
		PlayerConnected?.Invoke(PhotonNetwork.countOfPlayers);
	} 
	
	public override void OnPhotonPlayerDisconnected(PhotonPlayer player) {
		PlayerDisconnected?.Invoke();
	} 

	public override void OnJoinedRoom()
	{
		Debug.Log("Players in room: " + PhotonNetwork.room.PlayerCount);
		StartCoroutine(WaitingForSecondPlayer());
	}

	void StartSession() {
		StopAllCoroutines();
		SessionStarted?.Invoke();
	}

	IEnumerator WaitingForSecondPlayer() {
		while (PhotonNetwork.room.PlayerCount < 2) {
			print("Waitng for second player...");
			yield return null;
		}
		print("All connected");
		StartSession();
		
		
	}
}
