using Sirenix.OdinInspector;

public class CharacterInspector : SerializedMonoBehaviour
{
    public MainMenu MainMenu;

    public void OpenCharacterInspector(Character character)
    {
        
    }

    public void CloseCharacterInspector()
    {
        gameObject.SetActive(false);
        MainMenu.OpenMainMenu();
    }
}