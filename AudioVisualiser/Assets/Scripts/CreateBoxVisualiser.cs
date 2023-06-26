using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBoxVisualiser : MonoBehaviour
{
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private float spacing;
    [SerializeField] private float scaleIntensity;
    private GameObject[] boxes;



    private void Start()
    {
        AudioScript.OnAudioUpdate += InstantiateBoxes;
        AudioScript.OnAudioUpdate += ScaleBoxes;
    }

    private void OnDestroy()
    {
        AudioScript.OnAudioUpdate -= InstantiateBoxes;
        AudioScript.OnAudioUpdate -= ScaleBoxes;
    }

    private void InstantiateBoxes(float[] pBandValues)
    {
        if (boxes != null) return;

        boxes = new GameObject[pBandValues.Length];

        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i] = Instantiate(boxPrefab, new Vector3(spacing * i, 0, 0), Quaternion.identity);
        }

        AudioScript.OnAudioUpdate -= InstantiateBoxes;
    }

    private void ScaleBoxes(float[] pBandValues)
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            if (pBandValues[i] > 0) boxes[i].transform.localScale = new Vector3(1, scaleIntensity * pBandValues[i], 1);
            else boxes[i].transform.localScale = new Vector3(1, 0, 1);
        }
    }

}
