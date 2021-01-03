using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain
// : MonoBehaviour
{
  private LifeController lc;

  private const int HIDDEN_SIZE = 12;
  private const int MAX_WEIGHT = 10;
  public int[] layer_size = new int[3];
  private float[][][] weights;
  public float[][] activation;

  private int MEMORY_SIZE = 15;
  private float[] memory;
  private float[] x;


  public Brain(LifeController lc)
  {
    this.lc = lc;

    layer_size[0] = 1 + lc.VisionRays + MEMORY_SIZE;
    layer_size[1] = HIDDEN_SIZE;
    layer_size[2] = 4 + MEMORY_SIZE;

    this.memory = new float[MEMORY_SIZE];
    this.x = new float[1 + lc.VisionRays + MEMORY_SIZE];

    weights = new float[layer_size.Length - 1][][];

    activation = new float[3][];
    for(int i = 0 ;i < layer_size.Length; i++){
        activation[i] = new float[layer_size[i]];
    }
  }

  public void think(float[] vision, float healthP)
  {
    float []x = this.x;
    x[0] = healthP;
    for(int i =0; i < vision.Length; i++){
        x[i + 1] = vision[i];
    }
    for(int i = 0; i < memory.Length; i++){
        x[i + vision.Length +1] = memory[i];
    }

    for (int i = 0; i < weights.Length; i++)
    {
      for(int j = 0; j < x.Length; j++){
          activation[i][j] = x[j];
      }
      x = layer(x, i);
    }
    for(int j = 0; j < x.Length; j++){
        activation[2][j] = x[j];
    }
    for(int j = 0; j < MEMORY_SIZE; j++){
        memory[j] = activation[2][4+j];
    }
    lc.Hue = x[0];
    lc.Force = x[1] * 2 - 1;
    lc.Rotation = x[2] * 2 - 1;
    lc.scale = x[3] * (lc.maxScale - 1) + 1;
  }

  private float[] layer(float[] inp, int l)
  {
    float[] ans = new float[layer_size[l + 1]];

    for (int i = 0; i < ans.Length; i++)
    {
      ans[i] = 0;

      for (int k = 0; k < inp.Length; k++)
      {
        ans[i] += weights[l][k][i] * inp[k];
      }
      ans[i] += weights[l][inp.Length][i];
      ans[i] = sigmoid(ans[i]);
    }

    return ans;
  }

  public static float sigmoid(float x)
  {
    return 1 / (1 + Mathf.Exp(-x));
  }

  public void ImportDNA(float[] dna)
  {
    // TODO
    int counter = 0;
    for (int i = 0; i < weights.Length; i++)
    {
      weights[i] = new float[layer_size[i] + 1][];
      for (int j = 0; j <= layer_size[i]; j++)
      {
        weights[i][j] = new float[layer_size[i + 1]];
        for (int k = 0; k < layer_size[i + 1]; k++)
        {
          weights[i][j][k] = dna[counter];
          counter++;
        }
      }
    }
  }

  public float[] ExportDNA()
  {
    // TODO
    int size = 0;
    for (int i = 0; i < weights.Length; i++)
      size += (layer_size[i] + 1) * layer_size[i + 1];

    float[] ans = new float[size]; // TODO size?!
    int counter = 0;
    for (int i = 0; i < weights.Length; i++)
    {
      for (int j = 0; j <= layer_size[i]; j++)
      {
        for (int k = 0; k < layer_size[i + 1]; k++)
        {
          ans[counter] = weights[i][j][k];
          counter++;
        }
      }
    }
    return ans;
  }

  public void RandomDNA()
  {
    for (int i = 0; i < weights.Length; i++)
    {
      weights[i] = new float[layer_size[i] + 1][];
      for (int j = 0; j <= layer_size[i]; j++)
      {
        weights[i][j] = new float[layer_size[i + 1]];
        for (int k = 0; k < layer_size[i + 1]; k++)
        {
          weights[i][j][k] = Random.value * 2 * MAX_WEIGHT - MAX_WEIGHT;
        }
      }
    }
  }

  public static float[] MergeDNA(float[] dna1, float[] dna2, float mutate)
  {
    float[] ans = new float[dna1.Length];

    for (int i = 0; i < dna1.Length; i++)
    {
      ans[i] = Random.value < 0.5 ? dna1[i] : dna1[2];
      if (Random.value < mutate)
        ans[i] = Random.value * 2 * MAX_WEIGHT - MAX_WEIGHT;
    }

    return ans;
  }

}
