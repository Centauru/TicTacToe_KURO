﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBeh : MonoBehaviour
{
    private int [,] board = new int [3,3];//棋盘，0、1、2分别代表空、玩家1占据、玩家2占据
    private int turn = 0;//0表示当前为玩家回合，1表示人机回合（用于人机模式）
    private int square_size = Screen.width / 10;//一个格子的尺寸
    private int menu_width = Screen.width / 5, menu_height = Screen.width / 10;//主菜单每一个按键的宽度和高度
    private int mode = 0;//根据不同mode显示不同场景
    /*
    0 主菜单
    1 玩家VS玩家
    2 玩家VS人机
    */
    private GUIStyle bigStyle, blackStyle;//自定义字体格式
    public Texture2D empty, icon1, icon2;//不同玩家的图标（圈圈和勾勾）
    public float timer;//定时器，用于模拟人机的延迟
    public float default_timer = 0.5f;//默认定时器，用于给timer赋值

    // Start is called before the first frame update
    void Start()
    {
        timer = default_timer;
        //大字体初始化
        bigStyle = new GUIStyle();
        bigStyle.normal.textColor = Color.white;
        bigStyle.normal.background = null;
        bigStyle.fontSize = 50;

        //black
        blackStyle = new GUIStyle();
        blackStyle.normal.textColor = Color.black;
        blackStyle.normal.background = null;
        blackStyle.fontSize = 50;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnGUI() {
        switch(mode) {
            case 0:
                mainMenu();
                break;
            case 1:
                playerVsPlayer();
                break;
            case 2:
                playerVSComputer();
                break;
            
        }       
    }
    void mainMenu() {
        GUI.Label(new Rect(Screen.width / 2 - menu_width * 0.65f, Screen.height * 0.05f, menu_width, menu_height), "Main Menu", bigStyle);
        if (GUI.Button(new Rect(Screen.width / 2 - menu_width / 2, Screen.height * 2 / 7 - menu_height * 0.4f, menu_width, menu_height * 2 / 3), "Player VS Player")) {
            mode = 1;
        }
        if (GUI.Button(new Rect(Screen.width / 2 - menu_width / 2, Screen.height * 2 / 7 + menu_height * 0.4f, menu_width, menu_height * 2 / 3), "Player VS Computer")) {
            mode = 2;
        }
        GUI.Label(new Rect(Screen.width / 2 - menu_width * 0.7f, Screen.height * 0.5f, menu_width/3, menu_height/2), "Game Config", bigStyle);
        if (GUI.Button(new Rect(Screen.width / 2 - menu_width / 2, Screen.height * 2 / 7 + menu_height * 1.8f, menu_width, menu_height/2), "Let others take the lead"))
        {
            turn = 1;
        }
        if (GUI.Button(new Rect(Screen.width / 2 - menu_width / 2, Screen.height * 2 / 7 + menu_height * 2.4f, menu_width, menu_height / 2), "Random first mover"))
        {
            int firstHand = (int)Random.Range(0, 2);
            turn = firstHand;
        }

    }
    int checkStateWithoutOutput() {
        int res = -1; //1 : 玩家1获胜    2 : 玩家2或者人机获胜
        //检查游戏结果是否已经产生
        for (int i = 0; i < 3; ++i) {
            if (board[i,0] != 0 && board[i,0] == board[i,1] && board[i,0] == board[i,2]) {
                res = board[i,0];
                break;
            }
        }
        if (res == -1)
            for (int j = 0; j < 3; ++j) {
                if (board[0,j] != 0 && board[0,j] == board[1,j] && board[0,j] == board[2,j]) {
                    res = board[0,j];
                    break;
                }
            }
        if (res == -1)
            if (board[1,1] != 0 && (board[0,0] == board[1,1] && board[1,1] == board[2,2] || board[0,2] == board[1,1] && board[2,0] == board[1,1])) {
                res = board[1,1];
            }
        if (res == -1) {
            int cnt = 0;
            for (int i = 0; i < 3; ++i) {
                for (int j = 0; j < 3; ++j){
                    if (board[i,j] == 0) {
                        cnt++;
                        break;
                    }
                }
            }
            if (cnt == 0) {//没有可放置的块
                res = 3;
            }
        }
        if (res == -1) return 0;
        return res;
    }
    void checkState() {
        int res = checkStateWithoutOutput();
        //根据不同res对应不同操作
        if (res == 0) return ;//没有什么特殊状态
        if (res == 1) {//玩家一胜利
            if (mode == 1) {
                GUI.Label(new Rect(Screen.width / 2 - 3 * square_size, Screen.height / 2, square_size * 1.5f, square_size * 0.8f), "Player 1 wins!", blackStyle);
            }
            else if (mode == 2) {
                GUI.Label(new Rect(Screen.width / 2 - 3 * square_size, Screen.height / 2, square_size * 1.5f, square_size * 0.8f), "Player wins!", blackStyle);
            }
        }
        else if (res == 2) {//玩家二或人机胜利
            if (mode == 1) {
                GUI.Label(new Rect(Screen.width / 2 - 3 * square_size, Screen.height / 2, square_size * 1.5f, square_size * 0.8f), "Player 2 wins!", blackStyle);
            }
            else if (mode == 2) {
                GUI.Label(new Rect(Screen.width / 2 - 3 * square_size, Screen.height / 2, square_size * 1.5f, square_size * 0.8f), "Computer wins!", blackStyle);
            }
        }
        else if (res == 3) {
            GUI.Label(new Rect(Screen.width / 2 - 3 * square_size, Screen.height / 2, square_size * 1.5f, square_size * 0.8f), "Tie!", blackStyle);
        }
        
    }

    void playerMove() {
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                switch(board[i,j]) {//board[i,j]有三种状态，空、被玩家1占据、被玩家2占据
                    case 0:
                        if (GUI.Button(new Rect(Screen.width / 2 + (i - 1.5f) * square_size, Screen.height / 2 + (j - 1.5f)* square_size, square_size, square_size), empty)) {
                            //如果当前玩家选择了这个格子，则为此格子赋上代表玩家的值，在下一帧时这个格子上会显示相应的图标
                            board[i,j] = turn + 1;
                            turn = 1 - turn;
                            
                        }
                        break;
                    case 1:
                        GUI.Button(new Rect(Screen.width / 2 + (i - 1.5f) * square_size, Screen.height / 2 + (j - 1.5f) * square_size, square_size, square_size), icon1);
                        break;
                    case 2:
                        GUI.Button(new Rect(Screen.width / 2 + (i - 1.5f) * square_size, Screen.height / 2 + (j - 1.5f) * square_size, square_size, square_size), icon2);
                        break;
                }
            }
        }
        checkState();
        
        if (GUI.Button(new Rect(Screen.width - square_size , Screen.height - square_size * 0.7f, square_size, square_size * 0.7f), "Reset")) {
            reset();
        }
        if (GUI.Button(new Rect(0 , Screen.height - square_size * 0.7f, square_size * 1.6f, square_size * 0.7f), "Return to Menu")) {
            reset();
            mode = 0;
        }

    }
    void machineMove() {
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                switch(board[i,j]) {
                    case 0:
                        GUI.Button(new Rect(Screen.width / 2 + (i - 1.5f) * square_size, Screen.height / 2 + (j - 1.5f)* square_size, square_size, square_size), empty);
                        break;
                    case 1:
                        GUI.Button(new Rect(Screen.width / 2 + (i - 1.5f) * square_size, Screen.height / 2 + (j - 1.5f) * square_size, square_size, square_size), icon1);
                        break;
                    case 2:
                        GUI.Button(new Rect(Screen.width / 2 + (i - 1.5f) * square_size, Screen.height / 2 + (j - 1.5f) * square_size, square_size, square_size), icon2);
                        break;
                }
            }
        }
        if (GUI.Button(new Rect(Screen.width - square_size , Screen.height - square_size * 0.7f, square_size, square_size * 0.7f), "Reset")) {
            reset();
        }
        if (GUI.Button(new Rect(0 , Screen.height - square_size * 0.7f, square_size * 1.6f, square_size * 0.7f), "Return to Menu")) {
            reset();
            mode = 0;
        }
        
        //wait some time
        timer -= Time.deltaTime;
        if (timer <= 0) {
            if (checkStateWithoutOutput() != 0) {
                turn = 1 - turn;
                timer = default_timer;
                return;
            }
            //choose a square to fill in

            int idx=0;//电脑下的位置
            //Debug.Log("我的int值是: " + x);
            MinMaxSearch(ref idx, 1);
            //MinMaxSearch(x,1,-100,100);//a - b剪枝 
            board[idx / 3,idx % 3] = 2;
            /*else {
                int[] arr = new int[9];
                int cnt = 0;
                for (int i = 0; i < 3; ++i)
                {
                    for (int j = 0; j < 3; ++j)
                    {
                        if (board[i, j] == 0)
                        {
                            arr[cnt++] = i * 3 + j;
                        }
                    }
                }
                if (cnt > 0)
                {
                    int rand = (int)Random.Range(0, cnt);
                    board[arr[rand] / 3, arr[rand] % 3] = 2;
                }
            }*/
            
            checkState();
            turn = 1 - turn;
            timer = default_timer;
            return;
            
        }

        
        
    }
    void playerVsPlayer() {
        playerMove();
    }
    void playerVSComputer() {
        if (turn == 0) {
            playerMove();
        }
        else {
            machineMove();
        }
    }
    
    void reset() {
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                board[i,j] = 0;
            }
        }
        turn = 0;
    }



    int eval()//评估函数,当电脑赢的时候，评估为空格数+1,玩家赢的时候评估为-空格数-1,平局的时候为0 
    {
        int res = 0;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (board[i,j] == 0) res++;

        int flag = checkStateWithoutOutput();
        if (flag == 2) return res + 1;
        if (flag == 1) return -(res + 1);
        if (flag == 3 || flag==0) return 0;

        return 0;
    }

    int MinMaxSearch(ref int idx, int step)//step用来记录层数,奇数层为电脑操作,偶数层为玩家操作  
    {
        //Debug.Log("我的int值是: idx:" + idx);
        int val;//极值
        if ((step & 1)==1) val = -100;//奇数层取极大值 
        else val = 100;//偶数层取极小值

        if (checkStateWithoutOutput() == 1 || checkStateWithoutOutput() == 2) return eval(); //有一方赢了 

        // 创建一个List<int>来存储下标 
        List<int> positions = new List<int>();//记录还有那些位置可以下棋
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (board[i, j] == 0) positions.Add(i * 3 + j);

        //Debug.Log("我的(positions.Count值是: idx:" + positions.Count);
        if (positions.Count == 0) return eval();//平局的情况 
        

        for (int i = 0; i < positions.Count; i++)
        {
            int x = positions[i];
            int t = x;
            board[x / 3, x % 3] = ((step & 1) == 1) ? 2 : 1;
            int Sonval = MinMaxSearch(ref x, step + 1);
            board[t / 3, t % 3] = 0;

            if ((step & 1) == 1)
            {
                if (val < Sonval)
                {
                    val = Sonval;
                    if (step == 1) idx = positions[i];
                }
            }
            else
            {
                if (val > Sonval)
                {
                    val = Sonval;
                }
            }
        }
        return val;
    }

    public void ModifyValue(ref int value)
    {
        value = 10;
    }

}
