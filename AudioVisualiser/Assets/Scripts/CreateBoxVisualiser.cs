using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBoxVisualiser : MonoBehaviour
{
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private float scaleIntensity;
    [SerializeField] private float length = 10;
    [SerializeField] private float baseSize = 0.1f;
    [SerializeField] private float spacing = 0.5f;

    private float stepSize;
    private float boxSize;

    private GameObject[] boxes;



    private void Start()
    {
        AudioScript.OnAudioUpdate += onAudioUpdate;
    }

    private void OnDestroy()
    {
        AudioScript.OnAudioUpdate -= onAudioUpdate;
    }

    private void onAudioUpdate(float[] pBandValues, float pTime)
    {
        CalculateStepsAndSize(pBandValues.Length);

        if (boxes == null) instantiateBoxes(pBandValues);

        positionBoxes(pBandValues);
        scaleBoxes(pBandValues);
        centerPosition();
    }

    private void CalculateStepsAndSize(int pBoxAmount)
    {
        spacing = Mathf.Clamp(spacing, 0, length / (pBoxAmount - 1));
        float TotalSpacing = spacing * (pBoxAmount - 1);
        boxSize = (length - TotalSpacing) / pBoxAmount;
        stepSize = boxSize + spacing;
    }

    private void instantiateBoxes(float[] pBandValues)
    {
        if (boxes != null) return;

        boxes = new GameObject[pBandValues.Length];

        CalculateStepsAndSize(pBandValues.Length);

        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i] = Instantiate(boxPrefab, transform);
        }

    }

    private void positionBoxes(float[] pBandValues)
    {
        if (boxes == null) return;

        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i].transform.localPosition = new Vector3(stepSize * i, 0, 0);
        }
    }

    private void scaleBoxes(float[] pBandValues)
    {
        if (boxes == null) return;

        for (int i = 0; i < boxes.Length; i++)
        {
            if (pBandValues[i] > 0) boxes[i].transform.localScale = new Vector3(boxSize, baseSize +scaleIntensity * pBandValues[i], 1);
            else boxes[i].transform.localScale = new Vector3(boxSize, baseSize, 1);
        }
    }

    private void centerPosition()
    {
        transform.position = new Vector3(-length / 2 + boxSize / 2, 0, 0);
    }
}
