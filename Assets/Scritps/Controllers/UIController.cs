using System;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Handles UI
/// </summary>
public class UIController : MonoBehaviour {
    public static event Action<GameModes> GameStarted;
    public static event Action GameEnded;

    [SerializeField] Canvas _mainMenu, _connectingScreen, _ingameScreen;

    void Awake() {
        LobbyController.SessionStarted += OnSessionStarted;
        LobbyController.PlayerDisconnected += EndGame;
        LobbyController.RejectConnection += OnRejectConnection;
        Init();
    }

    void Init() {
        ShowMenu(_connectingScreen, false);
        ShowMenu(_ingameScreen, false);
    }
    
    void OnRejectConnection() {
        ShowMenu(_connectingScreen, false);
        ShowMenu(_mainMenu, true, Ease.OutBack);
    }

    void OnSessionStarted() {
        ShowMenu(_connectingScreen, false);
        ShowMenu(_ingameScreen);
    }

    public void StartSingleGame() {
        ShowMenu(_mainMenu, false, Ease.InBack, () => { GameStarted?.Invoke(GameModes.Offline); });
        ShowMenu(_ingameScreen);
    }

    public void StartOnlineGame() {
        ShowMenu(_mainMenu, false, Ease.InBack, () => { GameStarted?.Invoke(GameModes.Online); });
        ShowMenu(_connectingScreen, true, Ease.OutBack);
    }

    public void EndGame() {
        GameEnded?.Invoke();
        ShowMenu(_mainMenu, true, Ease.OutBack);
        ShowMenu(_ingameScreen, false);
    }

    void ShowMenu(Canvas menuPanel, bool isOpening = true, Ease ease = Ease.Flash, Action callback = null) {
        if (isOpening)
            menuPanel.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(menuPanel.transform.DOScale(isOpening ? 1 : 0, 0.5f).SetEase(ease));
        seq.AppendCallback(() => {
                               callback?.Invoke();
                               if (!isOpening)
                                   menuPanel.gameObject.SetActive(false);
                           });
    }
}