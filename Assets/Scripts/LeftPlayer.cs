using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class LeftPlayer : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------- */

    public static int strikeL = 0; //no action
    public static float pos = 0;
    public static bool llose = false, lcount = true, ai = false;
    public static string aitext = "AI: Off";
    public static Animator anim;

    public float speed = 50;
    public Button button1, button2, button3, aibutton;

    public static GameObject katanaOn, katanaOff;

    CharacterController controller;
    Vector3 movement, finalMovement;

    /* ------------------------------------------------------------------------------------------- */
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();

        katanaOn = GameObject.Find("Players/Shogun/Root/Ribs/Right_Shoulder_Joint_01/Right_Upper_Arm_Joint_01/Right_Forearm_Joint_01/Right_Wrist_Joint_01/KatanaOn");
        katanaOff = GameObject.Find("Players/Shogun/Root/Hip/Left_Thigh_Joint_01/KatanaOff");

        aibutton.onClick.AddListener(AIOnClick);
        button1.onClick.AddListener(() => ButtonClicked(1)); //rock - taka
        button2.onClick.AddListener(() => ButtonClicked(2)); //paper - tora
        button3.onClick.AddListener(() => ButtonClicked(3)); //scissor - hebi

        katanaOn.SetActive(false);
    }

    /* ------------------------------------------------------------------------------------------- */

    void AIOnClick()
    {
        ai = !ai;
        if (ai)
        {
            aitext = "AI: On";
            button1.gameObject.SetActive(false);
            button2.gameObject.SetActive(false);
            button3.gameObject.SetActive(false);
        }
        else
        {
            aitext = "AI: Off";
            button1.gameObject.SetActive(true);
            button2.gameObject.SetActive(true);
            button3.gameObject.SetActive(true);
        }
    }

    /* ------------------------------------------------------------------------------------------- */

    void ButtonClicked(int a)
    {
        if (GameLogic.ready == true) //countdown starts
        {
            if (GameLogic.begin == true) //third heartbeat
            {
                strikeL = a;
                lcount = false;
                anim.SetTrigger("TriggerRun");
                katanaOff.SetActive(false);
                katanaOn.SetActive(true);
            }
            else
            {
                //shogun is impatient
                strikeL = a;
                anim.SetTrigger("TriggerBad");
                katanaOff.SetActive(false);
                katanaOn.SetActive(true);
                llose = true;
            }
        }
            
    }

    /* ------------------------------------------------------------------------------------------- */

    void FixedUpdate()
    {
        pos = transform.position.x;
        movement = new Vector3(speed * Time.fixedDeltaTime, 0, 0);
        
        //AI on or Off text
        aibutton.GetComponentInChildren<Text>().text = aitext;

        //Don't show shogun buttons if AI on
        if (GameLogic.begin == false && ai == true)
        {
            button1.gameObject.SetActive(false);
            button2.gameObject.SetActive(false);
            button3.gameObject.SetActive(false);
        }

        //check player choice if player wasn't impatient

        if (strikeL != 0 && llose == false)
        {
            finalMovement = Vector3.Lerp(finalMovement, movement, Time.fixedDeltaTime);
            if (strikeL == 1)
            {
                button1.gameObject.SetActive(true);
                button2.gameObject.SetActive(false);
                button3.gameObject.SetActive(false);
            }
            else if (strikeL == 2)
            {
                button1.gameObject.SetActive(false);
                button2.gameObject.SetActive(true);
                button3.gameObject.SetActive(false);
            }
            else if (strikeL == 3)
            {
                button1.gameObject.SetActive(false);
                button2.gameObject.SetActive(false);
                button3.gameObject.SetActive(true);

            }
        }
        else
            finalMovement = new Vector3(0, 0, 0);

        //move player with the set speed

        controller.Move(finalMovement);

        //reset

        if (GameLogic.reset == true)
        {
            if (ai == false)
            {
                button1.gameObject.SetActive(true);
                button2.gameObject.SetActive(true);
                button3.gameObject.SetActive(true);
            }
            else
            {
                button1.gameObject.SetActive(false);
                button2.gameObject.SetActive(false);
                button3.gameObject.SetActive(false);
            }
        }
    }
}
