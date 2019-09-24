using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ReadDBInfo : MonoBehaviour
{
    public static ReadDBInfo m_Instance;

    /// <summary>
    /// 表示东善桥的唯一ID
    /// </summary>
    private const string m_UINTID = "8177a787a28b4f86a103fac9a023db05";

    public List<BllTreeNodeInfo> m_NeedInfo;
    private string m_ErroMsg;

    public string UINTID
    {
        get
        {
            return m_UINTID;
        }
    }

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        CreateTreeNodes();
    }

    /// <summary>
    /// 生成树
    /// </summary>
    public void OnClickCreateUITree()
    {
        Debug.Log(m_NeedInfo.Count);
    }

    /// <summary>
    /// 读取数据库数据
    /// </summary>
    public void CreateTreeNodes()
    {
        BllTreeNodeInfo topTreeNode = new BllTreeNodeInfo();

        List<BllTreeNodeInfo> bllTreeNodes = GetBllTreeNodes();
        List<DevNodeInfo> devNodes = GetDevNodes();

        ///topNodes即为读取的树信息
        m_NeedInfo = bllTreeNodes.FindAll((tn) =>
        {
            return tn.TreeParentID.Equals("0");
        });
        foreach (BllTreeNodeInfo tempNode in m_NeedInfo)
        {
            AddTreeNode(bllTreeNodes, devNodes, tempNode, topTreeNode);
        }

        //m_NeedInfo = topNodes;
        //return result;
    }

    /// <summary>
    /// 添加节点
    /// </summary>
    /// <param name="bllNodes"></param>
    /// <param name="devNodes"></param>
    /// <param name="child"></param>
    /// <param name="parent"></param>
    private void AddTreeNode(List<BllTreeNodeInfo> bllNodes,
                                    List<DevNodeInfo> devNodes,
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
            AddTreeNode(bllNodes, devNodes, tempNode, child);
        }

        //Add DevNodes
        if (findNodes == null || findNodes.Count <= 0)
        {
            //Bind Dev
            if (child.BindType == 2)
            {
                // Add DevNodes
                List<DevNodeInfo> tempDevNodes = devNodes.FindAll((dev) =>
                                { return dev.DevID.Equals(child.BindID); });

                child.DevNodes.AddRange(tempDevNodes);
                //foreach (var item in tempDevNodes)
                //{
                //    Debug.Log(item.Name);
                //}
                //devNodes.RemoveAll()
            }
        }

        bllNodes.Remove(child);
    }

    /// <summary>
    /// 读取数据库内容
    /// </summary>
    /// <returns></returns>
    private List<BllTreeNodeInfo> GetBllTreeNodes()
    {
        string strErr = "";
        string strSql = "SELECT treeid,parentid,vc_Name,vc_Code,i_BindType,BindID,i_Sort " +
                       "from m_blltree where unitid='" + UINTID + "'";
        DataTable dtBllTree = MysqlDB.GetDataTable(out strErr, strSql);
        List<BllTreeNodeInfo> lstNodes = new List<BllTreeNodeInfo>();
        if (dtBllTree != null && dtBllTree.Rows.Count > 0)
        {
            for (int i = 0; i < dtBllTree.Rows.Count; i++)
            {
                BllTreeNodeInfo node = new BllTreeNodeInfo()
                {
                    BindID = dtBllTree.Rows[i]["BindID"].ToString() + "",
                    //BindType = int.Parse(dtBllTree.Rows[i]["i_BindType"].ToString()),
                    //Code = dtBllTree.Rows[i]["vc_Code"].ToString() + "",
                    NodeName = dtBllTree.Rows[i]["vc_Name"].ToString() + "",
                    TreeID = dtBllTree.Rows[i]["TreeID"].ToString() + "",
                    TreeParentID = dtBllTree.Rows[i]["parentid"].ToString() + "",
                    //	Sort = int.Parse(dtBllTree.Rows[i]["i_Sort"].ToString()),
                };
                lstNodes.Add(node);
            }
        }
        return lstNodes;
    }

    /// <summary>
    /// 读取数据库内容
    /// </summary>
    /// <returns></returns>
    private List<DevNodeInfo> GetDevNodes()
    {
        string strErr = "";
        string strSql = "select  n.devid,n.nodeid,n.vc_name,n.i_NodeType " +
                        "from m_devnodes n " +
                         "INNER JOIN m_devinfo d on n.devid = d.DevID " +
                         "inner JOIN m_blltree b on b.BindID = d.DevID " +
                         "where b.unitid = '" + UINTID + "' and b.i_BindType = 2 " +
                         "order by n.nodeid;";
        DataTable dtDevNodes = MysqlDB.GetDataTable(out strErr, strSql);
        List<DevNodeInfo> lstNodes = new List<DevNodeInfo>();
        if (dtDevNodes != null && dtDevNodes.Rows.Count > 0)
        {
            for (int i = 0; i < dtDevNodes.Rows.Count; i++)
            {
                DevNodeInfo node = new DevNodeInfo()
                {
                    DevID = dtDevNodes.Rows[i]["DevID"].ToString() + "",
                    Name = dtDevNodes.Rows[i]["vc_name"].ToString() + "",
                    NodeID = dtDevNodes.Rows[i]["NodeID"].ToString() + "",
                    NodeType = int.Parse(dtDevNodes.Rows[i]["i_NodeType"].ToString())
                };
                lstNodes.Add(node);
            }
        }
        return lstNodes;
    }
}
