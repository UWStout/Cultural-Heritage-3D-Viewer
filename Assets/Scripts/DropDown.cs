using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DropDown : MonoBehaviour
{
    public GameObject allObjects;
    public GameObject[] objects;
    public GameObject[] objectsOldShaders;
    public Text textBox;
    public Text titleBox;
    GameObject currentObject;
    GameObject popupmenu;
    GameObject holder;

    GameObject currentObjectTest;

    [SerializeField]
    private GameObject SurveyPanel;
    [SerializeField]
    private GameObject NextArtifactButton;

    public string CurrentObjectName
    {
        get
        {
            return currentObject.name;
        }
    }

    private int sequenceIndex;
    private GameObject[] randomizedObjects;

    private GameObject CurrentObject
    {
        get
        {
            return randomizedObjects[sequenceIndex];
        }
    }

    private void Start()
    { 
        // Will have a problem if objects arrays aren't set up correctly.
        Assert.AreEqual(objects.Length, objectsOldShaders.Length); 

        // Randomization
        int[] sequence = new int[objects.Length];
        for (int i = 0; i < sequence.Length; i++)
        {
            sequence[i] = i;
        }

        for (int i = 0; i < sequence.Length; i++)
        {
            // Pick a random object not yet selected.
            int randomIndex = Random.Range(i, sequence.Length);

            // Swap
            int tmp = sequence[i];
            sequence[i] = sequence[randomIndex];
            sequence[randomIndex] = tmp;
        }

        randomizedObjects = new GameObject[sequence.Length * 2];
        for (int i = 0; i < sequence.Length; i++)
        {
            // Randomize which version they see first.
            if (Random.Range(0, 2) == 0)
            {
                randomizedObjects[2 * i] = objects[sequence[i]];
                randomizedObjects[2 * i + 1] = objectsOldShaders[sequence[i]];
            }
            else
            {
                randomizedObjects[2 * i] = objectsOldShaders[sequence[i]];
                randomizedObjects[2 * i + 1] = objects[sequence[i]];
            }
        }

        currentObject = Instantiate(CurrentObject, CurrentObject.transform.position, CurrentObject.transform.rotation);
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

   /* IEnumerator HideSurveyPanel()
    {
        SurveyPanel.SetActive(true);
        yield return new WaitForSeconds(.1f);
    }*/

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

        if (sequenceIndex == randomizedObjects.Length - 1) 
        {
            //Application.Quit();

            SurveyPanel.SetActive(true);
            NextArtifactButton.SetActive(false);



            //sequenceIndex = 0;

            //Destroy(currentObject);
            //currentObject = Instantiate(CurrentObject, CurrentObject.transform.position, CurrentObject.transform.rotation);
            //DontDestroyOnLoad(currentObject);
        }
        else
        {
      
            sequenceIndex++;
            Destroy(currentObject);
            currentObject = Instantiate(CurrentObject, CurrentObject.transform.position, CurrentObject.transform.rotation);
            DontDestroyOnLoad(currentObject);

        }
    }
}
