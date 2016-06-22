using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class inputController : MonoBehaviour {

	public meshLineGenerator theLineGenerator;

	public float yOffsetFromGround = 0.1f;

	public bool TRACKING;

	List<Vector3> linePoints;
	List<Vector3> linePointNormals;

	int numLinePoints;


	void Start () {	
		initiateTracking();
	}


	void Update () {
		if (Input.GetKeyDown(KeyCode.Mouse0) && !TRACKING) {
			TRACKING = true;
			initiateTracking();
		}

		if (Input.GetKey(KeyCode.Mouse0) && TRACKING) {
			trackPositions();
		}

		if (Input.GetKeyUp(KeyCode.Mouse0) && TRACKING) {
			TRACKING = false;
		}
	}

	void trackPositions () {

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 100)) {

			if (numLinePoints == 0) {
				linePoints.Add(hit.point + new Vector3(0f, yOffsetFromGround, 0f));
				linePointNormals.Add(hit.normal);
				numLinePoints = linePoints.Count;

			} else if ((hit.point - linePoints[linePoints.Count-1]).sqrMagnitude > 0.2f) {				

				linePoints.Add(hit.point + new Vector3(0f, yOffsetFromGround, 0f));
				linePointNormals.Add(hit.normal);
				numLinePoints = linePoints.Count;

//				Debug.DrawRay(hit.point, hit.normal*5f, Color.red, 50f);

				filterAndSubmitLine ();
			}

			// NEED

			// if (line segment is too short) dont add
			// if (line segment is too long) go 'fill in' the gap between the two
		}
	}


	void filterAndSubmitLine () {
		// adding filtering operations

		theLineGenerator.setLinePoints(linePoints.ToArray(),linePointNormals.ToArray());
		theLineGenerator.generateMesh();
	}

	void initiateTracking() {
		linePoints = new List<Vector3>();
		linePointNormals = new List<Vector3>();
		numLinePoints = linePoints.Count;
	}
}
