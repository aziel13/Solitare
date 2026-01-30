using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    
    public static GameInput Instance{get; private set;}

    private static GameObject previouslySelected;

    public GameObject PreviouslySelected
    {
        get => previouslySelected;
        set => previouslySelected = value;
    }
    
    private InputAction mouseClickAction;
    private InputAction mousePositionAction;
    private InputActions _inputActions;
    
    [SerializeField] private bool InputActions_enabled = false;
    
    [SerializeField] private Camera thisCamera;

    public event EventHandler OnPlaymatClickedDeselect;
    
    public event EventHandler OnDeckClicked;
    
    public event EventHandler<OnCardClickedEventArgs> OnCardClicked;
    public class OnCardClickedEventArgs : EventArgs
    {
        public GameObject currentObject;
        public GameObject previousObject;
        public CardActions.actions action;
    }
    
    public event EventHandler<OnTableauClickedEventArgs> OnTableauClicked;
    
    public class OnTableauClickedEventArgs : EventArgs
    {
        public GameObject currentGameObject;
        public GameObject previousObject;
    }
    
    public event EventHandler<OnFoundationClickedEventArgs> OnFoundationClicked;
    
    public class OnFoundationClickedEventArgs : EventArgs
    {
        public GameObject currentGameObject;
        public GameObject previousObject;
    }

    private void Awake()
    {
        Instance = this;
        _inputActions =  new InputActions();
        _inputActions.Enable();
        mouseClickAction = _inputActions.Player.OnLeftMouseClick;
        mousePositionAction = _inputActions.Player.MousePosition;
        
        mouseClickAction.performed += OnMouseClick;
        
        mouseClickAction.Enable();
        mousePositionAction.Enable();
    }
    
    public void OnEnable()
    {
        
        InputActions_enabled =  true;
        
    }
    

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        
        Vector2 mousePosition = mousePositionAction.ReadValue<Vector2>();
        
        Vector3 worldPosition = thisCamera.ScreenToWorldPoint(mousePosition);
        
        var rayHit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (rayHit.collider == null) {
            Debug.Log("Ray did not hit");
            
            
            return;
        }
        
        GameObject hitGameObject = rayHit.collider.gameObject;

        if (hitGameObject.TryGetComponent(out Playmat playmat))
        {
            Clicked($"Play mat hit");
            previouslySelected = null;
            OnPlaymatClickedDeselect?.Invoke(this, EventArgs.Empty);
        }

        if (hitGameObject.TryGetComponent(out Card card))
        {
            Clicked($"{card.CardScriptableObject.ToString()}");
            
            string istopCard = (card.IsTopCard()) ? "is the top card": "is not the top card";
            
           // Debug.Log($"{card.CardScriptableObject.ToString()} is {istopCard}");
            
            Card(hitGameObject);
            
        }
        
        if (hitGameObject.TryGetComponent(out Stockpile stockpile))
        {
            Clicked($"the deck button");
            Stockpile();

        }

        if (hitGameObject.TryGetComponent(out Foundation foundation))
        {
            Clicked($"{foundation.ToString()}");
            Foundation(hitGameObject);
        }

        if (hitGameObject.TryGetComponent(out Tableau tableau))
        {
            Clicked($"{tableau.ToString()}");
            Tableau(hitGameObject);
        }
        
    
    }

    private void Clicked( string name)
    {
        
        Debug.Log($"{name} was clicked");
        
    }
     


    private void OnDisable()
    {
        DisableInput();
    }

    private void OnDestroy()
    {
        DisableInput();
    }

    private void DisableInput()
    {
        InputActions_enabled = false;
        mouseClickAction.Disable();
        mousePositionAction.Disable();
    }
    
    private void Card(GameObject selectedCard)
    {
        
        if (previouslySelected == null )
        {
            previouslySelected = selectedCard;
            OnCardClicked?.Invoke(this, new OnCardClickedEventArgs()
                {
                    currentObject = selectedCard,
                    action = CardActions.actions.selectCard,
                }
            );            
        }

        if (previouslySelected != selectedCard)
        {
            OnCardClicked?.Invoke(this, new OnCardClickedEventArgs()
                {
                    currentObject = selectedCard,
                    previousObject = previouslySelected,
                    action = CardActions.actions.stackOnCard,
                }
            );   
        }

    }

    private void Stockpile()
    {
        OnDeckClicked?.Invoke(this, EventArgs.Empty);
    }

    private void Foundation(GameObject selectedFoundation)
    {
      
        OnFoundationClicked?.Invoke(this, new OnFoundationClickedEventArgs()
            {
                currentGameObject = selectedFoundation,
                previousObject = previouslySelected,
            }
        );   
        
    }

    private void Tableau(GameObject selectedTableau)
    {
        
        OnTableauClicked?.Invoke(this, new OnTableauClickedEventArgs()
            {
                currentGameObject = selectedTableau,
                previousObject = previouslySelected,
            }
        );   
        
    }

}