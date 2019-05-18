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
	public Dictionary<Fish, string> fishes { get; set; }

	private List<FishData> fishDataList = new List<FishData>();

	IEnumerator Start()
	{		
		fishes = new Dictionary<Fish, string>();

		while (fishes.Count < maxFishOnScreen)
		{
			FishData fishData = GetRandomFishData();

			if (fishData != null)
				AddFish(fishData);
			
			yield return null;
		}
	}

	public void LinkFishDataList(List<FishData> fishDataList)
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
		return new Vector3(startAndExitDistance,
						Random.Range(-startAndExitBounds.y, startAndExitBounds.y) / 2f,
						Random.Range(-startAndExitBounds.x, startAndExitBounds.x) / 2f);
	}

	public Vector3 GetNewExitPos()
	{
		return new Vector3(startAndExitDistance,
						Random.Range(-startAndExitBounds.y, startAndExitBounds.y) / 2f,
						Random.Range(-startAndExitBounds.x, startAndExitBounds.x) / 2f);
	}

	public void AddFish(string guid, string message, Texture texture)
	{
		Fish fish = Instantiate(fishPrefab);
		fish.fishController = this;
		fish.transform.position = GetNewStartPos();
		fish.transform.eulerAngles += Vector3.up * 180;
		fish.GetComponentInChildren<Animator>().Play("Swim", 0, Random.value);
		fish.currentTarget = GetNewTargetPos();
		fish.textureController.ApplyTexture(texture);
		fish.guid = guid;
		fish.message = message;

		fishes.Add(fish, guid);
	}

	public void ReassignFish(Fish fish)
	{
		StartCoroutine(ReassignFishCoro(fish));
	}

	IEnumerator ReassignFishCoro(Fish fish)
	{
		FishData fishData;

		while ((fishData = GetRandomFishData()) == null)
			yield return null;
		
		fishes[fish] = fishData.guid;
		
		fish.transform.position = GetNewStartPos();
		fish.transform.eulerAngles += Vector3.up * 180;
        fish.currentTarget = GetNewTargetPos();        
        fish.textureController.ApplyTexture(fishData.texture);
		fish.guid = fishData.guid;
		fish.message = fishData.message;
	}

	public void AddFish(FishData fishData)
	{
		AddFish(fishData.guid, fishData.message, fishData.texture);
	}

	public FishData GetRandomFishData(bool unique = true)
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

			if (fishes.ContainsValue(fishDataList[index].guid))
				return null;

			return fishDataList[index];
		}
	}

	public void AssignActiveBubble(BubbleButton bubble)
	{
		if (this.bubble != null)
			this.bubble.FadeOut();
		
		this.bubble = bubble;
	}
}
