using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class GameStateController : Singleton<GameStateController>
{
    public enum GameState
    {
        Initializing,
        StartScreen,
        Playing,
        Lose,
        EndScreen,
        Reset
    }

    public float StartTime = 30.0f;
    public float TimeLeft = 0;

    [Header("Runner")]
    public RunnerCharacterController RunnerPrefab;
    public Transform RunnerSpawnAnchor;
    private RunnerCharacterController Runner;

    public float KillZoneYValue = -10;

    public StateMachine<GameState> GameStateCtrl;

    private void Awake()
    {
        GameStateCtrl = StateMachine<GameState>.Initialize(this);
        GameStateCtrl.Changed += OnGameStateChanged;

        GameStateCtrl.ChangeState(GameState.Initializing);
    }

    private void OnGameStateChanged(GameState gameState)
    {
        Debug.Log("Game State changed to: " + gameState);
    }

    private void SpawnRunner()
    {
        Runner = Instantiate(RunnerPrefab, RunnerSpawnAnchor.position, Quaternion.identity);
    }

    #region states

    public IEnumerator Initializing_Enter()
    {

        PlayingUiController.Instance.Active = false;
        yield return new WaitForSeconds(1);
        SpawnRunner();
        GameStateCtrl.ChangeState(GameState.StartScreen);
    }

    [Header("Start Screen")]
    //time before accepting input on the start screen
    public float StartScreenInputCooldownTime = 1.0f;
    public IEnumerator StartScreen_Enter()
    {
        yield return new WaitForSeconds(StartScreenInputCooldownTime);
        yield return new WaitUntil(() => InputController.Instance.GetAnyInput());
        GameStateCtrl.ChangeState(GameState.Playing);
    }

    public void Playing_Enter()
    {
        TimeLeft = StartTime;

        PlayingUiController.Instance.Active = true;
        CameraController.Instance.SetFocus(Runner.transform);
        Runner.StartRunning();
    }

    public void Playing_Update()
    {
        TimeLeft -= Time.deltaTime;

        if(TimeLeft <= 0)
        {
            TimeLeft = 0;
            GameStateCtrl.ChangeState(GameState.Lose);
            return;
        }
    }

    public void Playing_Exit()
    {
        PlayingUiController.Instance.Active = false;
    }

    public void Lose_Enter()
    {
        GameStateCtrl.ChangeState(GameState.EndScreen);
    }

    public void EndScreen_Enter()
    {
        GameStateCtrl.ChangeState(GameState.Reset);
    }

    public void Reset_Enter()
    {
        GameStateCtrl.ChangeState(GameState.StartScreen);
    }

    #endregion
}
