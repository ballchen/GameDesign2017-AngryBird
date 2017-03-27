using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {
    private bool isDragging = false;
    private SpringJoint2D spring;
    private Vector3 startPosition;
    private Transform catapult;
    private Rigidbody2D rigidbody;
    private Ray ray;

    public Camera MainCamera;
    public float maxLengthStretch = 3.0f;
    

    private void OnMouseDown()
    {
        isDragging = true;
        rigidbody.gravityScale = 1;
        rigidbody.isKinematic = false;


    }

    private void OnMouseUp()
    {
        isDragging = false;
        
    }


    // Use this for initialization
    void Start () {
        startPosition = this.gameObject.transform.position;
        spring = this.gameObject.GetComponent<SpringJoint2D>();
        rigidbody = this.gameObject.GetComponent<Rigidbody2D>();
        rigidbody.isKinematic = true;
        catapult = spring.connectedBody.transform;
    }
	
	// Update is called once per frame
	void Update () {
		if(isDragging)
        {
            UpdateBallPosition();
        }
	}

    void UpdateBallPosition()
    {
        Vector3 mousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition); 
        Vector2 catapultToMouse = mousePosition - catapult.position;
        mousePosition.z = this.gameObject.transform.position.z;


        float dist = Vector3.Distance(mousePosition, catapult.position);
        

        if(dist > maxLengthStretch)
        {
            ray = new Ray(catapult.position, Vector3.zero);
            ray.direction = catapultToMouse;
            mousePosition = ray.GetPoint(maxLengthStretch);
            mousePosition.z = this.gameObject.transform.position.z;
        }
        this.gameObject.transform.position = mousePosition;


    }
}
