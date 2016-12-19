using UnityEngine;

public class PowerUpSpin : MonoBehaviour {

	[SerializeField] float spinSpeed = 75f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
	}
}
