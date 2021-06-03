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
	private Dictionary<string, EventHandler<InputInTime>> inputHandlers;

	protected virtual void Start() {
		isPlayer = true;
		inputs = new List<InputInTime>();
		startingPosition = transform.position;
		startingRotation = transform.rotation;
		startingTime = Time.time;

		inputHandlers = new Dictionary<string, EventHandler<InputInTime>>();
		RegisterHandler("Fire1", (sender, eventArgs) => {
			GameObject copy = Instantiate(dummy, eventArgs.mousePosition, Quaternion.identity);
			Destroy(copy, 1);
		});
		RegisterHandler("HorzVertAxis", (sender, eventArgs) => {
			transform.Translate(new Vector2(eventArgs.horizontalAxis, eventArgs.verticalAxis) * 0.5f);
		});
	} // Start

	protected virtual void Update() {
		if (isPlayer) {
			// Record inputs if this is the player
			if (Input.GetButtonDown("Fire1")) {
				HandleNewInput(new InputInTime("Fire1"));
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
				HandleNewInput(new InputInTime("HorzVertAxis", horizontalAxis, verticalAxis));
			} // if
		} // if
	} // FixedUpdate

	public virtual void RegisterHandler(string button, EventHandler<InputInTime> handler) {
		try {
			inputHandlers[button] += handler;
		} catch (KeyNotFoundException) {
			inputHandlers.Add(button, handler);
		} // try
	} // RegisterHandler

	protected virtual void ResetToInitialState() {
		Debug.Log(inputs.Count);
		isPlayer = false;
		transform.position = startingPosition;
		transform.rotation = startingRotation;
	} // ResetToInitialState

	protected virtual void HandleNewInput(InputInTime newInput) {
		inputs.Add(newInput);
		ExecuteInput(newInput);
	} // AddNewInput

	protected virtual void ExecuteInput(InputInTime input) {
		try {
			EventHandler<InputInTime> handler = inputHandlers[input.button];
			handler(this, input);
		} catch (KeyNotFoundException knfe) {
			Debug.LogError($"Did not find a handler for {input.button}.", this);
			Debug.LogException(knfe, this);
		} // try
	} // ExecuteInput

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
			ExecuteInput(input);
		} // for
	} // RespondToInput
} // CloneController
