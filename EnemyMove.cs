using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class EnemyMove : MonoBehaviour {

	[SerializeField] Transform player;
	[SerializeField] NavMeshAgent nav;
	private Animator anim;


	// Use this for initialization
	void Start () {
		nav = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
	}
	
	void Awake() {

		Assert.IsNotNull(player);
	}

	// Update is called once per frame
	void Update () {
		
		nav.SetDestination(player.position);


	}
}
