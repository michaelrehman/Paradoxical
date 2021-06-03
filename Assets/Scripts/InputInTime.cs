using System;
using UnityEngine;

/// Represents when an input took place.
public class InputInTime : EventArgs {
	// Encapsulation to prevent modification
	public float timestamp { get; }
	public string button { get; }
	public Vector2 mousePosition { get; }
	public float horizontalAxis { get; }
	public float verticalAxis { get; }

	public InputInTime(string button, float horizontalAxis = 0f, float verticalAxis = 0f) {
		this.timestamp = Time.time;
		this.button = button;
		this.mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		this.horizontalAxis = horizontalAxis;
		this.verticalAxis = verticalAxis;
	} // InputInTime

	// TODO: create subclasses to track more specific information (or choose to assign defaults)
		// TODO: create a method to check if certain casts are valid
		// TODO: create factory `fromButton` method to handle creation
} // InputInTime
