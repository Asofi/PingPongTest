using System;
using Photon;
using UnityEngine;

using Random = UnityEngine.Random;

public class Ball : PunBehaviour {

	public static event Action BallBounced, BallDissapeared;

	[SerializeField] protected BallSettings _ballSettings;
	
	protected SpriteRenderer _renderer;
	protected Rigidbody2D _rb;

	protected Color _ballColor;
	protected float _ballSpeed;
	protected float _ballSize;
	
	void Awake() {
		_rb = GetComponent<Rigidbody2D>();
		_renderer = GetComponent<SpriteRenderer>();
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.collider.CompareTag("Player")) {
			if(!PhotonNetwork.connected || PhotonNetwork.isMasterClient)
				BallBounced?.Invoke();
		}
	}

	protected void SetupBallProperties() {
		_rb.velocity = Vector2.zero;

		_ballSpeed = Random.Range(_ballSettings.MinSpeed, _ballSettings.MaxSpeed);
		_ballSize = Random.Range(_ballSettings.MinSize, _ballSettings.MaxSize);
		_ballColor = Random.ColorHSV(_ballSettings.MinHue, _ballSettings.MaxHue, _ballSettings.MinSaturation, _ballSettings.MaxSaturation, _ballSettings.MinValue, _ballSettings.MaxValue);

		transform.localScale = Vector3.one * _ballSize;
		_renderer.color = _ballColor;
	}

	protected void HideBall() {
		gameObject.SetActive(false);
		BallDissapeared?.Invoke();
	}

}
