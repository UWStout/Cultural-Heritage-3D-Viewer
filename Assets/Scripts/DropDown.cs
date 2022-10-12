using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDown : MonoBehaviour
{
    public GameObject allObjects;
    public GameObject[] objects;
    public Text textBox;
    public Text titleBox;
    GameObject currentObject;
    GameObject popupmenu;
    GameObject holder;

    public string CurrentObjectName
    {
        get
        {
            return currentObject.name;
        }
    }

    private int sequenceIndex;
    private int[] sequence;

    public int CurrentObjectIndex
    {
        get
        {
            return sequence[sequenceIndex];
        }
    }

    private void Start()
    {
        // Randomization
        sequence = new int[objects.Length];
        for (int i = 0; i < sequence.Length; i++)
        {
            sequence[i] = i;
        }

        for (int i = 0; i < sequence.Length; i++)
        {
            // Pick a random object not yet selected.
            int randomIndex = Random.Range(i, sequence.Length - 1);

            // Swap
            int tmp = sequence[i];
            sequence[i] = sequence[randomIndex];
            sequence[randomIndex] = tmp;
        }

        currentObject = Instantiate(objects[CurrentObjectIndex], objects[CurrentObjectIndex].transform.position, objects[CurrentObjectIndex].transform.rotation);
        DontDestroyOnLoad(currentObject);
        sequenceIndex = 0;
        holder = GameObject.Find("Annotations Popup Holder");
        popupmenu = holder.transform.GetChild(0).gameObject;
        if (!textBox)
        {
            textBox = popupmenu.transform.GetChild(1).GetComponent<Text>();
        }
        if (!titleBox)
        {
            titleBox = popupmenu.transform.GetChild(0).GetComponent<Text>();
        }
    }

    public void HandleInputData(int val)
    {
        // Clear the annotation box when changing objects
        if (!textBox)
        {
            textBox = popupmenu.transform.GetChild(1).GetComponent<Text>();
        }
        if (!titleBox)
        {
            titleBox = popupmenu.transform.GetChild(0).GetComponent<Text>();
        }
        textBox.text = "";
        titleBox.text = "";

        Destroy(currentObject);
        currentObject = Instantiate(objects[val], objects[val].transform.position, objects[val].transform.rotation);
        DontDestroyOnLoad(currentObject);
    }

    public void NextArtifact()
    {
        if (!textBox)
        {
            textBox = popupmenu.transform.GetChild(1).GetComponent<Text>();
        }
        if (!titleBox)
        {
            titleBox = popupmenu.transform.GetChild(0).GetComponent<Text>();
        }
        textBox.text = "";
        titleBox.text = "";

        if (sequenceIndex == objects.Length - 1) {
            sequenceIndex = 0;

            Destroy(currentObject);
            currentObject = Instantiate(objects[CurrentObjectIndex], objects[CurrentObjectIndex].transform.position, objects[CurrentObjectIndex].transform.rotation);
            DontDestroyOnLoad(currentObject);
        }
        else
        {
            sequenceIndex++;
            Destroy(currentObject);
            currentObject = Instantiate(objects[CurrentObjectIndex], objects[CurrentObjectIndex].transform.position, objects[CurrentObjectIndex].transform.rotation);
            DontDestroyOnLoad(currentObject);
        }
    }
}
