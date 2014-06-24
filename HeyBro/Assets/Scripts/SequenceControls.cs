using UnityEngine;
using System.Collections;
using System.IO.Ports; 

public class SequenceControls : MonoBehaviour {

	// ARDUINO STUFF ("PORT" is not right)
	SerialPort sp = new SerialPort("PORT", 9600);
	public byte[] byteBuffer; 
	public int byteOffset;
	public int byteCount; 

	// CONTACT INPUTS (person A and person B)
	private bool palmA 	= Input.GetKeyDown("Alpha1"); 		// these will correspond to specific button inputs 
	private bool fistA 	= Input.GetKeyDown("Alpha2");
	private bool elbowA	= Input.GetKeyDown("Alpha3");

	private bool palmB	= Input.GetKeyDown("Alpha8"); 
	private bool fistB	= Input.GetKeyDown("Alpha9");
	private bool elbowB	= Input.GetKeyDown("Alpha0");
	
	// TO CREATE THE EVENTS THAT WILL BE CHECKED
	private enum touch { palm, fist, elbow }; 
	public int contactA;
	public int contactB; 
	public int detectedA;
	public int detectedB; 
	private int minEnum = 0; 	// first index of the enum
	private int maxEnum = 3;	// number of elements in the enum

	public int minMovesPerSeq = 3;		// min number of moves within sequence of moves of certain speed
	public int maxMovesPerSeq = 5; 		// max number of moves within seq of moves of certain speed
	public float minSeqDelay = 0.7;		// min delay between moves in seq of certain speed
	public float maxSeqDelay = 1.3; 	// max delay between moves in seq of certain speed

	// TO CHECK THAT THE RIGHT CONTACT WAS MADE
	private bool touchDetectedA;
	private bool touchDetectedB; 

	// CHANGE FOR EACH LEVEL
	public int numMoves;			// total number of moves to do in the level
	public int movesPerSeqLen; 		// number of moves for each fast/slow sequence
	public int currentMove; 		// which move are we at in the overall sequence/in total moves
	public float seqDelay; 			// delay between moves for each sequence speed
	public float currentSeqTime; 	// how much time passed since the current sequence of a certain speed started


	void Start(){
	
		// ARDUINO STUFF
		sp.Open();				// open the port
		sp.ReadTimeout = 1; 	// how often unity checks (throws exception if isn't open)

		touchDetectedA = false; 
		touchDetectedB = false;

		currentMove = 0;  

		movesPerSeqLen = 0; 
		generateSeqParams(); 
	}

	void Update(){
		randomEvent(); 
		
	}

	void FixedUpdate(){

	}

	/* --------------------------------------------------------------------------------------------------------------------------
	 * NO ARG. NO RETURN.
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
	 	if (touchDetectedA){
	 		int i = 1; 
	 		while (byteBuffer[i] == contactA){
	 			i++;
	 		}
	 		detectedB = byteBuffer[i];
	 		touchDetectedB = true; 
	 	}
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
	 * 
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	 private void generateSeqParams(){
	 	movesPerSeqLen += Random.Range(minMovesPerSeq, maxMovesPerSeq); 
	 	if (movesPerSeqLen > numMoves){
	 		movesPerSeqLen = numMoves; 
	 	}
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
	 * (1) read inputs from arduino
	 * (2) check if the first byte corresponds to player A or B
	 * (3) if corresponded to player A, find the first different byte, set it to player B
	 * (4) otherwise, do the same but to player A
	 * -------------------------------------------------------------------------------------------------------------------------- */
	
	private void generateSequence(){

		if (currentMove < numMoves){
			if (currentMove < movesPerSeqLen){

			}
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