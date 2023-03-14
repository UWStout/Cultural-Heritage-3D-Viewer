using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class NextButtonScript : MonoBehaviour
{
    [SerializeField]
    DropDown drop;

    [SerializeField]
    private GameObject NextArtifactButton;

    [SerializeField]
    private GameObject SurveyPanel;

    [SerializeField]
    int idnum = 0;

    static int changenum = 1;
    static float lastnum = 0f;

    static float artifactChange = 0f;
    
    // Making this static for now just to keep things simple.
    static string filename = "testfile.txt";
    public static string Filename { get { return filename; } }
    
    StreamWriter writer;

    static bool iswritten = false;
    static bool finalwritten = false;

    public void Start()
    {
        // Generate filename from date / time.
        // Replace / with - and : with . so that it's a valid filename.
        filename = DateTime.Now.ToString().Replace('/', '-').Replace(':', '.').Replace(' ', '_') + ".txt";

        writer = new StreamWriter(filename, false); // Don't append for the first time the file is opened.
        if (!iswritten)
        {
            //writer.WriteLine("Participant number, " + idnum);
            iswritten = true;
        }
        writer.Close();
    }

    public void OnClick()
    {
        artifactChange = Time.unscaledTime;
        writer = new StreamWriter(filename, true);
        writer.WriteLine("Test number: " + changenum);
        writer.WriteLine("Test name: " + drop.CurrentObjectName);
        writer.WriteLine("Time spent: " + (artifactChange - lastnum) + " seconds");
        writer.Close();
        changenum++;
        lastnum = artifactChange;
        SurveyPanel.SetActive(true);
        NextArtifactButton.SetActive(false);
        
       // drop.NextArtifact();
        
    }

    public void OnDestroy()
    {
        writer = new StreamWriter(filename, true);

        if (!finalwritten)
        {
            writer.WriteLine("Experiment finished.");
            finalwritten = true;
        }
        writer.Close();
    }
}