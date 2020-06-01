using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public static int lchoice = 0;
    public static int rchoice = 0;
    public static bool reset = false;
    public Button restart;
    public Text finalresult;
    // Start is called before the first frame update
    void Start()
    {
        restart.onClick.AddListener(TaskOnClick);
        /* TODO rock paper scissor logic:
        left.strikeL; --input of left player
        right.strikeR; --input of right player
        */
    }

    void TaskOnClick()
    {
        reset = true;
    }

    // Update is called once per frame
    void Update()
    {
        string result = "Begin";
        lchoice = LeftPlayer.strikeL;
        rchoice = RightPlayer.strikeR;

        if (lchoice == 0 && rchoice != 0) { result = "Left loses"; }
        else if (rchoice == 0 && lchoice != 0) { result = "Right loses"; }
        else if (rchoice == 0 && lchoice == 0) { result = "Begin"; }
        else
        {
            if (rchoice != lchoice)
            {
                if (rchoice == 1)
                {
                    if (lchoice == 2)
                    {
                        result = "Left wins";
                    }
                    else if (lchoice == 3)
                    {
                        result = "Right wins";
                    }
                }
                else if (rchoice == 2)
                {
                    if (lchoice == 1)
                    {
                        result = "Right wins";
                    }
                    else if (lchoice == 3)
                    {
                        result = "Left wins";
                    }
                }
                else if (rchoice == 3)
                {
                    if (lchoice == 1)
                    {
                        result = "Left wins";
                    }
                    else if (lchoice == 2)
                    {
                        result = "Right wins";
                    }
                }
            }
            else { result = "Draw"; }
        }
        if (reset == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            LeftPlayer.clickL = 9;
            RightPlayer.clickR = 9;
            result = "Begin";
            lchoice = 0; rchoice = 0;
            LeftPlayer.strikeL = 0;
            RightPlayer.strikeR = 0;
            reset = false;
        }
        //Debug.Log(result);
        finalresult.text = result;
    } 
}
