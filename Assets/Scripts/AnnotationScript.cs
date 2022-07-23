using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationScript : MonoBehaviour
{
    [SerializeField]
    string annotationTitle = "This is a title";
    [SerializeField]
    string annotationText = "This is an annotation";
    public Text textBox;
    public Text titleBox;
    GameObject popupmenu;
    GameObject holder;
    // Start is called before the first frame update
    void Start()
    {
        holder = GameObject.Find("Annotations Popup Holder");
        popupmenu = holder.transform.GetChild(0).gameObject;
        textBox = popupmenu.transform.GetChild(1).GetComponent<Text>();
        titleBox = popupmenu.transform.GetChild(0).GetComponent<Text>();
        //popupmenu.SetActive(false);
    }


    private void OnMouseDown()
    {
        // Reset text if already active
        if(!popupmenu.activeSelf)
        {
            popupmenu.SetActive(true);
            titleBox.text = annotationTitle;
            textBox.text = annotationText;
        }
        else
        {
            popupmenu.SetActive(false);
            titleBox.text = "";
            textBox.text = "";
        }
    }
}
