using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	[SerializeField] GameObject player;
	[SerializeField] GameObject[] spawnPoints;
	[SerializeField] GameObject[] powerUpSpawns;
	// Enemies
	[SerializeField] GameObject tanker;
	[SerializeField] GameObject ranger_blue;
	[SerializeField] GameObject ranger_green;
	[SerializeField] GameObject ranger_red;
	[SerializeField] GameObject ranger_yellow;
	[SerializeField] GameObject soldier;
	[SerializeField] GameObject soldier_green;
	[SerializeField] GameObject soldier_red;
	[SerializeField] GameObject soldier_yellow;

	[SerializeField] GameObject skeleton_archer;
	[SerializeField] GameObject skeleton_grunt;
	[SerializeField] GameObject skeleton_mage;
	[SerializeField] GameObject skeleton_swordsman;
	[SerializeField] GameObject skeleton_king;

	[SerializeField] GameObject arrow;
	[SerializeField] GameObject mageFireball;
	[SerializeField] GameObject healthPowerUp;
	[SerializeField] GameObject speedPowerUp;
	[SerializeField] GameObject strengthPowerUp;
	[SerializeField] GameObject portal;
	[SerializeField] int maxPowerUps = 100;
	[SerializeField] int finalLevel = 20;
	// UI
	[SerializeField] Text levelText;
	[SerializeField] Text endGameText;
	[SerializeField] Text portalText;

	// Private Vars
	private bool gameOver = false;
	private int currentLevel;
	private float generatedSpawnTime = 1f;
	private float currentSpawnTime = 0f;
	private float powerUpSpawnTime = 45f;
	private float currentPowerUpSpawnTime = 0;
	private GameObject newEnemy;
	private GameObject newPowerUp;
	private int powerUps = 0;
	private int heroStrength = 10;
	private ParticleSystem portalFlame;
	private SphereCollider portalCollider;
	private int mapNumber = 1;
	private CharacterController characterController;

	private List<EnemyHealth> enemies = new List<EnemyHealth>();
	private List<EnemyHealth> killedEnemies = new List<EnemyHealth>();

	private int TIME_BETWEEN_LEVELS = 3;

	// Public Vars
	public static GameManager instance = null;
	
	// Getters / Setters
	public bool GameOver {
		get { return gameOver; }
	}

	public GameObject Player {
		get { return player; }
	}

	public GameObject Arrow {
		get { return arrow; }
	}

	public GameObject MageFireball {
		get { return mageFireball; }
	}

	public int HeroStrength {
		get { return heroStrength; }
		set {
			heroStrength = value;
		}
	}

	public int CurrentLevel {
		get { return currentLevel; }
		set {
			currentLevel = value;
		}
	}

	public int PowerUps {
		get { return powerUps; }
		set {
			powerUps = value;
		}
	}
	
	void Awake() {
		if(instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		endGameText.GetComponent<Text> ().enabled = false;
		portalText.GetComponent<Text>().enabled = false;

		portalFlame = portal.GetComponentInChildren<ParticleSystem>();
		portalFlame.Stop();
		portalCollider = portal.GetComponent<SphereCollider>();
		portalCollider.enabled = false;

		currentLevel = 1;
		// check which scene is loaded to assign MapNumber
		Scene scene = SceneManager.GetActiveScene();
		if(scene.name == "Level_1") {
			mapNumber = 1;	// Only Orc Enemies
		} else if(scene.name == "Level_2") {
			currentLevel = 1;
			mapNumber = 2;	// Only Skeleton Enemies
		}

		StartCoroutine(spawn());
		StartCoroutine(powerUpSpawn());
	}
	
	// Update is called once per frame
	void Update () {
		currentSpawnTime += Time.deltaTime;
		currentPowerUpSpawnTime += Time.deltaTime;
	}

	public void PlayerHit(int currentHP) {
		
		if(currentHP > 0) {
			gameOver = false;
		} else {
			gameOver = true;
			StartCoroutine(endGame("Defeat"));
		}
	} // End of PlayerHit

	public void RegisterEnemy(EnemyHealth enemy) {
		enemies.Add(enemy);
	}

	public void KilledEnemy(EnemyHealth enemy) {
		killedEnemies.Add(enemy);
	}

	public void RegisterPowerUp() {
		powerUps++;
	}

	IEnumerator spawn() {
		// check to see if the spawn time is greater than our set limit to spawn
		if(currentSpawnTime > generatedSpawnTime) {
			// reset the spawn time to spawn again
			currentSpawnTime = 0;

			createEnemy();

			if(killedEnemies.Count == currentLevel && currentLevel != finalLevel) {
				enemies.Clear();
				killedEnemies.Clear();
				
				yield return new WaitForSeconds(TIME_BETWEEN_LEVELS);
				currentLevel++;
				levelText.text = "Level " + currentLevel;
				yield return null;
				StartCoroutine(spawn());
			}

			if(killedEnemies.Count == finalLevel) {
				StartCoroutine(endGame("Victory!"));
				killedEnemies.Clear();
				//enemies.Clear();
			}

		} // end currentSpawnTime >

		yield return null;
		StartCoroutine (spawn ());
	} // End Spawn()

	IEnumerator powerUpSpawn() {
		
		if(currentPowerUpSpawnTime > powerUpSpawnTime) {
			currentPowerUpSpawnTime = 0f;
			if(powerUps < maxPowerUps) {
				
				// create location
				int randLocation = Random.Range(0, powerUpSpawns.Length - 1);
				GameObject powerUpLocation = powerUpSpawns[randLocation];

				// create power up
				int randNum = Random.Range(0,3); // choose type of power up
				if(randNum == 0) { 			// health
					newPowerUp = Instantiate(healthPowerUp) as GameObject;
				} else if(randNum == 1) { 	// speed
					newPowerUp = Instantiate(speedPowerUp) as GameObject;
				} else if(randNum == 2) {
					newPowerUp = Instantiate(strengthPowerUp) as GameObject;
				}
				newPowerUp.transform.position = powerUpLocation.transform.position;
			}
		}
		
		yield return null;
		StartCoroutine(powerUpSpawn());
	} // end powerUpSpawn()
	

	IEnumerator endGame(string outcome) {
		
		killedEnemies.Clear();
		endGameText.text = outcome;
		endGameText.GetComponent<Text>().enabled = true;
		yield return new WaitForSeconds(3f);
		endGameText.GetComponent<Text>().enabled = false;

		if(mapNumber == 1) {
			if(string.Equals(outcome, "Defeat")) {
				SceneManager.LoadScene("GameMenu");
			} else if(string.Equals(outcome,"Victory!")) {	// Victory, open portal to map 2
				portalFlame.Play(); // enable portal
				portalCollider.enabled = true;

				endGameText.text = "Portal Open!";
				portalText.GetComponent<Text>().enabled = true;
				yield return new WaitForSeconds(2f);
				portalText.GetComponent<Text>().enabled = false;
			}
		} else if(mapNumber == 2) {
			SceneManager.LoadScene("GameMenu");
		}

	} // END endGame

	void createEnemy() {
		// if our current number of enemies is less than the level number, spawn an enemy
		if(enemies.Count < currentLevel) {
			int randomNumber = Random.Range(0, spawnPoints.Length-1);
			GameObject spawnLocation = spawnPoints[randomNumber];
			int randomEnemy = 0;

			if(mapNumber == 1) { 

				if(currentLevel < 5) { // spawn only level 1 soldiers and archers
					
					
					Debug.Log("Level: " + currentLevel);
					randomEnemy = Random.Range(0, 2);
					Debug.Log("randEnemy: " + randomEnemy);
					if(randomEnemy == 0) {
						newEnemy = Instantiate(soldier) as GameObject;
					} else if(randomEnemy == 1) {
						newEnemy = Instantiate(ranger_blue) as GameObject;
					} 
					

				} else if(currentLevel == 5) { // allow level 1 enemies and Tanker
					
					Debug.Log("enemy number: " + enemies.Count);
					if(enemies.Count == 0) {
						newEnemy = Instantiate(tanker) as GameObject;
					} else {
						randomEnemy = Random.Range(0, 2);
						if(randomEnemy == 0) {
							newEnemy = Instantiate(soldier) as GameObject;
						} else if(randomEnemy == 1) {
							newEnemy = Instantiate(ranger_blue) as GameObject;
						}
						
					}

				} else if(currentLevel < 10) { // spawn only level 2 soldiers and archers

					Debug.Log("Level 2");
					randomEnemy = Random.Range(0, 4);
					if(randomEnemy == 0) {
						newEnemy = Instantiate(soldier) as GameObject;
					} else if(randomEnemy == 1) {
						newEnemy = Instantiate(ranger_blue) as GameObject;
					} else if(randomEnemy == 2) {
						newEnemy = Instantiate(soldier_green) as GameObject;
					} else if(randomEnemy == 3) {
						newEnemy = Instantiate(ranger_green) as GameObject;
					} 

				} else if(currentLevel == 10) { // allow level 2 enemies and Tanker

					Debug.Log("Level 2 + Tanker");
					randomEnemy = Random.Range(0, 5);
					if(randomEnemy == 0) {
						newEnemy = Instantiate(soldier) as GameObject;
					} else if(randomEnemy == 1) {
						newEnemy = Instantiate(ranger_blue) as GameObject;
					} else if(randomEnemy == 2) {
						newEnemy = Instantiate(soldier_green) as GameObject;
					} else if(randomEnemy == 3) {
						newEnemy = Instantiate(ranger_green) as GameObject;
					} else if(randomEnemy == 4) {
						newEnemy = Instantiate(tanker) as GameObject;
					}

				} else if(currentLevel < 15) { // spawn only level 3 soldiers and archers

					Debug.Log("Level 3");
					randomEnemy = Random.Range(0, 6);
					if(randomEnemy == 0) {
						newEnemy = Instantiate(soldier) as GameObject;
					} else if(randomEnemy == 1) {
						newEnemy = Instantiate(ranger_blue) as GameObject;
					} else if(randomEnemy == 2) {
						newEnemy = Instantiate(soldier_green) as GameObject;
					} else if(randomEnemy == 3) {
						newEnemy = Instantiate(ranger_green) as GameObject;
					} else if(randomEnemy == 4) {
						newEnemy = Instantiate(soldier_red) as GameObject;
					} else if(randomEnemy == 5) {
						newEnemy = Instantiate(ranger_red) as GameObject;
					}

				} else if(currentLevel == 15) { // allow level 3 enemies and Tanker

					Debug.Log("Level 3 + Tanker");
					randomEnemy = Random.Range(0, 7);
					if(randomEnemy == 0) {
						newEnemy = Instantiate(soldier) as GameObject;
					} else if(randomEnemy == 1) {
						newEnemy = Instantiate(ranger_blue) as GameObject;
					} else if(randomEnemy == 2) {
						newEnemy = Instantiate(soldier_green) as GameObject;
					} else if(randomEnemy == 3) {
						newEnemy = Instantiate(ranger_green) as GameObject;
					} else if(randomEnemy == 4) {
						newEnemy = Instantiate(soldier_red) as GameObject;
					} else if(randomEnemy == 5) {
						newEnemy = Instantiate(ranger_red) as GameObject;
					} else if(randomEnemy == 6) {
						newEnemy = Instantiate(tanker) as GameObject;
					}

				} else if(currentLevel <= 20) { // allow all enemies

					Debug.Log("Level 4");
					randomEnemy = Random.Range(0, 8);
					if(randomEnemy == 0) {
						newEnemy = Instantiate(soldier) as GameObject;
					} else if(randomEnemy == 1) {
						newEnemy = Instantiate(ranger_blue) as GameObject;
					} else if(randomEnemy == 2) {
						newEnemy = Instantiate(soldier_green) as GameObject;
					} else if(randomEnemy == 3) {
						newEnemy = Instantiate(ranger_green) as GameObject;
					} else if(randomEnemy == 4) {
						newEnemy = Instantiate(soldier_red) as GameObject;
					} else if(randomEnemy == 5) {
						newEnemy = Instantiate(ranger_red) as GameObject;
					} else if(randomEnemy == 6) {
						newEnemy = Instantiate(soldier_yellow) as GameObject;
					} else if(randomEnemy == 7) {
						newEnemy = Instantiate(ranger_yellow) as GameObject;
					}

				} else if(currentLevel == 20) { // allow all enemies and tanker

					Debug.Log("Level 4 + Tanker");
					randomEnemy = Random.Range(0, 9);
					if(randomEnemy == 0) {
						newEnemy = Instantiate(soldier) as GameObject;
					} else if(randomEnemy == 1) {
						newEnemy = Instantiate(ranger_blue) as GameObject;
					} else if(randomEnemy == 2) {
						newEnemy = Instantiate(soldier_green) as GameObject;
					} else if(randomEnemy == 3) {
						newEnemy = Instantiate(ranger_green) as GameObject;
					} else if(randomEnemy == 4) {
						newEnemy = Instantiate(soldier_red) as GameObject;
					} else if(randomEnemy == 5) {
						newEnemy = Instantiate(ranger_red) as GameObject;
					} else if(randomEnemy == 6) {
						newEnemy = Instantiate(soldier_yellow) as GameObject;
					} else if(randomEnemy == 7) {
						newEnemy = Instantiate(ranger_yellow) as GameObject;
					} else if(randomEnemy == 8) {
						newEnemy = Instantiate(tanker) as GameObject;
					}

				}

			} else if(mapNumber == 2) { // use skeleton enemies only

				if(currentLevel < (finalLevel / 4)) { // spawn only level 1
					
					Debug.Log("Skeleton Archer (1 - 4)");
					newEnemy = Instantiate(skeleton_archer) as GameObject;

				} else if(currentLevel == (finalLevel / 4)) { // allow level 1
					
					Debug.Log("Skeleton Archer and Grunt (5)");
					randomEnemy = Random.Range(0, 2);
					if(randomEnemy == 0) {
						newEnemy = Instantiate(skeleton_archer) as GameObject;
					} else if(randomEnemy == 1) {
						newEnemy = Instantiate(skeleton_grunt) as GameObject;
					} 

				} else if(currentLevel < (finalLevel / 2)) {

					Debug.Log("Skeleton Archer, Swordsman (6 - 9)");
					randomEnemy = Random.Range(0, 2);
					if(randomEnemy == 0) {
						newEnemy = Instantiate(skeleton_archer) as GameObject;
					} else if(randomEnemy == 1) {
						newEnemy = Instantiate(skeleton_swordsman) as GameObject;
					} 

				} else if(currentLevel == (finalLevel / 2)) { // allow level 2 enemies and grunt

					Debug.Log("Skeleton Archer, Swordsman, Grunt (10)");
					randomEnemy = Random.Range(0, 3);
					if(randomEnemy == 0) {
						newEnemy = Instantiate(skeleton_archer) as GameObject;
					} else if(randomEnemy == 1) {
						newEnemy = Instantiate(skeleton_swordsman) as GameObject;
					} else if(randomEnemy == 2) {
						newEnemy = Instantiate(skeleton_grunt) as GameObject;
					} 

				} else if(currentLevel <= (finalLevel*(3/4))) { // spawn only level 3 soldiers and archers

					Debug.Log("Skeleton Archer, Swordsman, Mage (11 - 15)");
					randomEnemy = Random.Range(0, 3);
					if(randomEnemy == 0) {
						newEnemy = Instantiate(skeleton_archer) as GameObject;
					} else if(randomEnemy == 1) {
						newEnemy = Instantiate(skeleton_swordsman) as GameObject;
					} else if(randomEnemy == 2) {
						newEnemy = Instantiate(skeleton_mage) as GameObject;
					} 

				} else if(currentLevel <= finalLevel) { // allow all enemies and tanker

					Debug.Log("Skeleton Archer, Swordsman, Mage, Grunt (16 - 19)");
					randomEnemy = Random.Range(0, 4);
					if(randomEnemy == 0) {
						newEnemy = Instantiate(skeleton_archer) as GameObject;
					} else if(randomEnemy == 1) {
						newEnemy = Instantiate(skeleton_swordsman) as GameObject;
					} else if(randomEnemy == 2) {
						newEnemy = Instantiate(skeleton_mage) as GameObject;
					} else if(randomEnemy == 3) {
						newEnemy = Instantiate(skeleton_grunt) as GameObject;
					} 

				} else if(currentLevel == finalLevel) { // allow all enemies and tanker

					Debug.Log("King + All (20)");
					if(enemies.Count == 1) {
						newEnemy = Instantiate(skeleton_king) as GameObject;
					} else {
						randomEnemy = Random.Range(0, 4);
						if(randomEnemy == 0) {
							newEnemy = Instantiate(skeleton_archer) as GameObject;
						} else if(randomEnemy == 1) {
							newEnemy = Instantiate(skeleton_swordsman) as GameObject;
						} else if(randomEnemy == 2) {
							newEnemy = Instantiate(skeleton_mage) as GameObject;
						} else if(randomEnemy == 3) {
							newEnemy = Instantiate(skeleton_grunt) as GameObject;
						} 
					}

				}

			} // end Map 2 enemy Spawns

			newEnemy.transform.position = spawnLocation.transform.position;
		} // end count enemies

	} // End createEnemy

} // end of GameManager()