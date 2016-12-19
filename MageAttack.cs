using UnityEngine;
using System.Collections;

public class MageAttack : MonoBehaviour {

	[SerializeField] private float range = 3f;
	[SerializeField] private float timeBetweenAttacks = 1f;
	[SerializeField] Transform fireLocation;

	private Animator anim;
	private GameObject player;
	private bool playerInRange;
	private EnemyHealth enemyHealth;
	private GameObject fireball;
	

	private const float ROTATION_SPEED = 15f;
	private const float FIREBALL_SPEED = 10.0f;

	// Use this for initialization
	void Start () {

		// get all needed components
		anim = GetComponent<Animator>();
		player = GameManager.instance.Player;
		enemyHealth = GetComponent<EnemyHealth> ();
		fireball = GameManager.instance.MageFireball;

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
			ThrowFireball();
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

	public void ThrowFireball() {
		GameObject  newFireball = Instantiate(fireball) as GameObject; // create a new fireball
		newFireball.transform.position = fireLocation.position;	// move it to the hand location
		newFireball.transform.rotation = transform.rotation;	// point the fireball towards the hero, 
																// which the enemy is already looking attack
		newFireball.GetComponent<Rigidbody>().velocity = transform.forward * FIREBALL_SPEED;
	}

} // End of EnemyAttack
// arrow augment
// start: 1.0
// end: 1.30