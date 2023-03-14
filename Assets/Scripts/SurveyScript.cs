using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SurveyScript : MonoBehaviour
{
    [SerializeField]
    private GameObject[] QuestionGroupArr;

    [SerializeField]
    private GameObject NextArtifactButton;


    private QAClass[] qaArr;


    private GameObject AnswerPanel;

    StreamWriter writer;

    
    string path = "testfile.txt";

    [SerializeField]
    private DropDown drop;

    [SerializeField]
    private GameObject SurveyPanel;

    // Start is called before the first frame update
    void Start()
    {
        SurveyPanel.SetActive(false);
        qaArr = new QAClass[QuestionGroupArr.Length];
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SubmitAnswer()
    {
        writer = new StreamWriter(path, true);


        for (int i = 0; i < qaArr.Length; i++)
        {
            qaArr[i] = ReadQuestionAndAnswer(QuestionGroupArr[i]);

            writer.WriteLine(qaArr[i].Question + " -> " + qaArr[i].Answer);
        }
        writer.Close();
        NextArtifactButton.SetActive(true);
        SurveyPanel.SetActive(false);

        drop.NextArtifact();
    }

    QAClass ReadQuestionAndAnswer(GameObject questionGroup)
    {
        QAClass result = new QAClass();

        GameObject q = questionGroup.transform.Find("Question").gameObject;
        GameObject a = questionGroup.transform.Find("Answer").gameObject;

        result.Question = q.GetComponent<TextMeshProUGUI>().text;

        if (a.GetComponent<ToggleGroup>() != null)
        {
            for (int i = 0; i < a.transform.childCount; i++)
            {
                if (a.transform.GetChild(i).GetComponent<Toggle>().isOn)
                {
                    result.Answer = a.transform.GetChild(i).Find("Label").GetComponent<Text>().text;
                    break;
                }
            }
        } 
        return result;
    }
}

[System.Serializable]
public class QAClass
{
    public string Question = "";
    public string Answer = "";

}
