using UnityEngine;
using System.Collections;
using System.IO.Ports; 

public class SplashScreen : MonoBehaviour {

	// ARDUINO STUFF ("PORT" is not right)
	SerialPort sp = new SerialPort("COM3", 9600);
	public byte[] byteBuffer; 
	public int byteOffset;
	public int byteCount;

	public int in1;
	public int in2; 
	public int current; 

	public bool inputted1; 
	public bool inputted2; 

	public float readDelay;
	public float currentTime; 

	// TO CHECK THAT THE RIGHT CONTACT WAS MADE
	private bool touchDetectedA;
	private bool touchDetectedB;

	public int detectedA;
	public int detectedB; 

	void Start () {
		 // ARDUINO STUFF
		sp.Open();				// open the port
		sp.ReadTimeout = 1; 	// how often unity checks (throws exception if isn't open)

		in1 = 0;
		in2 = 0; 
		readDelay = 5.0f;
		inputted1 = false; 
	}
	
	// Update is called once per frame
	void Update () {

		if (sp.IsOpen){

			try{
				readFromArduino(); 

				if (detectedA == 1 && detectedB == 4){
					Application.LoadLevel("MainScene");
				}
			}

			catch (System.Exception){
				// do nothing if there's an exception i.e. if port is not open
			}

		}
	}

	void FixedUpdate(){
		currentTime += Time.deltaTime; 
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
//	 	sp.Read(byteBuffer, byteOffset, byteCount);

		print ("in1 = " + in1 + ", in2 = " + in2);

		current = int.Parse (sp.ReadLine()); 

		if (!inputted1 && !inputted2) {
			if (current > 0 && current <= 2){
				in1 = current; 
				inputted1 = true; 
			}
			else if (current > 0 && current > 2){
				in2 = current; 
				inputted2 = true; 
			}
		}

		else if (inputted1 && !inputted2) {
			if (current > 0 && current > 2){
				in2 = current;
				inputted2 = true; 
			}
		}

		else if (!inputted1 && inputted2){
			if (current > 0 && current <= 2){
				in1 = current;
				inputted1 = true; 
			}
		}


		if (in1 > 0 && in2 > 0 && currentTime >= readDelay){
				inputted1 = false; 
				inputted2 = false; 
				in1 = 0;
				in2 = 0;
				currentTime = 0; 
		}


		/*

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
	 	*/
	 }
}
