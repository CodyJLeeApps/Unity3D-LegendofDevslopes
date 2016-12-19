using UnityEngine;
using System.Collections;

public class LavaDeath : MonoBehaviour {

	[SerializeField] float spinSpeed = 10f;

	private GameObject player;
	private PlayerHealth playerHealth;

	// Use this for initialization
	void Start () {
		
		player = GameManager.instance.Player;
		playerHealth = player.GetComponent<PlayerHealth>();

	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
	}

	void OnTriggerEnter(Collider other) {
		
		if(other.tag == "Player") {
			Debug.Log("Killing player");
			GameManager.instance.PlayerHit(0);
			playerHealth.KillPlayer();
			// stop player height
			// stop camer follow
			// sloly lower player into the lava
		}

	}

}
