using UnityEngine;
using System.Collections;

public class Sequence_Queue : MonoBehaviour {

	public GameObject[] sequenceObjects;
	private Sprite[] sequenceSprites;
	
	public Sprite pictogramHighfive;
	public Sprite pictogramFist;
	public Sprite pictogramElbow;

	// Use this for initialization
	void Start () {
		sequenceSprites = new Sprite[3] {pictogramHighfive, pictogramFist, pictogramElbow};
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// Sets the appropriate pictogram sprites and visibility
	public void LoadSequence (int[] seq) {
		
		for (int i = 0; i < seq.Length; i++) {
			sequenceObjects[i].renderer.enabled = true;
			sequenceObjects[i].GetComponent<SpriteRenderer>().sprite = sequenceSprites[seq[i]];
		}
		
		for (int i = seq.Length; i < sequenceObjects.Length; i++) {
			sequenceObjects[i].renderer.enabled = false;
		}
		
	}
}
