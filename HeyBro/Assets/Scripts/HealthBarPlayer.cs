using UnityEngine;
using System.Collections;

public class HealthBarPlayer : MonoBehaviour {

	public int max_XScale;
	public int yScale;
	public SequenceControls players;

	// Use this for initialization
	void Start () {
		players = GameObject.Find ("Players").GetComponent<SequenceControls>();
	}
	
	// Update is called once per frame
	void Update () {
		float percHealth = players.hp / 100;	//Should technically be / maxHP but it's okay 
		transform.localScale = new Vector3 (max_XScale * percHealth, yScale, 1);
		
	}
}
