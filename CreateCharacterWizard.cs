using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateCharacterWizard : ScriptableWizard
{
    public Texture2D portraitTexture;
    public string nickname = "Default Nickname";
    public Color color = Color.white;

    [MenuItem ("My Tools/Create Character Wizard...")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<CreateCharacterWizard>("Create Character", "Create New", "Update Selected");
    }

    void OnWizardCreate()
    {
        GameObject characterGO = new GameObject();

        Character characterComponent = characterGO.AddComponent<Character>();


        characterComponent.portrait = portraitTexture;
        characterComponent.nickname = nickname;
        characterComponent.color = color;

        PlayerMovement playerMovement = characterGO.AddComponent<PlayerMovement>();
        characterComponent.playerMovement = playerMovement;
    }

    private void OnWizardOtherButton()
    {
        if(Selection.activeTransform != null)
        {
            Character characterComponent = Selection.activeTransform.GetComponent<Character>();

            if(characterComponent != null)
            {
                characterComponent.portrait = portraitTexture;
                characterComponent.nickname = nickname;
                characterComponent.color = color;
            }
        }
    }

    private void OnWizardUpdate()
    {
        helpString = "Enter character details";
    }
}
