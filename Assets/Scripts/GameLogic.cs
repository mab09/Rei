using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public static int lchoice = 0;
    public static int rchoice = 0;
    // Start is called before the first frame update
    void Start()
    {
        /* TODO rock paper scissor logic:
        left.strikeL; --input of left player
        right.strikeR; --input of right player
        */
    }

    // Update is called once per frame
    void Update()
    {
        
        lchoice = LeftPlayer.strikeL;
        rchoice = RightPlayer.strikeR;
        if (lchoice == 0 && rchoice != 0) { Debug.Log("left loses"); }
        else if (rchoice == 0 && lchoice != 0) { Debug.Log("right loses"); }
        else if (rchoice == 0 && lchoice == 0) { Debug.Log("no outcome"); }
        else
        {
            if (rchoice != lchoice)
            {
                if (rchoice == 1)
                {
                    if (lchoice == 2)
                    {
                        Debug.Log("left wins");
                    }
                    else if (lchoice == 3)
                    {
                        Debug.Log("right wins");
                    }
                }
                else if (rchoice == 2)
                {
                    if (lchoice == 1)
                    {
                        Debug.Log("right wins");
                    }
                    else if (lchoice == 3)
                    {
                        Debug.Log("left wins");
                    }
                }
                else if (rchoice == 3)
                {
                    if (lchoice == 1)
                    {
                        Debug.Log("left wins");
                    }
                    else if (lchoice == 2)
                    {
                        Debug.Log("right wins");
                    }
                }
            }
            else { Debug.Log("draw"); }
        }
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    } 
}
