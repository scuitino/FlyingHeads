using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Launcher2D : MonoBehaviour {
    
    [SerializeField, Header("Throw Variables")]
    float _forceMultiplier;
    public float force = 150f;
    public GameObject objToLaunch;
    public Transform launchPoint;
    public Text infoText;
    public bool launch;

    //create a trajectory predictor in code
    TrajectoryPredictor tp;
	void Start(){
		tp = gameObject.GetComponent<TrajectoryPredictor>();
		tp.predictionType = TrajectoryPredictor.predictionMode.Prediction2D;
		tp.drawDebugOnPrediction = true;
		tp.accuracy = 0.99f;
		tp.lineWidth = 0.025f;
		tp.iterationLimit = 300;
	}

	// Update is called once per frame
	void Update () {
		
        // check fire input
		if (launch) {
			launch = false;
			Launch();
		}		

		//this static method can be used as well to get line info without needing to have a component and such
			//TrajectoryPredictor.GetPoints2D(launchPoint.position, launchPoint.right * force, Physics2D.gravity);


		//info text stuff
		if(infoText){
			//this will check if the predictor has a hitinfo and then if it does will update the onscreen text
			//to say the name of the object the line hit;
			if(tp.hitInfo2D)
				infoText.text = "Hit Object: " + tp.hitInfo2D.collider.gameObject.name;
		}
	}

	GameObject launchObjParent;
	void Launch(){
		if(!launchObjParent){
			launchObjParent = new GameObject();
			launchObjParent.name = "Launched Objects";
		}
		GameObject lInst = Instantiate (objToLaunch);
		lInst.name = "Ball";
		lInst.transform.SetParent(launchObjParent.transform);
		Rigidbody2D rbi = lInst.GetComponent<Rigidbody2D> ();
		lInst.transform.position = launchPoint.position;
		lInst.transform.rotation = launchPoint.rotation;
		rbi.velocity = launchPoint.right * force;
	}

    // update throw values
    public void UpdateThrowData(Vector2 aVectorForce)
    {
        Vector2 tDirection = aVectorForce - (Vector2)CPlayer._instance.transform.position;
        // update force
       // Debug.Log(tDirection + "direc");
        force = tDirection.magnitude * _forceMultiplier;
        // update look rotation

        tDirection.Normalize();

        float rot_z = Mathf.Atan2(tDirection.y, tDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 180);

        // predict
        //set line duration to delta time so that it only lasts the length of a frame
        tp.debugLineDuration = Time.unscaledDeltaTime;
        //tell the predictor to predict a 2d line. this will also cause it to draw a prediction line
        //because drawDebugOnPredict is set to true
        tp.Predict2D(launchPoint.position, launchPoint.right * force, Physics2D.gravity);
    }
}
