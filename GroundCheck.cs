using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private string groundTag = "StageTile";
    private bool isGround = false;
    private bool isGroundEnter, isGroundStay, isGroundExit;

    //接地判定処理関数
    public bool IsGround()
    {
        if(isGroundEnter || isGroundStay)
        {
            
            isGround = true;
        }
        else if(isGroundExit)
        {
            isGround = false;
        } 

        isGroundEnter = false;
        isGroundStay = false;
        isGroundExit = false;
        return isGround; 
    }
    //地面に接触したか判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            isGroundEnter = true;
        }
    }
    //地面に接触し続けているか判定
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            isGroundStay = true;
        }
    }
    //地面と離れているか判定  
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            isGroundExit = true;
        }
    }
}
