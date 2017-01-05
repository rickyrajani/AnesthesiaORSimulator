using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    public LeapWebProcessor processor;

    double startPositionX;
    double startPositionY;
    double startPositionZ;

    double startNormalX;
    double startNormalY;
    double startNormalZ;

    double currPositionX;
    double currPositionY;
    double currPositionZ;

    bool tracking;
    bool negativeNormalX;
    bool negativeNormalY;
    bool negativeNormalZ;
    bool negativePositionX;
    bool negativePositionY;
    bool negativePositionZ;
    bool gestureTrue;
    bool firstIter;

    // double offset;

    // Use this for initialization
    void Start()
    {
        tracking = false;
        negativeNormalX = false;
        negativeNormalY = false;
        negativeNormalZ = false;
        negativePositionX = false;
        negativePositionY = false;
        negativePositionZ = false;
        gestureTrue = true;
        firstIter = true;
        // offset = 0.5;
        GetComponent<TextMesh>().text = "";

        keywords.Add("Start tracking", () =>
        {
            // start data tracking here

            Debug.Log("tracking started");
            gestureTrue = true;
            List<object> pos = processor.palmPosition_list_global;
            startPositionX = (double)pos.ElementAt(0);
            startPositionY = (double)pos.ElementAt(1);
            startPositionZ = (double)pos.ElementAt(2);

            List<object> nor = processor.palmNormal_list_global;
            startNormalX = (double)nor.ElementAt(0);
            startNormalY = (double)nor.ElementAt(1);
            startNormalZ = (double)nor.ElementAt(2);

            tracking = true;
            if (startNormalX < 0) negativeNormalX = true;
            if (startNormalY < 0) negativeNormalY = true;
            if (startNormalZ < 0) negativeNormalZ = true;

            GetComponent<TextMesh>().text = "";
        });

        keywords.Add("End tracking", () =>
        {
            // end data tracking here
            tracking = false;
            string text = "";
            if (negativeNormalY && negativeNormalZ && negativePositionY) text = "Pushing left";
            else if (negativeNormalX && negativeNormalY && negativeNormalZ && negativePositionY) text = "Pushing down";
            else if (negativeNormalX && negativeNormalY && negativeNormalZ && negativePositionY && negativePositionZ) text = "Pushing away";
            else if (negativeNormalY && negativePositionY) text = "Pushing towards me";
            else if (negativeNormalX && negativeNormalY && negativeNormalZ && negativePositionY) text = "Pushing up";
            else
            {
                if (negativeNormalX) Debug.Log("negative normal x");
                if (negativeNormalY) Debug.Log("negative normal y");
                if (negativeNormalZ) Debug.Log("negative normal z");
                if (negativePositionX) Debug.Log("negative position x");
                if (negativePositionY) Debug.Log("negative position y");
                if (negativePositionZ) Debug.Log("negative position z");
            }
            Debug.Log(text);
            Debug.Log("tracking ended");
            GetComponent<TextMesh>().text = "Motion: " + text;
        });

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void Update()
    {
        if (tracking && gestureTrue) {
            Tracking();
        }
        GetComponent<TextMesh>().text = "";
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    private void Tracking()
    {
        List<object> pos = processor.palmPosition_list_global;
        double newPositionX = (double)pos.ElementAt(0);
        double newPositionY = (double)pos.ElementAt(1);
        double newPositionZ = (double)pos.ElementAt(2);

        List<object> nor = processor.palmNormal_list_global;
        double newNormalX = (double)nor.ElementAt(0);
        double newNormalY = (double)nor.ElementAt(1);
        double newNormalZ = (double)nor.ElementAt(2);

        if (negativeNormalX && newNormalX > 0) gestureTrue = false;
        if (negativeNormalY && newNormalY > 0) gestureTrue = false;
        if (negativeNormalZ && newNormalZ > 0) gestureTrue = false;

        if (firstIter)
        {
            if (newPositionX <= startPositionX) negativePositionX = true;
            if (newPositionY <= startPositionY) negativePositionY = true;
            if (newPositionZ <= startPositionZ) negativePositionZ = true;
            firstIter = false;
        }

        else if (gestureTrue)
        {
            if (newPositionX > currPositionX && negativePositionX) gestureTrue = false;
            if (newPositionY > currPositionY && negativePositionY) gestureTrue = false;
            if (newPositionZ > currPositionZ && negativePositionZ) gestureTrue = false;
        }

        currPositionX = newPositionX;
        currPositionY = newPositionY;
        currPositionZ = newPositionZ;
    }
}
