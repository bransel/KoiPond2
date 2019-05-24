using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
	public float minMoveSpeed = 1, maxMoveSpeed = 1;
	public float minRotSpeed = 1.5f, maxRotSpeed = 1.5f;
	public float distanceCheck = 1;
	public float minSwapTimer = 30f, maxSwapTimer = 120f;
	public float minExitTimer = 30f, maxExitTimer = 120f;
	public TextureController textureController;
	public Transform mouth;
	public GameObject bubblePrefab;
	
	public long id { get; set; }
	public string message { get; set; }

	public Transform worldCanvas { get; set; }
	public FishController fishController { get; set; }
	public BubbleButton bubbleButton { get; set; }
	public Vector3 currentTarget { get; set; }

	public float moveSpeed { get; set; }
	public float rotSpeed { get; set; }

	private new Rigidbody rigidbody;
	private bool exiting;
	private float swapTimer;
	private float exitTimer;

    public static Vector3 worldUp = -1 * Vector3.forward;

    // Start is called before the first frame update
    void Start()
	{
		moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
		rotSpeed = Random.Range(minRotSpeed, maxRotSpeed);

		rigidbody = GetComponent<Rigidbody>();

		swapTimer = Random.Range(minSwapTimer, maxSwapTimer);
		exitTimer = Random.Range(minExitTimer, maxExitTimer);

		worldCanvas = GameObject.FindWithTag("WorldCanvas").transform;
	}

	// Update is called once per frame
	void Update()
	{
		rigidbody.MovePosition(rigidbody.position + transform.forward * moveSpeed * Time.deltaTime);
		rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget - transform.position, worldUp), Time.deltaTime * rotSpeed));

		if (exiting && !IsVisibleFrom(textureController.meshRenderer, Camera.main))
		{
			exiting = false;
			exitTimer = Random.Range(minExitTimer, maxExitTimer);
			fishController.ReassignFish(this);
		}

		/* else if (fishController && (currentTarget - transform.position).magnitude <= distanceCheck)
		{
			if (exiting)
			{
				exiting = false;
				exitTimer = Random.Range(minExitTimer, maxExitTimer);
				fishController.ReassignFish(this);
			}
			else
			{
				currentTarget = fishController.GetNewTargetPos();
			}
		} */

		if (bubbleButton && !bubbleButton.triggered)
		{
			currentTarget = new Vector3(bubbleButton.transform.position.x, bubbleButton.transform.position.y, currentTarget.z);
		}
		else
		{
			swapTimer -= Time.deltaTime;

			if (swapTimer <= 0)
			{
				swapTimer = Random.Range(minSwapTimer, maxSwapTimer);
				currentTarget = fishController.Swap(currentTarget);
			}
		}

		if (!exiting)
		{
			exitTimer -= Time.deltaTime;

			if (exitTimer <= 0)
				Exit();
			
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 nearPoint = Camera.main.ScreenToWorldPoint(
					new Vector3(Input.mousePosition.x, 
								Input.mousePosition.y, 
								Camera.main.nearClipPlane));
				
				Vector3 farPoint = Camera.main.ScreenToWorldPoint(
					new Vector3(Input.mousePosition.x, 
								Input.mousePosition.y, 
								Camera.main.farClipPlane));
				
				RaycastHit hit;

				if (Physics.Raycast(nearPoint, farPoint - nearPoint, out hit))
				{
					if (hit.transform.root == transform)
					{
						/* textureController.Flash();
						SpawnBubble();
						Exit(); */

						moveSpeed = maxMoveSpeed * 1.5f;
						rotSpeed = maxRotSpeed * 2;
						
						textureController.Flash();
						SpawnBubble();
					}
				}
			}
		}
	}

	public bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

	public void SpawnBubble()
	{
		bubbleButton = Instantiate(bubblePrefab).GetComponentInChildren<BubbleButton>();
		bubbleButton.transform.parent.SetParent(worldCanvas);
		bubbleButton.transform.parent.localScale = Vector3.one;
		bubbleButton.transform.parent.position = mouth.position;
		bubbleButton.message = message;
		bubbleButton.fish = this;
		bubbleButton.fishController = fishController;

		//newBubble.rigidbody2D.AddForce(mouth.forward * 10f);
	}

	public void Exit()
	{
		if (!exiting)
		{
			exiting = true;
			currentTarget = fishController.GetNewExitPos();
		}
	}
}
