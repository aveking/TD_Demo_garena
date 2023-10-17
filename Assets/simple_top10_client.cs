using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;


public class simple_top10_client : MonoBehaviour
{
    public InputField ui_input_txt;
    public Text ui_top10_txt;

    static string player_name = "无名游客";

    //ͬ���а񽻻���
    byte[] send_data = new byte[(7 + 2 + 1) << 1];
    byte[] rawReceiveBuffer_a = new byte[4096];
    int rawReceiveBuffer_t = 0;

    void Start()
    {
        //global_gamesetting.current_stagelv ����Զ�Ĺؿ�
        Connect(IPAddress.Parse("118.31.60.208"), 65501);
        
        if (ui_input_txt != null)
            ui_input_txt.text = player_name;
    }

    public void change_playername()
    {
        player_name = ui_input_txt.text;
        if (player_name == null || player_name.Length <= 0) player_name = "无名游客";
        Debug.Log($"player_name={player_name}");
    }

    Socket socket;
    EndPoint remoteEndPoint;
    public void Connect(IPAddress address, ushort port)
    {
        remoteEndPoint = new IPEndPoint(address, port);
        socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        socket.Blocking = false;

        try
        {
            socket.ReceiveBufferSize = 4096;
            socket.SendBufferSize = 4096;
        }
        catch (SocketException)
        {
            Debug.LogError(@"failed to set Socket RecvBufSize -> 4096 SendBufSize -> 4096");
        }

        socket.Connect(remoteEndPoint);
    }

    public void SendMyScore()
    {
        try
        {
            Debug.Log("Hello SendMyScore " + player_name + " " + global_gamesetting.current_stagelv);
            if (!socket.Poll(0, SelectMode.SelectWrite)) return;

            //����
            string name = player_name;
            for (int i = 0; i < 7; ++i)
            {
                char c = '\0';
                if (i < name.Length) c = name[i];
                send_data[i * 2] = (byte)(c & 0xff);
                send_data[i * 2 + 1] = (byte)((c >> 8) & 0xff);
            }

            //Ӳ��Ψһ��ʶ
            int dev_id_hash = SystemInfo.deviceUniqueIdentifier.GetHashCode();
            Debug.Log($"dev_id_hash={dev_id_hash}");
            send_data[14] = (byte)((dev_id_hash >> 0) & 0xff);
            send_data[14 + 1] = (byte)((dev_id_hash >> 8) & 0xff);
            send_data[14 + 2] = (byte)((dev_id_hash >> 16) & 0xff);
            send_data[14 + 3] = (byte)((dev_id_hash >> 24) & 0xff);

            //��߹�
            send_data[18] = (byte)(global_gamesetting.current_stagelv & 0xff);
            send_data[18 + 1] = (byte)((global_gamesetting.current_stagelv >> 8) & 0xff);

            socket.Send(send_data, 0, send_data.Length, SocketFlags.None);
        }
        catch (SocketException e)
        {
            if (e.SocketErrorCode == SocketError.WouldBlock) return;
            Debug.LogError($"Client: ping pack: {e}");
        }
    }

    struct top10_player
    {
        internal string name;
        internal int score;
    }
    List<top10_player> top10_list = new List<top10_player>();
    int compare_AB(top10_player _a, top10_player _b)
    {
        if (_a.score < _b.score) return 1;
        return -1;
    }

    bool RawReceive()
    {
        if (socket == null) return false;

        try
        {
            if (!socket.Poll(0, SelectMode.SelectRead)) return false;
            rawReceiveBuffer_t = socket.Receive(rawReceiveBuffer_a, 0, rawReceiveBuffer_a.Length, SocketFlags.None);
            if (rawReceiveBuffer_t == 200)
            {
                get_top10_res = true;
                Debug.Log($"rawReceiveBuffer_t={rawReceiveBuffer_t}");

                //��ȡ���ݣ��Ÿ���
                top10_list.Clear();
                char[] name_c = new char[7];
                for (int i = 0; i < 10; ++i)
                {
                    top10_player item = new top10_player();
                    int p = i * 20;
                    int str_len = 0;
                    bool str_over = false;
                    for (int j = 0; j < 7; ++j)
                    {
                        name_c[j] = (char)(rawReceiveBuffer_a[p] | (rawReceiveBuffer_a[p + 1] << 8)); p += 2;
                        if (name_c[j] == '\0') str_over = true;
                        if (str_over == false) ++str_len;
                    }
                    //Debug.Log($"str_len[j]={str_len}");
                    item.name = new string(name_c, 0, str_len);
                    if (item.name == null || item.name.Length <= 0) item.name = "无名游客";
                    p += 4;

                    int score = (int)(rawReceiveBuffer_a[p] | (rawReceiveBuffer_a[p + 1] << 8)); p += 2;
                    item.score = score;

                    top10_list.Add(item);
                }

                top10_list.Sort((x, y) => compare_AB(x, y));

                string txt = "\n排行榜";
                for (int i = 0; i < top10_list.Count; ++i)
                {
                    txt += "\n" + top10_list[i].score + "关       " + top10_list[i].name;
                }

                ui_top10_txt.text = txt;
            }
            return false;
        }
        catch (SocketException e)
        {
            if (e.SocketErrorCode == SocketError.WouldBlock) return false;
            Debug.Log($"Client: looks like the other end has closed the connection. This is fine: {e}");
            return false;
        }
    }


    bool get_top10_res = false;
    int try_send_cnt = 0;
    float send_cd = 0;

    private void Update()
    {
        if (get_top10_res == false && try_send_cnt < 3)
        {
            if (send_cd <= 0f)
            {
                try_send_cnt++;
                send_cd = 3f;
                SendMyScore();
            }
            else
            {
                send_cd -= Time.deltaTime;
            }
        }

        if (ui_top10_txt != null)
            RawReceive();
    }

    private void OnDestroy()
    {
        socket?.Close();
        socket = null;
    }

}
