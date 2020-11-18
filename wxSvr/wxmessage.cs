using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Class1 的摘要说明
/// </summary>
public class wxmessage
{
    /// <summary>
    /// 本公众帐号
    /// </summary>
    public string ToUserName { get; set; }
    /// <summary>
    /// 用户帐号
    /// </summary>
    public string FromUserName { get; set; }
    /// <summary>
    /// 发送时间戳
    /// </summary>
    public string CreateTime { get; set; }
    /// <summary>
    /// 发送的文本内容 
    /// </summary>
    public string Content { get; set; }
    /// <summary>
    /// 消息的类型
    /// </summary>
    public string MsgType { get; set; }
    /// <summary>
    /// 事件名称
    /// </summary>
    public string EventName { get; set; }

    //这两个属性会在后面的讲解中提到
    public string Recognition { get; set; }
    public string EventKey { get; set; }

    public string Xml { get; set; }
}