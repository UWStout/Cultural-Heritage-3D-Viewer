using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NextButtonScript : MonoBehaviour
{
    [SerializeField]
    DropDown drop;

    [SerializeField]
    int idnum = 0;

    static int changenum = 1;
    static float lastnum = 0f;

    static float artifactChange = 0f;
    
    [SerializeField]
    string filename = "testfile.txt";
    
    StreamWriter writer;

    static bool iswritten = false;
    static bool finalwritten = false;

    public void Start()
    {writer = new StreamWriter(filename, true);
        if (!iswritten)
        {
            writer.WriteLine("Participant number: " + idnum);
            iswritten = true;
        }
        writer.Close();
    }

    public void OnClick()
    {
        artifactChange = Time.unscaledTime;
        writer = new StreamWriter(filename, true);
        writer.WriteLine("Change number: " + changenum);
        writer.WriteLine("Artifact switched to: " + drop.CurrentObjectName);
        writer.WriteLine("Time spent on artifact: " + (artifactChange - lastnum) + " seconds");
        writer.Close();
        changenum++;
        lastnum = artifactChange;
       
        drop.NextArtifact();
    }

    public void OnDestroy()
    {
        writer = new StreamWriter(filename, true);

        if (!finalwritten)
        {
            writer.WriteLine("Experiment finshed.");
            finalwritten = true;
        }
        writer.Close();
    }
}