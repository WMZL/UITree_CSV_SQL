using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public static test m_Instance;
    /// <summary>
    /// 读取到的所有的树信息
    /// </summary>
    public List<BllTreeNodeInfo> m_NeedInfo;

    public Text m_Text;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
    }

    void Start()
    {
        List<BllTreeNodeInfo> bllTreeNodes = TestDebug();
        BllTreeNodeInfo topTreeNode = new BllTreeNodeInfo();
        ///topNodes即为读取的树信息
        m_NeedInfo = bllTreeNodes.FindAll((tn) =>
        {
            return tn.TreeParentID.Equals("0");
        });
        foreach (BllTreeNodeInfo tempNode in m_NeedInfo)
        {
            AddTreeNode(bllTreeNodes, tempNode, topTreeNode);
        }
    }

    private void AddTreeNode(List<BllTreeNodeInfo> bllNodes,
                                 BllTreeNodeInfo child,
                                 BllTreeNodeInfo parent)
    {
        parent.Children.Add(child);

        List<BllTreeNodeInfo> findNodes = bllNodes.FindAll((tn) =>
        {
            return tn.TreeParentID.Equals(child.TreeID);
        });

        foreach (BllTreeNodeInfo tempNode in findNodes)
        {
            AddTreeNode(bllNodes, tempNode, child);
        }
        bllNodes.Remove(child);
    }

    public static DataTable OpenCSV(string filePath)
    {
        try
        {
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            DataTable dt = new DataTable();

            StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;//根据文件第一行,确定文件的列数
                                                   //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            //if (aryLine != null && aryLine.Length > 0)
            //{
            //    dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            //}

            sr.Close();
            fs.Close();
            return dt;
        }
        catch (Exception ex)
        {
            Debug.LogError("读取csv文件出错\r\n" + ex);
            return null;
        }
    }

    private List<BllTreeNodeInfo> TestDebug()
    {
        string path = Application.streamingAssetsPath + "/m_blltree.csv";
        //string path = @"C:\Users\Administrator\Desktop\beiyong\WebGL\StreamingAssets\m_blltree.csv";
        m_Text.text = path;

        DataTable dtBllTree = OpenCSV(path);
        List<BllTreeNodeInfo> lstNodes = new List<BllTreeNodeInfo>();
        if (dtBllTree != null && dtBllTree.Rows.Count > 0)
        {
            for (int i = 0; i < dtBllTree.Rows.Count; i++)
            {
                BllTreeNodeInfo node = new BllTreeNodeInfo()
                {
                    NodeName = dtBllTree.Rows[i][2].ToString(),
                    TreeID = dtBllTree.Rows[i][0].ToString(),
                    TreeParentID = dtBllTree.Rows[i][1].ToString(),
                };
                lstNodes.Add(node);
            }
        }
        return lstNodes;
    }
}
