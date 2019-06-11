using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
	public int maxFishOnScreen = 5;
	public Fish fishPrefab;

	public float startAndExitDistance;
	public Vector2 startAndExitBounds;
	public Transform[] targets;

	public BubbleButton bubble { get; set; }
	public Dictionary<Fish, long> fishes { get; set; }
    public float idleTime;
	private List<TwitterFishData> fishDataList = new List<TwitterFishData>();

	IEnumerator Start()
	{		
		fishes = new Dictionary<Fish, long>();

		while (fishes.Count < maxFishOnScreen)
		{
			TwitterFishData fishData = GetRandomFishData();

			if (fishData != null)
				AddFish(fishData);
			
			yield return null;
		}

		StartCoroutine(ClickOnRandomFish());
	}

	private float count;
	private Vector3 mouseDelta;
	IEnumerator ClickOnRandomFish()
	{
		while (true)
		{
			if (count < idleTime)
				count += Time.deltaTime;
			else
			{
				foreach (var fish in fishes)
				{
					if (!fish.Key.exitFlag)
					{
						fish.Key.ClickOnFish();
						break;
					}
				}

				count = 0;
			}
			
			if (Input.GetMouseButton(0) || Input.anyKey || mouseDelta != Input.mousePosition)
			{
				mouseDelta = Input.mousePosition;
				count = 0;
			}
			
			yield return null;
		}
	}

	public void LinkFishDataList(List<TwitterFishData> fishDataList)
	{
		this.fishDataList = fishDataList;
	}

	/* public Vector3 GetNewTargetPos()
	{
		Vector3 swimLimits = transform.localScale / 2f;

		return this.transform.position + new Vector3(Random.Range(-swimLimits.x, swimLimits.x),
													Random.Range(-swimLimits.y, swimLimits.y),
													Random.Range(-swimLimits.z, swimLimits.z));
	} */

	public Vector3 GetNewTargetPos()
	{
		Vector3 swimLimits = transform.localScale / 2f;

		return targets[Random.Range(0, targets.Length)].position;
	}

	public Vector3 Swap(Vector3 pos)
	{
		foreach (var target in targets)
		{
			if (pos != target.position)
				return target.position;
		}

		return pos;
	}

	public Vector3 GetNewStartPos()
	{
        float randomAngle = Random.Range(0, 360);
        float distance2Middle = 8;

        return new Vector3(Mathf.Cos(randomAngle) * distance2Middle, Mathf.Sin(randomAngle) * distance2Middle, Random.Range(-1.2f, 3f));
        /*
        //randomised spawn from origin
		return new Vector3(Random.Range(-startAndExitBounds.x, startAndExitBounds.x) / 2f,
						Random.Range(-startAndExitBounds.y, startAndExitBounds.y) / 2f, 
                        0);
        */
	}

	public Vector3 GetNewExitPos()
	{
		return new Vector3(startAndExitDistance,
						Random.Range(-startAndExitBounds.y, startAndExitBounds.y) / 2f,
						Random.Range(-startAndExitBounds.x, startAndExitBounds.x) / 2f);
	}

	public void AddFish(long id, string message, Texture texture = null)
	{
		Fish fish = Instantiate(fishPrefab);
		fish.fishController = this;
		fish.transform.position = GetNewStartPos();
		fish.transform.eulerAngles += Vector3.up * 180;
		//fish.GetComponentInChildren<Animator>().Play("forward", 0, Random.value);
		fish.currentTarget = GetNewTargetPos();
		if (texture)
			fish.textureController.ApplyTexture(texture);
		fish.id = id;
		fish.message = message;

		fishes.Add(fish, id);
	}

	public void ReassignFish(Fish fish)
	{
		StartCoroutine(ReassignFishCoro(fish));
	}

	IEnumerator ReassignFishCoro(Fish fish)
	{
		TwitterFishData fishData;

		while ((fishData = GetRandomFishData()) == null)
			yield return null;
		
		fishes[fish] = fishData.id;
		
		fish.transform.position = GetNewStartPos();
		fish.transform.eulerAngles += Vector3.up * 180;
        fish.currentTarget = GetNewTargetPos();
		if (fishData.texture)
			fish.textureController.ApplyTexture(fishData.texture);
		fish.id = fishData.id;
		fish.message = fishData.message;
	}

	public void AddFish(TwitterFishData fishData)
	{
		AddFish(fishData.id, fishData.message, fishData.texture);
	}

	public TwitterFishData GetRandomFishData(bool unique = true)
	{
		if (fishDataList.Count == 0)
			return null;
		
		if (maxFishOnScreen == fishDataList.Count || !unique)
		{
			return fishDataList[Random.Range(0, fishDataList.Count)];
		}
		else
		{
			int index = -1;

			index = Random.Range(0, fishDataList.Count);

			if (fishes.ContainsValue(fishDataList[index].id))
				return null;

			return fishDataList[index];
		}
	}

	public void AssignActiveBubble(BubbleButton bubble)
	{
		/* if (this.bubble != null)
			this.bubble.FadeOut(); */
		
		this.bubble = bubble;
	}
}
