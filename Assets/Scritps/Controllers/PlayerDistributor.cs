﻿using Photon;
using UnityEngine;

/// <summary>
/// Distrubute bates between players
/// </summary>
public class PlayerDistributor : PunBehaviour {
    [SerializeField] PhotonView _lowerPlayer, _upperPlayer;

    void OnEnable() {
        DistributePlayers();
    }

    void DistributePlayers() {
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
            if (i == 0)
                _lowerPlayer.TransferOwnership(PhotonNetwork.playerList[i].ID);

            if (i == 1)
                _upperPlayer.TransferOwnership(PhotonNetwork.playerList[i].ID);
        }
    }
}