using UnityEngine;
using System.Collections;

public class PlayerAnim : MonoBehaviour {

	public Sprite attack;
	public Sprite elbow;	
	public Sprite handsUp;
	public Sprite highfive;		
	public Sprite punch;	
	public Sprite sad;

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<SpriteRenderer>().sprite = elbow;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
