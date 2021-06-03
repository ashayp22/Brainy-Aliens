using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronLayer {

    //instance fields
    public int neuronNum; //number of neurons in the layer
    public List<Neurons> neuronLayer; 

    //constructor
    public void Init(int neuronNum, int numInputs)
    {
        neuronLayer = new List<Neurons>();
        this.neuronNum = neuronNum;

        for (int i = 0; i < neuronNum; i++)
        {
            neuronLayer.Add(new Neurons());
            neuronLayer[i].Init(numInputs);
        }
    }

   public List<double> getLayerWeights() //returns the weights of each neuron
    {
        List<double> layerWeights = new List<double>();

        for (int i = 0; i < neuronLayer.Count; i++)
        {
            layerWeights.AddRange(neuronLayer[i].getWeights()); //concats the total weights with the current neuron's weights
        }
        return layerWeights;
    }

    public int getNeuronNum()
    {
        return neuronNum;
    }

}
