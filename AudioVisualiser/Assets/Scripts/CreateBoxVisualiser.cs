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

    private enum BandValueDistribution {Averages, Maximums }
    [SerializeField] private BandValueDistribution bandValueDistribution = BandValueDistribution.Averages;

    [SerializeField] private bool UseBuffer;
    private float[] bands;
    private float[] bandBuffers;
    private float[] bufferDecreases;
    [SerializeField] private float bufferDecreaseBase = 0.0005f;
    [SerializeField] private float bufferDecreaseScale = 1.2f;


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

    private void onAudioUpdate(SpectrumInfo pSpectrumInfo)
    {
        bands = bandValues(pSpectrumInfo);
        setBandBuffer();

        if (UseBuffer) bands = bandBuffers;


        calculateStepsAndSize(bands.Length);
        if (boxes == null) instantiateBoxes(bands);

        positionBoxes(bands);
        scaleBoxes(bands);
    }

    private void setBandBuffer()
    {
        if (bandBuffers == null) bandBuffers = new float[bands.Length];
        if (bufferDecreases == null) bufferDecreases = new float[bands.Length];
        for (int i = 0; i < bands.Length; i++)
        {
            if (bands[i] > bandBuffers[i])
            {
                bandBuffers[i] = bands[i];
                bufferDecreases[i] = bufferDecreaseBase;
            }
            else
            {
                bandBuffers[i] -= bufferDecreases[i] * Time.deltaTime;
                bufferDecreases[i] *= 1 + (bufferDecreaseScale * Time.deltaTime);
            }
        }
    }


    private float[] bandValues(SpectrumInfo pSpectrumInfo)
    {
        switch (bandValueDistribution)
        {
            default:
                return pSpectrumInfo.GetBandAverages();
            case BandValueDistribution.Averages:
               return  pSpectrumInfo.GetBandAverages();
            case BandValueDistribution.Maximums:
               return pSpectrumInfo.GetBandMaximums();
        }
    }

    private void calculateStepsAndSize(int pBoxAmount)
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

        calculateStepsAndSize(pBandValues.Length);

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
            boxes[i].transform.localPosition = new Vector3(stepSize * i + startingY(), 0, 0);
        }
    }

    private void scaleBoxes(float[] pBandValues)
    {
        if (boxes == null) return;

        if (baseSize < 0) baseSize = 0;
        for (int i = 0; i < boxes.Length; i++)
        {
            if (pBandValues[i] > 0) boxes[i].transform.localScale = new Vector3(boxSize, baseSize +scaleIntensity * pBandValues[i], 1);
            else boxes[i].transform.localScale = new Vector3(boxSize, baseSize, 1);
        }
    }

    private float startingY()
    {
        return -length / 2 + boxSize / 2;
    }


}
