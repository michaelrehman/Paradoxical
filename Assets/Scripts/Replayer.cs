using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Capable of recording and executing player input.</summary>
/// <seealso cref="InputInTime"/>
public class Replayer {

	/// <summary>Used to control when the inputs should be played back.</summary>
	/// <value>The timestamp at which <see cref="controls"/> should be synced with.</value>
	public float consideredActiveAt { get; }
	/// <summary>The <c>GameObject</c> this <c>Replayer</c> is recording inputs for.</summary>
	/// <value>Returns the value set in the constructor.</value>
	public GameObject controls { get; }

	/// <summary>A list of all of the recorded inputs.</summary>
	private List<InputInTime> inputs;
	/// <summary>A dictionary that map input button names to a corresponding handler.</summary>
	private Dictionary<string, EventHandler<InputInTime>> inputHandlers;

	/// <summary>Creates a new <c>Replayer</c> and initializes all relevant variables.</summary>
	/// <param name="consideredActiveAt">The timestamp at which <paramref name="controls"/> should be synced with.</param>
	/// <param name="controls">The <c>GameObject</c> instantiating this <c>Replayer</c>.</param>
	public Replayer(float consideredActiveAt, GameObject controls) {
		this.consideredActiveAt = consideredActiveAt;
		this.controls = controls;
		this.inputs = new List<InputInTime>();
		this.inputHandlers = new Dictionary<string, EventHandler<InputInTime>>();
	} // Replayer

	/// <summary>
	/// Registers <paramref name="handler"/> to be
	/// called when a <paramref name="button"/> event happens.
	/// </summary>
	/// <param name="button">The type of input event to respond to.</param>
	/// <param name="handler">The handler to be called when the event happens.</param>
	public virtual void RegisterHandler(string button, EventHandler<InputInTime> handler) {
		try {
			inputHandlers[button] += handler;
		} catch (KeyNotFoundException) {
			inputHandlers.Add(button, handler);
		} // try
	} // RegisterHandler

	/// <summary>
	/// Removes <paramref name="handler"/> so that it no longer
	/// is called when a <paramref name="button"/> event happens.
	/// </summary>
	/// <param name="button">The type of input event to remove <paramref name="handler"/> from.</param>
	/// <param name="handler">The handler to be removed from the input event.</param>
	public virtual void RemoveHandler(string button, EventHandler<InputInTime> handler) {
		if (inputHandlers.ContainsKey(button)) {
			inputHandlers[button] -= handler;
		} // if
	} // RemoveHandler

	/// <summary>Records and executes the player's input through the <paramref name="input"> snapshot.</summary>
	/// <param name="input">A snapshot of the necessary info to execute the player's input.</param>
	public virtual void RecordInput(InputInTime input) {
		inputs.Add(input);
		ExecuteInput(input);
	} // RecordInput

	/// <summary>Executes the player's input through the <paramref name="input"> snapshot.</summary>
	/// <param name="input">A snapshot of the necessary info to execute the player's input.</param>
	public virtual void ExecuteInput(InputInTime input) {
		try {
			EventHandler<InputInTime> handler = inputHandlers[input.button];
			handler(this, input);
		} catch (KeyNotFoundException knfe) {
			Debug.LogError($"Did not find a handler for {input.button}.", this.controls);
			Debug.LogException(knfe, this.controls);
		} // try
	} // ExecuteInput

	/// <summary>Replays all of the inputs that have been recorded.</summary>
	public virtual IEnumerator Replay() {
		for (int i = 0; i < inputs.Count; ++i) {
			InputInTime input = inputs[i];

			// Convert the timestamp to a duration
			// Need only to wait the time /between/ inputs
			float inputOffset = input.timestamp;
			if (i > 0) {
				inputOffset -= inputs[i - 1].timestamp;
			} else {
				inputOffset -= consideredActiveAt;
			} // if

			yield return new WaitForSeconds(inputOffset);
			ExecuteInput(input);
		} // for
		CloneManager.instance.CloneCompleted();
	} // Replay
} // Replayer
