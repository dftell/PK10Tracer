using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// VewPressClass 的摘要说明
/// </summary>
public class ViewPressClass
{
    public ViewPressClass()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public string getUrl(string key)
    {
        return string.Format("view/view.aspx?type={0}",key);
    }
}