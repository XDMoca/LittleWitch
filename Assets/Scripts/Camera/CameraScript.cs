﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public enum CameraMode
    {
        Free = 1,
        Aim = 2,
        HardLockOn = 3,
    }

    public float yawInput = 0;
    public float pitchInput = 0;
    public float freeSmooth = 0.2f;
    public float aimSmooth = 0.5f;
    public CameraMode state { get; private set; }
    public float mouseSensitivity = 50f;
    public float controllerSensitivity = 180f;
    private float timeEffect = 1;
    private bool backToCharacter = false;
    private float yRangeLimit = 3.5f;

    private Transform lookTarget;
    private CameraPositionPivotScript pivotTarget;
    private PlayerStatus playerStatus;
    private PlayerMovementScript playerMovement;
    private LockOnManager lockOnManager;
    public Vector3 distanceFromTarget = new Vector3(0f, 0f, -4f);
    private Vector3 distanceFromPlayerInHardLockOn = new Vector3(0f, 2f, -4f);

    private const float PITCH_MAX_ANGLE = 45.0f;
    private const float PITCH_MIN_ANGLE = -10.0f;

    private float oldPitch = 0f;

    void Start()
    {
        lookTarget = GameObject.FindGameObjectWithTag(Helpers.Tags.CameraFollowTarget).transform;
        pivotTarget = GameObject.FindGameObjectWithTag(Helpers.Tags.CameraPositionPivot).GetComponent<CameraPositionPivotScript>();
        playerStatus = GameObject.FindGameObjectWithTag(Helpers.Tags.Player).GetComponent<PlayerStatus>();
        playerMovement = GameObject.FindGameObjectWithTag(Helpers.Tags.Player).GetComponent<PlayerMovementScript>();
        lockOnManager = playerStatus.GetComponent<LockOnManager>();
        state = CameraMode.Free;
        transform.position = lookTarget.position + transform.rotation * distanceFromTarget;
    }

    void LateUpdate()
    {
        if (state == CameraMode.Free)
        {
            BasicFreeCameraMovement();
            //JakCameraMovement();

        }
        else if (state == CameraMode.Aim)
        {
            transform.position = Vector3.Lerp(transform.position, pivotTarget.GetTagetPosition(), aimSmooth);
            transform.LookAt(pivotTarget.transform.position);
        }

        else if (state == CameraMode.HardLockOn)
        {
            Vector3 directionFromPlayer = (lockOnManager.LockOnTarget.transform.position - playerStatus.transform.position);
            Vector3 positionFromPlayer = (playerStatus.transform.position - directionFromPlayer.normalized) + (directionFromPlayer.sqrMagnitude > 1 ? (transform.rotation * distanceFromPlayerInHardLockOn) : new Vector3(0,1,0));
            transform.position = Vector3.Lerp(transform.position, positionFromPlayer, 0.1f);
            transform.LookAt(lockOnManager.LockOnTarget.transform.position);
        }
    }

    private void EnterAimMode() { if (state != CameraMode.Aim) state = CameraMode.Aim; }

    private void LeaveAimMode() { if (state != CameraMode.Free) state = CameraMode.Free; }

    public void SetCameraState()
    {
        if (playerStatus.state == PlayerStatus.PlayerState.Aiming)
            EnterAimMode();
        else if (playerStatus.state == PlayerStatus.PlayerState.FreeMovement)
            LeaveAimMode();
    }

    public void LockOnToTarget()
    {
        state = CameraMode.HardLockOn;
    }

    public void TurnOffLockOn()
    {
        state = CameraMode.Free;
    }

    #region TimeSlow
    private float DeltaTime
    { get { return Time.deltaTime / timeEffect; } }

    public void TimeSlowMovementActive(float TimeSlowMultiplier)
    {
        this.timeEffect = TimeSlowMultiplier;
        pivotTarget.SetTimeEffect(TimeSlowMultiplier);
    }

    public void TimeSlowMovementDeactive()
    {
        this.timeEffect = 1;
        pivotTarget.SetTimeEffect(1);
    }
    #endregion

    #region FreeCameraStyles

    private void BasicFreeCameraMovement()
    {
        if (yawInput != 0)
            transform.RotateAround(lookTarget.position, Vector3.up, DeltaTime * yawInput);

        
        Vector3 objRotation = transform.rotation.eulerAngles;

        if (pitchInput != 0)
            oldPitch = objRotation.x > 180 ? objRotation.x - 360 : objRotation.x;

        float newPitch = oldPitch + (pitchInput * DeltaTime);
        float clampedPitch = Mathf.Clamp(newPitch, PITCH_MIN_ANGLE, PITCH_MAX_ANGLE);
        transform.localEulerAngles = new Vector3(clampedPitch, objRotation.y, objRotation.z);

        transform.position = Vector3.Lerp(transform.position, lookTarget.position + transform.rotation * distanceFromTarget, freeSmooth);
        transform.LookAt(lookTarget.position);
    }

    private void JakCameraMovement()
    {

        if (yawInput != 0)
            transform.RotateAround(lookTarget.position, Vector3.up, DeltaTime * yawInput);

        Vector3 lerpDestination = lookTarget.position;
            bool cameraInYRange = Mathf.Abs(lerpDestination.y - transform.position.y) < yRangeLimit;
            bool playerInAir = (playerMovement.velocity.y > 0 || (playerMovement.velocity.y < 0 && !playerMovement.isGrounded));
            if (playerInAir)
            {
                if (cameraInYRange && !backToCharacter)
                {
                    lerpDestination.y = transform.position.y;
                }
                else
                    backToCharacter = true;
            }
        
            else if (Mathf.Abs(lookTarget.position.y - transform.position.y) < 0.15f)
            {
                backToCharacter = false;
            }

            Quaternion lerpAngle = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
            transform.position = Vector3.Lerp(transform.position, lerpDestination + lerpAngle* distanceFromTarget, 0.2f);

            Quaternion currentRot = transform.rotation;
            transform.LookAt(lerpDestination);
            Quaternion destinationRot = transform.rotation;
            transform.rotation = Quaternion.Lerp(currentRot, destinationRot, 0.2f);
    }
    #endregion
}