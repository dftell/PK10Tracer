function Translate(strIn)
{
	var txt = eval('(' + strIn+ ')'); 
	var res = EnCryptJsonParams(txt);
	return 'info=' + res.info +  '&hasZip=' + res.hasZip;	
}
function TranslateInst(ExpertNo,__Token,strIn)
{
		//n=δ֪ 8 ����
		//c=�ں�
		//totalTimesInput=����Ͷע����
		//l=�������
	    //gl.bet.betMode = 0 0����ͨ��1������׷�ţ�2���߼�׷��
	    //e = [n, c, $totalTimesInput.val(), l, gl.bet.betMode, 1, "", ""];
	    	var n, c, l;
		n = "8";
		c = ExpertNo;
		l = eval('(' + strIn + ')'); 
		r = __Token;
		e=[n,c,"1",l,0,1,"",""];
		var res = EnCryptJsonParams(e);
		return '__RequestVerificationToken=' + r +  '&hasZip=' + res.hasZip +'&info=' + res.info  ;	
}

function Login(strUserName,strPassword)
{
    ctrlUsr =  document.getElementById("txt_username");
    ctrlPwd = document.getElementById("txt_pwd");
    ctrlbtn = document.getElementById("login-submit-button");
    ctrlUsr.value = strUserName;
    ctrlPwd.value = strPassword;
    ctrlbtn.click();
}

function SendMsg(strExpertNo,strIn)
{
	var n, c, l;
	n = "8";
	c = strExpertNo;
	if(strIn.length == 0) return;
	l = eval('(' + strIn + ')'); 
	//r = __Token;
	e=[n,c,"1",l,0,1,"",""];
	//alert(EnCryptJsonParams(e).info);
	//var res = EnCryptJsonParams(e);
	//return '__RequestVerificationToken=' + r +  '&hasZip=' + res.hasZip +'&info=' + res.info  ;
	ctx.postTokenEx(
	{
		url:"/Bet/CqcSubmit",data:EnCryptJsonParams(e),
		beforeSend:function()
		{
			
		},
		complete:function()
		{
			
		},
		success:function(n)
		{
			//alert(n.Tip);
			n.Ok==1
			?(
				
				lv=n.gamePoint.toFixedNum(3),
				u="�ɹ�",
				t=3,
				gl.doLoop
				(
					{
						loopInt:t,
						backFunc:function()
						{
							$("#banIssueId").html(
							'״̬��{0}��ʣ�ࣺ{1};'.replaceFormat([u,lv])
							),gl.baseBettingBanTips._show()
						}
					}
				),
				refreshGamePoint(n.gamePoint.toFixedNum(3))
			)
			:(
				//refreshGamePoint(n.gamePoint.toFixedNum(3)),
				i=n.Tip,
				u="ʧ��",
				//$.appAlert({useTitle:"Ͷע���",title:u,message:i},1e3)
				t=3,
				gl.doLoop
				(
					{
						loopInt:t,
						backFunc:function()
						{
							$("#banIssueId").html(
							'״̬��{0}����ϸ��{1}'.replaceFormat([i,u])
							),
							gl.baseBettingBanTips._show()
						}
					}
				)
			);
		}
			
	}
	)

}

function ClickEgg()
{
                //var actionEggInstance = new ActionEgg('/Activity/GetZaJinDanActivityResult');
    this.requestUrl = '/Activity/GetZaJinDanActivityResult';
	ctx.postTokenEx({
                url: this.requestUrl,
                data: null,
                success: function (data) 
				{
					
                },
                error: function (e) {
                },
                complete: function () {
					
                }
            });
}

function FillMoney(e,t)
{
	o=t;
	p = "/Recharge/ThirdRecharge?amount={0}&t={1}".replaceFormat([e, o]),
            c = !1,
            window.location.href=p;
	return;
	var h=$("#tpui-funds-sum"),
	k=$("#recharge-model"),
	l=$(".recharge-demo"),
	r=$("#recharge-order"),
	v=$("#recharge-order-submit"),
	u=$("#recharge-order-refer"),
	p=$("#recharge-order-prop"),
	w=$("#recharge-order-demo"),
	tt=$("#recharge-order-back"),
	it=$("#recharge-order-close"),
	f=$("#recharge-account-checker"),
	o=$("#rt-cutdown"),
	nt=$("#rechargeBanktips"),
	g=$("#rechargeOrderCancelBtn"),
	ut=$("#rt-reload"),
	rt=$('[name="recharge-account"]'),
	i={},e111111={},
	a={
		gsyh:{name:"�й���������",site:"https://mybank.icbc.com.cn/icbc/perbank/index.jsp"},
		cft:{name:"��Ѷ�Ƹ�ͨ",site:"https://www.tenpay.com/v2/"},
		zfb:{name:"�й���������",site:"https://www.alipay.com/"},
		zfbyhk:{name:"֧����-���п�",site:"https://www.alipay.com/"},
		zfbqy:{name:"֧����",site:"https://www.alipay.com/"},
		qcode:{name:"ɨ��֧��",site:""},
		wx:{name:"ũҵ����",site:"https://pay.weixin.qq.com/index.php/core/home/login?return_url=%2F"},
		wg:{name:"����ת��",site:""}
	},
	s,c;
	alert(e+t);
	e111111=
	{
		sum:$("#rc-sum"),
		bankname:$("#rc-bankname"),
		username:$("#rc-username"),
		useraccount:$("#rc-useraccount"),
		orderid:$("#rc-orderid")
	}
	if(t==0)
		return alertTip("��ѡ���ʻ�����!"),!1;
	s=[e,t,""],
	s=EnCryptJsonParams(s),
	c=!0,
	v.css("cursor","wait"),
	ctx.postTokenEx(
	{
		url:"/Recharge/RechargeSubmit",
		data:s,
		complete:function()
		{
			c=!1,
			v.css("cursor","pointer"),
			alert("completed!");
		},
		success:function(n)
		{
			alert("success!");
			if(n.Ok==1)
				if(t==10211)
					$("#wxPayImgId").attr("src","/Recharge/GetWxImg?data="+n.Url),
					r.tpuiDialog("_close"),b(null,1),$("#normalTBodyId").hide(),
					$("#wxTBodyId").show(),
					$("#qCodeTBodyId").hide(),
					$("#recharge-order-back").show(),
					$(".btnsClass").hide(),
					u.tpuiDialog("_open");
				else if(t==1033)
				{
					var f=[];
					f.bankname=a[o].name,
					f.orderid=n.OrderNo,
					f.sum=e,
					f.bankurl=n.AccNo,
					d(f)
				}
				else 
					i.orderId=n.OrderNo,
					i.ZfbToBankName=n.ZfbToBankName,
					i.type=o,
					i.bankName=t==1021?"΢��֧��":a[o].name,
					i.bankUrl=a[o].site,
					i.username=n.AccName,
					i.useraccount=n.AccNo,
					i.sum=t==1001||t==1007?n.RechargeAmount:e,
					r.tpuiDialog("_close"),
					b(i),
					$("#normalTBodyId").show(),
					$("#wxTBodyId").hide(),
					$("#qCodeTBodyId").hide(),
					n.bankType=="1007"?$("#recharge-order-prop").hide():$("#recharge-order-prop").show(),
					u.tpuiDialog("_open");
			else 
				alertTip(n.Tip,"��ֵ����")
		}
	})
}