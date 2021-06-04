using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Handles player inputs.</summary>
/// <seealso cref="Replayer"/>
public class CloneController : MonoBehaviour {

	/// <summary>A test dummy to test core functionality.</summary>
	public GameObject dummy;

	/// <summary>Denotes whether or not the player is in control.</summary>
	[SerializeField] private bool isPlayer;

	/// <summary>The starting position of this GameObject.</summary>
	private Vector2 startingPosition;
	/// <summary>The starting rotation of this GameObject.</summary>
	private Quaternion startingRotation;

	/// <summary>The <c>Replayer</c> that records and executes inputs for this clone.</summary>
	public Replayer replayer;

	protected virtual void Awake() {
		this.isPlayer = true;
		GetComponent<SpriteRenderer>().color = Color.red; // debug
		this.startingPosition = transform.position;
		this.startingRotation = transform.rotation;
	} // Awake

	protected virtual void Start() {
		// All the clones need to move together based on when the first clone started to move
		this.replayer = new Replayer(Time.time - CloneManager.instance.offset, this.gameObject);

		replayer.RegisterHandler("Fire1", (sender, eventArgs) => {
			GameObject copy = Instantiate(dummy, eventArgs.mousePosition, Quaternion.identity);
			Destroy(copy, 1);
		});
		replayer.RegisterHandler("HorzVertAxis", (sender, eventArgs) => {
			transform.Translate(new Vector2(eventArgs.horizontalAxis, eventArgs.verticalAxis) * 0.5f);
		});

		CloneManager.instance.AddClone(this);
	} // Start

	protected virtual void Update() {
		if (isPlayer) {
			// Record inputs if this is the player
			if (Input.GetButtonDown("Fire1")) {
				replayer.RecordInput(new InputInTime("Fire1"));
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

	/// <summary>
	/// Resets this clone to its initial state,
	/// prevents any additional inputs from the player, and
	/// spawns a new clone for the player to control.
	/// </summary>
	public virtual void ResetToInitialState() {
		// Spawn one and only one additional clone for the player to control
		if (isPlayer) {
			Instantiate(this.gameObject, Vector3.zero, Quaternion.identity);
			GetComponent<SpriteRenderer>().color = Color.white; // debug
		} // if

		isPlayer = false;
		transform.position = startingPosition;
		transform.rotation = startingRotation;
	} // ResetToInitialState

} // CloneController
