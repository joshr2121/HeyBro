using UnityEngine;
using System.Collections;
using System.IO.Ports; 

public class SequenceControls : MonoBehaviour {

	// ARDUINO STUFF ("PORT" is not right)
	SerialPort sp = new SerialPort("PORT", 9600);
	public byte[] byteBuffer; 
	public int byteOffset;
	public int byteCount; 
	
	// TO CREATE THE EVENTS THAT WILL BE CHECKED
	private enum touch { palm, fist, elbow }; 
	public int[] contactA;
	public int[] contactB; 
	public int detectedA;
	public int detectedB; 
	private int minEnum = 0; 	// first index of the enum
	private int maxEnum = 3;	// number of elements in the enum

	public float minSeqDelay = 0.7f;		// min delay between moves in seq of certain speed
	public float maxSeqDelay = 1.3f; 	// max delay between moves in seq of certain speed


	// CONTACT INPUTS (person A and person B)
	private bool palmA; 		// these will correspond to specific button inputs 
	private bool fistA;
	private bool elbowA;

	private bool palmB; 
	private bool fistB;
	private bool elbowB;

	// TO CHECK THAT THE RIGHT CONTACT WAS MADE
	private bool touchDetectedA;
	private bool touchDetectedB; 

	private bool hi5; 				// begin and end a battle with a hi5
	private bool seqGenerated; 		// true if a sequence has been generated but not completed 

	// TO KEEP TRACK OF CURRENT MOVE
	private enum sequence { three, fourA, fourB, fiveA, fiveB, six }; 
	private int currentSeq; 		// will take one of the enum values/indeces
	private int seqMoves;			// which sequence type within the sequence enum
	private int seqDamage; 			// damage to deal if succeed sequence
	private float seqDelay; 		// delay between moves in current sequence
	private float seqWindow; 		// response window for current seq

	private int currentMove; 		// the move we're at in the current sequence, used as index for contactA/contactB arrays to get the move we want 
	private int correctMoves; 
	private float currentSeqTime; 	// 
	
	// PLAYER STUFF
	private int hp = 100; 
	private bool attacking;
	private bool defending; 
	private enum reaction { block, counter, fail };




	// 0: num moves per sequence, 1: dmg, 2: delay, 3: window
	private float[][] sequences = 	new float[4][] { new float[] { 3, 40, .9f, .175f }, new float[]{ 4, 75, .8f, .150f }, 
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


	}

	void Update(){
		
	}

	void FixedUpdate(){
		currentSeqTime += Time.deltaTime; 

		readFromArduino(); 
		battleProceed(); 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. NO RETURN.
	 * (1) read inputs from arduino
	 * (2) check if the first byte corresponds to player A or B
	 * (3) if corresponded to player A, find the first different byte, set it to player B
	 * (4) otherwise, do the same but to player A
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	 private void readFromArduino(){
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
	 * (1) chec 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	private void battleProceed(){
		if (hi5) { 
			if (attacking){
				if (!seqGenerated){
					generateSeqParams(); 
					generateSequence(currentSeq); 
					seqGenerated = true; 
				}
				else {
					if (checkBothEvents()){
					 	correctMoves++; 
						seqMoves--; 
					}
					// check if sequence is finished 
					if (seqMoves <= 0){
						// check if all moves in sequence were correctly done  
						if (correctMoves >= seqMoves){ 
							//deal damage
							//gameObject.SendMessage("DamageEnemy", seqDamage);
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
						break; 

					case (int) reaction.counter:
						
						break;
				}
			}
		}
		else if (detectedA == 1 && detectedB == 4){
			hi5 = true; 
		}
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. NO RETURN.
	 * (1) generate a number of moves within the next seq of whatever speed
	 * (2) generate the delay between moves 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	 private void generateSeqParams(){
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
	
	private void generateSequence(int seq){
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

	 private bool checkBothEvents(){
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

	private bool checkTouchA(int touchA){

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

	private bool checkTouchB(int touchB){

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

	private int enemyResponse(){

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

}