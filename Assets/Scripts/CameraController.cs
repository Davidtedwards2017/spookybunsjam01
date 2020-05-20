using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MonsterLove.StateMachine;

public class CameraController : Singleton<CameraController>
{
    public enum State
    {
        notfollowing,
        following
    }
    private StateMachine<State> StateCtrl;

    private float xRightMargin = 1f; // Distance in the x axis the player can move before the camera follows.
    private float yMargin = 1f; // Distance in the y axis the player can move before the camera follows.
    public float xSmooth = 8f; // How smoothly the camera catches up with it's target movement in the x axis.
    public float ySmooth = 8f; // How smoothly the camera catches up with it's target movement in the y axis.
    
    public Transform CameraStartingAnchor;
    public Transform Focus;
    public Vector3 FocusOffset;
    
    public void SetFocus(Transform focus)
    {
        Focus = focus;
    }

    private void Start()
    {
        StateCtrl = StateMachine<State>.Initialize(this);
        StateCtrl.ChangeState(State.notfollowing);
    }

    public void ResetCamera()
    {
        transform.position = CameraStartingAnchor.position + FocusOffset;
    }


    protected void TrackPosition(Vector3 target)
    {
        // By default the target x and y coordinates of the camera are it's current x and y coordinates.
        float targetX = transform.position.x;
        float targetY = transform.position.y;

        // If the player has moved beyond the x margin...
        if (CheckXMargin(target))
        {
            // ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
            targetX = Mathf.Lerp(transform.position.x, target.x, xSmooth * Time.fixedDeltaTime);
        }

        // If the player has moved beyond the y margin...
        if (CheckYMargin(target))
        {
            // ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
            targetY = Mathf.Lerp(transform.position.y, target.y, ySmooth * Time.fixedDeltaTime);
        }

        MoveCamera(new Vector3(targetX, targetY));
    }

    protected bool CheckXMargin(Vector3 target)
    {
        return Mathf.Abs(transform.position.x - target.x) > xRightMargin;
    }

    protected bool CheckYMargin(Vector3 target)
    {
        // Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
        return Mathf.Abs(transform.position.y - target.y) > yMargin;
    }

    // Set the camera's position to the target position with the same z component.
    protected void MoveCamera(Vector3 target)
    {
        //target.z = DefaultZ;
        transform.position = target;
    }

    #region state
    public void notfollowing_FixedUpdate()
    {
        if(Focus != null)
        {
            StateCtrl.ChangeState(State.following);
            return;
        }
    }

    public void following_FixedUpdate()
    {
        if(Focus == null)
        {
            StateCtrl.ChangeState(State.notfollowing);
            return;
        }

        TrackPosition(Focus.position + FocusOffset);

    }

    #endregion
}
