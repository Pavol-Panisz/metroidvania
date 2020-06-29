#region README
/* Place this class on a gameObject and its child
 * gameObjects will move in a sin-wave fashion
 */
#endregion README

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavyTransforms : MonoBehaviour
{
    [Tooltip("How much can the transforms be pushed in one direction")]
    public float amplitude=1f;
    public float ampRandAdd=0f;

    [Tooltip("How often the transforms complete one wave")]
    public float frequency=1f;
    public float freqRandAdd=0f;

    [Tooltip("how much do we offset a transform from the previous one")]
    [Range(0f, 1f)] public float offset=0f;
    public float offsetRandAdd=0f;

    [Tooltip("how much do we offset all")]
    [Range(0f, Mathf.PI*2)] public float generalOffset=0f;

    private List<Transform> transforms = new List<Transform>();
    private float startY;
    private bool doWave=true;
    private float time; //has the same function as Time.time, but can be reset to zero

    //a class for holding "random addition values" for each
    //transform child.
    //these "addition values" get added to these children's
    //sin-wave parameters
    private class TransformRandoms {
        public float randAmp;
        public float randFre;
        public float randOff;

        public TransformRandoms(float a, float f, float o) {
            randAmp = a;
            randFre = f;
            randOff = o;
        }
    }
    private List<TransformRandoms> randoms = new List<TransformRandoms>();

    private void Start() {

        foreach (Transform child in transform) {

            //fill the transforms list with children transforms
            transforms.Add(child);

            //make random addition values for each child transform
            randoms.Add(new TransformRandoms(Random.Range(0f, ampRandAdd), 
                                            Random.Range(0f, freqRandAdd), 
                                            Random.Range(0f, offsetRandAdd)));
        }

        time = 0f;
    }

    private void Update() {
        if (!doWave) {return;}

        float f;
        float seqOff = 0f;
        float amp, freq, offs;

        for (int iii=0; iii < transforms.Count; iii++) {
            Transform ch = transforms[iii]; 

            //add the random addition values
            amp = amplitude + randoms[iii].randAmp;
            freq = frequency + randoms[iii].randFre;
            offs = 2*Mathf.PI * (seqOff + randoms[iii].randOff);


            f = amp * Mathf.Sin(time * 2*Mathf.PI * freq + offs + generalOffset);
            //Debug.Log(f);

            ch.localPosition = new Vector3(ch.localPosition.x, f, ch.localPosition.z);

            seqOff += offset;
        }

        time += Time.deltaTime;
    }

    public void StopWaving() {
        doWave = false;
        foreach (Transform tr in transforms) {
            tr.localPosition = new Vector3(tr.localPosition.x, 0f, tr.localPosition.z);
        }
    }
    public void StartWaving() {
        doWave = true;
        time = 0f; //by resetting time to zero, all transforms will start waving from their original positions
    }
}
