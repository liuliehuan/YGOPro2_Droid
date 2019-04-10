﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class SelectServer : WindowServantSP
{
    UIPopupList list;
    UIPopupList serversList;

    UIInput inputIP;
    UIInput inputPort;
    UIInput inputPsw;
    UIInput inputVersion;

    UISprite inputIP_;
    UISprite inputPort_;

    public override void initialize()
    {
        createWindow(Program.I().new_ui_selectServer);
        UIHelper.registEvent(gameObject, "exit_", onClickExit);
        UIHelper.registEvent(gameObject, "face_", onClickFace);
        UIHelper.registEvent(gameObject, "join_", onClickJoin);
        serversList = UIHelper.getByName<UIPopupList>(gameObject, "server");
        //serversList.fontSize = 30;
        serversList.value = Config.Get("serversPicker", "[Custom]");
        UIHelper.registEvent(gameObject, "server", pickServer);
        UIHelper.getByName<UIInput>(gameObject, "name_").value = Config.Get("name", "YGOPro2 User");
        list = UIHelper.getByName<UIPopupList>(gameObject, "history_");
        UIHelper.registEvent(gameObject, "history_", onSelected);
        name = Config.Get("name", "YGOPro2 User");
        inputIP = UIHelper.getByName<UIInput>(gameObject, "ip_");
        inputPort = UIHelper.getByName<UIInput>(gameObject, "port_");
        inputPsw = UIHelper.getByName<UIInput>(gameObject, "psw_");
        inputIP_ = UIHelper.getByName<UISprite>(gameObject, "ip_");
        inputPort_ = UIHelper.getByName<UISprite>(gameObject, "port_");
        //inputVersion = UIHelper.getByName<UIInput>(gameObject, "version_");
        set_version("0x" + String.Format("{0:X}", Config.ClientVersion));

        if (Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
        {}
        else //如果非要改成其他语言的话
        {
            UIHelper.getByName<UIInput>(gameObject, "name_").defaultText = "Name cannot be blank";
            inputIP.defaultText = "IP address or domain name";
            inputPort.defaultText = "Port";
            inputPsw.defaultText = "Room password";
        }
        //方便免修改 [selectServerWithRoomlist.prefab]
        serversList.items.Add("[OCG]Mercury233");
        serversList.items.Add("[OCG]Koishi");
        serversList.items.Add("[TCG]Koishi");
        serversList.items.Add("[轮抽服]2Pick");
        serversList.items.Add("[OCG&TCG]한국서버");
        serversList.items.Add("[Custom]");

        SetActiveFalse();
    }

    private void pickServer()
    {
        string server = serversList.value;
        switch (server)
        {
            case "[OCG]Mercury233":
            {
                UIHelper.getByName<UIInput>(gameObject, "ip_").value = "s1.ygo233.com";
                UIHelper.getByName<UIInput>(gameObject, "port_").value = "233";
                Config.Set("serversPicker", "[OCG]Mercury233");

                list.enabled = false;
                inputIP_.enabled = false;
                inputPort_.enabled = false;
                break;
            }
            case "[OCG]Koishi":
            {
                UIHelper.getByName<UIInput>(gameObject, "ip_").value = "koishi.moecube.com";
                UIHelper.getByName<UIInput>(gameObject, "port_").value = "7210";
                Config.Set("serversPicker", "[OCG]Koishi");

                list.enabled = false;
                inputIP_.enabled = false;
                inputPort_.enabled = false;
                break;
            }
            case "[TCG]Koishi":
            {
                UIHelper.getByName<UIInput>(gameObject, "ip_").value = "koishi.moecube.com";
                UIHelper.getByName<UIInput>(gameObject, "port_").value = "1311";
                Config.Set("serversPicker", "[TCG]Koishi");

                list.enabled = false;
                inputIP_.enabled = false;
                inputPort_.enabled = false;
                break;
            }
            case "[轮抽服]2Pick":
            {
                UIHelper.getByName<UIInput>(gameObject, "ip_").value = "2pick.mycard.moe";
                UIHelper.getByName<UIInput>(gameObject, "port_").value = "765";
                Config.Set("serversPicker", "[轮抽服]2Pick");

                list.enabled = false;
                inputIP_.enabled = false;
                inputPort_.enabled = false;
                break;
            }
            case "[OCG&TCG]한국서버":
            {
                UIHelper.getByName<UIInput>(gameObject, "ip_").value = "cygopro.fun25.co.kr";
                UIHelper.getByName<UIInput>(gameObject, "port_").value = "17225";
                Config.Set("serversPicker", "[OCG&TCG]한국서버");

                list.enabled = false;
                inputIP_.enabled = false;
                inputPort_.enabled = false;
                break;
            }
            default:
            {
                Config.Set("serversPicker", "[Custom]");

                list.enabled = true;
                inputIP_.enabled = true;
                inputPort_.enabled = true;
                break;
            }
        }

    }

    void onSelected()
    {
        if (list != null)
        {
            readString(list.value);
        }
    }

    private void readString(string str)
    {
        str = str.Substring(1, str.Length - 1);
        string version = "", remain = "";
        string[] splited;
        splited = str.Split(")");
        try
        {
            version = splited[0];
            remain = splited[1];
        }
        catch (Exception)
        {
        }
        splited = remain.Split(":");
        string ip = "";
        try
        {
            ip = splited[0];
            remain = splited[1];
        }
        catch (Exception)
        {
        }
        splited = remain.Split(" ");
        string psw = "", port = "";
        try
        {
            port = splited[0];
            psw = splited[1];
        }
        catch (Exception)
        {
        }
        inputIP.value = ip;
        inputPort.value = port;
        inputPsw.value = psw;
        //inputVersion.value = version;
    }

    public override void show()
    {
        base.show();
        Program.I().room.RMSshow_clear();
        printFile(true);
        Program.charge();
    }

    public override void preFrameFunction()
    {
        base.preFrameFunction();
        Menu.checkCommend();
    }

    void printFile(bool first)
    {
        list.Clear();
        if (File.Exists("config/hosts.conf") == false)
        {
            File.Create("config/hosts.conf").Close();
        }
        string txtString = File.ReadAllText("config/hosts.conf");
        string[] lines = txtString.Replace("\r", "").Split("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 0)
            {
                if (first)
                {
                    readString(lines[i]);
                }
            }
            list.AddItem(lines[i]);
        }
    }

    void onClickExit()
    {
        Program.I().shiftToServant(Program.I().menu);
        if (TcpHelper.tcpClient != null)
        {
            if (TcpHelper.tcpClient.Connected)
            {
                TcpHelper.tcpClient.Close();
            }
        }
    }

    public void set_version(string str)
    {
        UIHelper.getByName<UIInput>(gameObject, "version_").value = str;
    }

    void onClickJoin()
    {
        if (!isShowed)
        {
            return;
        }
        string Name = UIHelper.getByName<UIInput>(gameObject, "name_").value;
        string ipString = UIHelper.getByName<UIInput>(gameObject, "ip_").value;
        string portString = UIHelper.getByName<UIInput>(gameObject, "port_").value;
        string pswString = UIHelper.getByName<UIInput>(gameObject, "psw_").value;
        string versionString = UIHelper.getByName<UIInput>(gameObject, "version_").value;
        KF_onlineGame(Name, ipString, portString, versionString, pswString);
    }

    public void onClickRoomList()
    {
        if (!isShowed)
        {
            return;
        }
        string Name = UIHelper.getByName<UIInput>(gameObject, "name_").value;
        string ipString = UIHelper.getByName<UIInput>(gameObject, "ip_").value;
        string portString = UIHelper.getByName<UIInput>(gameObject, "port_").value;
        string pswString = "L";
        string versionString = UIHelper.getByName<UIInput>(gameObject, "version_").value;
        KF_onlineGame(Name, ipString, portString, versionString, pswString);
    }

    public void KF_onlineGame(string Name, string ipString, string portString, string versionString, string pswString = "")
    {
        name = Name;
        Config.Set("name", name);
        if (ipString == "" || portString == "" || versionString == "")
        {
            RMSshow_onlyYes("", InterString.Get("非法输入！请检查输入的主机名。"), null);
        }
        else
        {
            if (name != "")
            {
                string fantasty = "(" + versionString + ")" + ipString + ":" + portString + " " + pswString;
                list.items.Remove(fantasty);
                list.items.Insert(0, fantasty);
                list.value = fantasty;
                if (list.items.Count>5) 
                {
                    list.items.RemoveAt(list.items.Count - 1);
                }
                string all = "";
                for (int i = 0; i < list.items.Count; i++)
                {
                    all += list.items[i] + "\r\n";
                }
                File.WriteAllText("config/hosts.conf", all);
                printFile(false);
                (new Thread(() => { TcpHelper.join(ipString, name, portString, pswString,versionString); })).Start();
            }
            else
            {
                RMSshow_onlyYes("", InterString.Get("昵称不能为空。"), null);
            }
        }
    }

    GameObject faceShow = null;

    public string name = "";

    void onClickFace()
    {
        name = UIHelper.getByName<UIInput>(gameObject, "name_").value;
        RMSshow_face("showFace", name);
        Config.Set("name", name);
    }

}
