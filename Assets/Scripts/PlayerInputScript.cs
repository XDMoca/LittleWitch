﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputScript : MonoBehaviour {

    PlayerMovementScript playerMovement;
    CameraScript camera;
	// Use this for initialization
	void Start () {
        playerMovement = gameObject.GetComponent<PlayerMovementScript>();
        camera = Camera.main.GetComponent<CameraScript>();
	}
	
	// Update is called once per frame
	void Update () {

        playerMovement.forwardInput = Input.GetAxis("Vertical");
        playerMovement.sidewaysInput = Input.GetAxis("Horizontal");
        
        if (Input.GetButtonDown("Jump"))
            playerMovement.Jump();

    }
}