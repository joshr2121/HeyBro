using UnityEngine;
using System.Collections;

public class Sequence_Queue : MonoBehaviour {

	public GameObject[] sequenceObjects;
	private Sprite[] sequenceSprites;
	
	public Sprite pictogramHighfive;
	public Sprite pictogramFist;
	public Sprite pictogramElbow;

	// delay stuff
	public SequenceControls player; 
	public float seqDelay;
	public float currentTime;
	public float yDistanceBetweenPictograms;
	public float yTicksBetweenPositions;
	
	// Use this for initialization
	void Start () {
		sequenceSprites = new Sprite[3] {pictogramHighfive, pictogramFist, pictogramElbow};
		yDistanceBetweenPictograms = 0.8f;
		yTicksBetweenPositions = 2f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate () {
		currentTime += Time.deltaTime;
		MoveSpriteDown();
		
	}
	
	// Sets the appropriate pictogram sprites and visibility
	public void LoadSequence (int[] seq, float delay) {
		for (int i = 0; i < seq.Length; i++) {
			sequenceObjects[i].renderer.enabled = true;
			sequenceObjects[i].GetComponent<SpriteRenderer>().sprite = sequenceSprites[seq[i]];
		}
		
		for (int i = seq.Length; i < sequenceObjects.Length; i++) {
			sequenceObjects[i].renderer.enabled = false;
		}
		//Multiplying delay for easier debugging
		seqDelay = 10*delay / yTicksBetweenPositions;
	}
	public void MoveSpriteDown(){
		float yTranslation = yDistanceBetweenPictograms / yTicksBetweenPositions;
		
		if (player.attacking){
			if (currentTime >= seqDelay){
				foreach (GameObject o in sequenceObjects) {
					o.gameObject.transform.position = new Vector3 (o.gameObject.transform.position.x, o.gameObject.transform.position.y - yTranslation, 
					o.gameObject.transform.position.z);
				}
				currentTime = 0;				
			}
		}
	}
}
