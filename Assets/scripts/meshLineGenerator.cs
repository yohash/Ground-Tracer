using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class meshLineGenerator : MonoBehaviour {

	public Vector3[] linePoints;
	public Vector3[] lineNormals;
	int numPoints, numSegments;

	public Vector3[] newVerts;
	public Vector2[] newUV;
	public int[] newTriangles;

	MeshFilter meshFilter;
	Mesh mesh;

	float lineWidth = 0.5f;

	void Start () {
		meshFilter = GetComponent<MeshFilter>();
		mesh = meshFilter.mesh;
	}

	// ************************************************************************
	// 		PUBLIC ACCESSORS
	public void setData (float width) {
		lineWidth = width;
	}

	public void setLinePoints (Vector3[] points, Vector3[] normals) {
		linePoints = points;
		lineNormals = normals;
		numPoints = linePoints.Length;
		numSegments = numPoints-1;

		rebuildMesh();
	}

	// ************************************************************************
	// 		MESH BUILDING OPERATIONS
	void rebuildMesh() {

		newVerts = new Vector3[4 + (numSegments-1)*2];
		newUV = new Vector2[4 + (numSegments-1)*2];

		Vector3 xformL, localPointL = new Vector3(-lineWidth/2,0f,0f);
		Vector3 xformR, localPointR = new Vector3(lineWidth/2,0f,0f);

		Vector3 worldPoint1, worldPoint2;

		Vector3 v1,v2,v3,v4;

		// initiate the first two triangles manually, so set proper initial path direction
		// and to more easily loop remaining new points in each respective triangle
		worldPoint1 = linePoints[0];
		worldPoint2 = linePoints[1];

		Vector3[] newPoints = steerNewLineSegments(worldPoint1,worldPoint1+(worldPoint2-worldPoint1),localPointL,localPointR);
		newPoints = alignNewLineSegmentsWithNormal(worldPoint1,worldPoint1+(worldPoint2-worldPoint1),lineNormals[0]);

		xformL = newPoints[0];
		xformR = newPoints[1];

		v1 = worldPoint1 + xformL;
		v2 = worldPoint1 + xformR;

		newVerts[0] = v1;
		newVerts[1] = v2;

		newPoints = steerNewLineSegments(worldPoint1,worldPoint2,localPointL,localPointR);
		newPoints = alignNewLineSegmentsWithNormal(worldPoint1,worldPoint2,lineNormals[1]);

		xformL = newPoints[0];
		xformR = newPoints[1];

		v3 = worldPoint2 + xformL;
		v4 = worldPoint2 + xformR;

		newVerts[2] = v3;
		newVerts[3] = v4;

		newUV[0] = new Vector2(newVerts[0].x,newVerts[0].z);
		newUV[1] = new Vector2(newVerts[2].x,newVerts[2].z);

		for (int i=1; i<numSegments;i++) {
			v1 = v3;
			v2 = v4;

			worldPoint1 = worldPoint2;
			worldPoint2 = linePoints[i+1];

			// perform y-axis rotation to align local points with direction line is moving
			// perform x and z-axis rotations to align local points with surface normal
			newPoints = steerNewLineSegments(worldPoint1,worldPoint2,localPointL,localPointR);
			newPoints = alignNewLineSegmentsWithNormal(worldPoint1,worldPoint2,lineNormals[i+1]);

			xformL = newPoints[0];
			xformR = newPoints[1];

			v3 = worldPoint2 + xformL;
			v4 = worldPoint2 + xformR;

			newVerts[2*i+2] = v3;
			newVerts[2*i+3] = v4;

			newUV[2*i] = new Vector2(newVerts[i].x,newVerts[i].z);
			newUV[2*i+1] = new Vector2(newVerts[i+2].x,newVerts[i+2].z);
		}

		newTriangles = new int[numSegments*6];
		for (int i=0; i<(numSegments);i++) {
			newTriangles[i*6] = i*2+1;
			newTriangles[i*6+1] = i*2;
			newTriangles[i*6+2] = i*2+2;

			newTriangles[i*6+3] = i*2+1;
			newTriangles[i*6+4] = i*2+2;
			newTriangles[i*6+5] = i*2+3;
		}

		mesh.Clear();
		mesh.vertices = newVerts;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
	}


	Vector3[] steerNewLineSegments(Vector3 p1, Vector3 p2, Vector3 offsetL, Vector3 offsetR) {
		Vector3 dir = p2-p1;
		Vector3[] offsets = new Vector3[2];

		offsets[0] = offsetL;
		offsets[1] = offsetR;

		float roteAngle = Vector3.Angle (new Vector3(0f,0f,1f), dir);
		if (dir.x < 0) {
			roteAngle = 360f - roteAngle;
		}
		Vector3 temp;
		for (int i=0; i<offsets.Length; i++) {
			temp = offsets[i];
			temp.x = Mathf.Cos (roteAngle * Mathf.Deg2Rad) * offsets[i].x - Mathf.Sin (roteAngle * Mathf.Deg2Rad) * offsets[i].z;
			temp.z = Mathf.Sin (roteAngle * Mathf.Deg2Rad) * offsets[i].x + Mathf.Cos (roteAngle * Mathf.Deg2Rad) * offsets[i].z;
			offsets[i] = temp;
		}

		return offsets;
	}

	Vector3[] alignNewLineSegmentsWithNormal(Vector3 p1, Vector3 p2, Vector3 n) {
		Vector3 dir = p2-p1;
		Vector3[] offsets = new Vector3[2];

		Vector3 rightSide = Vector3.Cross(n, dir).normalized * lineWidth/2;
		Vector3 leftSide = -rightSide;

		offsets[0] = leftSide;
		offsets[1] = rightSide;

		return offsets;
	}
}
