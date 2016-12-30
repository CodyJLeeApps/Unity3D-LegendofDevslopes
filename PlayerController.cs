using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	[SerializeField] private float moveSpeed = 6;
	[SerializeField] private float cameraTurnSpeed = 2f;
	[SerializeField] private LayerMask layerMask;

	private CharacterController characterController;
	private Vector3 currentLookTarget = Vector3.zero;
	private BoxCollider[] swordColliders;

	private Animator anim;
	private GameObject fireTrail;
	private ParticleSystem fireTrailParticles;
	private GameObject berzerkFire;
	private ParticleSystem berzerkFireParticles;

	private const int NORM_STRENGTH = 10;
	private const int POWER_STRENGTH = 30;

	public CharacterController CharacterController {
		get { return characterController; }
		set {
			characterController = value;
		}
	}

	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController>();
		anim = GetComponent<Animator>();
		swordColliders = GetComponentsInChildren<BoxCollider>();
		fireTrail = GameObject.FindWithTag("Fire") as GameObject;
		fireTrail.SetActive(false);
		berzerkFire = GameObject.FindWithTag("BerzerkFire") as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(!GameManager.instance.GameOver) {
			Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = Camera.main.transform.TransformDirection(moveDirection);
			characterController.SimpleMove(moveDirection * moveSpeed);

			// Handle walking logic
			if(moveDirection == Vector3.zero) {
				anim.SetBool("IsWalking", false);
			} else {
				anim.SetBool("IsWalking", true);
			}

			// ***** handle attack logic *****
			if(Input.GetMouseButtonDown(0)) { // triggered by left mouse click
				anim.Play("DoubleChop"); // Light single chop attack
			}

			if(Input.GetMouseButtonDown(1)) { // triggered by right mouse click
				anim.Play("SpinAttack"); // Heavy double chop attack
			}
		} // end if !GameOver
		
		// Handle running logic

	}

	// FixedUpdate() should be used for physics
	void FixedUpdate() {

		if(!GameManager.instance.GameOver) {
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
		} // end if !GameOver

	} // End FixedUpdate()

	public void BeginAttack() {
		foreach(var weapon in swordColliders) {
			weapon.enabled = true;
		}
	}

	public void EndAttack() {
		foreach(var weapon in swordColliders) {
			weapon.enabled = false;
		}
	}

	public void SpeedPowerUp() {
		StartCoroutine(fireTrailRoutine());
	}

	public void StrengthPowerUp() {
		StartCoroutine(heroGains());
	}

	IEnumerator fireTrailRoutine() {
		fireTrail.SetActive(true);
		moveSpeed = 10f;
		yield return new WaitForSeconds(10f);
		moveSpeed = 6f;

		fireTrailParticles = fireTrail.GetComponent<ParticleSystem>();
		var em = fireTrailParticles.emission;
		em.enabled = false;
		yield return new WaitForSeconds(3f);
		fireTrail.SetActive(false);
		em.enabled = true;
	}

	IEnumerator heroGains() {
		GameManager.instance.HeroStrength = POWER_STRENGTH;
		berzerkFireParticles = berzerkFire.GetComponent<ParticleSystem>();
		berzerkFireParticles.Play();
		yield return new WaitForSeconds(10);
		GameManager.instance.HeroStrength = NORM_STRENGTH;
		berzerkFireParticles.Stop();
	}

}
