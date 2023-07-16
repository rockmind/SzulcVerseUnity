using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : PortalTraveller {

    public Vector3 XYZInput { get; set; }
    public Vector3 RollPitchYawInput { get; set; }

    [Header("--- Ship movement settings ---")] [SerializeField]
    private InputActionReference xYZ, rollPitchYaw;
    
    public float walkSpeed = 3;
    public float runSpeed = 6;
    public float smoothMoveTime = 0.1f;
    
    public float rotationSmoothTime = 0.1f;

    private CharacterController _controller;
    public float roll;
    public float yaw;
    public float pitch;
    private float _smoothRoll;
    private float _smoothYaw;
    private float _smoothPitch;

    private float _rollSmoothV;
    private float _yawSmoothV;
    private float _pitchSmoothV;
    private float _verticalVelocity;
    private Vector3 _velocity;
    private Vector3 _smoothV;
    private Vector3 _rotationSmoothVelocity;
    private Vector3 _currentRotation;

    private void Start () {
        _controller = GetComponent<CharacterController> ();

        var eulerAngles = transform.eulerAngles;
        roll = eulerAngles.x;
        yaw = eulerAngles.y;
        pitch = eulerAngles.z;
        _smoothRoll = roll;
        _smoothYaw = yaw;
        _smoothPitch = pitch;
    }

    private void Update () {
    
        XYZInput = xYZ.action.ReadValue<Vector3>();

        Vector3 inputDir = XYZInput.normalized;
        Vector3 worldInputDir = transform.TransformDirection (inputDir);

        float currentSpeed = (Input.GetKey (KeyCode.LeftShift)) ? runSpeed : walkSpeed;
        Vector3 targetVelocity = worldInputDir * currentSpeed;
        _velocity = Vector3.SmoothDamp (_velocity, targetVelocity, ref _smoothV, smoothMoveTime);

        _velocity = new Vector3 (_velocity.x, _velocity.y, _velocity.z);

        _controller.Move (_velocity * Time.deltaTime);

        RollPitchYawInput = rollPitchYaw.action.ReadValue<Vector3>();

        // // Verrrrrry gross hack to stop camera swinging down at start
        // // float mMag = RollPitchYawInput.magnitude;
        // float mMag = RollPitchYawInput.magnitude;
        // if (mMag > .001) {
        //     RollPitchYawInput = Vector3.zero;
        // }
        
        var angularInput = new Vector3(
            Mathf.SmoothDampAngle (_smoothRoll, RollPitchYawInput.z, ref _rollSmoothV, rotationSmoothTime),
            Mathf.SmoothDampAngle (_smoothPitch, RollPitchYawInput.y, ref _pitchSmoothV, rotationSmoothTime),
            Mathf.SmoothDampAngle (_smoothYaw, RollPitchYawInput.x, ref _yawSmoothV, rotationSmoothTime)
        );
        transform.Rotate(angularInput);

    }

    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        // Vector3 eulerRot = rot.eulerAngles;
        // float delta = Mathf.DeltaAngle (_smoothYaw, eulerRot.y);
        // yaw += delta;
        // _smoothYaw += delta;
        // transform.eulerAngles = Vector3.up * _smoothYaw;
        transform.rotation = rot;
        _velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (_velocity));
        Physics.SyncTransforms ();
    }

}