using UnityEngine;
using System.Collections;

public class HealthBarEnemy : MonoBehaviour {

	public int max_XScale;
	public int yScale;
	public EnemyControls enemy;

	// Use this for initialization
	void Start () {
		enemy = GameObject.Find ("Enemy").GetComponent<EnemyControls>();
	}
	
	// Update is called once per frame
	void Update () {
		float percHealth = enemy.hp / 100;	//Should technically be / maxHP but it's okay 
		transform.localScale = new Vector3 (max_XScale * percHealth, yScale, 1);
		
	}
}
