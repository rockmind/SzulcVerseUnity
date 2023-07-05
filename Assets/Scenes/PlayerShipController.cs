using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShipController : MonoBehaviour
{
    private Rigidbody _rb;
    private Transform _tr;

    [Header("--- Ship movement settings ---")] [SerializeField]
    private InputActionReference translation, pitchYaw, roll;

    [SerializeField] private float maxSpeed = 2, acceleration = 10, deceleration = 100;
    [SerializeField] private float rollSpeed = 100, pitchSpeed = 100, yawSpeed = 100;


    private float _currentSpeed = 0;

    private Vector3 _oldTranslationInput;
    private Vector2 _oldPitchYawInput;
    private float _oldRollInput;
    private Vector3 _oldAngularSpeed;

    public Vector3 TranslationInput { get; set; }
    public Vector2 PitchYawInput { get; set; }
    public float RollInput { get; set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _tr = GetComponent<Transform>();
    }


    private void Update()
    {
        TranslationInput = translation.action.ReadValue<Vector3>();
        PitchYawInput = pitchYaw.action.ReadValue<Vector2>();
        RollInput = roll.action.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        var angularInput = new Vector3(
            pitchSpeed * PitchYawInput.y,
            yawSpeed * PitchYawInput.x,
            rollSpeed * RollInput
        );

        if (TranslationInput.magnitude > 0 && _currentSpeed >= 0)
        {
            _oldTranslationInput = _rb.rotation * TranslationInput;
            _currentSpeed += acceleration * maxSpeed * Time.deltaTime;
        }
        else
        {
            _currentSpeed -= deceleration * maxSpeed * Time.deltaTime;
        }

        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, maxSpeed);
        _rb.velocity = _oldTranslationInput * _currentSpeed;

        _rb.transform.Rotate(angularInput * Time.deltaTime);
    }
}