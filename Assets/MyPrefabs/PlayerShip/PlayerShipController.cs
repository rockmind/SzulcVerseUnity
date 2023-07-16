using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShipController : PortalTraveller
{
    private Rigidbody _rb;
    private Transform _tr;

    [Header("--- Ship movement settings ---")] [SerializeField]
    private InputActionReference xyz, rollPitchYaw;

    [SerializeField] private float maxSpeed = 2, acceleration = 10, deceleration = 100;
    [SerializeField] private float rollSpeed = 100, pitchSpeed = 100, yawSpeed = 100;


    private float _currentSpeed = 0;

    private Vector3 _oldXYZInput;
    private Vector2 _oldPitchYawInput;
    private float _oldRollInput;
    private Vector3 _oldAngularSpeed;

    public Vector3 XYZInput { get; set; }
    public Vector3 RollPitchYawInput { get; set; }


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _tr = GetComponent<Transform>();
    }


    private void Update()
    {
        XYZInput = xyz.action.ReadValue<Vector3>();
        RollPitchYawInput = rollPitchYaw.action.ReadValue<Vector3>();
    }

    private void FixedUpdate()
    {
        var angularInput = new Vector3(
            rollSpeed * RollPitchYawInput.x,
            pitchSpeed * RollPitchYawInput.y,
            yawSpeed * RollPitchYawInput.z
        );

        if (XYZInput.magnitude > 0 && _currentSpeed >= 0)
        {
            _oldXYZInput = _rb.rotation * XYZInput;
            _currentSpeed += acceleration * maxSpeed * Time.deltaTime;
        }
        else
        {
            _currentSpeed -= deceleration * maxSpeed * Time.deltaTime;
        }

        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, maxSpeed);
        _rb.velocity = _oldXYZInput * _currentSpeed;

        _rb.transform.Rotate(angularInput * Time.deltaTime);
    }
    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        // Vector3 eulerRot = rot.eulerAngles;
        // float delta = Mathf.DeltaAngle (yawSpeed, eulerRot.y);
        // yaw += delta;
        // yawSpeed += delta;
        // transform.eulerAngles = Vector3.up * yawSpeed;
        _rb.velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (_rb.velocity));
        Physics.SyncTransforms ();
    }
}