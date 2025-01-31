using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ArtifactManager : MonoBehaviour
{
    [SerializeField]
    int index;
    [SerializeField]
    DropDown drop;
    
    int idnum = 0;
    static int changenum = 1;
    static float lastnum = 0f;

    static float artifactChange = 0f;
    string filename = "testfile.txt";
    StreamWriter writer;

    static bool iswritten = false;
    static bool finalwritten = false;

    public void Awake()
    {
        writer = new StreamWriter(filename, true);
        if (!iswritten)
        {
            writer.WriteLine("Participant number: " + idnum);
            writer.WriteLine("First artifact is Guan Yu.");
            iswritten = true;
        }
        writer.Close();
    }

    public void OnClick()
    {
        artifactChange = Time.unscaledTime;
        writer = new StreamWriter(filename, true);
        writer.WriteLine("Time spent on artifact: " + (artifactChange - lastnum) + " seconds");
        writer.WriteLine("Change number: " + changenum);
        if (index == 0)
        {
            writer.WriteLine("Artifact switched to: Guan Yu");
        }
        else if (index == 1)
        {
            writer.WriteLine("Artifact switched to: Polyena");
        }
        else if (index == 2)
        {
            writer.WriteLine("Artifact switched to: Ding");
        }
        else if (index == 3)
        {
            writer.WriteLine("Artifact switched to: Autumn Jade");
        }
        else if (index == 4)
        {
            writer.WriteLine("Artifact switched to: Altarpiece");
        }
        else if (index == 5)
        {
            writer.WriteLine("Artifact switched to: Boreas");
        }
        else if (index == 6)
        {
            writer.WriteLine("Artifact switched to: Vase");
        }
        else
        {
            writer.WriteLine("Artifact switched to: Tiger Hat");
        }
        writer.Close();
        changenum++;
        lastnum = artifactChange;
        drop.HandleInputData(index);
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