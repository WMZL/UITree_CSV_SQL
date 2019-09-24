using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UTree : MonoBehaviour
{
    /// <summary>
    /// 树节点的高度
    /// </summary>
    public float itemHeight;

    /// <summary>
    /// 树数据
    /// </summary>
    private IList<UTreeData> dataList;

    /// <summary>
    /// 树节点列表
    /// </summary>
    private IList<UTreeItem> itemList;

    /// <summary>
    /// 节点索引
    /// </summary>
    private int itemIndex;

    /// <summary>
    /// 树的高度
    /// </summary>
    protected float treeHeight;

    /// <summary>
    /// 设置树节点对象
    /// </summary>
    public UTreeItem uTreeItem;

    /// <summary>
    /// 所有的树信息
    /// </summary>
    private List<BllTreeNodeInfo> m_AllTreeInfo;
    /// <summary>
    /// 树信息
    /// </summary>
    private List<UTreeData> m_TestList;
    /// <summary>
    /// 单个元素的宽度
    /// </summary>
    public int ItemWidth = 200;
    /// <summary>
    /// 树形菜单的横向排列间距
    /// </summary>
    private int HorizontalItemSpace = 25;
    /// <summary>
    /// 根节点
    /// </summary>
    public Transform m_RootParent;

    void Start()
    {
        //InitTree();
        m_TestList = new List<UTreeData>();
        m_AllTreeInfo = ReadDBInfo.m_Instance.m_NeedInfo;
        //Debug.Log(m_AllTreeInfo.Count + " " + m_AllTreeInfo);
    }

    /// <summary>
    /// 根据数据库的信息生成树
    /// </summary>
    public void OnClickGenerate()
    {
        //TestList.Add(new UTreeData("0", "0", "东善桥变", null));
        //foreach (BllTreeNodeInfo first in m_AllTreeInfo)
        //{
        //    AddInfo(first);
        //}
        foreach (BllTreeNodeInfo one in m_AllTreeInfo)
        {
            //1号主变
            m_TestList.Add(new UTreeData(one.TreeID, one.TreeParentID, one.NodeName, null));
            foreach (BllTreeNodeInfo two in one.Children)
            {
                //500kV侧
                m_TestList.Add(new UTreeData(two.TreeID, two.TreeParentID, two.NodeName, null));
                foreach (BllTreeNodeInfo three in two.Children)
                {
                    //5021电流互感器
                    m_TestList.Add(new UTreeData(three.TreeID, three.TreeParentID, three.NodeName, null));
                    foreach (BllTreeNodeInfo four in three.Children)
                    {
                        //Debug.Log(three.Children.Count + "第三层子节点数");
                        //A相
                        m_TestList.Add(new UTreeData(four.TreeID, four.TreeParentID, four.NodeName, null));
                        foreach (BllTreeNodeInfo five in four.Children)
                        {
                            //Debug.Log(four.Children.Count + "第四层子节点数");
                            if (four.Children.Count == 0)
                            {
                                Debug.Log("进入设备测点");
                            }
                            else
                            {
                                Debug.Log("到达最底层");
                                m_TestList.Add(new UTreeData(five.TreeID, five.TreeParentID, five.NodeName, null));
                            }
                        }
                    }
                }
            }
        }


        ChangeData(m_TestList);
    }

    /// <summary>
    /// 添加信息
    /// </summary>
    private void AddInfo(BllTreeNodeInfo info)
    {
        if (info.Children.Count > 0)
        {
            foreach (BllTreeNodeInfo item in info.Children)
            {
                m_TestList.Add(new UTreeData(item.TreeID, item.TreeParentID, item.NodeName, null));
                AddInfo(item);
            }
        }
        else
        {
            Debug.Log("进入测点层");
        }
    }

    /// <summary>
    /// 初始化树数据
    /// </summary>
    protected virtual void InitTree()
    {
        List<UTreeData> testList = new List<UTreeData>();

        testList.Add(new UTreeData("2", "1", "220kV区域", null));
        testList.Add(new UTreeData("3", "2", "A相", null));
        testList.Add(new UTreeData("3", "1", "35kV区域", null));
        testList.Add(new UTreeData("4", "1", "1号主变", null));
        testList.Add(new UTreeData("5", "1", "2号主变", null));
        testList.Add(new UTreeData("6", "1", "3号主变", null));
        testList.Add(new UTreeData("7", "1", "500kV区域", null));
        testList.Add(new UTreeData("8", "6", "西部荒野豺狼人", null));
        testList.Add(new UTreeData("9", "5", "西部荒野豺狼人", null));
        testList.Add(new UTreeData("10", "7", "西部荒野豺狼人", null));
        testList.Add(new UTreeData("11", "8", "西部荒野豺狼人", null));

        this.ChangeData(testList);
    }

    /// <summary>
    /// 创建树节点
    /// </summary>
    /// <returns>The tree item.</returns>
    /// <param name="index">Index.</param>
    private UTreeItem CreateTreeItem(int index)
    {
        if (index >= 0 && this.itemList != null && index <= this.itemList.Count)
        {
            return this.itemList[index - 1].GetComponent<UTreeItem>();
        }

        if (this.uTreeItem == null)
            return null;
        GameObject treeItemObject = (GameObject)Instantiate(this.uTreeItem.gameObject);
        treeItemObject.GetComponent<RectTransform>().SetParent(m_RootParent);
        treeItemObject.transform.localScale = Vector3.one;

        UTreeItem uTreeItem = treeItemObject.GetComponent<UTreeItem>();

        this.itemList.Add(uTreeItem);

        return uTreeItem;
    }

    /// <summary>
    /// 更新树数据，程序入口 001
    /// </summary>
    /// <param name="dataList">Data list.</param>
    public void ChangeData(IList<UTreeData> dataList)
    {
        this.InitItemList();

        this.dataList = dataList;
        this.whileItem("0", this.GetChildrenDataList("0"));

        this.treeHeight = this.itemIndex * this.itemHeight;

        this.ChangeTreeHeight();
    }

    /// <summary>
    /// 递归创建树节点
    /// </summary>
    /// <param name="parentID">Parent I.</param>
    /// <param name="parentDataList">Parent data list.</param>
    public void whileItem(string parentID, IList<UTreeData> parentDataList)
    {
        UTreeData parentTreeData = this.GetItemData(parentID);

        int level = 0;
        if (parentTreeData != null)
        {
            level = parentTreeData.level + 1;
        }

        foreach (UTreeData uTreeData in parentDataList)
        {
            this.itemIndex++;
            uTreeData.level = level;

            UTreeItem uTreeItem = this.CreateTreeItem(this.itemIndex);
            uTreeItem.gameObject.SetActive(true);

            uTreeItem.ChangeData(uTreeData, OnItemClickHandler);
            uTreeItem.transform.localPosition = new Vector3(0f, this.itemHeight - this.itemIndex * this.itemHeight, 0f);

            if (uTreeData.expand)
            {
                List<UTreeData> childDataList = this.GetChildrenDataList(uTreeData.id);
                if (childDataList != null && childDataList.Count > 0)
                {
                    this.whileItem(uTreeData.id, childDataList);
                }
            }
        }
    }

    /// <summary>
    /// 树节点点击事件
    /// </summary>
    /// <param name="uTreeData">U tree data.</param>
    public void OnItemClickHandler(UTreeData uTreeData)
    {
        List<UTreeData> dataList = this.GetChildrenDataList(uTreeData.id);
        UTreeData resultData = this.GetItemData(uTreeData.id);
        if (resultData != null && dataList != null && dataList.Count > 0)
        {
            //有子节点
            resultData.expand = resultData.expand == true ? false : true;
            this.ChangeData(this.dataList);
            //Debug.Log("当前选择任务：" + uTreeData.name + "(ID:" + uTreeData.id + ")");
        }
        else
        {
            //没有子节点
            this.ChangeTreeSelect(uTreeData);
        }
    }

    /// <summary>
    /// 更新树高度
    /// </summary>
    protected virtual void ChangeTreeHeight()
    {
        Debug.Log("树的总高度" + treeHeight);
        SetScrollView();
    }

    /// <summary>
    /// 更新树节点选择
    /// </summary>
    /// <param name="uTreeData">U tree data.</param>
    protected virtual void ChangeTreeSelect(UTreeData uTreeData)
    {
        Debug.Log("当前选择任务：" + uTreeData.name + "(自身ID:" + uTreeData.id + ")+ (父ID: " + uTreeData.parentID + ")");
    }

    /// <summary>
    /// 获取节点的子节点数据
    /// </summary>
    /// <returns>The children data list.</returns>
    /// <param name="parentID">Parent I.</param>
    private List<UTreeData> GetChildrenDataList(string parentID)
    {
        List<UTreeData> resultList = new List<UTreeData>();
        foreach (UTreeData uTreeData in this.dataList)
        {
            if (uTreeData.parentID == parentID)
            {
                resultList.Add(uTreeData);
            }
        }
        return resultList;
    }

    /// <summary>
    /// 根据节点 ID 数据获取树数据
    /// </summary>
    /// <returns>The item data.</returns>
    /// <param name="id">Identifier.</param>
    private UTreeData GetItemData(string id)
    {
        foreach (UTreeData uTreeData in this.dataList)
        {
            if (uTreeData.id == id) return uTreeData;
        }
        return null;
    }

    /// <summary>
    /// 初始化树节点
    /// </summary>
    private void InitItemList()
    {
        this.itemIndex = 0;

        if (this.itemList == null)
            this.itemList = new List<UTreeItem>();
        if (this.itemList.Count > 0)
        {
            foreach (UTreeItem uTreeItem in this.itemList)
            {
                uTreeItem.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 设置scrollView刷新
    /// </summary>
    /// <param name="item"></param>
    public void SetScrollView()
    {
        float x = HorizontalItemSpace + ItemWidth;
        float y = Mathf.Abs(treeHeight) + 50f;
        transform.GetComponent<ScrollRect>().content.sizeDelta = new Vector2(x, y);
    }
}
