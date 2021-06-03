using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neurons
{
    //instance fields
    public int numInputs; //number of inputs
    public List<double> weights; //weights going into the neuron

    //constructor
    public void Init(int numInputs)
    {
        this.numInputs = numInputs;
        weights = new List<double>();
        for (int i = 0; i < numInputs + 1; i++) //plus one for the bias 
        {
            weights.Add(this.RandomClamped(-1, 1));
        }
    }

    private double RandomClamped(float min, float max) //random weight
    {
        return Random.Range(min, max);
    }

    public List<double> getWeights() //returns all weights
    {
        return weights;
    }

}
