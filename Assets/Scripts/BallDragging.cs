using UnityEngine;
using System.Collections;

public class BallDragging : MonoBehaviour {
	public float maxStretch = 3.0f;
	public LineRenderer slingshotFront;
	public LineRenderer slingshotBack;

	private SpringJoint2D spring;
	private Rigidbody2D rigidbody2d;
	private bool clickedOn;
	private Ray rayToMouse;
	private Transform slingshot;
	private Ray leftSlingshotToBall;
	private float maxStretchSqr;
	private float circleRadius;
	private Vector2 prevVelocity;

	void Awake ()
	{
		spring = GetComponent<SpringJoint2D> ();
		rigidbody2d = GetComponent<Rigidbody2D> ();
		slingshot = spring.connectedBody.transform;
	}

	void Start () {
		LineRendererSetup ();
		rayToMouse = new Ray (slingshot.position, Vector3.zero);
		maxStretchSqr = maxStretch*maxStretch;
		leftSlingshotToBall = new Ray (slingshotFront.transform.position, Vector3.zero);
		Collider2D collider2D = GetComponent<Collider2D> ();
		CircleCollider2D circle = collider2D as CircleCollider2D;
		circleRadius = circle.radius;
	}

	void Update () {
		if (clickedOn)
			Dragging ();
		if (spring != null)
		{
			if (!rigidbody2d.isKinematic && (prevVelocity.sqrMagnitude > rigidbody2d.velocity.sqrMagnitude)) {
					Destroy (spring);
					rigidbody2d.velocity = prevVelocity;
			}
			if (!clickedOn)
				prevVelocity = rigidbody2d.velocity;
			LineRendererUpdate();
		} else {
			slingshotFront.enabled = false;
			slingshotBack.enabled = false;
		}
	}

	void LineRendererSetup()
	{
		slingshotFront.SetPosition (0, slingshotFront.transform.position);
		slingshotBack.SetPosition (0, slingshotBack.transform.position);
		slingshotFront.sortingLayerName = "Front";
		slingshotBack.sortingLayerName = "Front";
		slingshotFront.sortingOrder = 3;
		slingshotBack.sortingOrder = 1;
	}

	void LineRendererUpdate()
	{
		Vector2 slingshotToBall = transform.position - slingshotFront.transform.position;
		leftSlingshotToBall.direction = slingshotToBall;
		Vector3 holdPoint = leftSlingshotToBall.GetPoint (slingshotToBall.magnitude + circleRadius);
		slingshotFront.SetPosition (1, holdPoint);
		slingshotBack.SetPosition (1, holdPoint);
	}

	void OnMouseDown ()
	{
		if (spring != null)
		{
			spring.enabled = false;
			clickedOn = true;
		}

	}

	void OnMouseUp ()
	{
		if (spring != null)
		{
			spring.enabled = true;
			rigidbody2d.isKinematic = false;
			clickedOn = false;
		}
		
	}

	void Dragging ()
	{
		Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector2 slingshotToMouse = mouseWorldPoint - slingshot.position;
		if (slingshotToMouse.sqrMagnitude > maxStretchSqr)
		{
			rayToMouse.direction = slingshotToMouse;
			mouseWorldPoint = rayToMouse.GetPoint(maxStretch);
		}
		mouseWorldPoint.z = 0f;
		transform.position = mouseWorldPoint;
	}
}
