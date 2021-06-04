using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replayer {

	/// Used to control when the inputs should be played back;
	public float timeToFirstInput { get; }
	public GameObject controls { get; }

	private List<InputInTime> inputs;
	private Dictionary<string, EventHandler<InputInTime>> inputHandlers;

	public Replayer(float timeToFirstInput, GameObject controls) {
		this.timeToFirstInput = timeToFirstInput;
		this.controls = controls;
		this.inputs = new List<InputInTime>();
		this.inputHandlers = new Dictionary<string, EventHandler<InputInTime>>();
	} // Replayer

	public virtual void RegisterHandler(string button, EventHandler<InputInTime> handler) {
		try {
			inputHandlers[button] += handler;
		} catch (KeyNotFoundException) {
			inputHandlers.Add(button, handler);
		} // try
	} // RegisterHandler

	public virtual void RemoveHandler(string button, EventHandler<InputInTime> handler) {
		if (inputHandlers.ContainsKey(button)) {
			inputHandlers[button] -= handler;
		} // if
	} // RemoveHandler

	public virtual void RecordInput(InputInTime input) {
		inputs.Add(input);
		ExecuteInput(input);
	} // RecordInput

	public virtual void ExecuteInput(InputInTime input) {
		try {
			EventHandler<InputInTime> handler = inputHandlers[input.button];
			handler(this, input);
		} catch (KeyNotFoundException knfe) {
			Debug.LogError($"Did not find a handler for {input.button}.", this.controls);
			Debug.LogException(knfe, this.controls);
		} // try
	} // ExecuteInput

	public virtual IEnumerator Replay() {
		for (int i = 0; i < inputs.Count; ++i) {
			InputInTime input = inputs[i];

			// Convert the timestamp to a duration
			// Need only to wait the time /between/ inputs
			float inputOffset = input.timestamp;
			if (i > 0) {
				inputOffset -= inputs[i - 1].timestamp;
			} else {
				inputOffset -= timeToFirstInput;
			} // if

			yield return new WaitForSeconds(inputOffset);
			ExecuteInput(input);
		} // for
		CloneManager.instance.CloneCompleted();
	} // Replay
} // Replayer
