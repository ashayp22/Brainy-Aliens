using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNet
{

    private int numInputs;
    private int numOutputs;
    private int numHiddenLayers;
    private int neuronsPerHiddenLayer;

    private List<NeuronLayer> layers = new List<NeuronLayer>();

    public void Init(int numInputs, int numOutputs, int hiddenLayers, int neuronsPerLayer)
    {
        this.numInputs = numInputs;
        this.numOutputs = numOutputs;
        this.numHiddenLayers = hiddenLayers;
        this.neuronsPerHiddenLayer = neuronsPerLayer;

        createNet();
    }

    private void createNet() //creates the neural network
    {
        if (numHiddenLayers > 0)
        {
            //input
            layers.Add(new NeuronLayer());
            layers[0].Init(neuronsPerHiddenLayer, numInputs);

            //hidden
            for (int i = 0; i < this.numHiddenLayers - 1; i++)
            {
                layers.Add(new NeuronLayer());
                layers[i + 1].Init(neuronsPerHiddenLayer, neuronsPerHiddenLayer);
            }

            //output
            layers.Add(new NeuronLayer());
            layers[layers.Count - 1].Init(numOutputs, neuronsPerHiddenLayer);

        }
        else
        {
            layers.Add(new NeuronLayer()); //only one layer
            layers[0].Init(numOutputs, numInputs);
        }
    }

    public List<double> getAllWeights() //returns all the weights in the neural network
    {
        List<double> allWeights = new List<double>();

        for (var i = 1; i < layers.Count; i++) //doesn't include the first layer(input)
        {
            allWeights.AddRange(layers[i].getLayerWeights());
        }
        return allWeights;
    }

    public void brainSurgery(List<double> newWeights) //inserts mutated weights back into the neural network
    {
        int indexOn = 0;
        for (var i = 1; i < layers.Count; i++)
        { //each layer
            for (var j = 0; j < layers[i].neuronLayer.Count; j++)
            { //for each neuron in the layer
                var num_of_weights = layers[i].neuronLayer[j].numInputs + 1; //gets how many weights connected to neuron
                layers[i].neuronLayer[j].weights = sliceList(newWeights, indexOn, num_of_weights); //adds in the weights
                indexOn += num_of_weights; //index for next neuron increases by number of weights already added
            }

        }

    }

    private List<double> sliceList(List<double> list, int start, int items) //slices a list from a certain index for # of items
    {
        List<double> newList = new List<double>();
        for (int i = 0; i < items; i++)
        {
            newList.Add(list[start + i]);
        }
        return newList;
    }


    private double Sigmoid(double activation) //sigmoid function
    {
        return 1 / (1 + Mathf.Pow(Mathf.Exp(1), (float)((-1 * activation)/0.2)));
    }


    public List<double> FeedForward(List<double> doubleInputs) //returns an output based on inputs
    {
        List<double> doubleOutputs = new List<double>();
        int weight = 0;

        for (int i = 0; i < numHiddenLayers + 1; i++)
        { //for each layer

            if (i > 0) //if it is the hiddenlayer +, then the inputs are the previous outputs
            {
                doubleInputs = doubleOutputs;
            }

            doubleOutputs = new List<double>(); //outputs are reset
            weight = 0; //weights of each neuron

            for (int j = 0; j < layers[i].neuronNum; j++)
            { //for each neuron
                double netinput = 0;

                int NumInputs = layers[i].neuronLayer[j].numInputs; //the inputs of the neuron

                for (var k = 0; k < NumInputs - 1; k++) //for each weight
                { //for each weight
                    netinput += layers[i].neuronLayer[j].weights[k] * doubleInputs[weight++];
                }

                netinput += layers[i].neuronLayer[j].weights[NumInputs - 1] * -1; //adds on the weight to the netinput of the neuron, times the bias

                doubleOutputs.Add(Sigmoid(netinput)); //adds to the outputs with the activation function
                weight = 0; //weight is reset
            }
        }

        return doubleOutputs; //returns the outputs

    }

}
