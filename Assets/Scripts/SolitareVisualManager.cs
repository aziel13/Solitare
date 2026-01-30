using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolitareVisualManager : MonoBehaviour
{
    public static SolitareVisualManager Instance{ get; private set; }
    
    [SerializeField] private Sprite[] cardFaces;
     
    
    private Dictionary<string,Sprite> cardFacesDictionary = new Dictionary<string,Sprite>();
    
    public Sprite[] CardFaces => cardFaces;

    private void Awake()
    {
        Instance = this;
    }

    public bool CardFaceExists(string cardKey)
    {
        return cardFacesDictionary.ContainsKey(cardKey);
    }

    public Sprite GetCardFace(string cardKey)
    {
        return cardFacesDictionary[cardKey];
    }


}
