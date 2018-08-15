﻿using System.Text;
using Photon;
using TMPro;
using UnityEngine;

public class ScoreHandler : PunBehaviour {

	[SerializeField] TMP_Text _scoreText;
	StringBuilder _stringBuilder = new StringBuilder(32,32);
	int _score = 0;

	public int Score {
		get { return _score; }
		private set {
			_score = value;
			_stringBuilder.Length = 0;
			_stringBuilder.Append(value);
			_scoreText.SetText(_stringBuilder);
		}
	}

	void Awake() {
		Ball.BallBounced += OnBallBounced;
		UIController.GameStarted += OnGameStarted;
		Ball.BallDissapeared += ResetScore;
		UIController.GameEnded += ResetScore;
	}

	void Start() {
		_scoreText.GetComponent<Renderer>().sortingOrder = -1;
	}

	void OnGameStarted(GameModes obj) {
		ResetScore();
	}

	void ResetScore() {
		Score = 0;
	}

	void OnBallBounced() {
		Score++;
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(Score);
		} else {
			Score = (int) stream.ReceiveNext();
		}
	}

}