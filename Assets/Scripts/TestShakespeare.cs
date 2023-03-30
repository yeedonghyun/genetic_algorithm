using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TestShakespeare : MonoBehaviour
{
	[Header("Genetic Algorithm")]
	[SerializeField] int targetInt = 0;
	[SerializeField] string validCharacters;
	[SerializeField] int populationSize;
	[SerializeField] float mutationRate;
    [SerializeField] int elitism;

	[Header("Other")]
	[SerializeField] int numCharsPerText;
	[SerializeField] Text targetText;
	[SerializeField] Text bestText;
	[SerializeField] Text bestFitnessText;
	[SerializeField] Text numGenerationsText;
	[SerializeField] Transform populationTextParent;
	[SerializeField] Text textPrefab;

	private GeneticAlgorithm<char> ga;
	private System.Random random;

    private string Binary;

    void Start()
	{
		//targetText.text = targetInt;
		Binary = Ten2To(targetInt);

        if (Binary.Length == 0)
		{
			Debug.LogError("Target string is null or empty");
			this.enabled = false;
		}

		random = new System.Random();
		ga = new GeneticAlgorithm<char>(populationSize, Binary.Length, random, GetRandomCharacter, FitnessFunction, elitism, mutationRate);
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
		DNA<char> dna = ga.Population[index];

		for (int i = 0; i < dna.Genes.Length; i++)
		{
			if (dna.Genes[i] == Binary[i])
			{
				score += 1;
			}
		}

		score /= Binary.Length;

		score = (Mathf.Pow(2, score) - 1) / (2 - 1);

		return score;
	}
    
	private int numCharsPerTextObj;
	private List<Text> textList = new List<Text>();

	void Awake()
	{
		numCharsPerTextObj = numCharsPerText / validCharacters.Length;
		if (numCharsPerTextObj > populationSize) numCharsPerTextObj = populationSize;

		int numTextObjects = Mathf.CeilToInt((float)populationSize / numCharsPerTextObj);

		for (int i = 0; i < numTextObjects; i++)
		{
			textList.Add(Instantiate(textPrefab, populationTextParent));
		}
	}

	private void UpdateText(char[] bestGenes, float bestFitness, int generation, int populationSize, Func<int, char[]> getGenes)
	{
		bestText.text = CharArrayToString(bestGenes);
		bestFitnessText.text = bestFitness.ToString();

		numGenerationsText.text = generation.ToString();

		for (int i = 0; i < textList.Count; i++)
		{
			var sb = new StringBuilder();
			int endIndex = i == textList.Count - 1 ? populationSize : (i + 1) * numCharsPerTextObj;
			for (int j = i * numCharsPerTextObj; j < endIndex; j++)
			{
				foreach (var c in getGenes(j))
				{
					sb.Append(c);
				}
				if (j < endIndex - 1) sb.AppendLine();
			}

			textList[i].text = sb.ToString();
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

    private string Ten2To(int num)
	{
		string binary = "";

		while (num != 0)
		{
			binary += num % 2;
			num /= 2;
        }

        return new string(binary.Reverse().ToArray());
    }

    private int To2Ten(string num)
    {
        int deci = 0;
		int index = 1;

        for(int i = 0; i < num.Length; i++)
		{
			deci += num[i * index];
			index *= 2;
        }

        return deci;
    }
}
