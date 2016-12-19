using UnityEngine;
using UnityEngine.Assertions;

public class EnemyMove : MonoBehaviour {

	[SerializeField] NavMeshAgent nav;
	private Animator anim;
	private EnemyHealth enemyHealth;
	private Transform player;


	// Use this for initialization
	void Start () {
		nav = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		enemyHealth = GetComponent<EnemyHealth >();
		player = GameManager.instance.Player.transform;
	}
	
	void Awake() {
		
	}

	// Update is called once per frame
	void Update () {
		if(!GameManager.instance.GameOver && enemyHealth.IsAlive) {
			nav.SetDestination(player.position);
		} else if( (!GameManager.instance.GameOver || GameManager.instance.GameOver) && !enemyHealth.IsAlive ) {
			nav.enabled = false;
		} else { // if player has died, idle enemies
			nav.enabled = false;
			anim.Play("Idle");
		} 


	} // end Update

} // End EnemyMove