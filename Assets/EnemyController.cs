using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private bool isDied = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            BirdDied();
        }
    }

    void BirdDied()
    {
        Debug.Log("Died");
        this.gameObject.SetActive(false);
        isDied = true;
    }
}
