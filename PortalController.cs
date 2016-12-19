using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour {

	void OnTriggerEnter(Collider other) {

		if(other.tag == "Player") {
			SceneManager.LoadScene("Level_2");
		} else {
			Debug.Log(other.tag);
		}

	}
}
