using UnityEngine;

public class Arrow : MonoBehaviour {

	void OnCollisionEnter(Collision col) {
		Destroy(gameObject);
	}
}
