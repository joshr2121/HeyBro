using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {

	//public SequenceControls player;	// player script
	public SequenceControls player; 
	public EnemyControls enemy; 	// enemy script
	public Sequence_Queue seqQueueLeft;
	public Sequence_Queue seqQueueRight;
	public float seqObjectCloseEnoughDistance;

	public int turn; 
	public bool charging;
	public bool playersTurn;

	public bool hi5; 				// begin and end a battle with a hi5
	public bool seqGenerated; 		// true if a sequence has been generated but not completed 

	public enum reaction { block, counter, fail };

	public float responseTime; 

	public AudioSource srcSeqSound;
	public AudioClip clipMoveSuccess;
	public AudioClip clipWholeSeqSuccess;
	
	public AudioSource srcRobot;
	public AudioSource srcPlayersDie;

	void Start () {
		hi5 = true;
		startPlayerTurn ();
		seqObjectCloseEnoughDistance = 0.4f;
	}

	void Update () {
		
	/* --------------------------------------------------------------------------------------------------------------------------
	 * ~ PLAYER'S TURN
	 * (1)	Generate a sequence
	 * (2)	Check if player succeeds current move
	 * (3)	check if finished seq
	 * (4)	if finished, check #correct moves = compare with number of moves in the sequence => deal damage to enemy
	 * (5)	check enemy HP (if 0 => win game)
	 * 
	 * ~ ENEMY'S TURN 
	 * (6)	randomly generate an attack	
	 * (7)	if player counters within counter window => damage enemy
	 * (8)	else if player blocks within block window => nothing happens
	 * (9)	else if player fails => damage player
	 * 
	 * ~ GENERAL CHECKS
	 * (10)	check enemy HP (if 0 => win game)
	 * (11)	check player HP (if 0 => lose game)
	 * PLAYER'S TURN AGAIN
	 * -------------------------------------------------------------------------------------------------------------------------- */


	}

	void FixedUpdate(){
		responseTime += Time.deltaTime; 

		if (hi5){
			if (playersTurn) playerTurn ();
			else enemyTurn ();
		}

		//else if (player.detectedA == 1 && player.detectedB == 4){
		else if (player.palmA && player.palmB){
			hi5 = true; 
		}	

		if (enemy.hp <= 0){
			Debug.LogWarning ("Win");
			Invoke ("loadSplashScreen", 5.0f);
//			Application.LoadLevel("Win");
		}

		else if (player.hp <= 0){
			Debug.LogWarning ("Lose");
//			Application.LoadLevel("Lose");
		}
//		player.attacking = true; 
//		player.defending = false; 
	}

	private void startPlayerTurn () {
		playersTurn = true;
		seqQueueLeft.movingSpritesDown = true;
		seqQueueRight.movingSpritesDown = true;
		seqGenerated = false;
		GameObject.Find ("Player_Left").GetComponent<PlayerAnim>().SetSprite (-1);
		GameObject.Find ("Player_Right").GetComponent<PlayerAnim>().SetSprite (-1);
	}
	
	private void startEnemyTurn () {
		playersTurn = false;
		Invoke ("createBlockSequence", 2.0f);
		//seqQueueLeft.movingSpritesDown = false;
		//seqQueueRight.movingSpritesDown = false;
	}
	
	private void createBlockSequence () {
		player.generateBlockSequence ();
		seqQueueLeft.LoadSequence (player.contactA, player.seqDelay);
		seqQueueRight.LoadSequence (player.contactB, player.seqDelay);
		//Hax!
		Invoke ("checkBlocked", 4.0f);
	}
		
	private void checkBlocked () {
		if (!player.blocked) {
			player.hp -= 20;
		}
		if (player.hp <= 0) {
			srcPlayersDie.Play ();
			Invoke ("loadSplashScreen", 5.0f);			
		}
		else {
			Invoke ("startPlayerTurn", 5.0f);
		}
	}

	private void playerTurn(){
		// (1) generate a sequence
		if (!seqGenerated){
			player.generateSeqParams(); 
			player.generateSequence(player.currentSeq);
			seqQueueLeft.LoadSequence (player.contactA, player.seqDelay);
			seqQueueRight.LoadSequence (player.contactB, player.seqDelay);
			seqGenerated = true; 
			turn++;
		}
		else {
			if (pictogramsTooLow ()) {
				for (int i = 0; i < 6; i++) {
					seqQueueLeft.sequenceObjects[i].GetComponent<SpriteRenderer>().enabled = false;
					seqQueueRight.sequenceObjects[i].GetComponent<SpriteRenderer>().enabled = false;
				}	
				startEnemyTurn ();
			}
			else if (player.checkBothEvents() && pictogramsInRange()){
				seqQueueLeft.sequenceObjects[player.correctMoves].GetComponent<SpriteRenderer>().enabled = false;
				seqQueueRight.sequenceObjects[player.correctMoves].GetComponent<SpriteRenderer>().enabled = false;
				player.correctMoves++;
				//player.generateNextMove();
			
				if (player.correctMoves < player.seqMoves) {
					srcSeqSound.clip = clipMoveSuccess;
					srcSeqSound.Play ();
					GameObject.Find ("Player_Left").GetComponent<PlayerAnim>().SetSprite (player.contactA[player.currentMove]);
					GameObject.Find ("Player_Right").GetComponent<PlayerAnim>().SetSprite (player.contactB[player.currentMove]);
					player.generateNextMove ();
				}
				else {
					srcSeqSound.clip = clipWholeSeqSuccess;
					srcSeqSound.Play ();
					GameObject.Find ("Player_Left").GetComponent<PlayerAnim>().SetSprite (3);
					GameObject.Find ("Player_Right").GetComponent<PlayerAnim>().SetSprite (3);
					enemy.DamageEnemy (player.seqDamage);
//					player.attacking = false;
//					player.defending = true;
					if (enemy.hp <= 0) {
						playersTurn = false;
						srcRobot.Play ();
					}
					else {
						startEnemyTurn ();
					}	
				}
			}

			/** Yeah fuck all this  
			player.seqMoves--; 
			// check if sequence is finished
			if (player.seqMoves <= 0){
				// check if all moves in sequence were correctly done  
				if (player.correctMoves >= player.seqMoves){ 
					//deal damage
				//	enemy.DamageEnemy(player.seqDamage);
				}
				player.seqGenerated = false; // to generate a new sequence 
				player.attacking = false;
				player.defending = true;
				//playersTurn = false;
			}
			*/			
		}
	}

	private void enemyTurn(){
		//enemy.generateAttack(); 
		responseTime = 0;
		//playerResponse(); 
		
		if (player.checkBothEvents() && pictogramsInRange()) {
			player.blocked = true;
			seqQueueLeft.sequenceObjects[0].GetComponent<SpriteRenderer>().enabled = false;
			seqQueueRight.sequenceObjects[0].GetComponent<SpriteRenderer>().enabled = false;
		}
		
		if (pictogramsTooLow ()) {
			for (int i = 0; i < 6; i++) {
				seqQueueLeft.sequenceObjects[i].GetComponent<SpriteRenderer>().enabled = false;
				seqQueueRight.sequenceObjects[i].GetComponent<SpriteRenderer>().enabled = false;
			}
			
		}
		
	}

	private void playerResponse(){
		int resp = player.enemyResponse(); 
		switch (resp){
			case (int) reaction.counter:
				if (responseTime <= enemy.attackParams[(int) enemy.currentAttack][3]){
					gameObject.SendMessage("DamageEnemy", player.counterDamage); 
					responseTime = 0; 
				}
				break;


			case (int) reaction.fail:
				if (responseTime <= enemy.attackParams[(int) enemy.currentAttack][2]){
				//	player.hp -= (int) enemy.attackParams[(int) enemy.currentAttack][0]; 
					responseTime = 0;
				}
				break; 

			case (int) reaction.block:
				break;

			default:
				break; 
		}
	}
	
	private bool pictogramsInRange () {
		//Just check left, they're the same
		return (Mathf.Abs (seqQueueLeft.sequenceObjects[player.currentMove].transform.localPosition.y) <= seqObjectCloseEnoughDistance);
	}
	
	private bool pictogramsTooLow () {
		return ((seqQueueLeft.sequenceObjects[player.currentMove].transform.localPosition.y) < -1*seqObjectCloseEnoughDistance);
	}
	
	private void loadSplashScreen () {
		Application.LoadLevel ("SplashScreenScene");
	}

}
