using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneController : MonoBehaviour {

	/// A test dummy to test core functionality.
	public GameObject dummy;

	/// Denotes whether or not the player is in control.
	[SerializeField] private bool isPlayer;
	/// The inputs recorded by this instance.
	private List<InputInTime> inputs;

	/// The starting position of this GameObject.
	private Vector2 startingPosition;
	/// The starting rotation of this GameObject.
	private Quaternion startingRotation;
	/// The time since the start of the game when `Start` gets called.
	private float startingTime;

	/// Associates each input with code that responds to it.
	public Dictionary<string, EventHandler<InputInTime>> inputHandlers;

	protected virtual void Start() {
		isPlayer = true;
		inputs = new List<InputInTime>();
		startingPosition = transform.position;
		startingRotation = transform.rotation;
		startingTime = Time.time;

		inputHandlers = new Dictionary<string, EventHandler<InputInTime>>();
		inputHandlers.Add("Fire1", (sender, eventArgs) => {
			GameObject copy = Instantiate(dummy, eventArgs.mousePosition, Quaternion.identity);
			Destroy(copy, 1);
		});
		inputHandlers.Add("HorzVertAxis", (sender, eventArgs) => {
			transform.Translate(new Vector2(eventArgs.horizontalAxis, eventArgs.verticalAxis));
		});
	} // Start

	protected virtual void Update() {
		if (isPlayer) {
			// Record inputs if this is the player
			if (Input.GetButtonDown("Fire1")) {
				inputs.Add(new InputInTime("Fire1"));
				inputHandlers["Fire1"](this, inputs[inputs.Count - 1]);
			} // if

			// Switch from player control to clone control
			if (Input.GetButtonDown("Fire2")) {
				ResetToInitialState();
				StartCoroutine(HandleInputs());
			} // if
		} // if
	} // Update

	protected virtual void FixedUpdate() {
		// Movement code makes too many events in Update
		if (isPlayer) {
			float horizontalAxis = Input.GetAxis("Horizontal");
			float verticalAxis = Input.GetAxis("Vertical");
			if (horizontalAxis != 0 || verticalAxis != 0) {
				inputs.Add(new InputInTime("HorzVertAxis", horizontalAxis, verticalAxis));
				inputHandlers["HorzVertAxis"](this, inputs[inputs.Count - 1]);
			} // if
		} // if
	} // FixedUpdate

	protected virtual void ResetToInitialState() {
		Debug.Log(inputs.Count);
		isPlayer = false;
		transform.position = startingPosition;
		transform.rotation = startingRotation;
	} // ResetToInitialState

	protected virtual IEnumerator HandleInputs() {
		for (int i = 0; i < inputs.Count; ++i) {
			InputInTime input = inputs[i];

			// Convert the timestamp to a duration
			// Need only to wait the time /between/ inputs
			float inputOffset = input.timestamp - startingTime;
			if (i > 0) {
				inputOffset -= inputs[i - 1].timestamp;
			} // if

			yield return new WaitForSeconds(inputOffset);
			inputHandlers[input.button](this, input);
		} // for
	} // RespondToInput
} // CloneController
