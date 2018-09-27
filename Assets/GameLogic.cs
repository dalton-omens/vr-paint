using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {

    public GameObject leftHand;
    public GameObject rightHand;
    
    SteamVR_TrackedObject leftObj;
    SteamVR_TrackedObject rightObj;

    public bool disable = false;

    bool triggerActive = false;
    LineRenderer currentLine = null;
    List<Vector3> currentLinePositions = null;

    Stack<GameObject> lineObjects = new Stack<GameObject>();
    Stack<GameObject> oldObjects = new Stack<GameObject>();

    static float smallStroke = 0.01f;
    static float medStroke = 0.05f;
    static float largeStroke = 0.1f;
    float stroke = 0.1f;

    static Color goldColor = new Color(0.992f, 0.710f, 0.0824f);
    static Color blueColor = new Color(0.0f, 0.196f, 0.384f);
    Color brushColor = goldColor;

    public GameObject cursor;

    private SteamVR_Controller.Device LeftController
    {
        get { return SteamVR_Controller.Input((int)leftObj.index); }
    }

    private SteamVR_Controller.Device RightController
    {
        get { return SteamVR_Controller.Input((int)rightObj.index); }
    }

    // Use this for initialization
    void Start () {
        leftObj = leftHand.GetComponent<SteamVR_TrackedObject>();
        rightObj = rightHand.GetComponent<SteamVR_TrackedObject>();
        cursor.SetActive(true);
        setCursorColor(goldColor);
    }

    void setCursorColor(Color newColor)
    {
        if (cursor != null)
        {
            var mat = cursor.GetComponent<Renderer>().material;
            mat.color = newColor;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (!disable)
        {
            if (RightController.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis1).x > 0)
            {
                if (!triggerActive)
                {
                    oldObjects.Clear();
                    triggerActive = true;
                    rightHand.GetComponent<SteamVR_LaserPointer>().disable = true;
                    // Spawn something
                    // Add starting point
                    GameObject obj = new GameObject();
                    lineObjects.Push(obj);

                    currentLine = obj.AddComponent<LineRenderer>();
                    currentLine.positionCount = 0;
                    currentLine.startWidth = stroke;
                    currentLine.endWidth = stroke;
                    currentLine.receiveShadows = true;
                    currentLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    currentLine.material = new Material(Shader.Find("Vertex Colors"));
                    currentLine.startColor = brushColor;
                    currentLine.endColor = brushColor;
                }
                else
                {
                    // add a point
                    currentLine.positionCount += 1;
                    currentLine.SetPosition(currentLine.positionCount - 1, cursor.transform.position);
                }

            }
            else if (triggerActive)
            {
                triggerActive = false;
                rightHand.GetComponent<SteamVR_LaserPointer>().disable = false;
                currentLinePositions = null;
                currentLine.positionCount -= 1;
                currentLine.Simplify(0.0001f);
                currentLine = null;
                // End drawing
            }
        }

       

	}

    public void onClickClearScene()
    {
        foreach (GameObject obj in lineObjects)
        {
            Destroy(obj);
        }
        lineObjects.Clear();
        oldObjects.Clear();
    }

    public void onClickGoldColor()
    {
        brushColor = goldColor;
        setCursorColor(brushColor);
    }

    public void onClickBlueColor()
    {
        brushColor = blueColor;
        setCursorColor(brushColor);
    }

    public void onChangeStrokeSize()
    {

    }

    public void onClickSmallStroke()
    {
        stroke = smallStroke;
        if (cursor != null)
        {
            cursor.transform.localScale = new Vector3(smallStroke, smallStroke, smallStroke);
        }
    }

    public void onClickMedStroke()
    {
        stroke = medStroke;
        if (cursor != null)
        {
            cursor.transform.localScale = new Vector3(medStroke, medStroke, medStroke);
        }
    }

    public void onClickLargeStroke()
    {
        stroke = largeStroke;
        if (cursor != null)
        {
            cursor.transform.localScale = new Vector3(largeStroke, largeStroke, largeStroke);
        }
    }

    public void onClickUndo()
    {
        if (lineObjects.Count > 0)
        {
            oldObjects.Push(lineObjects.Pop());
            oldObjects.Peek().SetActive(false);
        }
    }

    public void onClickRedo()
    {
        if (oldObjects.Count > 0)
        {
            lineObjects.Push(oldObjects.Pop());
            lineObjects.Peek().SetActive(true);
        }
    }
}
