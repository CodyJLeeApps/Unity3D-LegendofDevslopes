using UnityEngine;
using System.Collections;

public class fireball : MonoBehaviour {

	void OnCollisionEnter(Collision col) {
		Destroy(gameObject);
	}
}
