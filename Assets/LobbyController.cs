using System;
using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class LobbyController : PunBehaviour{

	public static event Action<int> PlayerConnected;
	public static event Action SessionStarted, PlayerDisconnected;
	
	const string _gameVersion = "1";
	public byte MaxPlayersPerRoom = 2;

	void Awake()
	{
		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.automaticallySyncScene = true;
		UIController.GameStarted += OnGameStarted;
		PhotonNetwork.sendRate = 20;
		PhotonNetwork.sendRateOnSerialize = 20;
	}

	void OnGameStarted(GameModes obj){
		if(obj != GameModes.Online)
			return;
		Connect();
	}
	
	public void Connect()
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

	public override void OnJoinedLobby()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	public void OnPhotonRandomJoinFailed()
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

	public override void OnLeftRoom() {
		
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
