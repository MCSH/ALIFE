using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{

  public GameObject lifePrefab;
  public GameObject wallPrefab;
  public GameObject foodPrefab;

  public int RoomSize = 10;

  public int remainingCells = 0;

  public int FoodLimit = 500;
  public int remainingFood = 0;

  public int FoodStep = 200;

  void Start()
  {
    for (int i = 0; i < 10; i++)
    {
      Instantiate(lifePrefab, new Vector3(i * 2, i, 0), Quaternion.Euler(0, 0, Random.Range(0, 360)));
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

  public void addCell(LifeController o)
  {
    remainingCells++;
  }

  public void removeCell(LifeController o)
  {
    remainingCells--;
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
      Stop();
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

}
