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
	public int contactA;
	public int contactB; 
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

	// TO KEEP TRACK OF CURRENT MOVE
	public int currentSeq; 
	public int currentMove; 		// which move are we at in the overall sequence/in total moves
	public int numSequences;
	public float currentSeqTime; 
	public float currentWindow; 
	public bool correctCombo; 


	private float[][] sequences; 


	void Start(){
	
		// ARDUINO STUFF
		sp.Open();				// open the port
		sp.ReadTimeout = 1; 	// how often unity checks (throws exception if isn't open)

		touchDetectedA = false; 
		touchDetectedB = false;

		// 0: num sets of that sequence, 1: num moves per sequence, 2: dmg, 3: delay, 4: window
		if (Application.loadedLevelName.Equals("SpaceJellyfish")){
			sequences = new float[2][5];
			sequences[0] = { 2, 2, 25, 1, .2f };
			sequences[1] = { 1, 3, 40, .9f, .175f };
		}
		else if (Application.loadedLevelName.Equals("SentientMeteor")){
			sequences = new float[3][5];
			sequences[0] = { 1,	2, 25, 1, .2f };
			sequences[1] = { 2,	3, 40, .9f, .175f };
			sequences[2] = { 1,	4, 75, .8f, .150f };
		}
		else if (Application.loadedLevelName.Equals("RobotCrimelord")){
			sequences = new float[4][5];
			sequences[0] = { 1, 3, 40, .9f, .175f };
			sequences[1] = { 2, 4, 75, .8f, .150f };
			sequences[2] = { 2, 5, 100, .75f, .150f };
			sequences[3] = { 1, 6, 150, .7f, .125f };
		}
	}

	void Update(){
		randomEvent(); 
		
	}

	void FixedUpdate(){
		currentSeqTime += Time.deltaTime; 
		currentWindow += Time.deltaTime; 

		if (currentSeqTime >= sequences[currentSeq][3]){
			currentSeqTime = 0;
			if (correctCombo){
				
			}
		}
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
	 		while (byteBuffer[i] == contactA){
	 			i++;
	 		}
	 		detectedB = byteBuffer[i];
	 		touchDetectedB = true; 
	 	}
	 	// (4)
	 	else if (touchDetectedB){
	 		int i = 1; 
	 		while (byteBuffer[i] == contactB){
	 			i++;
	 		}
	 		detectedA = byteBuffer[i];
	 		touchDetectedA = true; 
	 	}
	 }

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. NO RETURN.
	 * (1) generate a number of moves within the next seq of whatever speed
	 * (2) generate the delay between moves 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	 private void generateSeqParams(){
	 	// (1) generate number of moves for next seq
	 	movesPerSeqLen += Random.Range(minMovesPerSeq, maxMovesPerSeq); 

	 	// (1*) check that it doesn't surpass total moves in level
	 	if (movesPerSeqLen > numMoves){
	 		movesPerSeqLen = numMoves; 
	 	}

	 	// (2) what is the delay between the next set of moves
	 	seqDelay = Random.Range(minSeqDelay, maxSeqDelay); 
	 }

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. NO RETURN.
	 * set random value in enum range (from minEnum to maxEnum) to contactA and contactB 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void randomEvent(){
		contactA = Random.Range(minEnum, maxEnum);
		contactB = Random.Range(minEnum, maxEnum);
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARGS. NO RETURN.
	 * (1) check if we've completed the total sequence of the level
	 * (2) if current set of moves not completed, trigger a randomEvent and increment the currentMove (go to next move)
	 * (3) otherwise, generate a new set
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void generateSequence(){

		// (1) if currentMove >= numMoves -> completed the level (either win or lose)
		if (currentMove < numMoves){

			// (2) trigger randomEvent and inc currentMove
			if (currentMove < movesPerSeqLen){
				randomEvent();
				currentMove++; 
			}
			// (3) generate a new set 
			else {
				generateSeqParams(); 
			}
		}
		//else win current level
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * ARG: the demanded contact input for player A
	 * (1) touchDetectedA bool set to TRUE;
	 * (2) if hit the right contact then return true. 
	 * (3) Otherwise return false
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private bool checkTouchA(int touchA){

		palmA 	= (detectedA == 1);
		fistA 	= (detectedA == 2);
		elbowA 	= (detectedA == 3);

		// (1) touch detected from player A
		touchDetectedA = true;

		// (2) if right input, return true
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
		// (3) if haven't returned true = wrong input (CHECK ANY KEY?)
		return false; 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * ARG: the demanded contact input for player B
	 * (1) touchDetectedB bool set to TRUE;
	 * (2) if hit the right contact then return true. 
	 * (3) Otherwise return false
	 * -------------------------------------------------------------------------------------------------------------------------- */

	private bool checkTouchB(int touchB){

		palmB 	= (detectedB == 4);
		fistB 	= (detectedB == 5);
		elbowB 	= (detectedB == 6);

		// (1) touch detected from player B
		touchDetectedB = true; ; 

		// (2) if right input, return true
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

		// (3) if haven't returned true = wrong input (CHECK ANY KEY?)
		return false; 
	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * 
	 * -------------------------------------------------------------------------------------------------------------------------- */

	 private bool checkBothEvents(){
/*
	 	// (1) if both players have touched 
	 	if (touchDetectedA && touchDetectedB){

	 		// (2) if AT LEAST ONE player touched the wrong thing
		 	if (!checkTouchA || !checkTouchB){

		 	}


		 	else if (checkTouchA && checkTouchB){

		 	}
		}*/
		return false; 
	 }

}