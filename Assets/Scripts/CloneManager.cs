using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Singleton class.
public class CloneManager : MonoBehaviour {

	public static CloneManager instance;
	public List<CloneController> clones;
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
		} // if
	} // Awake

	protected virtual void Update() {
		// TODO: prevent this from happening again until the last input is done
		if (Input.GetButtonDown("Fire2")) {
			instance.clones.ForEach((clone) => {
				clone.ResetToInitialState();
				StartCoroutine(clone.replayer.Replay());
			});
		} // if
	} // Update

	public virtual void AddClone(CloneController clone) {
		instance.clones.Add(clone);
	} // AddClone

} // CloneManager
