using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SelectAllOfTag : ScriptableWizard
{
    public string searchTag = "Your tag here";

    [MenuItem ("My Tools/Select All Of Tag...")]
    static void SelectAllOfTagWizard()
    {
        ScriptableWizard.DisplayWizard<SelectAllOfTag>("Select All Of Tag...", "Make Selection");
    }

    private void OnWizardCreate()
    {
        GameObject[] gameObjects = null;

        searchTag = searchTag.ToLower();
        if (GameObject.FindWithTag(searchTag) != null)
            gameObjects = GameObject.FindGameObjectsWithTag(searchTag);

        searchTag = (char)(searchTag[0] - ' ') + searchTag.Substring(1);
        if (GameObject.FindWithTag(searchTag) != null)
        {
            GameObject[] gameObjects2 = GameObject.FindGameObjectsWithTag(searchTag);
            GameObject[] temp = new GameObject[gameObjects.Length + gameObjects2.Length];

            for (int i = 0; i < gameObjects.Length + gameObjects2.Length; i++)
            {
                if(i < gameObjects.Length)
                    temp[i] = gameObjects[i];
                else
                    temp[i] = gameObjects2[i - gameObjects.Length];
            }

            gameObjects = temp;
        }


        Selection.objects = gameObjects;
    }
}
