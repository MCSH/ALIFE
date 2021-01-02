using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{

  public GameObject lifePrefab;
  public GameObject wallPrefab;
  public GameObject foodPrefab;
  public GameObject uiCellPrefab;
  public GameObject generationText;
  public GameObject descriptiveText;
  public GameObject nCameraObj;

  private Camera nCamera;

  public LifeController selected;

  public int MaxHP = 100;

  public int RoomSize = 10;

  public int remainingCells = 0;

  public int FoodLimit = 500;
  public int remainingFood = 0;

  public int FoodStep = 200;
  public int populationSize = 10;

  private LifeController[] cells;

  public float mutationRate = 0.3f;

  private int generationCount = 0;

  public bool paused = false;

  public int nX = -2060, nY = -2025;
  public int ncw = 5, nch = 5;

  void Start()
  {
    nCamera = nCameraObj.GetComponent<Camera>();
    nCamera.enabled = false;

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

    incGenerationText();
    setDescriptiveText("Nothing Selected.");

    createBrainUI();
  }

  private UICell[][] uicells;

  private void createBrainUI(){
      int s = cells[0].brain.layer_size.Length;
      uicells = new UICell[s][];

      for(int i = 0; i < s; i++){
          uicells[i] = new UICell[cells[0].brain.layer_size[i]];
      }

      for(int i = 0; i < s; i ++){
          for(int j = 0; j < uicells[i].Length; j++){
              // Instantiate the UI
              int x = nX + (2* i+1)*ncw;
              int y = nY + (2* j+1) * nch;
              GameObject go = Instantiate(uiCellPrefab, new Vector3(x, y, 0), Quaternion.identity);
              uicells[i][j] = go.GetComponent<UICell>();
          }
      }
  }

  public void selectCell(LifeController lc){
      selected = lc;
      if(lc != null){
          nCamera.enabled = true;
      } else {
          nCamera.enabled = false;
          setDescriptiveText("Nothing Selected.");
      }
  }

  private void updateUICells(){
      if(selected == null) return;
      for(int i = 0; i < uicells.Length; i++){
          for(int j = 0; j < uicells[i].Length; j++){
              uicells[i][j].hue = selected.brain.activation[i][j];
          }
      }
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

    if(selected != null){
        setDescriptiveText("HP: " + selected.hp);
    }

    if(Input.GetKeyDown("space")){
        Debug.Log("Pause");
        paused = !paused;
    }

    if(selected && selected.hp <= 0)
        selectCell(null);

    updateUICells();
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

  void incGenerationText(){
    generationCount++;
    UnityEngine.UI.Text gt = generationText.GetComponent<UnityEngine.UI.Text>();
    gt.text = "Generation " + generationCount;
  }

  void setDescriptiveText(string str){
    UnityEngine.UI.Text dt = descriptiveText.GetComponent<UnityEngine.UI.Text>();
    dt.text = str;
  }

  void Breed()
  {
    selectCell(null);
    incGenerationText();
    print("Breeding " + generationCount);

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
