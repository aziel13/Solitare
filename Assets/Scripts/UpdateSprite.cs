
using System;
using UnityEngine;
using System.Collections.Generic;

public class UpdateSprite : MonoBehaviour
{
    
    
    
    [SerializeField]private Sprite cardFace;

    public Sprite CardFace
    {
        get => cardFace;
        set => cardFace = value;
    }

    [SerializeField]private Sprite cardBack;
    private SpriteRenderer _spriteRenderer;
   
    private GameManager _gameManager;

    private List<string> _unsortedDeck;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = cardBack;
        
        GameInput.Instance.OnPlaymatClickedDeselect += Playmat_OnPlaymatClickedDeselect;
    }

    
    
    private void Start()
    {
        
        _gameManager = GameManager.Instance;
        

    }

    private void Playmat_OnPlaymatClickedDeselect(object sender, EventArgs e)
    {
        if (_spriteRenderer.color == Color.yellow)
        {
            
            Debug.Log($"Clearing Select on {gameObject.GetComponent<Card>().CardScriptableObject.ToString()}");
            SetCardSelected(false);
        }
        
    }
    
    public void setFaceSprite(Sprite sprite)
    {
        cardFace = sprite;
    }

    public void SetCardOrientation(bool is_face_up)
    {
        if (is_face_up)
        {
            _spriteRenderer.sprite = CardFace;
        }
        else
        {
            _spriteRenderer.sprite = cardBack;
        }

    }

    public void SetCardSelected(bool is_selected)
    {

        if (is_selected)
        {
            _spriteRenderer.color = Color.yellow;
        }
        else
        {
            _spriteRenderer.color = Color.white;
        }

    }
}
