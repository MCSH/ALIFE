using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain
// : MonoBehaviour
{
  private LifeController lc;

  private const int HIDDEN_SIZE = 10;
  private const int MAX_WEIGHT = 10;
  private int[] layer_size = new int[3];
  private float[][][] weights;


  public Brain(LifeController lc)
  {
    this.lc = lc;

    layer_size[0] = lc.VisionRays;
    layer_size[1] = HIDDEN_SIZE;
    layer_size[2] = 3;

    weights = new float[layer_size.Length - 1][][];
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

  public void think(float[] vision)
  {

    float[] x = vision;
    for (int i = 0; i < weights.Length; i++)
    {
      x = layer(x, i);
    }
    // TODO

    lc.Hue = x[0];
    lc.Force = x[1] * 2 - 1;
    lc.Rotation = x[2] * 2 - 1;

    // lc.Hue = Random.value;
    // lc.Force = Random.value * 2 - 1;
    // lc.Rotation = Random.value * 2 - 1;
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

}
