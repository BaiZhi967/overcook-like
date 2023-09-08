using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameManager : NetworkBehaviour {
    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnMultiplayerGamePause;
    public event EventHandler OnMultiplayerGameUnpause;    
    public event EventHandler OnLocalPlayerReadyChanged;

    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    //private State state;
    private bool isLocalPlayerReady = false;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private  NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private float gamePlayingTimerMax = 600f;
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePause = new NetworkVariable<bool>(false);
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;
    private bool autoTestGamePauseState;

    private void Awake() {
        Instance = this;
        //state = State.WaitingToStart;
        playerReadyDictionary = new Dictionary<ulong, bool>();     
        playerPausedDictionary = new Dictionary<ulong, bool>();        
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        
        //DEBUG 快速开始游戏以便于测试
        //state = State.CountdownToStart;
        //OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePause.OnValueChanged += IsGamePause_OnValueChanged;
        
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        autoTestGamePauseState = true;
    }

    private void IsGamePause_OnValueChanged(bool previousvalue, bool newvalue)
    {
        if (isGamePause.Value)
        {
            Time.timeScale = 0f;
            OnMultiplayerGamePause?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnMultiplayerGameUnpause?.Invoke(this, EventArgs.Empty);            
        }
    }

    private void State_OnValueChanged(State previousvalue, State newvalue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientReady = true;
        foreach (var clientsId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientsId) || !playerReadyDictionary[clientsId])
            {
                allClientReady = false;
                break;
            }
        }

        if (allClientReady)
        {
            state.Value = State.CountdownToStart;
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if (state.Value == State.WaitingToStart) {
            //state = State.CountdownToStart;
            //OnStateChanged?.Invoke(this, EventArgs.Empty);
            isLocalPlayerReady = true;
            
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            
            SetPlayerReadyServerRpc();
        }
    }


    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
        
    }
    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }

    private void Update() {
        if(!IsServer)return;
        switch (state.Value) {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value  < 0f) {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value  = gamePlayingTimerMax;
                    //OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value  -= Time.deltaTime;
                if (gamePlayingTimer.Value  < 0f) {
                    state.Value = State.GameOver;
                    //OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
    }

    private void LateUpdate()
    {
        if (autoTestGamePauseState)
        {
            autoTestGamePauseState = false;
            TestGamePausedState();
        }
    }

    public bool IsGamePlaying() {
        return state.Value == State.GamePlaying;
    }

    public bool IsCountdownToStartActive() {
        return state.Value == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer() {
        return countdownToStartTimer.Value ;
    }

    public bool IsGameOver() {
        return state.Value == State.GameOver;
    }

    public float GetGamePlayingTimerNomalized() {
        return 1 - (gamePlayingTimer.Value  / gamePlayingTimerMax);
    }

    public void TogglePauseGame() {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused)
        {
            PauseGameServerRpc();
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else {
            UnpauseGameServerRpc();
            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePausedState();
    }


    private void TestGamePausedState()
    {
        
        foreach (var clientsId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientsId)&& playerPausedDictionary[clientsId])
            {
                //暂停游戏
                isGamePause.Value = true;
                return;
            }
        }
        //未暂停游戏
        isGamePause.Value = false;
    }
}
