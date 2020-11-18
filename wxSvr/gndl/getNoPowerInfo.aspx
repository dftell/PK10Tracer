<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="../WXMainFrame.aspx.cs" Inherits="WXMainFrame" %>
<html>
<head>
    <title>南方电网-客户查询</title>
    <meta charset="gb2312">
    <link rel="stylesheet" href="../css/style.css">
    <script src="../js/zepto.min.js"></script>
    <script>
	function changeTime()
	{
		
		
	}
    Date.prototype.Format=function(fmt){//author:meizz
    var o={
"M+":this.getMonth()+1,//月份
"d+":this.getDate(),//日
"h+":this.getHours(),//小时
"m+":this.getMinutes(),//分
"s+":this.getSeconds(),//秒
"q+":Math.floor((this.getMonth()+3)/3),//季度
"S":this.getMilliseconds()//毫秒
};
    if(/(y+)/.test(fmt))
	    fmt=fmt.replace(RegExp.$1,(this.getFullYear()+"").substr(4-RegExp.$1.length));
    for(var k in o)
        if(new RegExp("("+k+")").test(fmt))
	        fmt=fmt.replace(RegExp.$1,(RegExp.$1.length==1)?(o[k]):(("00"+o[k]).substr((""+o[k]).length)));
        return fmt;
    }
</script>
    <script src="http://res.wx.qq.com/open/js/jweixin-1.6.0.js"></script>
    <script type="text/javascript" src="http://api.map.baidu.com/api?v=2.0&ak=fIuybDQHQGVjdw5hjNVKerEvOz6Mwv2K"></script>
    <script type="" />
</head>
<body>
<div class="wxapi_container">
    <h2>名称
    <input id="customName" value="" /><button id="btn_search" onclick="search()">搜索</button></h2>
    <div id="custom_list" />
</div>
</body>


<script>
  /*
   * 
   * 注意：
   * 1. 所有的JS接口只能在公众号绑定的域名下调用，公众号开发者需要先登录微信公众平台进入“公众号设置”的“功能设置”里填写“JS接口安全域名”。
   * 2. 如果发现在 Android 不能分享自定义内容，请到官网下载最新的包覆盖安装，Android 自定义分享接口需升级至 6.0.2.58 版本及以上。
   * 3. 常见问题及完整 JS-SDK 文档地址：http://mp.weixin.qq.com/wiki/7/aaa137b55fb2e0456bf8dd9148dd613f.html
   *
   * 开发中遇到问题详见文档“附录5-常见错误及解决办法”解决，如仍未能解决可通过以下渠道反馈：
   * 邮箱地址：weixin-open@qq.com
   * 邮件主题：【微信JS-SDK反馈】具体问题
   * 邮件内容说明：用简明的语言描述问题所在，并交代清楚遇到该问题的场景，可附上截屏图片，微信团队会尽快处理你的反馈。
   */
  wx.config({
      debug: false,
      appId: '<%=AppId%>',
      timestamp: <%=TimeStamp%>,
      nonceStr: '<%=NonStr%>',
      signature: '<%=Sign%>',
      jsApiList: [
        'checkJsApi',        
        'onMenuShareQQ',
        'onMenuShareWeibo',
        'onMenuShareQZone',
        'hideMenuItems',
        'showMenuItems',
        'hideAllNonBaseMenuItem',
        'showAllNonBaseMenuItem',
        'translateVoice',
        'startRecord',
        'stopRecord',
        'onVoiceRecordEnd',
        'playVoice',
        'onVoicePlayEnd',
        'pauseVoice',
        'stopVoice',
        'uploadVoice',
        'downloadVoice',
        'chooseImage',
        'getLocalImgData',
        'previewImage',
        'uploadImage',
        'downloadImage',
        'getNetworkType',
        'openLocation',
        'getLocation',
        'hideOptionMenu',
        'showOptionMenu',
        'closeWindow',
        'scanQRCode',
        'chooseWXPay',
        'openProductSpecificView',
        'addCard',
        'chooseCard',
		'updateTimelineShareData',
        'updateAppMessageShareData',
        'openCard'
      ]
  });
 
  
</script>
<script src="../js/zepto.min.js"></script>
<script src="../js/util.js"> </script>
</html>
