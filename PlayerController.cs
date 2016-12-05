using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	[SerializeField] private float moveSpeed = 10;
	[SerializeField] private float cameraTurnSpeed = 10f;
	[SerializeField] private LayerMask layerMask;

	private CharacterController characterController;
	private Vector3 currentLookTarget = Vector3.zero;

	private Animator anim;




	// Use this for initialization
	void Start () {

		characterController = GetComponent<CharacterController>();
		anim = GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		characterController.SimpleMove(moveDirection * moveSpeed);

		// Handle walking logic
		if(moveDirection == Vector3.zero) {
			anim.SetBool("IsWalking", false);
		} else {
			anim.SetBool("IsWalking", true);
		}

		// ***** handle attack logic *****
		if(Input.GetMouseButtonDown(0)) { // triggered by left mouse click
			anim.Play("Chop"); // Light single chop attack
		}

		if(Input.GetMouseButtonDown(1)) { // triggered by right mouse click
			anim.Play("DoubleChop"); // Heavy double chop attack
		}

		/*
		if(Input.GetKeyDown("Keypad1")) { // triggered by holding LeftControl AND left mouse click
			anim.Play("SpinAttack"); // special spin attack
		}
		*/


		
		// Handle running logic

	}

	// FixedUpdate() should be used for physics
	void FixedUpdate() {

		RaycastHit hit; // the point that the raypoint is hitting the layer mask
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //

		Debug.DrawRay(ray.origin, ray.direction * 500, Color.blue);

		if(Physics.Raycast(ray, out hit, 500, layerMask, QueryTriggerInteraction.Ignore)) {
			
			if(hit.point != currentLookTarget) {
				currentLookTarget = hit.point;
			}

			Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
			Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * cameraTurnSpeed);
		}

	} // End FixedUpdate()

}
