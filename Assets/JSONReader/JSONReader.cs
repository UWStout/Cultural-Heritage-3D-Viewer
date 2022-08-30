using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public TextAsset textJSON;

    [System.Serializable]
    public class Lights
    {
        public float[] color;
        public float intensity;
    }

    [System.Serializable]
    public class LightList
    {
        public Lights[] lights;

        public float getIntensity(int lightNum)
        {
            return lights[lightNum].intensity;
        }


        public float getColor(int lightNum, int colorNum)
        {
            return lights[lightNum].color[colorNum];
        }

      /*  public bool changeColors()
        {
            bool colorsChanged = false;

            for 
        }*/
    }

    public LightList myLightList = new LightList();
    // Start is called before the first frame update
    void Start()
    {
        myLightList = JsonUtility.FromJson<LightList>(textJSON.text);

        GameObject.FindWithTag("LightSource1").GetComponent<Light>().intensity = myLightList.getIntensity(0);
        GameObject.FindWithTag("LightSource2").GetComponent<Light>().intensity = myLightList.getIntensity(1);
        GameObject.FindWithTag("LightSource3").GetComponent<Light>().intensity = myLightList.getIntensity(2);

        GameObject.FindWithTag("LightSource1").GetComponent<Light>().color = new Color(myLightList.getColor(0, 0), myLightList.getColor(0, 1), myLightList.getColor(0, 2));
        GameObject.FindWithTag("LightSource2").GetComponent<Light>().color = new Color(myLightList.getColor(1, 0), myLightList.getColor(1, 1), myLightList.getColor(1, 2));
        GameObject.FindWithTag("LightSource2").GetComponent<Light>().color = new Color(myLightList.getColor(2, 0), myLightList.getColor(2, 1), myLightList.getColor(2, 2));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
