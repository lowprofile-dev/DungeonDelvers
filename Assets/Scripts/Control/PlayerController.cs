using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EncryptStringSample;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using SkredUtils;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : SerializedMonoBehaviour
{
    private static PlayerController _instance = null;
    public static PlayerController Instance => _instance;

    public int PartyLevel = 1;
    public int CurrentExp = 0;
    [ShowInInspector] public int ExpToNextLevel => (int)(5 + 10*PartyLevel + 4*Mathf.Pow(PartyLevel-1,2));
    
    [HideInEditorMode] public List<Character> Party = new List<Character>();
    [HideInEditorMode] public List<Item> Inventory = new List<Item>();

//#if UNITY_EDITOR
    [HideInPlayMode] public List<CharacterBase> PartyBases = new List<CharacterBase>();
    [HideInPlayMode] public List<ItemBase> InventoryBase = new List<ItemBase>();
//#endif

    public float movementSpeed;
    public GameObject OverworldCharacter;
    [ReadOnly] public PlayerDirection Direction;
    [ReadOnly] public PlayerState State;
    private Vector2 Front
    {
        get
        {
            switch (Direction)
            {
                case PlayerDirection.Up:
                    return new Vector2(0, 1);
                case PlayerDirection.Right:
                    return new Vector2(1,0);
                case PlayerDirection.Down:
                    return new Vector2(0,-1);
                case PlayerDirection.Left:
                    return new Vector2(-1,0);
                default:
                    return new Vector2(0, 0);
            }
        }
    }
    private Animator OverworldCharacterAnimator;
    public GameObject MainMenuPrefab;
    
    private void Awake()
    {
        if (_instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
//#if UNITY_EDITOR
        Party = PartyBases.EachDo(characterBase => new Character(characterBase));
        Inventory = InventoryBase.EachDo(ItemInstanceBuilder.BuildInstance);
//#endif
        GameController.Instance.OnBeginEncounter.AddListener(PauseGame);
        GameController.Instance.OnEndEncounter.AddListener(UnpauseGame);
        SetOverworldCharacter();
    }

    private void SetOverworldCharacter()
    {
        if (Party.Count == 0)
            return;
        
//        if (OverworldCharacter != null && OverworldCharacter.name == Party.First().Base.uniqueIdentifier)
//            return;
        
        if (OverworldCharacter != null)
            Destroy(OverworldCharacter);

        OverworldCharacter = Instantiate(Party.First().Base.CharacterPrefab,transform);
        OverworldCharacter.name = Party.First().Base.uniqueIdentifier;
        OverworldCharacterAnimator = OverworldCharacter.GetComponent<Animator>();
    }

    private void AnimateOverworldCharacter()
    {
        if (OverworldCharacterAnimator == null)
            return;
        
        OverworldCharacterAnimator.SetBool("Walking",isWalking);
        var absH = Mathf.Abs(hInput);
        var absV = Mathf.Abs(vInput);
        
        if (absH == absV)
            return;

        int dir;

        if (vInput == 1)
            dir = 0;
        else if (hInput == 1)
            dir = 1;
        else if (vInput == -1)
            dir = 2;
        else
            dir = 3;
        
        OverworldCharacterAnimator.SetInteger("Direction", dir);
        Direction = (PlayerDirection)dir;
    }

    private void Update()
    {
        AnimateOverworldCharacter();
        
        switch (State)
        {
            case PlayerState.Active:
                if (Input.GetButtonDown("Submit"))
                    TryInteract();
                if (Input.GetButtonDown("Cancel"))
                    OpenMainMenu();
                break;
            case PlayerState.Busy:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (State)
        {
            case PlayerState.Active:
                hInput = (int)Input.GetAxisRaw("Horizontal");
                vInput = (int)Input.GetAxisRaw("Vertical");
                Movement();
                break;
            case PlayerState.Busy:
                hInput = 0;
                vInput = 0;
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        var position = transform.position;
        Gizmos.DrawLine(position,position + (Vector3)Front);
    }

    private void Movement()
    {
        transform.position += new Vector3(hInput*movementSpeed*Time.fixedDeltaTime, vInput*movementSpeed*Time.fixedDeltaTime);
    }

    private void TryInteract()
    {
        var mask = LayerMask.GetMask("Interactable");

        var ray = Physics2D.Raycast(transform.position, Front, 1f, mask);

        if (ray.collider != null)
        {
            var interactableObject = ray.collider.gameObject;
            var interactable = interactableObject.GetComponent<Interactable>();
            
            if (interactable == null || interactable.interactableType != Interactable.InteractableType.Action)
                return;
            
            interactable.Interact();
        }
    }

    public void GainEXP(int experience)
    {
        CurrentExp += experience;
        if (CurrentExp >= ExpToNextLevel)
        {
            LevelUp();
        }
    }

    public UnityEvent OnLevelUpEvent = new UnityEvent();
    private void LevelUp()
    {
        if (CurrentExp < ExpToNextLevel)
            return;
        
        CurrentExp -= ExpToNextLevel;
        PartyLevel++;
        OnLevelUpEvent.Invoke();
    }

    private void OpenMainMenu()
    {
        PauseGame();
        var mainMenuObject = Instantiate(MainMenuPrefab);
        var mainMenu = mainMenuObject.GetComponent<MainMenu>();
        
        mainMenu.OnMenuClose.AddListener(UnpauseGame);
    }

    public void PauseGame()
    {
        //Time.timeScale = 0;
        State = PlayerState.Busy;
    }
    
    public void UnpauseGame()
    {
        //Time.timeScale = 1;
        State = PlayerState.Active;
    }

    //Ver depois como fazer se o inventário estiver cheio (se tiver limite de inventário)
    
    public void AddItemToInventory(ItemBase itemBase)
    {
        if (itemBase is IStackableBase iStackable)
        {
            AddStackableToInventory(iStackable,1);
            return;
        }

        var itemInstance = ItemInstanceBuilder.BuildInstance(itemBase);
        Inventory.Add(itemInstance);
    }

    public void AddStackableToInventory(IStackableBase stackableBase, int quantity)
    {
        if (quantity == 0)
            return;

        if (stackableBase.MaxStack == 0)
        {
            Debug.LogError("Max Stack is 0");
        }

        var instancesOfStackable = Inventory.Where(item => item.Base == stackableBase.ItemBase);

        if (!instancesOfStackable.Any())
        {
            while (quantity > 0)
            {
                var instance = ItemInstanceBuilder.BuildInstance(stackableBase.ItemBase);
                var stackable = instance as IStackable;
                if (quantity > stackableBase.MaxStack)
                {
                    stackable.Quantity = stackableBase.MaxStack;
                    quantity -= stackableBase.MaxStack;
                }
                else
                {
                    stackable.Quantity = quantity;
                    quantity = 0;
                }
                Inventory.Add(instance);
            }
        }
        else
        {
            //Restack
            int totalQuantity = quantity;
            foreach (var instanceOfStackable in instancesOfStackable)
            {
                var stackable = instanceOfStackable as IStackable;
                totalQuantity += stackable.Quantity;
            }

            Inventory.RemoveAll(item => instancesOfStackable.Contains(item));
            
            Debug.Log("Total Quantity = " + totalQuantity);
            
            //Rebuild
            while (totalQuantity > 0)
            {
                var instance = ItemInstanceBuilder.BuildInstance(stackableBase.ItemBase);
                var stackable = instance as IStackable;
                if (totalQuantity > stackableBase.MaxStack)
                {
                    stackable.Quantity = stackableBase.MaxStack;
                    totalQuantity -= stackableBase.MaxStack;
                }
                else
                {
                    stackable.Quantity = totalQuantity;
                    totalQuantity = 0;
                }
                Inventory.Add(instance);
            }
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (State == PlayerState.Active && other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            var interactable = other.gameObject.GetComponent<Interactable>();
            
            if (interactable == null || interactable.interactableType != Interactable.InteractableType.Collision)
                return;

            interactable.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (State == PlayerState.Active && other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            var interactable = other.gameObject.GetComponent<Interactable>();
            
            if (interactable == null || interactable.interactableType != Interactable.InteractableType.Collision)
                return;
            
            interactable.Interact();
        }
    }

    private int hInput { get; set; }
    private int vInput { get; set; }
    public bool CanWalk = true;
    private bool isWalking => CanWalk && (hInput != 0 || vInput != 0);

    public enum PlayerDirection
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    public enum PlayerState
    {
        Active = 0,
        Busy = 1
    }
}