using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EncryptStringSample;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SkredUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class PlayerController : AsyncMonoBehaviour
{
    #region SingletonImpl
    private static PlayerController _instance = null;
    public static PlayerController Instance => _instance;

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
    #endregion

    #region Params
    [HideInPlayMode] public List<CharacterBase> PartyBases = new List<CharacterBase>();
    [HideInPlayMode] public List<ItemBase> InventoryBase = new List<ItemBase>();

    public float MovementSpeed;
    public GameObject MainMenuPrefab;
    public GameObject MainCameraPrefab;

    #endregion

    #region Vars
    [HideInEditorMode, ReadOnly] public int PartyLevel = 1;
    [HideInEditorMode, ReadOnly] public int CurrentExp = 0;
    public int CurrentGold = 0;
    [ShowInInspector] public int ExpToNextLevel => (int)(5 + 10 * PartyLevel + 4 * Mathf.Pow(PartyLevel - 1, 2));
    [HideInEditorMode] public List<Character> Party = new List<Character>();
    [HideInEditorMode] public List<Item> Inventory = new List<Item>();
    [ReadOnly] public GameObject OverworldCharacter;
    private PlayerDirection _direction;
    [ShowInInspector] public PlayerDirection Direction
    {
        get { return _direction; }
        set
        {
            _direction = value;
            OverworldCharacterAnimator.SetInteger("Direction", (int)value);
        }
    }
    [ReadOnly] public PlayerState State;
    private Vector2 Front {
        get {
            switch (Direction)
            {
                case PlayerDirection.Up:
                    return new Vector2(0, 1);
                case PlayerDirection.Right:
                    return new Vector2(1, 0);
                case PlayerDirection.Down:
                    return new Vector2(0, -1);
                case PlayerDirection.Left:
                    return new Vector2(-1, 0);
                default:
                    return new Vector2(0, 0);
            }
        }
    }
    private Animator OverworldCharacterAnimator;
    private int hInput { get; set; }
    private int vInput { get; set; }
    public bool CanWalk = true;
    private bool isWalking => CanWalk && (hInput != 0 || vInput != 0);
    #endregion

    #region UnityEvents
    private void Start()
    {
        //#if UNITY_EDITOR
        Party = PartyBases.EachDo(characterBase => new Character(characterBase));
        Inventory = InventoryBase.EachDo(ItemInstanceBuilder.BuildInstance);
        //#endif
        GameController.Instance.OnBeginEncounter.AddListener(PauseGame);
        GameController.Instance.OnEndEncounter.AddListener(UnpauseGame);
        BuildOverworldCharacter();
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

        //test
        if (Input.GetKeyDown(KeyCode.LeftShift))
            MovementSpeed = 10;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            MovementSpeed = 5;
        if (Input.GetKeyDown(KeyCode.Z))
            GameSaveController.Save();
        if (Input.GetKeyDown(KeyCode.X))
            GameSaveController.Load();
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
        Gizmos.DrawLine(position, position + (Vector3)Front);
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
    #endregion

    #region ControlFunctions
    private void Movement()
    {
        transform.position += new Vector3(hInput * MovementSpeed * Time.fixedDeltaTime, vInput * MovementSpeed * Time.fixedDeltaTime);
    }

    private void BuildOverworldCharacter()
    {
        if (Party.Count == 0)
            throw new ArgumentNullException();

        if (OverworldCharacter != null)
            Destroy(OverworldCharacter);

        OverworldCharacter = Instantiate(Party.First().Base.CharacterPrefab, transform);
        OverworldCharacter.name = Party.First().Base.uniqueIdentifier;
        OverworldCharacterAnimator = OverworldCharacter.GetComponent<Animator>();
    }

    private void AnimateOverworldCharacter()
    {
        if (OverworldCharacterAnimator == null)
            return;

        OverworldCharacterAnimator.SetBool("Walking", isWalking);
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

    private void TryInteract()
    {
        Debug.Log("Trying to Interact");
        var mask = LayerMask.GetMask("Interactable");

        var ray = Physics2D.Raycast(transform.position, Front, 1f, mask);

        if (ray.collider != null)
        {
            var interactableObject = ray.collider.gameObject;
            Debug.Log($"Interacting with {interactableObject.name}");
            var interactable = interactableObject.GetComponent<Interactable>();

            if (interactable == null || interactable.interactableType != Interactable.InteractableType.Action)
                return;

            interactable.Interact();
        }
        else
        {
            Debug.Log("No interactables found");
        }
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
    #endregion

    #region StatFunctions

    public void GainEXP(int experience)
    {
        CurrentExp += experience;
        if (CurrentExp >= ExpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        CurrentExp -= ExpToNextLevel;
        CurrentExp = Mathf.Max(0, CurrentExp);
        PartyLevel++;

        Party.ForEach(partyMember => partyMember.LevelUp());

        OnLevelUpEvent.Invoke();
    }

    #endregion

    #region InventoryFunctions

    public void ValidateInventory()
    {
        var invalidStackables = Inventory.Where(item =>
        {
            if (item is IStackable stackable)
            {
                return stackable.Quantity <= 0;
            }

            return false;
        });

        foreach (var invalidStackable in invalidStackables)
        {
            Inventory.Remove(invalidStackable);
        }
    }
    
    public void AddItemToInventory(Item item)
    {
        if (item is IStackable stackable)
        {
            AddStackableToInventory(stackable);
            return;
        }
        
        Inventory.Add(item);
    }
    
    public void AddStackableToInventory(IStackable stackable)
    {
        Inventory.Add(stackable as Item);
        Restack(stackable.StackableBase);
    }
    
    public void AddItemBaseToInventory(ItemBase itemBase)
    {
        if (itemBase is IStackableBase iStackable)
        {
            AddStackableBaseToInventory(iStackable, 1);
            return;
        }

        var itemInstance = ItemInstanceBuilder.BuildInstance(itemBase);
        Inventory.Add(itemInstance);
    }

    public void RemoveItemFromInventory(Item item)
    {
        Inventory.Remove(item);
    }

    public void RemoveItemBaseFromInventory(ItemBase itemBase, int amount)
    {
        if (itemBase is IStackableBase stackableBase)
        {
            RemoveStackableBaseFromInventory(stackableBase, amount);
            return;
        }

        //Fazer loopar pelos itens enquanto tem item e amount
        var itemOfType = Inventory.FirstOrDefault(item => item.Base == itemBase);

        if (itemOfType != null)
        {
            Inventory.Remove(itemOfType);
        }
    }

    public void RemoveStackableFromInventory(IStackable stackable, int amount)
    {
        if (stackable.Quantity <= amount)
            RemoveItemFromInventory(stackable.Item);
        else
        {
            stackable.Quantity -= amount;
        }
    }

    public void RemoveStackableBaseFromInventory(IStackableBase stackableBase, int amount)
    {
        var allStacks = Inventory.FindAll(item => item.Base == (ItemBase)stackableBase);

        var totalCount = 0;

        allStacks.ForEach(item => totalCount += (item as IStackable).Quantity);

        Restack(stackableBase, totalCount - amount);
    }

    public void AddStackableBaseToInventory(IStackableBase stackableBase, int quantity)
    {
        if (quantity == 0)
            return;

        if (stackableBase.MaxStack == 0)
        {
            Debug.LogError("Max Stack is 0");
            return;
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

            Restack(stackableBase, totalQuantity);
        }
    }

    public void Restack(IStackableBase stackableBase)
    {
        var totalCount = Inventory.Where(item => item.Base == stackableBase.ItemBase).Cast<IStackable>()
            .Select(stackable => stackable.Quantity).Sum();
        
        Restack(stackableBase,totalCount);
    }
    
    public void Restack(IStackableBase stackableBase, int totalQuantity)
    {
        Inventory.RemoveAll(item => item.Base == (ItemBase)stackableBase);

        if (stackableBase.MaxStack == 0)
        {
            Debug.LogError("Max Stack is 0");
            return;
        }

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

    public void ReorderInventory()
    {
        Inventory.OrderBy(item => item.InspectorName.ToLower());
    }


    //Limpar o código de mexer em inventário
    //Fazer um modo de tirar um item especificamente de uma stack
    public void CleanupInventory()
    {
        var stackableBases = Inventory.Where(item => item is IStackable).Cast<IStackable>()
            .Select(stackable => stackable.StackableBase)
            .Distinct();

        foreach (var stackableBase in stackableBases)
        {
            var quantity = GetQuantityOfItem(stackableBase as ItemBase);
            Restack(stackableBase, quantity);
        }

        ReorderInventory();
    }

    public int GetQuantityOfItem(ItemBase itemBase)
    {
        if (itemBase is IStackableBase stackableBase)
        {
            return GetQuantityOfStackable(stackableBase);
        }

        return Inventory.Count(item => item.Base == itemBase);
    }

    private int GetQuantityOfStackable(IStackableBase stackableBase)
    {
        var stacks = Inventory.Where(item => item.Base == (ItemBase)stackableBase).Cast<IStackable>().ToArray();
        int totalCount = 0;
        stacks.ForEach(stack => { totalCount += stack.Quantity; });
        return totalCount;
    }

    #endregion

    #region Events

    public UnityEvent OnLevelUpEvent = new UnityEvent();

    #endregion

    #region Declarations

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

    #endregion
}