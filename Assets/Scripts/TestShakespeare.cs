using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class TestShakespeare : MonoBehaviour
{
	[Header("Genetic Algorithm")]
	[SerializeField] int targetInt = 0;
	[SerializeField] string validCharacters;
	[SerializeField] int populationSize;
	[SerializeField] float mutationRate;
    [SerializeField] int elitism;

	[Header("Other")]
	string max = "111111111111";
    [SerializeField] Text targetText;
	[SerializeField] Text bestText;
	[SerializeField] Text bestFitnessText;
	[SerializeField] Text numGenerationsText;
	[SerializeField] Transform populationTextParent;
	[SerializeField] Text textPrefab;

	private GeneticAlgorithm<char> ga;
	private System.Random random;
    int maxInt;

    private string Binary;
    void Start()
	{
        //targetText.text = targetInt;
        Binary = Convert.ToString(targetInt, 2);

        if (Binary.Length == 0)
		{
			Debug.LogError("Target string is null or empty");
			this.enabled = false;
		}

		random = new System.Random();
		ga = new GeneticAlgorithm<char>(populationSize, max.Length, random, GetRandomCharacter, FitnessFunction, elitism, mutationRate);
	}

	void Update()
	{
		ga.NewGeneration();

		UpdateText(ga.BestGenes, ga.BestFitness, ga.Generation, ga.Population.Count, (j) => ga.Population[j].Genes);

		if (ga.BestFitness == 1)
		{
			this.enabled = false;
		}
	}

	private char GetRandomCharacter()
	{
		int i = random.Next(validCharacters.Length);
		return validCharacters[i];
	}

	private float FitnessFunction(int index)
	{
		float score = 0;
        int target = Convert.ToInt32(Binary, 2);
		int dna = Convert.ToInt32(new string(ga.Population[index].Genes), 2);
        score = 1 - (float)(Math.Abs(target - dna)) / maxInt;

        return score;
	}
    
	private int numCharsPerTextObj;
	private List<Text> textList = new List<Text>();

	void Awake()
	{
        maxInt = Convert.ToInt32(max, 2);
        numCharsPerTextObj = max.Length / validCharacters.Length;
		if (numCharsPerTextObj > populationSize) numCharsPerTextObj = populationSize;

		int numTextObjects = Mathf.CeilToInt((float)populationSize / numCharsPerTextObj);

		for (int i = 0; i < numTextObjects; i++)
		{
			textList.Add(Instantiate(textPrefab, populationTextParent));
		}
	}

	private void UpdateText(char[] bestGenes, float bestFitness, int generation, int populationSize, Func<int, char[]> getGenes)
	{
        bestText.text = Convert.ToInt32(new string(bestGenes), 2).ToString();

        //bestText.text = CharArrayToString(bestGenes);
		bestFitnessText.text = bestFitness.ToString();

		numGenerationsText.text = generation.ToString();

		for (int i = 0; i < textList.Count; i++)
		{
			int endIndex = i == textList.Count - 1 ? populationSize : (i + 1) * numCharsPerTextObj;
			for (int j = i * numCharsPerTextObj; j < endIndex; j++)
            {
                var sb = new StringBuilder();
                sb.Append(getGenes(j));
                textList[i].text = Convert.ToInt32(sb.ToString(), 2).ToString();
            }
		}
	}

	private string CharArrayToString(char[] charArray)
	{
		var sb = new StringBuilder();
		foreach (var c in charArray)
		{
			sb.Append(c);
		}

		return sb.ToString();
	}
}
