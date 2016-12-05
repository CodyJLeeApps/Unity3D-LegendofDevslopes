using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	[SerializeField] Transform target;
	[SerializeField] float smoothing = 5f;

	Vector3 offset; // offset between the player and camera

	void Awake() {

		// SerializeField Assertions
		Assert.IsNotNull(target);
		
	}

	// Use this for initialization
	void Start () {
		
		offset = (transform.position - target.position);

	}
	
	// Update is called once per frame
	void Update () {

		Vector3 targetCamPosition = (target.position + offset);
		transform.position = Vector3.Lerp(transform.position, targetCamPosition, (smoothing * Time.deltaTime));
	
	}
}
