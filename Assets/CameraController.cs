using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float speed;
    private bool isReseting = false;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        Vector3 targetPosition = new Vector3(0, 0, this.gameObject.transform.position.z);
        float step = speed * Time.deltaTime;
        if (isReseting)
        {       
            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, targetPosition, step);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                Debug.Log("Camera reseted");
                isReseting = false;
            }
        }        
    }

    public void ResetCamera()
    {
        isReseting = true;
    }

    public bool isCameraReseting()
    {
        return isReseting;
    }
}
