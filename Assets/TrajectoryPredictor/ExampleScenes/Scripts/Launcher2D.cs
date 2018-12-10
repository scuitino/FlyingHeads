using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Launcher2D : MonoBehaviour {

    #region SINGLETON PATTERN
    public static Launcher2D _instance = null;
    #endregion

    [SerializeField, Header("Throw Variables")]
    float _forceMultiplier;
    [SerializeField]
    float _minForce, _maxForce;
    public float force = 150f;
    public GameObject objToLaunch;
    public Transform launchPoint;
    public Text infoText;
    public bool launch;

    //create a trajectory predictor in code
    public TrajectoryPredictor tp;

    private void Awake()
    {
        //singleton check
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }
    
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
            CThrowController._instance._proCamera.CameraTargets[0].TargetTransform = CThrowController._instance._activeHead.transform;
            CThrowController._instance._proCamera.CameraTargets[0].TargetOffset = new Vector2(0, 3.52f);
        }

        //this static method can be used as well to get line info without needing to have a component and such
        //TrajectoryPredictor.GetPoints2D(launchPoint.position, launchPoint.right * force, Physics2D.gravity);


        //info text stuff
        //if(infoText){
        //this will check if the predictor has a hitinfo and then if it does will update the onscreen text
        //to say the name of the object the line hit;
        if (tp.hitInfo2D)
        {
            //CThrowController._instance._secondCameraTarget.transform.position = tp.hitInfo2D.collider.transform.position;
                //infoText.text = "Hit Object: " + tp.hitInfo2D.collider.gameObject.name;
        }
			
		//}
	}

    // launch the head
	GameObject launchObjParent;
	void Launch(){
		if(!launchObjParent){
			launchObjParent = new GameObject();
			launchObjParent.name = "Launched Objects";
		}
		GameObject lInst = Instantiate (objToLaunch);
        CThrowController._instance._activeHead = lInst;
		lInst.name = "ActiveHead";
		lInst.transform.SetParent(launchObjParent.transform);
		Rigidbody2D rbi = lInst.GetComponent<Rigidbody2D> ();
		lInst.transform.position = launchPoint.position;
		lInst.transform.rotation = launchPoint.rotation;        
		rbi.velocity = launchPoint.right * force;
        rbi.AddTorque(Random.Range(-5,5));
        lInst.GetComponent<CHead>().ChangeArtOrientation(Mathf.Sign(rbi.velocity.x));
    }

    // update throw values
    public void UpdateThrowData(Vector2 aVectorForce)
    {
        Vector2 tDirection = aVectorForce - (Vector2)CThrowController._instance._throwZoneLimit.transform.position;// (Vector2)CPlayer._instance.transform.position;
        // update force
       // Debug.Log(tDirection + "direc");
        force = tDirection.magnitude * _forceMultiplier + _minForce;
        if (force > _maxForce)
        {
            force = _maxForce;
        }

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
