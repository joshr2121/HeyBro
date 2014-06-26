using UnityEngine;
using System.Collections;

public class HealthBarEnemy : MonoBehaviour {

	public float max_XScale;
	public float yScale;
	public EnemyControls enemy;
	
	public float curPerc;
	public float targetPerc;

	// Use this for initialization
	void Start () {
		enemy = GameObject.Find ("Enemy").GetComponent<EnemyControls>();
		curPerc = 0.1f;
		targetPerc = 1f;
	}
	
	// Update is called once per frame
	void Update () {
		//float percHealth = (float) enemy.hp / (float) enemy.maxHP
		//transform.localScale = new Vector3 (max_XScale * percHealth, yScale, 1f);
		targetPerc = (float) enemy.hp / (float) enemy.maxHP;
		
		if (targetPerc < 0f) {
			targetPerc = 0f;
		}
		
		if (targetPerc != curPerc) {
		
			if (Mathf.Abs (curPerc - targetPerc) <= 0.1f) {
				curPerc = targetPerc;
			}
			else {
				curPerc = Mathf.Lerp (curPerc, targetPerc, 0.1f);
			}			
			transform.localScale = new Vector3 (max_XScale * curPerc, yScale, 1f);
		}		
	}
}
