using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

public class TextEditWindow : EditorWindow
{
    string text = "Nothing Opened...";
    string hiddenText;
    ArrayList methods = new ArrayList();
    GenericMenu menu;
    Vector2 scroll;
    Object source;
    TextAsset txtAsset;

    [MenuItem("Window/TextEditWindow")]
    static void Init()
    {
        TextEditWindow window = (TextEditWindow)EditorWindow.GetWindow(typeof(TextEditWindow), false, "Text Editor Window");
        window.Show();
    }


    void OnGUI()
    {
        Object newSource = EditorGUILayout.ObjectField(source, typeof(TextAsset), true);
        if (source == null && newSource == null) return;
        else if (source == null) source = new Object();

        if (!newSource.Equals(source))
        {
            source = newSource;
            txtAsset = (TextAsset)source;

            methods.Clear();
            GetMethods();
            hiddenText = txtAsset.text;
            text = txtAsset.text;
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select Method to Edit", GUILayout.Width(200)))
        {
            menu = new GenericMenu();

            GUIContent firstItem = new GUIContent("Full Script");
            menu.AddItem(firstItem, false, OnMethodSelect, "Full Script");

            foreach (string str in methods)
            {
                GUIContent menuItem = new GUIContent(str);
                menu.AddItem(menuItem, false, OnMethodSelect, str);
            }

            menu.ShowAsContext();
        }

        if (GUILayout.Button("Save", GUILayout.Width(100)))
        {
            if (hiddenText.Contains("~~!!"))
                hiddenText = hiddenText.Replace("~~!!", text);
            else hiddenText = text;

            string[] temp = hiddenText.Split('\n');
            Debug.Log("Last Line: " + temp[temp.Length - 1]);
            using (StreamWriter outfile = new StreamWriter(AssetDatabase.GetAssetPath(txtAsset)))
            {
                foreach (string str in temp)
                    outfile.WriteLine(str);
            }

            methods.Clear();
            GetMethods();
            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();

        scroll = EditorGUILayout.BeginScrollView(scroll);
        text = EditorGUILayout.TextArea(text);
        EditorGUILayout.EndScrollView();
    }

    /*********************************** Helper Methods *******************************/
    /*
     * This method has a couple caveats...
     * 1. This is (as of writing this comment) only set-up to work with my particular coding style.
     * 2. This will break if it attempts to find overloaded methods.
     * 
     */
    void OnMethodSelect(object data)
    {
        int i = 0, bracketCount = 1;
        string methName = (string)data,
               method = "";

        if (methName == "Full Script")
        {
            text = txtAsset.text;
            return;
        }

        if (hiddenText.Contains("~~!!"))
            hiddenText = hiddenText.Replace("~~!!", text);
        else hiddenText = text;

        string[] strArr = txtAsset.text.Split('\n'),
                 saveStr = hiddenText.Split('\n');

        Debug.Log("strArr's Last Line: " + strArr[strArr.Length - 1]);
        Debug.Log("saveStr's Last Line: " + saveStr[saveStr.Length - 1]);

        while (!strArr[i].Contains("{") && i < strArr.Length - 1) i++;//Now we are into the class

        for(i++; i < strArr.Length - 2; i++)
        {
            while (!strArr[i].Contains(methName) && i < strArr.Length - 2) i++;  //skip to a line that has the method's name in it
            
            if (strArr[i].Contains("/*"))
            {
                while (!strArr[i].Contains("*/") && i < strArr.Length - 1) i++;
            }
            else if (strArr[i + 1].Contains("{") && !strArr[i].Contains("//"))      //check to make sure this call is the actual method
            {
                method = strArr[i];
                saveStr[i] = "~~!!";
                method += "\n" + strArr[++i];
                saveStr[i] = "!!!";
                i++;
                do
                {
                    if (strArr[i].Contains("{")) bracketCount++;
                    if (strArr[i].Contains("}")) bracketCount--;

                    saveStr[i] = "!!!";
                    method += "\n" + strArr[i++];
                } while (bracketCount > 0 && i < strArr.Length - 2);
            }
        }

        hiddenText = saveStr[0];
        for (i = 0; i < saveStr.Length - 1; i++)
        {
            if (i != saveStr.Length - 2 || saveStr[i] != "")
                if (!saveStr[i].Contains("!!!") && i != 0)
                    hiddenText += "\n" + saveStr[i];
        }

        text = method;
    }

    //Will not function properly if uneven numbers of opposite direction brackets are on the same line
    private void GetMethods()
    {
        int bracketCount = 1, i = 0;
        string[] strArr = txtAsset.text.Split('\n'), temp = {};
        
        while (!strArr[i].Contains("{")) i++;


        for (i++; i < strArr.Length; i++)
        {
            if (strArr[i].Contains("/*") || strArr[i].Contains("//"))
            {
                if(strArr[i].Contains("/*"))
                    while (!strArr[i++].Contains("*/") && i < strArr.Length) ;
            }
            else
            {
                if (strArr[i].Contains("{"))
                {
                    if (++bracketCount == 2)
                    {
                        if (strArr[i].Contains("("))
                            temp = strArr[i].Split('(');
                        else
                            temp = strArr[i - 1].Split('(');

                        temp = temp[0].Split(' ');
                        methods.Add(temp[temp.Length - 1]);
                    }
                }

                if (strArr[i].Contains("}")) bracketCount--;
            }
        }
    }
}