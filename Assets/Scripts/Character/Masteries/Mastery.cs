using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Mastery : SerializedMonoBehaviour, ISelectHandler
{
    [SerializeField] public MasteryGrid Owner;

    #region Button Info

    [ShowInInspector] public MasteryStatus Status { get; private set; } = MasteryStatus.Locked;

    public void SetStatus(MasteryStatus status, bool echoValidate = true)
    {
        Status = status;

        switch (Status)
        {
            case MasteryStatus.Locked:
            case MasteryStatus.Unlocked:
            {
                foreach (var connector in GetConnectionsFrom())
                {
                    connector.SetLocked();
                    if (echoValidate) connector.To.ValidateStatus();
                }

                break;
            }
            case MasteryStatus.Learned:
            {
                foreach (var connector in GetConnectionsFrom())
                {
                    connector.SetUnlocked();
                    if (echoValidate) connector.To.ValidateStatus();
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ValidateStatus()
    {
        switch (Status)
        {
            case MasteryStatus.Locked:
            {
                if (GetConnectionsTo().Select(c => c.From).All(m => m.Status == MasteryStatus.Learned) &&
                    PlayerController.Instance.PartyLevel >= RequiredLevel)
                    SetStatus(MasteryStatus.Unlocked);
                break;
            }
            case MasteryStatus.Unlocked:
            case MasteryStatus.Learned:
            {
                if (GetConnectionsTo().Select(c => c.From).Any(m => m.Status != MasteryStatus.Learned) ||
                    PlayerController.Instance.PartyLevel < RequiredLevel)
                    SetStatus(MasteryStatus.Locked);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public Image ButtonFilter;
    public Image ButtonImage;
    public Button Button;

    public void SetSprite(Sprite sprite) => ButtonImage.sprite = sprite;
    public void SetFilter(Color color) => ButtonFilter.color = color;

    public MasteryConnector[] GetConnectionsFrom()
        => Owner.Connections.Where(c => c.From == this)
            .ToArray();

    public MasteryConnector[] GetConnectionsTo()
        => Owner.Connections.Where(c => c.To == this)
            .ToArray();

    public void OnClick() => Owner.MasteryButtonClicked(this);

#if UNITY_EDITOR

    [FoldoutGroup("Grid Movement"), Button("Up")]
    private void moveup() => transform.localPosition += new Vector3(0, Owner.GridStep);

    [FoldoutGroup("Grid Movement"), Button("Down")]
    private void movedown() => transform.localPosition += new Vector3(0, -Owner.GridStep);

    [FoldoutGroup("Grid Movement"), Button("Right")]
    private void moveright() => transform.localPosition += new Vector3(Owner.GridStep, 0);

    [FoldoutGroup("Grid Movement"), Button("Left")]
    private void moveleft() => transform.localPosition += new Vector3(-Owner.GridStep, 0);

    [FoldoutGroup("Grid Movement"), Button("Snap")]
    private void snap()
    {
        var newX = Owner.GridStep * (int) (transform.localPosition.x / Owner.GridStep);
        var newY = Owner.GridStep * (int) (transform.localPosition.y / Owner.GridStep);
        transform.localPosition = new Vector3(newX, newY);
    }

#endif

    #endregion

    #region Mastery Info

    public int Id;

    [OnValueChanged("updateName")] public string MasteryName;

    //public Sprite MasterySprite;
    [TextArea(minLines: 4, maxLines: 10)] public string MasteryDescription;
    public List<MasteryEffect> MasteryEffects = new List<MasteryEffect>();
    public List<Mastery> MasteryPrerequisites = new List<Mastery>();
    public int RequiredLevel = 0;
    public bool AutoLearned = false;

    public bool PrerequisitesAchieved(Character context)
        => MasteryPrerequisites.All(m => throw new NotImplementedException());

    #endregion

    public enum MasteryStatus
    {
        Locked,
        Unlocked,
        Learned
    }

    public MasteryInstance Save()
    {
        return new MasteryInstance
        {
            Id = Id,
            Status = Status
        };
    }

#if UNITY_EDITOR

    private void updateName()
    {
        gameObject.name = MasteryName;
    }

#endif
    public void Clicked() => Owner.MasteryButtonClicked(this);

    public void OnSelect(BaseEventData eventData) => Clicked();
}