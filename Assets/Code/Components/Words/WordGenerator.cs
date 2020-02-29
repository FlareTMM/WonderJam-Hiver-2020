﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This represents the word generator, it spawns words depending on a variety of variables
 * using a generator configuration (WordGenConfig) to know what to generate.
 */
public class WordGenerator : MonoBehaviour
{
    [Tooltip("This generator's configuration values. Will update the generator if hotswappped")]
    public WordGenConfig GeneratorConfiguration;

    [Tooltip("The time it takes to reach difficulty 10 from 1")]
    public int TimeToReachDiffCap;

    private WordGenConfig m_genConfig;
    private int CurrentDifficulty;

    [SerializeField] private List<WordWrapper> m_wordDictionary;
    [SerializeField] private List<WordWrapper> m_activeWords;

    void Start()
    {
        CurrentDifficulty = 1;
        m_wordDictionary = new List<WordWrapper>();
        m_activeWords = new List<WordWrapper>();
        m_genConfig = GeneratorConfiguration;

        Generate();

        StartCoroutine(IncrementDifficulty());
    }

    void Update()
    {
        if(m_genConfig != GeneratorConfiguration)
        {
            m_genConfig = GeneratorConfiguration;

            Generate();
        }
    }

    private IEnumerator IncrementDifficulty()
    {
        while(true) // TODO: change this to not run until it hits diff 10
        {
            yield return new WaitForSeconds(TimeToReachDiffCap / 9f);

            CurrentDifficulty++;

            Generate();

            if(CurrentDifficulty == 10) break;
        }
    }

    public WordWrapper GetWord()
    {
        if(m_wordDictionary.Count == 0) Generate();

        int selectedIndex = Random.Range(0, m_wordDictionary.Count);
        WordWrapper selected = m_wordDictionary[selectedIndex];

        m_wordDictionary.RemoveAt(selectedIndex);
        m_activeWords.Add(selected);

        return selected;
    }

    public List<WordWrapper> GetActiveWords()
    {
        return new List<WordWrapper>(m_activeWords);
    }

    public void RemoveActiveWord(WordWrapper p_activeWord)
    {
        if(m_activeWords.Contains(p_activeWord))
            m_activeWords.Remove(p_activeWord);
    }

    private void Generate()
    {
        if(m_genConfig && m_genConfig.IsValid())
        {
            Debug.Log("Generating word dictionary at difficulty " + CurrentDifficulty + "...");

            m_wordDictionary.Clear();

            List<WordWrapper> availableWords = new List<WordWrapper>();

            foreach(WordWrapper ww in m_genConfig.Words)
                if(ww.Difficulty <= CurrentDifficulty)
                    availableWords.Add(ww);

            float totalProbability = 0;

            foreach(WordWrapper ww in availableWords)
                totalProbability += ww.Probability;

            float probabilityScale = 100f / totalProbability;

            foreach(WordWrapper ww in availableWords)
                for(int i = 0; i < Mathf.RoundToInt(m_genConfig.DictionarySize * (ww.Probability * probabilityScale / 100f)); i++)
                    m_wordDictionary.Add(ww);

            Debug.Log("Generated a " + m_wordDictionary.Count + " word dictionary with " + 
                      availableWords.Count + " different words!");
        }
    }
}