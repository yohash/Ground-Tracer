using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFollower : MonoBehaviour {

	public float yOffsetFromGround = 0.1f;


	public bool TRACKING;


	public List<Vector3> linePoints;
	public List<Vector3> linePointNormals;

	public int numLinePoints;



	LineRenderer theLine;


	void Start () {
		theLine = GetComponent<LineRenderer>() ;
		InitiateLineRenderer ();
	}


	void Update () {
		if (Input.GetKeyDown(KeyCode.Mouse0) && !TRACKING) {
			InitiateLineRenderer();
			TRACKING = true;
		}

		if (Input.GetKey(KeyCode.Mouse0) && TRACKING) {
			trackPositions();
		}

		if (Input.GetKeyUp(KeyCode.Mouse0) && TRACKING) {

			TRACKING = false;
		}
	}

	void InitiateLineRenderer () {
		linePoints = new List<Vector3>();
		linePointNormals = new List<Vector3>();
		numLinePoints = 0;

		theLine.SetVertexCount(numLinePoints);
		theLine.SetPositions(linePoints.ToArray());
	}

	void trackPositions () {

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 100)) {

			if (numLinePoints == 0) {
				linePoints.Add(hit.point + new Vector3(0f, yOffsetFromGround, 0f));
				linePointNormals.Add(hit.normal);
				numLinePoints = linePoints.Count;

			} else if ((hit.point - linePoints[linePoints.Count-1]).sqrMagnitude > 1) {				

				linePoints.Add(hit.point + new Vector3(0f, yOffsetFromGround, 0f));
				linePointNormals.Add(hit.normal);
				numLinePoints = linePoints.Count;

				theLine.SetVertexCount(numLinePoints);
				theLine.SetPositions(linePoints.ToArray());
			}

			// NEED

			// if (line segment is too short) dont add
			// if (line segment is too long) go 'fill in' the gap between the two
		}
	}




	void meshBuilder () {

	}
}
