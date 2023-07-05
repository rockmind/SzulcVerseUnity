using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShipController : MonoBehaviour
{

    private Rigidbody rb;
    private Transform tr;
    
    [Header("--- Ship movement settings ---")]
    [SerializeField] private InputActionReference translation, pitchYaw, roll;
    [SerializeField] private float maxSpeed = 2, acceleration = 10, deacceleration = 100, maxRotationSpeed = 10f;
    
    
    private float currentSpeed = 0;
    private float currentRollRotation = 0;
    private float currentPitchYawRotation = 0;
    private Vector3 currentAngularSpeed = new Vector3(0, 0, 0);
    
    private Vector3 oldTranslationInput;
    private Vector2 oldPitchYawInput;
    private float oldRollInput;
    private Vector3 oldAngularSpeed;
    
    public Vector3 translationInput { get; set; }
    public Vector2 pitchYawInput { get; set; }
    public float rollInput { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
    }
    

    private void Update()
    {
        translationInput = translation.action.ReadValue<Vector3>();
        pitchYawInput = pitchYaw.action.ReadValue<Vector2>();
        rollInput = roll.action.ReadValue<float>();
    }
    void FixedUpdate(){
        var angularInput = new Vector3(pitchYawInput.y, pitchYawInput.x, rollInput);

        if (translationInput.magnitude > 0 && currentSpeed >= 0)
        {
            oldTranslationInput = rb.rotation * translationInput;
            currentSpeed += acceleration * maxSpeed * Time.deltaTime;
        }
        else
        {
            currentSpeed -= deacceleration * maxSpeed * Time.deltaTime;
        }
        
        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        rb.velocity = oldTranslationInput * currentSpeed;

        rb.transform.Rotate(angularInput);
        if (angularInput.magnitude > 0)
        {
        }
    }
}