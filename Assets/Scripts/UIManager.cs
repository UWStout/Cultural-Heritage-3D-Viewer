using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager: MonoBehaviour
{
    public GameObject AnnotationParent;
    public Transform[] annotations;
    public Text textBox;
    public Text titleBox;
    public GameObject helpOverlay;
    // Start is called before the first frame update
    void Start()
    {
        if (!AnnotationParent)
        {
            AnnotationParent = GameObject.FindGameObjectWithTag("AnnotationParent");
            if (AnnotationParent)
            {
                annotations = AnnotationParent.GetComponentsInChildren<Transform>();
            }
        }
        if (!textBox)
        {
            textBox = GameObject.Find("AnnotationTextbox").GetComponent<Text>();
        }
        if (!titleBox)
        {
            titleBox = GameObject.Find("AnnotationTitlebox").GetComponent<Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!AnnotationParent)
        {
            AnnotationParent = GameObject.FindGameObjectWithTag("AnnotationParent");
            if (AnnotationParent)
            {
                annotations = AnnotationParent.GetComponentsInChildren<Transform>();
            }
        }
        if (!textBox)
        {
            textBox = GameObject.Find("AnnotationTextbox").GetComponent<Text>();
        }
        if (!titleBox)
        {
            titleBox = GameObject.Find("AnnotationTitlebox").GetComponent<Text>();
        }
    }

    public void ToggleAnnotations()
    {
        // Ensure text boxes are cleared
        titleBox.text = "";
        textBox.text = "";
        if (AnnotationParent)
        {
            foreach (Transform annotation in annotations)
            {
                if (annotation.gameObject.activeSelf == true)
                {
                    annotation.gameObject.SetActive(false);
                }
                else
                {
                    annotation.gameObject.SetActive(true);
                }
            }
        }

    }

    //public void toggleHelpOverlay()
    //{
        //if (helpOverlay.activeSelf)
        //{
            //helpOverlay.SetActive(false);
        //}
        //else
        //{
            //helpOverlay.SetActive(true);
        //}
    //}
}
