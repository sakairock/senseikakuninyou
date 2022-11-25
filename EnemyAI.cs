using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    //列挙型で分かりやすくステートを作る
    public enum EnemyAIState
    {
        WAIT, //完全停止
        JUMP, //飛翔
        SHORT_ATTACK, //近距離攻撃
        MIDDLE_ATTACK, //中距離攻撃
        LONG_ATTACK, //長距離攻撃
    }

    //変数初期化宣言
    public EnemyAIState aiState = EnemyAIState.WAIT;
    //他スクリプト変数持ってくる変数
    public EnemyMove enemymove;
    //ゲーム停止フラグ
    private bool waitflg = false;
    //AIが次に実行するステート
    private EnemyAIState nextState;
    //それぞれランダムに行動を決定する変数
    private int jumpnumber = 0;
    private int attacknumber = 0;
    //コルーチンの動作状態
    private bool isRunning = false;

    //ステート代入関数
    void SetAI()
    {
        if (enemymove.ATflg)
        {
            StartCoroutine("Idle");
        }
        //AIの状況判断ルーチン
        AIMainRoutine();
        //次の実行するステート代入
        aiState = nextState;
    }

    //AIの判断基準
    void AIMainRoutine()
    {
        //ジャンプ決定変数がランダムで0か1に決まる
        jumpnumber = Random.Range(0,10) % 2;
        //攻撃決定変数がランダムで0か1か2に決まる
        attacknumber = Random.Range(0,10) % 3;
        //waitflgがtrueなら実行
        if (waitflg)
        {
            //停止ステート代入
            nextState = EnemyAIState.WAIT;
            //フラグは戻す
            waitflg = false;
            return;
        }
        //ジャンプ決定変数が0で直前のステートがジャンプじゃ無いなら実行
        if (jumpnumber == 0 && nextState != EnemyAIState.JUMP)
        {
            //ジャンプステート代入
            nextState = EnemyAIState.JUMP;
        }
        //攻撃決定変数の値で攻撃を選択
        if (attacknumber == 0)
        {
            //近距離攻撃ステートを代入
            nextState = EnemyAIState.SHORT_ATTACK;
        }
        else if (attacknumber == 1)
        {
            //中距離攻撃ステートを代入
            nextState = EnemyAIState.MIDDLE_ATTACK;
        }
        else if (attacknumber == 2)
        {
            //遠距離攻撃ステートを代入
            nextState = EnemyAIState.LONG_ATTACK;
        }
        
        
    }

    //エネミーのAI
    void Update()
    {
        //AIのステートセット
        SetAI();
        
        //AIのステートの内容によって呼び出す動作の関数を決める
        switch (aiState)
        {
            //ステートがWAITなら停止関数実行
            case EnemyAIState.WAIT:
                enemymove.Wait();
                break;
            //ステートがJUMPならジャンプ関数実行
            case EnemyAIState.JUMP:
                enemymove.Jump();
                StartCoroutine("JumpIdle");
                break;
            //ステートがSHORT_ATTACKなら近距離攻撃関数実行
            case EnemyAIState.SHORT_ATTACK:
                enemymove.ShortAttack();
                break;
            //ステートがMIDDLE_ATTACKなら中距離攻撃関数実行
            case EnemyAIState.MIDDLE_ATTACK:
                StartCoroutine("AttackCharge");
                enemymove.MiddleAttack();
                break;
            //ステートがLONG_ATTACKなら長距離攻撃関数実行
            case EnemyAIState.LONG_ATTACK:
                enemymove.LongAttack();
                break;
        }
    }

    IEnumerator AttackCharge()
    {
        if (isRunning)
        yield break;
        isRunning = true;

        //２秒待機
        yield return new WaitForSeconds(2);

        isRunning = false;
    }

    IEnumerator Idle()
    {
        if (isRunning)
        yield break;
        isRunning = true;

        //２秒待機
        yield return new WaitForSeconds(2);
        //攻撃フラグオフにする
        enemymove.ATflg = true;

        isRunning = false;
    }

    IEnumerator JumpIdle()
    {
        if (isRunning)
        yield break;
        isRunning = true;

        yield return new WaitUntil(() => enemymove.isGround);

        isRunning = false;
    }

}

/*
ジャンプするかどうか←
↓　　　　　　　　　　↑
攻撃の種類          ↑
↓                 　↑
硬直→→→→→→→→→→→→→→→ ↑
*/
