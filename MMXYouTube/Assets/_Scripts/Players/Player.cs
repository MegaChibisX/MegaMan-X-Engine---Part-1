using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{

    [Header("Input")]
    public InputCtrl inputCtrl;

    [Header("Physics")]
    public Rigidbody body;

    public float height = 1f;
    public float radius = 0.5f;

    protected bool wasGrounded = true;
    protected RaycastHit hitGround;

    [Header("World")]
    public Vector3 rightVec = Vector3.right;
    public Vector3 gravityVec = Vector3.down;

    [Header("Player Stats")]
    public float moveSpeed = 10f;
    public float jumpForce = 30f;


    protected virtual void OnEnable()
    {
        inputCtrl = new InputCtrl();
        inputCtrl.OnEnable();
    }
    protected virtual void OnDisable()
    {
        inputCtrl.OnDisable();
    }
    public virtual void Start()
    {
        body = GetComponent<Rigidbody>();

        body.useGravity = false;
        body.drag = 0;
    }
    public virtual void Update()
    {
        inputCtrl.UpdateInputStates();

        HandleInput();
    }
    public virtual void FixedUpdate()
    {
        ApplyPhysics();

        Move_Regular();
    }
    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position - transform.up * height, radius);
    }


    public void ApplyPhysics()
    {
        bool onGround = isGrounded(0.2f);

        // Gravity
        if (!onGround)
            body.velocity += gravityVec * -Physics.gravity.y * Time.fixedDeltaTime;


        if (!wasGrounded && onGround)
        {
            Vector3 velH = Vector3.ProjectOnPlane(body.velocity, gravityVec);
            body.velocity = velH;
        }

        wasGrounded = onGround;
    }
    public virtual void HandleInput()
    {
        if (inputCtrl.keyJump == InputCtrl.KeyState.Press)
        {
            if (isGrounded(0.3f))
                body.velocity = new Vector3(body.velocity.x, jumpForce, body.velocity.z);
        }
    }
    public virtual void Move_Regular()
    {
        if (inputCtrl.input.x != 0)
            transform.rotation = Quaternion.LookRotation(rightVec * inputCtrl.input.x, -gravityVec);

        Vector3 moveVec = rightVec;
        if (isGrounded(0.3f))
        {
            moveVec = Quaternion.AngleAxis(90f, transform.right) * hitGround.normal;

            float dot = Vector3.Dot(rightVec, moveVec);
            if (dot != 0)
                moveVec = moveVec / dot;
        }

        Vector3 velH = Vector3.ProjectOnPlane(body.velocity, gravityVec);
        Vector3 velV = Vector3.Project(body.velocity, gravityVec);
        
        velH = moveVec * inputCtrl.input.x * moveSpeed;

        Vector3 velHFrameDisp = Vector3.Project(velH, gravityVec);
        velH = Vector3.ProjectOnPlane(velH, gravityVec);


        body.velocity = velH + velV;
        body.position += velHFrameDisp * Time.fixedDeltaTime;
    }
    public virtual void HandleAnimations() { }



    public bool isGrounded(float addDistance = 0.1f)
    {
        int solidLayer = 1 << 8;


        return Physics.SphereCast(transform.position, radius * 0.9f, gravityVec, out hitGround, height + addDistance - radius, solidLayer);
    }

}
