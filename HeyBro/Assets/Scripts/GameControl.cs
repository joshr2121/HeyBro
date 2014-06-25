using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {

	public SequenceControls player;	// player script
	public EnemyControls enemy; 	// enemy script 

	public int turn; 
	public bool charging; 

	public bool hi5; 				// begin and end a battle with a hi5
	public bool seqGenerated; 		// true if a sequence has been generated but not completed 
	

	void Start () {
	
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
	 * // (6)	randomly generate an attack
	 *	
	 * (7)	if player counters within counter window => damage enemy
	 * (8)	else if player blocks within block window => nothing happens
	 * (9)	else if player fails => damage player
	 * 
	 * ~ GENERAL CHECKS
	 * (10)	check enemy HP (if 0 => win game)
	 * (11)	check player HP (if 0 => lose game)
	 * PLAYER'S TURN AGAIN
	 * -------------------------------------------------------------------------------------------------------------------------- */

		if (hi5){
			playerTurn(); 
		}

		else if (player.detectedA == 1 && player.detectedB == 4){
			hi5 = true; 
		}	


	}

	private void playerTurn(){
		// (1) generate a sequence
		if (!seqGenerated){
			player.generateSeqParams(); 
			player.generateSequence(player.currentSeq);
			seqGenerated = true; 
			turn++; 	
		}	
		else {
			if (player.checkBothEvents()){
			 	player.correctMoves++; 
			}
			player.seqMoves--; 
			// check if sequence is finished 
			if (player.seqMoves <= 0){
				// check if all moves in sequence were correctly done  
				if (player.correctMoves >= player.seqMoves){ 
					//deal damage
					gameObject.SendMessage("DamageEnemy", player.seqDamage);
				}
				player.seqGenerated = false; // to generate a new sequence 
				player.attacking = false;
				player.defending = true; 
			}
		}
	}

}
