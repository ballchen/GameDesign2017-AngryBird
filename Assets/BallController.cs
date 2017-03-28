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
    
    private Vector3 mousePosition;

    public Camera MainCamera;
    public float maxLengthStretch = 3.0f;
    public Material LineMaterial;
    public LineRenderer LineToCatapult;
    public LineRenderer LineToCatapultFront;
    public Transform CatapultFront;
    public float cameraFixLimit;


    private void OnMouseDown()
    {
        isDragging = true;
        rigidbody.gravityScale = 1;
        rigidbody.isKinematic = false;


    }

    private void OnMouseUp()
    {
        isDragging = false;

        Destroy(spring);
        LineToCatapult.enabled = false;
        LineToCatapultFront.enabled = false;
        
    }


    // Use this for initialization
    void Start () {
        LineRendererSetup();

        startPosition = this.gameObject.transform.position;
        spring = this.gameObject.GetComponent<SpringJoint2D>();
        rigidbody = this.gameObject.GetComponent<Rigidbody2D>();
        rigidbody.isKinematic = true;
        catapult = spring.connectedBody.transform;
        ray = new Ray(catapult.position, Vector3.zero);


    }

    void LineRendererSetup()
    {
        LineToCatapult.sortingLayerName = "Foreground";
        LineToCatapultFront.sortingLayerName = "Foreground";

        LineToCatapult.sortingOrder = 0;
        LineToCatapultFront.sortingOrder = 2;

        LineToCatapult.material = LineMaterial;
        LineToCatapultFront.material = LineMaterial;

        LineToCatapult.widthMultiplier = 0.1f;
        LineToCatapultFront.widthMultiplier = 0.1f;
    }
	
	// Update is called once per frame
	void Update () {

        if (isDragging)
        {
            updateBallPosition();
        }

        if(spring == null)
        {
            updateCameraPosition();

            if(Mathf.Approximately(rigidbody.velocity.x, 0.0f))
            {
                Debug.Log("Stop");
            }
            
        }
        
        setLineRendererPosition();
    }

    void updateBallPosition()
    {
        mousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition); 
        Vector2 catapultToMouse = mousePosition - catapult.position;
        mousePosition.z = this.gameObject.transform.position.z;
        

        float dist = Vector3.Distance(mousePosition, catapult.position);
        

        if(dist > maxLengthStretch)
        {
            ray.direction = catapultToMouse;
            mousePosition = ray.GetPoint(maxLengthStretch);
            mousePosition.z = this.gameObject.transform.position.z;
        }
        this.gameObject.transform.position = mousePosition;
    }

    void setLineRendererPosition()
    {
        

        Vector3 linePosStart = new Vector3(catapult.position.x, catapult.position.y, 0);
        Vector3 linePosEnd = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0);
        Vector3 CatapultFrontPos = CatapultFront.position;
        Vector3 lineFrontPosStart = new Vector3(CatapultFrontPos.x, CatapultFrontPos.y, 0);

        Ray BallRay = new Ray(this.gameObject.transform.position, Vector3.zero);
        BallRay.direction = this.gameObject.transform.position - CatapultFrontPos;
		Vector3 linePosBallEnd = BallRay.GetPoint((this.gameObject.GetComponent<CircleCollider2D>().radius)* this.gameObject.transform.localScale.x);

        LineToCatapult.SetPosition(0, linePosStart);
        LineToCatapult.SetPosition(1, linePosEnd);
        LineToCatapultFront.SetPosition(0, lineFrontPosStart);
        LineToCatapultFront.SetPosition(1, linePosEnd);
        LineToCatapultFront.SetPosition(2, linePosBallEnd);
    }

    void updateCameraPosition()
    {
        Vector3 currentBallPosition = this.gameObject.transform.position;
        Vector3 currentCamPosition = MainCamera.transform.position;
        if(currentBallPosition.x >= currentCamPosition.x)
        {
            Vector3 adjustCamPosition;
            if (currentBallPosition.x >= cameraFixLimit)
            {
                adjustCamPosition = new Vector3(cameraFixLimit, currentCamPosition.y, currentCamPosition.z);
            }
            else
            {
                adjustCamPosition = new Vector3(currentBallPosition.x, currentCamPosition.y, currentCamPosition.z);

            }

            MainCamera.transform.position = adjustCamPosition;
        } 
    }
}
