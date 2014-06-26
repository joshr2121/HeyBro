using UnityEngine;
using System.Collections;
using System.IO.Ports; 

// NB: STILL HAVEN'T IMPLEMENTED THE DELAY BETWEEN MOVES IN SEQUENCE

public class SequenceControls : MonoBehaviour {

	// ARDUINO STUFF ("PORT" is not right)
	SerialPort sp = new SerialPort("PORT", 9600);
	public byte[] byteBuffer; 
	public int byteOffset;
	public int byteCount; 
	
	// TO CREATE THE EVENTS THAT WILL BE CHECKED
	public enum touch { palm, fist, elbow }; 
	public int[] contactA;
	public int[] contactB; 
	public int detectedA;
	public int detectedB; 
	public int minEnum = 0; 	// first index of the enum
	public int maxEnum = 3;	// number of elements in the enum

	// CONTACT INPUTS (person A and person B)
	public bool palmA; 		// these will correspond to specific button inputs 
	public bool fistA;
	public bool elbowA;

	public bool palmB; 
	public bool fistB;
	public bool elbowB;

	// TO CHECK THAT THE RIGHT CONTACT WAS MADE
	public bool touchDetectedA;
	public bool touchDetectedB; 

	public bool hi5; 				// begin and end a battle with a hi5
	public bool seqGenerated; 		// true if a sequence has been generated but not completed 

	// TO KEEP TRACK OF CURRENT MOVE
	public enum sequence { three, fourA, fourB, fiveA, fiveB, six }; 
	public int currentSeq; 		// will take one of the enum values/indeces
	public int seqMoves;			// which sequence type within the sequence enum
	public int seqDamage; 			// damage to deal if succeed sequence
	public float seqDelay; 		// delay between moves in current sequence
	public float seqWindow; 		// response window for current seq

	public int currentMove; 		// the move we're at in the current sequence, used as index for contactA/contactB arrays to get the move we want 
	public int correctMoves; 		// number of correctly done moves in the current sequence
	public float currentSeqTime; 	// currently accumulated time since the sequence began
	
	// PLAYER STUFF
	public int hp = 100; 			
	public bool attacking;			
	public bool defending; 
	public enum reaction { block, counter, fail };

	public int counterDamage = 10;
	public int turn;  

	// ENEMY STUFF
	public EnemyControls enemy; 




	// 0: num moves per sequence, 1: dmg, 2: delay, 3: window
	public float[][] sequences = 	new float[4][] { new float[] { 3, 40, .9f, .175f }, new float[]{ 4, 75, .8f, .150f }, 
													 new float[] { 5, 100, .75f, .150f }, new float[] { 6, 150, .7f, .125f }};	

	void Start(){
	
		// ARDUINO STUFF
		sp.Open();				// open the port
		sp.ReadTimeout = 1; 	// how often unity checks (throws exception if isn't open)

		touchDetectedA = false; 
		touchDetectedB = false;

		hi5 = false; 

		hp = 100;
		attacking = true;
		defending = false;

		turn = 0; 


	}

	void Update(){
		
	}

	void FixedUpdate(){
		currentSeqTime += Time.deltaTime; 

		readFromArduino(); 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. NO RETURN.
	 * (1) read inputs from arduino
	 * (2) check if the first byte corresponds to player A or B
	 * (3) if corresponded to player A, find the first different byte, set it to player B
	 * (4) otherwise, do the same but to player A
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	 public void readFromArduino(){
	 	// (1) read from arduino into an array
	 	sp.Read(byteBuffer, byteOffset, byteCount);

	 	// (2) check if current byte 
	 	if (byteBuffer[0] < 4){
	 		detectedA = byteBuffer[0];
	 		touchDetectedA = true; 
	 	}
	 	else {
	 		detectedB = byteBuffer[0];
	 		touchDetectedB = true; 
	 	}
	 	// (3)
	 	if (touchDetectedA){
	 		int i = 1; 
	 		while ((int) byteBuffer[i] == detectedA){
	 			i++;
	 		}
	 		detectedB = byteBuffer[i];
	 		touchDetectedB = true; 
	 	}
	 	// (4)
	 	else if (touchDetectedB){
	 		int i = 1; 
	 		while ((int) byteBuffer[i] == detectedB){
	 			i++;
	 		}
	 		detectedA = byteBuffer[i];
	 		touchDetectedA = true; 
	 	}
	 }
	 /* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. NO RETURN.
	 * (1) check if players have highfived to begin the battle
	 * (2) check if there is a sequence that has been generated/is being done 
	 *		(2i)	generate a random sequence type and its corresponding parameters
	 *		(2ii) 	generate the moves in the sequence
	 * (3) if a sequence has already been generated - 
	 *		(3i)	check if the players did the current move correctly
	 *		(3ii)	reduce the number of moves left
	 * (4) if run out of moves in sequence (completed the sequence)
	 *		(4i)	if did all moves in sequence right, deal damage
	 * 		(4ii)	finished the sequence, so prepare to receive attack 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	/*public void battleProceed(){
		if (hi5) { 
			if (attacking){
				if (!seqGenerated){
					generateSeqParams(); 
					generateSequence(currentSeq); 
					seqGenerated = true; 
					turn++; 
				}
				else {
					if (checkBothEvents()){
					 	correctMoves++; 
					}
					seqMoves--; 
					// check if sequence is finished 
					if (seqMoves <= 0){
						// check if all moves in sequence were correctly done  
						if (correctMoves >= seqMoves){ 
							//deal damage
							gameObject.SendMessage("DamageEnemy", seqDamage);
						}
						seqGenerated = false; // to generate a new sequence 
						attacking = false;
						defending = true; 
					}
				}
			}
			else if (defending){

				int resp = enemyResponse(); 
				switch (resp){
					case (int) reaction.fail:
						// hp -= current enemy attack damage
						hp -= (int) enemy.attackParams[(int) enemy.currentAttack][0]; 
						break; 

					case (int) reaction.counter:
						gameObject.SendMessage("DamageEnemy", counterDamage); 
						break;
				}
			}
		}
		else if (detectedA == 1 && detectedB == 4){
			hi5 = true; 
		}
	}*/

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. NO RETURN.
	 * (1) generate a number of moves within the next seq of whatever speed
	 * (2) generate the delay between moves 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void generateNextMove(){
		if (currentSeqTime >= seqDelay){
			currentMove++; 
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. NO RETURN.
	 * (1) generate a number of moves within the next seq of whatever speed
	 * (2) generate the delay between moves 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void generateSeqParams(){
	 	currentSeq = Random.Range(0, 6); 
	 	switch (currentSeq){
	 		case (int) sequence.three:
	 			seqMoves 	= (int) sequences[0][0];
	 			seqDamage 	= (int) sequences[0][1];
	 			seqDelay 	= sequences[0][2];
	 			seqWindow 	= sequences[0][3];
	 			break;

	 		case (int) sequence.fourA:
	 			seqMoves 	= (int) sequences[1][0];
	 			seqDamage 	= (int) sequences[1][1];
	 			seqDelay 	= sequences[1][2];
	 			seqWindow 	= sequences[1][3];
	 			break;

	 		case (int) sequence.fourB:
	 			seqMoves 	= (int) sequences[1][0];
	 			seqDamage 	= (int) sequences[1][1];
	 			seqDelay 	= sequences[1][2];
	 			seqWindow 	= sequences[1][3];
	 			break;

	 		case (int) sequence.fiveA:
	 			seqMoves 	= (int) sequences[2][0];
	 			seqDamage 	= (int) sequences[2][1];
	 			seqDelay 	= sequences[2][2];
	 			seqWindow 	= sequences[2][3];
	 			break;

	 		case (int) sequence.fiveB:
	 			seqMoves 	= (int) sequences[2][0];
	 			seqDamage 	= (int) sequences[2][1];
	 			seqDelay 	= sequences[2][2];
	 			seqWindow 	= sequences[2][3];
	 			break;

	 		case (int) sequence.six:
	 			seqMoves 	= (int) sequences[3][0];
	 			seqDamage 	= (int) sequences[3][1];
	 			seqDelay 	= sequences[3][2];
	 			seqWindow 	= sequences[3][3];
	 			break;

	 		default: 
	 			break; 
	 	}
	 }

	 /* --------------------------------------------------------------------------------------------------------------------------
	 * ARG: current sequence type being executed 
	 * (1) initialize currentMove and correctMoves to 0 since starting a new seq
	 * (2) initialize contactA and contactB arrays of the appropriate size (number of moves to do within current sequence type)
	 * (3) generate a random command for each move in the array
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	public void generateSequence(int seq){
		currentMove = 0; 
		correctMoves = 0; 
		contactA = new int[seqMoves];
		contactB = new int[seqMoves]; 

		// generate a random move for each in the seq
		for (int i = 0; i < seqMoves; i++){
			contactA[i] = Random.Range(minEnum, maxEnum); 
			contactB[i] = Random.Range(minEnum, maxEnum); 
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. 
	 * RETURN: 
	 * - true if both players did the move that was asked
	 * - false otherwise 
	 * -------------------------------------------------------------------------------------------------------------------------- */

	 public bool checkBothEvents(){
	 	bool correctA = checkTouchA(contactA[currentMove]);
	 	bool correctB = checkTouchB(contactB[currentMove]); 

	 	if (correctA && correctB){
	 		return true; 
	 	}
		return false; 
	 }

	/* --------------------------------------------------------------------------------------------------------------------------
	 * ARG: the demanded contact input for player A
	 * (1) touchDetectedA bool set to TRUE;
	 * (2) if the player did a move within the window of time 
	 * (3) if hit the right contact then return true. 
	 * (4) Otherwise return false
	 * -------------------------------------------------------------------------------------------------------------------------- */

	public bool checkTouchA(int touchA){

		palmA 	= (detectedA == 1);
		fistA 	= (detectedA == 2);
		elbowA 	= (detectedA == 3);

		// (1) touch detected from player A
		touchDetectedA = true;

		// (2) check that hit within window
		if (currentSeqTime < seqWindow){
			// (3) if right input, return true
			switch (touchA){ 
				case (int) touch.palm:
					if (palmA){
						return true; 
					}
					break;

				case (int) touch.fist:
					if (fistA){
						return true; 
					}
					break;

				case (int) touch.elbow:
					if (elbowA){
						return true; 
					}
					break;

				default:
					break;

			}
		}
		// (4) if haven't returned true = wrong input (CHECK ANY KEY?)
		return false; 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * ARG: the demanded contact input for player B
	 * (1) touchDetectedB bool set to TRUE;
	 * (2) if the player did a move within the window of time 
	 * (3) if hit the right contact then return true. 
	 * (4) Otherwise return false
	 * -------------------------------------------------------------------------------------------------------------------------- */

	public bool checkTouchB(int touchB){

		palmB 	= (detectedB == 4);
		fistB 	= (detectedB == 5);
		elbowB 	= (detectedB == 6);

		// (1) touch detected from player B
		touchDetectedB = true; ; 

		// (2) if the players hit within the window of time 
		if (currentSeqTime < seqWindow){
			// (3) if right input, return true
			switch (touchB){
				case (int) touch.palm:
					if (palmB){
						return true; 
					}
					break;

				case (int) touch.fist:
					if (fistB){
						return true; 
					}
					break;

				case (int) touch.elbow:
					if (elbowB){
						return true; 
					}
					break;
			
				default:
					break;
			
			}
		}
		// (3) if haven't returned true = wrong input (CHECK ANY KEY?)
		return false; 
	}

	public int enemyResponse(){

		elbowA 	= (detectedA == 3);
		elbowB 	= (detectedB == 6);

		fistA 	= (detectedB == 2);
		fistB 	= (detectedB == 5);

		if (fistA && fistB){
			return (int) reaction.block; 
		}

		else if (elbowA && elbowB){
			return (int) reaction.counter; 
		}

		 else {
		 	return (int) reaction.fail; 
		 }
	}

	public void DamagePlayers(int damage){
		hp -= damage; 
	}

}