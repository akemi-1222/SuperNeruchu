using UnityEngine;  //Unityの基本機能
using UnityEngine.InputSystem;  //InputSystemを使用するため

public class PlayerMove : MonoBehaviour
{
    PlayerInputActions inputActions;  //InputActionの管理
    //Generate C# classで生成されたPlayerInputActions.csを参照するための変数
    float moveInput;　  //x,yの2次元値 

    public float speed = 10.0f;  //移動速度の変数
    public float jumpPower = 15.0f;  //跳ぶ強さの変数
    public float dashPower = 20.0f;  //ダッシュの強さの変数

    int jumpCount = 0;  //ジャンプ回数のカウント
    public int maxJumpCount = 2;  //最大ジャンプ回数　現在は2段ジャンプ

    Rigidbody rb;





    private void Awake()  //Initと同意義でゲーム開始時に最初に呼ばれる
    {
        inputActions = new PlayerInputActions();  //InputActionsの実体を作成
        rb = GetComponent<Rigidbody>();
    }




    private void OnEnable()  //このオブジェクトが有効になった時によばれる
    {
        inputActions.Enable();  //InputSystemを有効にする（必須）

        inputActions.Player.Move.performed += OnMove;  
        //perfoemed ; 入力された瞬間
        //+= : イベントに関数を登録
        //OnMove ; 入力された瞬間に呼び出される関数（下記で定義）
        inputActions.Player.Move.canceled += OnMove;
        //canceled ; 入力がキャンセルされた瞬間（キーを離した瞬間など）
        //これがないとキーを話しても移動し続ける

        inputActions.Player.Jump.started += OnJump;

        inputActions.Player.Dash.started += OnDash;
    }




    private void OnDisable()
    {
        inputActions.Disable();  //オブジェクトが無効になった時にInputSystemを無効にする（必須）
    }




    void OnMove(InputAction.CallbackContext context)  //InputSystemから渡される入力情報
    {
        moveInput = context.ReadValue<float>();  //入力値をVector2型で読み取る
    }



    //ジャンプも移動もとりあえずで作っているから、状況に合わせてRbなのか、Transformなのか、速度管理なのかに変更させる
    void OnJump(InputAction.CallbackContext context)
    {
        //Debug.Log("Jump");
        if(jumpCount < maxJumpCount)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x,0,rb.linearVelocity.z);  //ジャンプ前にYの速度をリセット(落下速度リセット)

            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

            jumpCount++;
        }
    }




    void OnDash(InputAction.CallbackContext context)
    {
        //地上ダッシュ
        if (jumpCount == 0)
        {
            //A or D の方向へ加速
            Vector3 dashDirection = new Vector3(moveInput, 0, 0);

            //入力がないときにはダッシュしない
            if (dashDirection != Vector3.zero)
            {
                rb.AddForce(dashDirection.normalized * dashPower, ForceMode.Impulse);
            }
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))  //地面判定
        {
            jumpCount = 0;  //地面に触れたらカウントリセット
        }
    }


    //FixedUpdate() と Update()を使い分ける
    private void FixedUpdate()
    {
        //Debug.Log(moveInput);
        Vector3 move = new Vector3(moveInput, 0, 0);  //3D空間のためVector3に変換

        //transform.position += move * speed * Time.deltaTime;  //Transformで実際に移動させる

        rb.linearVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
    }
}
