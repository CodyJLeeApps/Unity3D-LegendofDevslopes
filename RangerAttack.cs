using UnityEngine;
using System.Collections;

public class RangerAttack : MonoBehaviour {

	[SerializeField] private float range = 3f;
	[SerializeField] private float timeBetweenAttacks = 1f;
	[SerializeField] Transform fireLocation;

	private Animator anim;
	private GameObject player;
	private bool playerInRange;
	private EnemyHealth enemyHealth;
	private GameObject arrow;

	private const float ROTATION_SPEED = 10f;
	private const float ARROW_SPEED = 25.0f;

	// Use this for initialization
	void Start () {

		// get all needed components
		anim = GetComponent<Animator>();
		player = GameManager.instance.Player;
		enemyHealth = GetComponent<EnemyHealth> ();
		arrow = GameManager.instance.Arrow;

		// Start coroutines
		StartCoroutine(attack());
	}
	
	// Update is called once per frame
	void Update () {
		
		if( (Vector3.Distance(transform.position, player.transform.position) < range) && enemyHealth.IsAlive ) {
			playerInRange = true;
			anim.SetBool("PlayerInRange", true);
			RotateTowards(player.transform);
		} else {
			playerInRange = false;
			anim.SetBool("PlayerInRange", false);
		}

	} // End Update

	IEnumerator attack() {
		if(playerInRange && !GameManager.instance.GameOver) {
			anim.Play("Attack");
			yield return new WaitForSeconds(timeBetweenAttacks); // adjust attack speed
		}
		yield return null;
		StartCoroutine(attack());
	}

	private void RotateTowards(Transform player) {
		Vector3 direction = (player.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * ROTATION_SPEED);
	}

	public void FireArrow() {
		GameObject  newArrow = Instantiate(arrow) as GameObject; // create a new arror
		newArrow.transform.position = fireLocation.position;	// move it to the crossbow location
		newArrow.transform.rotation = transform.rotation;		// point the arrow towards the hero, which the enemy is already looking attack
		newArrow.GetComponent<Rigidbody>().velocity = transform.forward * ARROW_SPEED;
	}

} // End of EnemyAttack
// arrow augment
// start: 1.0
// end: 1.30