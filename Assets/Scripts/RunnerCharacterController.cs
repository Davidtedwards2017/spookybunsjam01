using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using MonsterLove.StateMachine;

public class RunnerCharacterController : MonoBehaviour
{

    public float jumpForce = 400f;

    private const float GROUNDED_RADIUS = 0.2f;
    public bool grounded;

    public LayerMask whatIsGround;
    public Transform groundCheck;

    public float slopeFriction;

    private Vector3 velocity = Vector3.zero;

    public Animator AnimationController;

    [Range(0, 200)] public float movementSpeed = 10f;
    [Range(0, 0.3f)] public float movementSmoothing = 0.05f;
    public bool airControl = false;

    public Rigidbody2D Rigidbody;

    [Header("Events")]
    [Space]

    public UnityEvent onLandEvent;

    public class BoolEvent : UnityEvent<bool> { }

    public enum RunnerState
    {
        Inactive,
        Running
    }

    private StateMachine<RunnerState> RunnerStateCtrl;
    

    public void Awake()
    {
        if(onLandEvent != null)
        {
            onLandEvent = new UnityEvent();
        }

    }

    private void Start()
    {
        RunnerStateCtrl = StateMachine<RunnerState>.Initialize(this);
        RunnerStateCtrl.ChangeState(RunnerState.Inactive);
    }

    public void FixedUpdate()
    {
        bool wasGrounded = grounded;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, GROUNDED_RADIUS, whatIsGround);

        grounded = false;
        if (colliders.Any(col => col.gameObject != null))
        {
            grounded = true;
            if (!wasGrounded)
            {
                onLandEvent.Invoke();
            }
        }

        AnimationController.SetBool("Grounded", grounded);

        if (grounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 2f, whatIsGround);

            if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f)
            {
                Debug.DrawRay(transform.position, hit.normal * 3, Color.red);

                //NormalizeSlope();
            }
        }

        Debug.DrawRay(transform.position, /*Quaternion.FromToRotation(Vector3.up, hit.normal) * */ velocity, Color.blue);
    }


    public void Move(float move, bool jump)
    {

        if (grounded || airControl)
        {

            Vector3 targetVelocity = Vector3.zero;

            //targetVelocity = new Vector2(move * movementSpeed, rigidbody.velocity.y);

            if (grounded)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 2f, whatIsGround);
                //targetVelocity = new Vector2(move * movementSpeed, rigidbody.velocity.y);

                targetVelocity = Quaternion.FromToRotation(Vector3.up, hit.normal) * new Vector2(move * movementSpeed, Rigidbody.velocity.y);
               // targetVelocity = Vector2.Perpendicular(hit.normal) * targetVelocity.magnitude;
            }
            else
            {
                targetVelocity = new Vector2(move * movementSpeed, Rigidbody.velocity.y);
            }

            Debug.DrawRay(transform.position, targetVelocity * 3, Color.yellow);

            Rigidbody.velocity = Vector3.SmoothDamp(Rigidbody.velocity, targetVelocity, ref velocity, movementSmoothing);
        }


        AnimationController.SetFloat("velocity.mag", velocity.magnitude);

        if (grounded && jump)
        {
            AnimationController.SetTrigger("Jump");
            grounded = false;
            Rigidbody.AddForce(new Vector2(0f, jumpForce));
        }
    }

    public void StartRunning()
    {
        if(RunnerStateCtrl.IsOrWillBeState(RunnerState.Inactive))
        {
            RunnerStateCtrl.ChangeState(RunnerState.Running);
        }
    }

    #region states
       
    public void Inactive_Enter()
    {

    }

    public void Running_Update()
    {
        Move(1, InputController.Instance.GetJumpInput());
    }

    #endregion
}
