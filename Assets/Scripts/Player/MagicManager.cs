﻿using System;
using System.Text;
using UnityEngine;

public class MagicManager : MonoBehaviour
{

    private FireMode fireMode = null;
    private WindMode windMode = null;
    private IceMode iceMode = null;
    private ASpellMode activeSpellMode = null;

    private UIManager uiManager;
    private Transform spellSource;
    private CameraManager cameraManager;
    private LockOnManager lockOnManager;

    public void Start()
    {
        uiManager = this.gameObject.GetComponent<UIManager>();
        spellSource = this.gameObject.FindObjectInChildren("SpellSource").transform;
        fireMode = gameObject.GetComponentInChildren<FireMode>();
        windMode = gameObject.GetComponentInChildren<WindMode>();
        iceMode = gameObject.GetComponentInChildren<IceMode>();
        activeSpellMode = fireMode;

        cameraManager = Camera.main.GetComponent<CameraManager>();
        lockOnManager = gameObject.GetComponent<LockOnManager>();
        uiManager.UISetReadySpellIcon();
    }

    public void Update()
    {
    }

    public void BasicAttackPressed()
    {
        if(cameraManager.state == CameraMode.Free)
        {
            if (lockOnManager.LockOnTarget == null)
                activeSpellMode.AttackPressed(spellSource.position, transform.forward);
            else
                activeSpellMode.AttackPressed(spellSource.position, lockOnManager.LockOnTarget.transform.position - spellSource.position, lockOnManager.LockOnTarget.transform.position);
        }
        else if (cameraManager.state == CameraMode.Aim)
            activeSpellMode.AttackPressed(spellSource.position, uiManager.crosshair.position - spellSource.position, uiManager.crosshair.position);
        else if (cameraManager.state == CameraMode.HardLockOn)
            activeSpellMode.AttackPressed(spellSource.position, lockOnManager.LockOnTarget.transform.position - spellSource.position, lockOnManager.LockOnTarget.transform.position);
    }

    public void BasicAttackReleased()
    {
        if (cameraManager.state == CameraMode.Free)
        {
            if(lockOnManager.LockOnTarget == null)
                activeSpellMode.AttackReleased(spellSource.position, transform.forward);
            else
                activeSpellMode.AttackReleased(spellSource.position, lockOnManager.LockOnTarget.transform.position - spellSource.position, lockOnManager.LockOnTarget.transform.position);
        }
        else if (cameraManager.state == CameraMode.Aim)
            activeSpellMode.AttackReleased(spellSource.position, uiManager.crosshair.position - spellSource.position, uiManager.crosshair.position);
        else if (cameraManager.state == CameraMode.HardLockOn)
            activeSpellMode.AttackReleased(spellSource.position, lockOnManager.LockOnTarget.transform.position - spellSource.position, lockOnManager.LockOnTarget.transform.position);
    }

    public Sprite GetSpellIcon()
    {
        return activeSpellMode.spellIcon;
    }

    public void ActivateFireMode()
    {
        ChangeSpellMode(fireMode);
    }

    public void ActivateWindMode()
    {
        ChangeSpellMode(windMode);
    }

    public void ActivateIceMode()
    {
        ChangeSpellMode(iceMode);
    }

    private void ChangeSpellMode(ASpellMode nextMode)
    {
        activeSpellMode.OnSpellChangedFrom();
        if (activeSpellMode != nextMode)
            activeSpellMode = nextMode;
    }

}
