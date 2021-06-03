using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneController : MonoBehaviour {

	/// A test dummy to test core functionality.
	public GameObject dummy;

	/// Denotes whether or not the player is in control.
	[SerializeField] private bool isPlayer;

	/// The starting position of this GameObject.
	private Vector2 startingPosition;
	/// The starting rotation of this GameObject.
	private Quaternion startingRotation;

	private Replayer replayer;

	protected virtual void Start() {
		this.isPlayer = true;
		this.startingPosition = transform.position;
		this.startingRotation = transform.rotation;
		this.replayer = new Replayer(Time.time, this.gameObject);

		replayer.RegisterHandler("Fire1", (sender, eventArgs) => {
			GameObject copy = Instantiate(dummy, eventArgs.mousePosition, Quaternion.identity);
			Destroy(copy, 1);
		});
		replayer.RegisterHandler("HorzVertAxis", (sender, eventArgs) => {
			transform.Translate(new Vector2(eventArgs.horizontalAxis, eventArgs.verticalAxis) * 0.5f);
		});
	} // Start

	protected virtual void Update() {
		if (isPlayer) {
			// Record inputs if this is the player
			if (Input.GetButtonDown("Fire1")) {
				replayer.RecordInput(new InputInTime("Fire1"));
			} // if

			// Switch from player control to clone control
			if (Input.GetButtonDown("Fire2")) {
				ResetToInitialState();
				StartCoroutine(replayer.Replay());
			} // if
		} // if
	} // Update

	protected virtual void FixedUpdate() {
		// Movement code makes too many events in Update
		if (isPlayer) {
			float horizontalAxis = Input.GetAxis("Horizontal");
			float verticalAxis = Input.GetAxis("Vertical");
			if (horizontalAxis != 0 || verticalAxis != 0) {
				replayer.RecordInput(new InputInTime("HorzVertAxis", horizontalAxis, verticalAxis));
			} // if
		} // if
	} // FixedUpdate

	protected virtual void ResetToInitialState() {
		isPlayer = false;
		transform.position = startingPosition;
		transform.rotation = startingRotation;
	} // ResetToInitialState

} // CloneController
