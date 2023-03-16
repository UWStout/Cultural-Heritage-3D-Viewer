using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyStart : MonoBehaviour
{
    [SerializeField]
    DropDown dropDown;

    public void StartSurvey()
    {
        this.gameObject.SetActive(false); // Disable panel
        dropDown.NextArtifact(); // Advance to first artifact
    }
}
