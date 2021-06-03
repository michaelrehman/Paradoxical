using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replayer {

	public float startingTime { get; }
	public GameObject controls { get; }

	private List<InputInTime> inputs;
	private Dictionary<string, EventHandler<InputInTime>> inputHandlers;

	public Replayer(float startingTime, GameObject controls) {
		this.startingTime = startingTime;
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
		Debug.Log(inputs.Count);
		for (int i = 0; i < inputs.Count; ++i) {
			InputInTime input = inputs[i];

			// Convert the timestamp to a duration
			// Need only to wait the time /between/ inputs
			float inputOffset = input.timestamp - startingTime;
			if (i > 0) { inputOffset -= inputs[i - 1].timestamp; } // if

			yield return new WaitForSeconds(inputOffset);
			ExecuteInput(input);
		} // for
	} // Replay
} // Replayer
