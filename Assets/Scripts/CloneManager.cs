using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Singleton class to manage all clones created by the player.</summary>
public class CloneManager : MonoBehaviour {

	/// <summary>The singleton of this class</summary>
	public static CloneManager instance;

	/// <summary>All clones managed by this class.</summary>
	/// <see cref="AddClone"/>
	private List<CloneController> clones;
	/// <summary>
	/// The number of clones that have completed their replay.
	/// This value should be compared against <c>instance.clones.Count</c>
	/// to check if all clones have finished their replays.
	/// </summary>
	private int completedClones;

	/// <summary>The offset for all clones to use at the start of their replays.</summary>
	/// <value>
	/// <c>0.0f</c> for the first clone and the first clone's
	/// <c>replayer.consideredActiveAt</c> for all other clones.
	/// </value>
	public float offset {
		get {
			try {
				return clones[0].replayer.consideredActiveAt;
			} catch (ArgumentOutOfRangeException) {
				return 0.0f;
			} // try
		} // get
	} // offset

	protected virtual void Awake() {
		if (instance == null) {
			instance = this;
			instance.clones = new List<CloneController>();
			instance.completedClones = 0;
		} // if
	} // Awake

	protected virtual void Update() {
		// Only allow replays when all clones are not currently replaying
		if (Input.GetButtonDown("Fire2") && instance.completedClones == instance.clones.Count) {
			instance.completedClones = 0; // replaying clones are not complete
			instance.clones.ForEach((clone) => {
				clone.ResetToInitialState();
				StartCoroutine(clone.replayer.Replay());
			});
		} // if
	} // Update

	/// <summary>Adds <paramref name="clone"/> to the list of clones to manage.</summary>
	/// <param name="clone">The clone to be added.</param>
	public virtual void AddClone(CloneController clone) {
		// The most recent clone is going to be the player's,
		// so it is "complete" always; just need the player's
		// input to begin the replay.
		instance.clones.Add(clone);
		++instance.completedClones;
	} // AddClone

	/// <summary>Updates the singleton's that a clone has finished its replay.</summary>
	public virtual void CloneCompleted() {
		++instance.completedClones;
	} // CloneCompleted

} // CloneManager
