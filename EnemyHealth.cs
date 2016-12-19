using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

	[SerializeField] private int startingHealth = 20;
	[SerializeField] private float timeSinceLastHit = 0.5f;
	[SerializeField] private float dissapearSpeed = 2f;

	private AudioSource audio;
	private float timer = 0f;
	private Animator anim;
	private NavMeshAgent nav;
	private bool isAlive;
	private Rigidbody rigidBody;
	private CapsuleCollider capsule;
	private bool dissapearEnemy = false;
	private int currentHealth;
	private ParticleSystem blood;

	public bool IsAlive {
		get { return isAlive; }
	}

	// Use this for initialization
	void Start () {
		
		GameManager.instance.RegisterEnemy(this);
		audio = GetComponent<AudioSource>();
		anim = GetComponent<Animator>();
		nav = GetComponent<NavMeshAgent>();
		rigidBody = GetComponent<Rigidbody>();
		capsule = GetComponent<CapsuleCollider>();
		blood = GetComponentInChildren<ParticleSystem>();

		currentHealth = startingHealth;
		isAlive = true;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;

		if(dissapearEnemy) {
			transform.Translate(-Vector3.up * dissapearSpeed * Time.deltaTime);
		}
	}

	void OnTriggerEnter(Collider other) {
		if(timer >= timeSinceLastHit && !GameManager.instance.GameOver) {
			if(other.tag == "PlayerWeapon") {
				TakeHit();
				blood.Play();
				timer = 0f;
			}
		}
	}

	void TakeHit() {
		if(currentHealth > 0) {
			audio.PlayOneShot(audio.clip);
			anim.Play("Hit");
			currentHealth -= GameManager.instance.HeroStrength;
		}
		
		if(currentHealth <= 0) {
			isAlive = false;
			KillEnemy();
		}

	}

	void KillEnemy() {
		
		GameManager.instance.KilledEnemy(this);
		capsule.enabled = false;
		nav.enabled = false;
		anim.SetTrigger("EnemyDie");
		rigidBody.isKinematic = true;

		StartCoroutine(removeEnemy());
	}

	IEnumerator removeEnemy() {
		// wait 4 seconds after enemy dies
		yield return new WaitForSeconds(4f);
		// begin to sink the enemy off the map
		dissapearEnemy = true;
		yield return new WaitForSeconds(2f);
		Destroy(gameObject);
	}

} // End of EnemyHealth()
