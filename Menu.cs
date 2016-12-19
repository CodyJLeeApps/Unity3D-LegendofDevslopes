using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	[SerializeField] GameObject Hero;
	[SerializeField] GameObject Tanker;
	[SerializeField] GameObject Ranger;
	[SerializeField] GameObject Soldier;

	private Animator heroAnim;
	private Animator tankerAnim;
	private Animator rangerAnim;
	private Animator soldierAnim;

	void Awake() {
		Assert.IsNotNull(Hero);
		Assert.IsNotNull(Tanker);
		Assert.IsNotNull(Soldier);
		Assert.IsNotNull(Ranger);
	}

	// Use this for initialization
	void Start () {
		heroAnim = Hero.GetComponent<Animator>();
		tankerAnim = Tanker.GetComponent<Animator>();
		rangerAnim = Ranger.GetComponent<Animator>();
		soldierAnim = Soldier.GetComponent<Animator>();

		StartCoroutine(showCase());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Battle() {
		SceneManager.LoadScene("Level_1");

	}

	public void Quit() {
		Application.Quit();
	}

	IEnumerator showCase() {
		yield return new WaitForSeconds(3f);
		heroAnim.Play("SpinAttack");
		yield return new WaitForSeconds(1.5f);
		tankerAnim.Play("Attack");
		yield return new WaitForSeconds(1.5f);
		rangerAnim.Play("Attack");
		yield return new WaitForSeconds(1.5f);
		soldierAnim.Play("Attack");
		yield return new WaitForSeconds(3f);
		StartCoroutine(showCase());
	}

}
