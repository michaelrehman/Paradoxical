using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Singleton class.
public class CloneManager : MonoBehaviour {

	public static CloneManager instance;

	private List<CloneController> clones;
	private int completedClones;

	public float offset {
		get {
			try {
				return clones[0].replayer.timeToFirstInput;
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
		if (Input.GetButtonDown("Fire2") && instance.completedClones == instance.clones.Count) {
			instance.completedClones = 0;
			instance.clones.ForEach((clone) => {
				clone.ResetToInitialState();
				StartCoroutine(clone.replayer.Replay());
			});
		} // if
	} // Update

	public virtual void AddClone(CloneController clone) {
		// The most recent clone is going to be the player's,
		// so it is "complete" always; just need the player's
		// input to begin the replay.
		instance.clones.Add(clone);
		++instance.completedClones;
	} // AddClone

	public virtual void CloneCompleted() {
		++instance.completedClones;
	} // CloneCompleted

} // CloneManager
