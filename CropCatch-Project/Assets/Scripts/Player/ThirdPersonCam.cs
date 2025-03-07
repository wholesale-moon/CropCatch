using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    [SerializeField] private InputManager IM;
    [SerializeField] private GameSaveData _GameSaveData;

    private bool rotationDisabled = false;
    [Range(0.2f,1.2f)] public float mouseSensitivity = 1f; // update this with slider, should go between 0.2 and 1.2 i believe
    public float rotationSpeed;
    private Vector2 moveDir = Vector2.zero;

    private void Awake()
    {
        //IM.MovementEvent += OnMovement;
    }

    private void OnEnable()
    {
        IM.MovementEvent += OnMovement;
    }

    private void OnDisable()
    {
        IM.MovementEvent -= OnMovement;
    }

    void Start()
    {
        GameObject GameManager = GameObject.FindWithTag("GameManager");
        
        playerObj = GameManager.GetComponent<LevelLoader>().playerAppearance[_GameSaveData._playerAppearance].transform;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateMouseSensitivity();
    }

    void Update()
    {
        if(rotationDisabled)
        {
            return;
        }

        // Set forward direction
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // Rotate player
        Vector3 inputDir = orientation.forward * moveDir.y + orientation.right * moveDir.x;

        if (inputDir != Vector3.zero)
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed * mouseSensitivity);
    }

    public void UpdateMouseSensitivity()
    {
        mouseSensitivity = _GameSaveData._mouseSensitivity;
    }

    private void OnMovement(Vector2 dir)
    {
        moveDir = dir;
    }

    public void DisableLookMove()
    {
        rotationDisabled = true;
    }

    public void EnableLookMove()
    {
        rotationDisabled = false;
    }
}
