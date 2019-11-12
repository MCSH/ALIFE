using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{

  public GameObject lifePrefab;
  public GameObject wallPrefab;
  public GameObject foodPrefab;

  public int MaxHP = 100;

  public int RoomSize = 10;

  public int remainingCells = 0;

  public int FoodLimit = 500;
  public int remainingFood = 0;

  public int FoodStep = 200;
  public int populationSize = 10;

  private LifeController[] cells;

  public float mutationRate = 0.3f;

  void Start()
  {
    cells = new LifeController[populationSize];
    for (int i = 0; i < populationSize; i++)
    {
      GameObject go = Instantiate<GameObject>(
        lifePrefab,
        new Vector3(Random.Range(-RoomSize + 5, RoomSize - 5), Random.Range(-RoomSize + 5, RoomSize - 5), 0),
        Quaternion.Euler(0, 0, Random.Range(0, 360)));

      cells[i] = go.GetComponent<LifeController>();
      cells[i].brain.RandomDNA();
      remainingCells++;
    }

    for (int i = -RoomSize; i < RoomSize; i++)
    {
      Instantiate(wallPrefab, new Vector3(i + 1, -RoomSize, 0), Quaternion.identity);
      Instantiate(wallPrefab, new Vector3(-RoomSize, i, 0), Quaternion.identity);
      Instantiate(wallPrefab, new Vector3(RoomSize, i + 1, 0), Quaternion.identity);
      Instantiate(wallPrefab, new Vector3(i, RoomSize, 0), Quaternion.identity);
    }

    GenerateFood(FoodLimit);

  }

  public void removeCell(GameObject o)
  {
    remainingCells--;
    o.SetActive(false);
  }

  public void removeFood(GameObject o)
  {
    remainingFood--;
    Destroy(o);
    // TODO
  }

  // Update is called once per frame
  void Update()
  {
    if (remainingCells == 0)
    {
      // Stop();
      Breed();
    }

    if (remainingFood < FoodLimit - FoodStep)
    {
      // TODO
      GenerateFood(FoodLimit - remainingFood);
    }
  }

  void Stop()
  {
    // TODO stop the game
    print("No more remaining cells");
#if UNITY_EDITOR
      if (Application.isEditor) UnityEditor.EditorApplication.isPlaying = false; 
#endif
    Application.Quit();
  }

  void GenerateFood(int amount)
  {
    for (int i = 0; i < amount; i++)
    {
      Instantiate(foodPrefab, new Vector3(
        Random.Range(-RoomSize + 5, RoomSize - 5),
       Random.Range(-RoomSize + 5, RoomSize - 5),
       0), Quaternion.identity);
    }
    remainingFood += amount;
  }

  void Breed()
  {
    print("Breeding");

    // int j;
    for (int i = 1; i < populationSize; i++)
    {
      int j = i - 1;
      LifeController lc = cells[i];
      while (j >= 0 && cells[j].score < lc.score)
      {
        cells[j + 1] = cells[j];
        j--;
      }
      cells[j + 1] = lc;
    }

    // Create the second half based on first half
    for (int i = populationSize / 2; i < populationSize; i++)
    {
      cells[i].brain.ImportDNA(
        Brain.MergeDNA(
          cells[i - populationSize / 2].brain.ExportDNA(),
          cells[i - populationSize / 2 + 1].brain.ExportDNA(),
          mutationRate
        )
      );
    }

    for (int i = 0; i < populationSize; i++)
    {
      cells[i].gameObject.SetActive(true);
      cells[i].transform.position = new Vector3(Random.Range(-RoomSize + 5, RoomSize - 5), Random.Range(-RoomSize + 5, RoomSize - 5), 0);
      cells[i].hp = MaxHP;
      remainingCells++;
    }

    // repopulate food
    GenerateFood(FoodLimit - remainingFood);
  }

}
