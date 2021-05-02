using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event System.Action OnPlayerWin;

    public float moveSpeed = 8;
    public float turnSpeed = 8;
    public float smoothMoveTime = .1f;

    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;

    bool disabled = false;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Guard.OnGuardHasSpottedPlayer += Disable;
    }

    void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if (!disabled)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
         
        float inputMagnitude = inputDirection.magnitude;    // Jeśli się niema inputu == 0, nie można się ruszyć ani obracać
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, turnSpeed * Time.deltaTime * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
        rb.MoveRotation(Quaternion.Euler(Vector3.up * angle));
    }

    void OnTriggerEnter(Collider triggerColider)
    {
        if (triggerColider.CompareTag("Finish"))
        {
            Disable();
            if (OnPlayerWin != null)
            {
                OnPlayerWin();
            }
        }
    }

    void Disable()
    {
        disabled = true;
    }

    private void OnDestroy()
    {
        Guard.OnGuardHasSpottedPlayer -= Disable;
    }

    //void Update()
    //{
    //    inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
    //    velocity = inputDirection * speed;
    //    Debug.DrawRay(transform.position, inputDirection * 10, Color.green);
    //    //StartCoroutine(TurnToDirection(inputDirection));

    //    //transform.Translate(moveAmount, Space.World);
    //    inputAngle = Mathf.Atan2(inputDirection.z, inputDirection.x) * Mathf.Rad2Deg;
    //    transform.eulerAngles = Vector3.up * inputAngle;
    //}

    //IEnumerator TurnToDirection(Vector3 direction)
    //{
    //    float targetAngle = 90 - Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

    //    while (Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle) > 0.05f)
    //    {
    //        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
    //        transform.eulerAngles = Vector3.up * angle;
    //        yield return null;
    //    }
    //}


}
