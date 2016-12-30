using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {

	[SerializeField] private GameObject target;
	private Vector3 offset;
	private float cameraDamping = 5;

	// Use this for initialization
	void Start () {
		offset = target.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Do all camera scripting here
	void LateUpdate () {
		float currentAngle = transform.eulerAngles.y;
        float desiredAngle = target.transform.eulerAngles.y;
        float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * cameraDamping);
         
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        transform.position = target.transform.position - (rotation * offset);

		Vector3 tempVector = new Vector3(target.transform.position.x, (target.transform.position.y + 1), target.transform.position.z);

        transform.LookAt(tempVector);
	}

} // End ThirdPersonCamera
