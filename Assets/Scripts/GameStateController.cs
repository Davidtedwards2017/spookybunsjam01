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

    [Header("Runner")]
    public RunnerCharacterController RunnerPrefab;
    public Transform RunnerSpawnAnchor;
    private RunnerCharacterController Runner;

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
        CameraController.Instance.SetFocus(Runner.transform);
        Runner.StartRunning();
    }

    public void EndScreen_Enter()
    {

    }

    public void Reset_Enter()
    {

    }

    #endregion
}
