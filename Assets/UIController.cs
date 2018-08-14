using System;
using System.Collections;

using DG.Tweening;

using TMPro;

using UnityEngine;

public class UIController : MonoBehaviour {
    public static event Action<GameModes> GameStarted;
    public static event Action GameEnded;

    [SerializeField] Canvas _mainMenu, _connectingScreen, _ingameScreen;

    void Awake() {
        LobbyController.SessionStarted += OnSessionStarted;
    }

    void Init() {
        ShowMenu(_connectingScreen, false);
        ShowMenu(_ingameScreen, false);
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
        ShowMenu(_connectingScreen, true, Ease.InBack);
    }

    public void EndGame() {
        GameEnded?.Invoke();
        ShowMenu(_mainMenu, true, Ease.InBack);
        ShowMenu(_ingameScreen, false);
    }

    void ShowMenu(Canvas menuPanel, bool isOpening = true, Ease ease = Ease.Flash, Action callback = null) {
        if(isOpening)
            menuPanel.gameObject.SetActive(true);
        
        Sequence seq = DOTween.Sequence();
        seq.Append(menuPanel.transform.DOScale(isOpening ? 1 : 0, 0.5f).SetEase(ease));
        seq.AppendCallback(() => {
                               callback?.Invoke(); 
                               if(!isOpening)
                                   menuPanel.gameObject.SetActive(false);
                           });
    }
}