using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private bool isDied = false;
    public ParticleSystem BloodParticle;
    public Camera MainCamera;
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ball") || collision.gameObject.tag == "Obstacle")
        {
            BirdDied();
        }
    }

    void BirdDied()
    {
        Debug.Log("Died");
        this.gameObject.SetActive(false);
        isDied = true;

        BloodParticle.gameObject.transform.position = this.gameObject.transform.position;
        BloodParticle.gameObject.SetActive(true);

        MainCamera.gameObject.GetComponent<CameraController>().ResetCamera();
        

    }
}
