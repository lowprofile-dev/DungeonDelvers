using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class CharacterSelector : MonoBehaviour
{
    public CharacterEvent CharactedSelected;
    public UnityEvent SelectionCancelled;

    public List<CharacterPanel> CharacterPanels;

    public void StartSelection()
    {
        gameObject.SetActive(true);

        CharactedSelected = new CharacterEvent();
        SelectionCancelled = new UnityEvent();

        for (int i = 0; i < 4; i++)
        {
            var index = i;
            CharacterPanels[index].SetupCharacterPanel(PlayerController.Instance.Party[index]);
            CharacterPanels[index].SetOnClick(() => ConfirmSelection(PlayerController.Instance.Party[index]));
        }
    }

    public void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            CancelSelection();
        }
    }

    public void CancelSelection()
    {
        gameObject.SetActive(false);
        SelectionCancelled.Invoke();
    }

    public void ConfirmSelection(Character character)
    {
        //ver como faz se for tocar animação no selector
        gameObject.SetActive(false);
        CharactedSelected.Invoke(character);
    }
}



