<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="../WXMainFrame.aspx.cs" Inherits="WXMainFrame" %>
<html>
<head>
    <title>南方电网-功率因素/力调电费测估工具</title>
    <meta charset="gb2312">
    <link rel="stylesheet" href="../css/style.css">
    <script src="../js/zepto.min.js"></script>
    <script>
	function calc()
    {

        var iTimes = $('#times').val();
        var allPower = Math.round(($('#currPower').val()-$('#preHighPower').val()-$('#preNormalPower').val()-$('#preLowPower').val())*iTimes);
        var dNonePower = Math.round(($('#NoPower').val() - $('#preNoPower').val())*iTimes);//Math.round((NoPower.value-preNoPower.value)*iTimes);
        var powerFactor = allPower/ Math.pow((Math.pow(allPower,2)+ Math.pow(dNonePower,2)),0.5);
	
        //document.querySelector('#powerMoney').text="kldfjldfdf";
		
        $('#diffPower').text(allPower);
        $('#diffNoPower').text(dNonePower);
		var lPowerFactor = Math.round(powerFactor*100)/100;
        $('#powerFactor').val(lPowerFactor);
		var std = $("#chkStd").val();
		var gtStd = true;
		if (lPowerFactor<std)
		{
			gtStd = false;
		}
		var rate = 0;
		var useStd = std;
		if(!gtStd)
		{
			if(lPowerFactor<0.65)
			{
				useStd = 0.65;
				rate = 0.02;
			}
			else if(lPowerFactor>=0.65 && lPowerFactor<0.7)
			{
				useStd = 0.7;
				rate = 0.01;
			}
			else
			{
				rate = 0.005;
			}
		}
		else
		{
			rate = 0.0015;
			if(lPowerFactor>0.95)
			{
				useStd = lPowerFactor-0.05;
			}
		}
		var lastRes = (lPowerFactor-useStd)/0.01*rate;
		var money = -1*Math.round(lastRes* ($('#preMonthPowerMoney').val()-$('#preMonthNoPowerMoney').val())*100)/100;
		$('#NoRate').val(Math.round(lastRes*-10000)/100);
		$('#NoPowerMoney').val(money);
        //alert(powerFactor);
    }
		
	

 

	function changeTime()
	{
		
		$('#checkTime').val(new Date().Format("yyyy-MM-dd hh:mm:ss"));
		$('#checkDate').val(new Date().Format("yyyy-MM-dd"));
		try
		{
			//alert("加载地址");		
			
			wx.getLocation({
			 type: 'wgs84',
            success: function (res) {
				//alert(JSON.stringify(res));
                $("#ltion").val(res.longitude+","+res.latitude);
            },
            cancel: function (res) {
                alert('用户拒绝授权获取地理位置');
            }
			});
			
		}
		catch (e)
		{
			alert(e);
		}
	}

    function shareInfo()
    {
            var pms = "";
			var inputs = $('input');
			//alert(inputs);
            $.each(inputs,function(i,item){
				var iname = item.id;
				var ival = item.value;
				//alert(item.value);
				//alert(ival);
				//alert(iname);
				try
				{
					if(ival =='')
					{
						return;
						}
					pms = pms+"&"+iname+"="+ival;
				}
				catch (e)
				{
					//alert(e);
				}
            });
            var url = "http://www.wolfinv.com/lsi/gndl/calcpwoer.aspx?rnd=1" + pms;
			alert(url);
			try
			{
				wx.checkJsApi({jsApiList: ['onMenuShareAppMessage','updateAppMessageShareData']});
				wx.onMenuShareAppMessage({ 
				title: '南方电网-功率因素/力调电费测估工具',// 分享标题
				desc: '计算用户当前功率及估算当月力调电费',// 分享描述
				link: url,// 分享链接，该链接域名或路径必须与当前页面对应的公众号JS安全域名一致
				imgUrl: '',// 分享图标
				success: function () {
				// 设置成功
					alert("分享成功！");
					}
				});
				wx.updateAppMessageShareData({ 
				title: '南方电网-功率因素/力调电费测估工具',// 分享标题
				desc: '计算用户当前功率及估算当月力调电费',// 分享描述
				link: url,// 分享链接，该链接域名或路径必须与当前页面对应的公众号JS安全域名一致
				imgUrl: '',// 分享图标
				success: function () {
				// 设置成功
					alert("分享成功！");
					}
				});
			}
			catch (e)
			{
				alert(e);
			}
            


    }

	function submitInfo()
	{
		var pms = "";
			var inputs = $('input');
			//alert(inputs);
            $.each(inputs,function(i,item){
				var iname = item.id;
				var ival = item.value;
				//alert(item.value);
				//alert(ival);
				//alert(iname);
				try
				{
					
					pms = pms+"&"+iname+"="+ival;
				}
				catch (e)
				{
					//alert(e);
				}
            });
		var data = {
				customName : $("#useCompany").val(),
				customCode : $("#machineCode").val(),
				noPowerFactor:$("#powerFactor").val(),
				noPowerMoney:$("#NoPowerMoney").val(),
				ltion:$("#ltion").val(),
				checkDate:$("#checkDate").val(),
				checkTime:$("#checkTime").val(),
				wxOpenId:$("#wxOpenId").val(),
				customPowData:"n=1"+pms
			};
		//alert(JSON.stringify(data));
		//return;
		$.post('./noPowerInfo.asp', JSON.stringify(data), 
		function (res) {
			//alert(JSON.stringify(res));
    // process response
			if(res.succ==1)
			{
				alert("保存成功！");
				return;
			}
			else
			{
				alert(res.msg);
			}
		},"json"
		);
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
</head>
<body>
<div class="wxapi_container">
<input type="hidden" id="ltion" value=""/> 
<input type="hidden" id="wxOpenId" value=""/>
<input type="hidden" id="checkDate"/>
<h2>
单&nbsp;&nbsp;位&nbsp;&nbsp;名&nbsp;&nbsp;称&nbsp;
<input type="string" id="useCompany" value="<%=Request["useCompany"]%>" size="40"/></h2>
<h2>变&nbsp;&nbsp;压&nbsp;&nbsp;器&nbsp;&nbsp;号&nbsp;
<input  id="machineCode" type="number"  value="<%=Request["machineCode"]%>" size="40"/></h2>
<h2>采&nbsp;&nbsp;样&nbsp;&nbsp;&nbsp;时&nbsp;&nbsp;间
<input type="string" id="checkTime" size="20"/></h2>
<H2>功&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;率
<input id="powerRate" value="<%=Request["powerRate"]%>" type="number" size="10"/>KVA
</H2>
<H2>电&nbsp;表&nbsp;倍&nbsp;率
<input id="times" value="<%=Request["times"]%>" type="number" size="10"/></H2>
<hr/>
<H2>上月末峰量
<input id="preHighPower" type="number" value="<%=Request["preHighPower"]%>"  size="10"/>
</H2>
<H2>上月末平量
<input id="preNormalPower" type="number"   value="<%=Request["preNormalPower"]%>"  size="10"/></H2>
<H2>
上月末谷量
<input id="preLowPower"  type="number"  value="<%=Request["preLowPower"]%>"  size="10"/>
</H2>
<h2>
上月末无功
<input id="preNoPower" type="number"   value="<%=Request["preNoPower"]%>"  size="10"/></h2>
<h2>上月总电费
<input id="preMonthPowerMoney" type="number" value="<%=Request["preMonthPowerMoney"]%>"  size="10"/>(不计减免)
</h2>
<h2>前力调电费
<input id="preMonthNoPowerMoney" type="number" value="<%=Request["preMonthNoPowerMoney"]%>"  size="10"/></h2>
</div>
<hr/>
<H2>当前有功量
<input id="currPower" type="number"  value="" size="10"/>
<span id="diffPower"/>
</H2>
<h2>
当前无功量
<input id="NoPower" type="number"  value="" size="10"/>
<span id="diffNoPower"/>
</H2>
<h2>
参照标准值
<input id="chkStd" type="number"  value="0.90" size="10"/>
</h2>
<hr/>
<h2>功&nbsp;&nbsp;率&nbsp;&nbsp;因&nbsp;&nbsp;素&nbsp;&nbsp;
<input id="powerFactor" enable="0" size="10" disabled="true"/>
</H2>
<h2>力调电费比例
<input type="number" id="NoRate" size="10" disabled="true"/>%</h2>
<h2>
估算力调电费
<input type="string" id="NoPowerMoney" size="10" disabled="true"/>
</h2>
<button class="btn btn_primary" onclick="calc()" >计算</button>
<button class="btn btn_primary" onclick="submitInfo()">上报</button>
<!--
<button class="btn btn_primary" onclick="shareInfo()">分享</button>-->

</div></div>
</body>
<script src="http://res.wx.qq.com/open/js/jweixin-1.6.0.js"></script>
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
  wx.ready(function () {changeTime();});
  
</script>
<script src="../js/zepto.min.js"></script>
<script src="../js/util.js"> </script>
</html>
