using UnityEngine;
using System.Collections;

public class EnemyControls : MonoBehaviour {

	public SequenceControls bros; 

	public int hp;
	public int maxHP;
	public int currentAttack; 
	private enum attack { smack, siphoningGrasp, laserStrike };
	public bool charging; 
	public bool attacking; 
	public int chargeTurn;
	public int turn; 

	// 0: attack damage, 1: animation length, 2: block window, 3: counter window 
	public float[][] attackParams = new float[3][] { new float[] { 25, 2, .02f, .01f }, new float[]{ 20, 2, .03f, .01f }, new float[] { 50, 3, .03f, .01f }};

	void Start () {
		maxHP = 400;
		hp = maxHP;
		charging = false; 
		attacking = false; 
	}
	
	void FixedUpdate () {
		
	}

	public void DamageEnemy(int damage){
		hp -= damage; 
	}

	public void enemyAttack(int atk){
		switch (atk){
			case (int) attack.smack:

				break;

			case (int) attack.siphoningGrasp:
				break;

			case (int) attack.laserStrike:
				if (turn > chargeTurn){
					// attack!
				}
				break; 

			default:
				break; 
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. NO RETURN. 
	 * 
	 * -------------------------------------------------------------------------------------------------------------------------- */

	public void generateAttack(){

		if (!charging){
			int atk;
			atk = Random.Range (0, 100);

			// 20% chance to get laser strike
			if (atk < 20){
				currentAttack = (int) attack.laserStrike;
				charging = true; 
				chargeTurn = turn; 
			}
			// 30% chance to get siphoning grasp
			else if (atk < 50){
				currentAttack = (int) attack.siphoningGrasp; 
			}
			// 50% chance to get smack 
			else {
				currentAttack = (int) attack.smack; 
			}
		}
	}
}
