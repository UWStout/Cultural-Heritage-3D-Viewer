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

    private void Start()
    {
        currentObject = Instantiate(objects[0], objects[0].transform.position, objects[0].transform.rotation);
        DontDestroyOnLoad(currentObject);
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
}
