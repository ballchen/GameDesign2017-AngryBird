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
    private bool isCameraReachEnd = false;
    private Rigidbody2D BallShouldConnect;
    private bool hasHitObstable = false;
    private bool isWaiting = false;

    private Vector3 mousePosition;

    public Camera MainCamera;
    public float maxLengthStretch = 3.0f;
    public Material LineMaterial;
    public LineRenderer LineToCatapult;
    public LineRenderer LineToCatapultFront;
    
    public Transform CatapultFront;
    public float cameraFixLimit;
    public GameObject GameData;

    private void OnMouseDown()
    {
		if (spring != null) {
			isDragging = true;
			LineToCatapult.enabled = true;
			LineToCatapultFront.enabled = true;
			rigidbody.gravityScale = 1;
			rigidbody.isKinematic = false;
		}
    }

    private void OnMouseUp()
    {
		if (spring != null) {
			isDragging = false;

			Destroy(spring);
			LineToCatapult.enabled = false;
			LineToCatapultFront.enabled = false;
			catapult.gameObject.GetComponent<AudioSource>().Play();
		}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            this.gameObject.GetComponent<AudioSource>().Play();
            hasHitObstable = true;
        }
        
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
        BallShouldConnect = spring.connectedBody;
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
            if(!isCameraReachEnd)
            {
                updateCameraPosition();
            }

            if((this.gameObject.transform.position.x > 45) || (rigidbody.velocity.x <= 0.05f && rigidbody.velocity.y <= 0.05f))
            {
                if(!hasHitObstable)
                {
                    isCameraReachEnd = true;
                    CameraController CamScript = MainCamera.gameObject.GetComponent<CameraController>();
                    if (!CamScript.isCameraReseting())
                    {
                        Debug.Log("RESET");
                        CamScript.ResetCamera();
                        isCameraReachEnd = false;
                        ResetBall();
                    }
                }
                else
                {
                    if (!isWaiting)
                    {
                            StartCoroutine(WaitBulletFly());
                    }
                }
                
            }

            
            
        }
        
        setLineRendererPosition();
    }

    IEnumerator WaitBulletFly()
    {
        isWaiting = true;
        yield return new WaitForSeconds(5);
       
        isCameraReachEnd = true;
        CameraController CamScript = MainCamera.gameObject.GetComponent<CameraController>();
        
        if (!CamScript.isCameraReseting() && GameObject.Find("Enemy") != null)
        {
            Debug.Log("RESET");
            CamScript.ResetCamera();
            isCameraReachEnd = false;
            ResetBall();
        }
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
                isCameraReachEnd = true;
            }
            else
            {
                adjustCamPosition = new Vector3(currentBallPosition.x, currentCamPosition.y, currentCamPosition.z);

            }

            MainCamera.transform.position = adjustCamPosition;
        } 
    }

    Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }


    public void ResetBall()
    {
        Reset();
        GameData.gameObject.GetComponent<GameManager>().UpdateLife(-1);
    }

    private void Reset()
    {
        isCameraReachEnd = false;
        hasHitObstable = false;
        isWaiting = false;

        // reset spring
        if (spring == null)
        {
            spring = this.gameObject.AddComponent<SpringJoint2D>();
            spring.connectedBody = BallShouldConnect;
            spring.anchor = Vector2.zero;
            spring.connectedAnchor = new Vector2(3.123283e-07f, 6.35783e-09f);
            spring.distance = 0.2f;
            spring.dampingRatio = 0;
            spring.frequency = 0.75f;
            spring.autoConfigureDistance = false;
        }

        //reset ball
        this.gameObject.transform.position = new Vector3(-5.897f, -1.21f, 0);
        rigidbody.gravityScale = 0;
        rigidbody.isKinematic = true;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = 0;

    }
}
