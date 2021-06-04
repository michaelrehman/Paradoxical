using System;
using UnityEngine;

/// <summary>Represents relevant data about player input at a point in time.</summary>
public class InputInTime : EventArgs {

	/// <summary>The timestamp at which this input occured.</summary>
	/// <value>The current value of <c>Time.time</c> when this instance was created.</value>
	public float timestamp { get; }
	/// <summary>The input type.</summary>
	/// <summary>Typically, the name of a button input as specified in Unity's Input Manager.</summary>
	public string button { get; }
	/// <summary>The mouse position when this input occured.</summary>
	/// <value>The world point at where the mouse was.</value>
	public Vector2 mousePosition { get; }
	/// <summary>How much left or right the player was moving.</summary>
	/// <value>The value of <c>Input.GetAxis("Horizontal")</c> at the time.</value>
	public float horizontalAxis { get; }
	/// <summary>How much up or down the player was moving.</summary>
	/// <value>The value of <c>Input.GetAxis("Vertical")</c> at the time.</value>
	public float verticalAxis { get; }

	/// <summary>Creates a snapshot of the necessary info to execute this input.</summary>
	/// <param name="button">The input type.</param>
	/// <param name="horizontalAxis">How much left or right the player was moving.</param>
	/// <param name="verticalAxis">How much up or down the player was moving.</param>
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
