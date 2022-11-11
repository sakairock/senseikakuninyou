using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMove : MonoBehaviour
{
    //プレイヤー格納してターゲット
    public GameObject targetObject;
    //空のゲームオブジェクトを使って長距離攻撃の座標指定
    public GameObject longAT_Point_R;
    public GameObject longAT_Point_L;

    //エネミーの速度設定
    public float speed;
    //エネミーのジャンプ速度設定
    public float jumpspeed;
    //エネミーの攻撃射程距離
    public float ShortAttackRange;
    public float MiddleAttackRange;

    //各オブジェクトのx,y座標格納変数
    private Vector2 posEN;
    private Vector2 posPL;
    private Vector2 posLAT_R;
    private Vector2 posLAT_L;
    private float PLx;
    private float ENx;
    private float LAT_Rx;
    private float LAT_Lx;
    //オブジェクト距離間隔変数
    private float dis_x;
    private float LATdis_Rx;
    private float LATdis_Lx;
    //クールタイムカウント変数
    private float coolTime = 0.0f;
    //クールタイムライン
    private float cool_LimTime = 2.0f;
    //攻撃チャージタイムカウント変数
    private float chargeTime = 0.0f;
    //攻撃チャージタイムライン
    private float charge_LimTime = 2.0f;
    //攻撃判定フラグ
    private bool ATflg = false;
    //接地判定フラグ
    private bool isGround = false;
    //Rigidbody2D取得
    private Rigidbody2D rb2d = null;
    //他スクリプト変数を取得する為の変数
    public GroundCheck ground;

    //ゲーム開始時の処理
    void Start()
    {
        //敵キャラのRigidbodyコンポーネントを取得
        rb2d = GetComponent<Rigidbody2D>();
    }

    //ゲーム中の処理
    void Update()
    {
        //プレイヤー座標取得
        Vector2 posPL = targetObject.transform.position;
        //エネミー座標取得
        Vector2 posEN = this.transform.position;
        //2つの長距離攻撃用ポイント座標取得
        Vector2 posLAT_R = longAT_Point_R.transform.position;
        Vector2 posLAT_L = longAT_Point_L.transform.position;
        //それぞれ個別のx座標取得
        PLx = posPL.x;
        ENx = posEN.x;
        LAT_Rx = posLAT_R.x;
        LAT_Lx = posLAT_L.x;
        //プレイヤーとのx座標の距離取得
        dis_x = ENx - PLx;
        //長距離攻撃用指定座標とのx座標距離取得
        LATdis_Rx = ENx - LAT_Rx;
        LATdis_Lx = ENx - LAT_Lx;
        //接地判定を得る
        isGround = ground.IsGround();
        if (posEN.x > posPL.x)
        {
            this.transform.eulerAngles = new Vector3(0,0,0);
        }
        else if (posEN.x < posPL.x)
        {
            this.transform.eulerAngles = new Vector3(0,180,0);
        }
    }

    /**/
    public void Wait() //強制終了関数
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
        #endif
    }
    
    public void Jump() //ジャンプ関数
    {
        //プレイヤーを回り込む様ににジャンプ(ジャンプは一定)
        if (isGround)
        {
            rb2d.AddForce(Vector2.up * jumpspeed);
        }
    }

    public void ShortAttack() //近距離攻撃関数
    {
        //攻撃フラグがオフで射程距離に入ってない場合
        if (dis_x > ShortAttackRange && ATflg == false)
        {
            //射程距離までプレイヤーとの距離を詰める
            while (dis_x > ShortAttackRange)
            {
                posEN = Vector2.MoveTowards(posEN, posPL, speed * Time.deltaTime); //移動処理
            }
            if (isGround)
            {
                //攻撃判定振る(未実装)
                Debug.Log("近距離攻撃");
            }
            ATflg = true; //攻撃フラグオン
        }
        //攻撃フラグオフで既に射程距離なら
        else if (dis_x <= 200 && ATflg == false)
        {
            if (isGround)
            {
                //攻撃判定振る(未実装)
                Debug.Log("近距離攻撃");
            }
            ATflg = true; //攻撃フラグオン
        }
    }
    
    public void MiddleAttack() //中距離攻撃関数
    {
        //チャージタイム
        chargeTime += Time.deltaTime;
        //攻撃フラグがオフ&チャージ済みで射程距離に入ってない場合
        if (dis_x > MiddleAttackRange && ATflg == false && chargeTime >= charge_LimTime)
        {
            //射程距離までプレイヤーとの距離を詰める
            while (dis_x > MiddleAttackRange)
            {
                posEN = Vector2.MoveTowards(posEN, posPL, speed * Time.deltaTime); //移動処理
            }
            if (isGround)
            {
                //攻撃判定振る(未実装)
                Debug.Log("中距離攻撃");
            }
            chargeTime = 0.0f; //チャージタイムリセット
            ATflg = true; //攻撃フラグオン
        }
        //攻撃フラグオフ&チャージ済みで既に射程距離なら
        else if (dis_x <= MiddleAttackRange && ATflg == false && chargeTime >= charge_LimTime)
        {
            if (isGround)
            {
                //攻撃判定振る(未実装)
                Debug.Log("中距離攻撃");
            }
            chargeTime = 0.0f; //チャージタイムリセット
            ATflg = true; //攻撃フラグオン
        }
    }
    
    public void LongAttack() //遠距離攻撃関数
    {
        //攻撃フラグがオフで指定ポイントでは無い場合
        if (ENx != LAT_Rx && ENx != LAT_Lx && ATflg == false)
        {
            //左の指定ポイントより右の指定ポイントが近い又は指定ポイント距離が同じ場合
            if (LATdis_Rx <= LATdis_Lx)
            {
                while (ENx != LAT_Rx)
                {
                    //指定ポイントまで移動
                    posEN = Vector2.MoveTowards(posEN, posLAT_R, speed * Time.deltaTime); //移動処理
                }
            }
            //右の指定ポイントより左の指定ポイントが近い場合
            else if (LATdis_Rx > LATdis_Lx)
            {
                while (ENx != LAT_Lx)
                {
                    //指定ポイントまで移動
                    posEN = Vector2.MoveTowards(posEN, posLAT_L, speed * Time.deltaTime); //移動処理
                }
            }

            if (isGround)
            {
                //攻撃判定振る(未実装)
                Debug.Log("長距離攻撃");
            }
            ATflg = true; //攻撃フラグオン
        }
        //攻撃フラグオフで既に指定ポイントなら
        else if ((ENx == LAT_Rx || ENx == LAT_Lx) && ATflg == false)
        {
            if (isGround)
            {
                //攻撃判定振る(未実装)
            }
            ATflg = true; //攻撃フラグオン
        }
    }

    public void Idle() //待機関数
    {
        //攻撃フラグがオンの最中
        while (ATflg)
        {
            coolTime += Time.deltaTime; //クールタイムカウントアップ

            //クールタイム経過なら
            if (coolTime >= cool_LimTime)
            {
                coolTime = 0.0f; //クールタイムリセット
                ATflg = false; //攻撃フラグオフ
                Debug.Log("待機解除");
            }
        }
    }

}