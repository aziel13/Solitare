using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI _debugText;
    [SerializeField] private TextMeshProUGUI[] Columns;
    [SerializeField] private TextMeshProUGUI deckTextMesh;
    [SerializeField] private TextMeshProUGUI tripsAndDiscardTextMesh;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.OnPrintDebugMessage += GameManager_OnPrintDebugMessage;
    }

    private void GameManager_OnPrintDebugMessage(object sender, GameManager.OnPrintDebugMessageEventArgs e)
    {
        
      //  Debug.Log($"Updating debug text values");
        string outtext = "Deck:\n\n";
            
        
        Debug.Log("Deck: ");

        // output whats in the deck list
        
        foreach (CardScriptableObject card in e.deck)
        {
         //   Debug.Log($"{card.ToString()}");
            
            outtext  += $"{card.ToString()}"; 
             
            outtext  += $"\n"; 
        }
        
        deckTextMesh.text = outtext ;
        
        outtext = "Trips:\n\n";
      //  Debug.Log("Trips:");

        // output what trips are on display if any
        foreach (CardScriptableObject card in e.trips)
        {
            outtext += $"{card.ToString()}";
        //    Debug.Log($"{card.ToString()}");

            outtext += $"\n";

        }
        outtext += "\nDiscard Pile:\n\n";
      //  Debug.Log("Discard Pile:");

        
        // output whats in the discard pile if any
        foreach (CardScriptableObject card in e.discard)
        {
            outtext += $"{card.ToString()}\n";
         //   Debug.Log($"{card.ToString()}");
        }
        
        tripsAndDiscardTextMesh.text = outtext ;
        
        
        int j = 0;
        // output contents of the tableau and foundation columns lists
       for(int i = 0; i < 7; i++)
       {
           Columns[i].text = "";
           if (i < 3)
           {
               
               Columns[i].text += "\n\n\n\n";
               
           }
           else
           {
               Columns[i].text += $"Foundation Column {j}\n";
               
               if (e.tops.Length == 4 && e.tops[j].Count > 0)
               {
                   foreach (CardScriptableObject card in e.tops[j])
                   {
                       Columns[i].text += $"{card.ToString()}\n";
                   }

               }
               else
               {
                   Columns[i].text += "\n\n\n";
               }

               j++;
           }

           Columns[i].text += $"Tableau Column {i+1}\n";
           foreach (CardScriptableObject card in e.bottoms[i])
           {
               Columns[i].text += $"{card.ToString()}\n";
           }

       }
        
        
        
        
    }

}
