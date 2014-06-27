using UnityEngine;
using System.Collections;
using System.IO.Ports; 

public class SplashScreen : MonoBehaviour {

	// ARDUINO STUFF ("PORT" is not right)
	SerialPort sp = new SerialPort("COM3", 9600);

	public int in1;
	public int in2; 
	public int current; 

	public bool inputted1; 
	public bool inputted2; 

	public float readDelay;
	public float currentTime; 
	
	public bool loadingNewScene;
	public AudioSource srcLoading;
	public AudioSource srcMusic;

	void Start () {
		 // ARDUINO STUFF
		sp.Open();				// open the port
		sp.ReadTimeout = 1; 	// how often unity checks (throws exception if isn't open)

		in1 = 0;
		in2 = 0; 
		readDelay = 5.0f;
		inputted1 = false; 
		loadingNewScene = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (loadingNewScene) {
			srcMusic.volume -= 0.003f;
		}

		if (sp.IsOpen){

			try{
				readFromArduino(); 

				if (in1 == 1 && in2 == 4){
					if (!loadingNewScene) {
						srcLoading.Play ();
						loadingNewScene = true;
						Invoke ("loadMainScene", 1.0f);
					}
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

		if (in1 != 0 && in2 != 0){//(currentTime > readDelay){
			current = 0; 
			inputted1 = false; 
			inputted2 = false; 
		}
		
		print ("in1 = " + in1 + ", in2 = " + in2);

		current = int.Parse (sp.ReadLine()); 

		print ("current = " + current); 

		if (!inputted1 && !inputted2) {
			if (current > 0 && current <= 3){
				in1 = current; 
				inputted1 = true; 
			}
			else if (current > 0 && current > 3){
				in2 = current; 
				inputted2 = true; 
			}
		}

		else if (inputted1 && !inputted2) {
			if (current > 0 && current > 3){
				in2 = current;
				inputted2 = true; 
			}
		}

		else if (!inputted1 && inputted2){
			if (current > 0 && current <= 3){
				in1 = current;
				inputted1 = true; 
			}
		}
	 }
	 
	void loadMainScene () {
		Application.LoadLevel("MainScene");
	}
}
