using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public struct BreakPower
{
    public int noBreakPower;//����
    public int maxBreakPower;

    internal void SetFull()
    {
        noBreakPower = maxBreakPower;
    }

    public void SetMaxValue(int max)
    {
        maxBreakPower = max;
        SetFull();
    }
}

[System.Serializable]
public struct PlayerData
{
    public string playerName;
    public int hp;
    public int mp;
    public int lv;
    public int exp;
    public int MaxHp;
    public float BaseAck => lv * 20 +20;

    public void SetFull()
    {
        hp = MaxHp;
    }
    public void SetMaxValue(int maxHp)
    {
        MaxHp = maxHp;
        SetFull();
    }

    public static PlayerData LoadData()
    {
        return IOHelper.GetData<PlayerData>(DataPath.playerDataJson);
    }
    public static void SetData(PlayerData playerData)
    {
        IOHelper.SetData(DataPath.playerDataJson,playerData);
    }

    public override string ToString()
    {
        return string.Format("hp:{0} mp{1} lv{2} maxHp{3}", hp, mp, lv, MaxHp);
    }
}


public static class IOHelper
{
    public static bool IsFileExists(string fileName)
    {
        return File.Exists(fileName);
    }

    public static bool IsDirectoryExists(string fileName)
    {
        return Directory.Exists(fileName);
    }

    public static void CreateFile(string fileName, string content)

    {

        StreamWriter streamWriter = File.CreateText(fileName);

        streamWriter.Write(content);

        streamWriter.Close();

    }

    public static void CreateDirectory(string fileName)

    {

        //�ļ��д����򷵻�

        if (IsDirectoryExists(fileName))

            return;

        Directory.CreateDirectory(fileName);

    }

    public static void SetData(string fileName, object pObject)
    {
        //���������л�Ϊ�ַ���
        string toSave = JsonUtility.ToJson(pObject);

        //���ַ������м���,32λ������Կ

        //toSave = RijndaelEncrypt(toSave, "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

        StreamWriter streamWriter = File.CreateText(fileName);

        streamWriter.Write(toSave);

        streamWriter.Close();
    }

    public static T GetData<T>(string fileName) where T:new() 
    {
        if (!IsFileExists(fileName))
        {
            return new T();
        }

        StreamReader streamReader = File.OpenText(fileName);
        string data = streamReader.ReadToEnd();
        //�����ݽ��н��ܣ�32λ������Կ
        //data = RijndaelDecrypt(data, "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        streamReader.Close();

        return JsonUtility.FromJson<T>(data);
    }


    #region ����
    private static string RijndaelEncrypt(string pString, string pKey)
    {

        //��Կ

        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(pKey);

        //��������������

        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(pString);

        //Rijndael�����㷨

        RijndaelManaged rDel = new RijndaelManaged();

        rDel.Key = keyArray;

        rDel.Mode = CipherMode.ECB;

        rDel.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = rDel.CreateEncryptor();

        //���ؼ��ܺ������

        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        return Convert.ToBase64String(resultArray, 0, resultArray.Length);

    }

    private static String RijndaelDecrypt(string pString, string pKey)

    {

        //������Կ

        byte[] keyArray = UTF8Encoding.UTF8.GetBytes(pKey);

        //��������������

        byte[] toEncryptArray = Convert.FromBase64String(pString);

        //Rijndael�����㷨

        RijndaelManaged rDel = new RijndaelManaged();

        rDel.Key = keyArray;

        rDel.Mode = CipherMode.ECB;

        rDel.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = rDel.CreateDecryptor();

        //���ؽ��ܺ������

        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        return UTF8Encoding.UTF8.GetString(resultArray);

    }
    #endregion


}

