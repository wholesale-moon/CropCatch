using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed = 5f;
    public LayerMask floor;
    [SerializeField] private InputManager IM;
    [SerializeField] private GameSaveData _GameSaveData;
    [SerializeField] private GameObject cam;
    //public GameInputs gameInputs;

    private Vector2 moveDir = Vector3.zero;
    private bool grounded;
    public float groundCheckHeight = 1f;
    public bool applyGravity = true;
    public float gravForce = 4f;
    public float maxSlopeAngle = 60f;

    [Space(10)]
    public Animator[] anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    private void OnEnable()
    {
        IM.MovementEvent += OnMovement;
    }

    private void OnDisable()
    {
        IM.MovementEvent -= OnMovement;
    }

    private void FixedUpdate()
    {
        Vector3 camMoveDir = new Vector3(moveDir.x, 0, moveDir.y);
        camMoveDir = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0) * camMoveDir;
        //Debug.Log(camMoveDir);
        RaycastHit hit;
        float gravity = 0f;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckHeight, floor))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        if(!grounded && applyGravity)
        {
            gravity = gravForce;
        }

        Vector3 slopeAdjustmentVec = SlopeAdjustment(camMoveDir);
        
        rb.linearVelocity = new Vector3(slopeAdjustmentVec.x * moveSpeed, -gravity, slopeAdjustmentVec.z * moveSpeed); // change the 0f to -2 if is not grounded
        //Debug.Log(rb.velocity);
        //Debug.Log(grounded);


        float movementCheckX = Mathf.Abs(moveDir.x);
        float movementCheckY = Mathf.Abs(moveDir.y);

        if (movementCheckX >= 0.1f || movementCheckY >= 0.1f)
        {
            anim[_GameSaveData._playerAppearance].SetBool("isWalking", true);
        } 
        else
        {
            anim[_GameSaveData._playerAppearance].SetBool("isWalking", false);
        }
    }

    private Vector3 SlopeAdjustment(Vector3 move)
    {
        applyGravity = true; //update this so no gravity on lvl 3
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckHeight, floor))
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if(angle < maxSlopeAngle && angle > 0)
            {
                applyGravity = false;
                return Vector3.ProjectOnPlane(move, hit.normal).normalized;
            }
            else
            {
                return move;
            }
        }
        else
        {
            return move;
        }
    }

    private void OnMovement(Vector2 dir)
    {
        moveDir = dir;
    }

    public void PlayAnimation(string animName)
    {
        if(anim[_GameSaveData._playerAppearance] != null)
            anim[_GameSaveData._playerAppearance].Play(animName);
    }
}
