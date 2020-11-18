<%@language="javascript" codepage="65001"%>
<%Session.CodePage = 65001;
//Response.charset="utf-8"
//Response.charset="gb2312";
//'Response.write "{""succ"":1,""msg"":""保存成功！""}"
main();
function main()
{
	var ret = {succ:0,msg:"",data:{}};
	var getpostjson = Request.TotalBytes;//'得到字节数
	if(getpostjson == 0)
	{
		ret.succ = 0;
		ret.msg = "json null"
		Response.Write(ret);
		Response.End();
		return;
	}
	var readjson = Request.BinaryRead(getpostjson);//
	var jsonstr = bytes2bstr(readjson);//
	var jobj = eval("(" + jsonstr + ")");
	ret.data = jobj;
	var conn = Server.CreateObject("Adodb.Connection")
	var res  = Server.CreateObject("Adodb.RecordSet")
	var ConnString = "Provider=sqloledb;Persist Security Info=False;Server=47.95.222.142;User ID=sa;Password=bolts;Initial Catalog=pk10db;"
	//ConnString = "Provider=SQLNCLI.1;Persist Security Info=False;Server=db.wolfinv.com;User ID=sa;Password=bolts;Initial Catalog=pk10db;"
    ConnString = "Provider=SQLNCLI10.1;Data Source=db.wolfinv.com;Initial Catalog=chinazhou;User ID=sa;Password=bolts"
	//ConnString = "Provider=SQLNCLI.1;Data Source=db.wolfinv.com;Initial Catalog=chinazhou;User ID=sa;Password=bolts"
	try
	{
		//res.close();
		//conn.close();
		conn.open(ConnString)
		var sql = "select * from FSMS_CustomNoPowerInfo";
		res.open(sql,conn,1,3)
		//Response.write("stemp1");
		res.AddNew();
		//Response.write("stemp1.5");
		/*
				customName : $("#useCompany").val(),
				customCode : $("#machineCode").val(),
				noPowerFactor:$("#powerFactor").val(),
				noPowerMoney:$("#NoPowerMoney").val(),
				ltion:$("#ltion").val(),
				checkDate:$("#checkDate").val(),
				checkTime:$("#checkTime").val(),
				wxOpenId:$("#wxOpenId").val(),
				customPowData:{
					currPowerVol:$("#diffPower").text(),
					currNoPowerVol:$("#diffNoPower").text(),
					NoRate:$("#NoRate").val()
		*/
		res("wxId") = jobj.wxOpenId;
		//Response.write("stemp1.6");
		res("checkDate") = jobj.checkDate;
		//Response.write("stemp1.7");
		res("checkTime") = jobj.checkTime;
		//Response.write("stemp1.8");
		res("customCode") = jobj.customCode;
		//Response.write("stemp1.9");
		res("customName") = jobj.customName;
		res("noPowerFactor") = jobj.noPowerFactor;
		res("noPowerMoney") = jobj.noPowerMoney;
		res("location") = jobj.ltion;
		res("customPowData") = jsonstr;
		//Response.write("stemp2");
		res.Update();
		//Response.write("stemp3");
		
	}
	catch(e)
	{
		ret.succ= 0;
		ret.msg = e.message;
		Response.write(jsonToString(ret));
		Response.End();
		return ;
	}
	finally
	{
		res.Close();
		//Response.write("stemp4");
		conn.Close();
		//Response.write("stemp5");
	}
	ret.succ= 1;
	Response.write(jsonToString(ret));
	Response.End();
}

function bytes2bstr(vin)
{
    //stringreturn
	var bytesstream = Server.CreateObject("adodb.stream");
	bytesstream.type = 2
	bytesstream.open();
	bytesstream.writeText(vin);
	bytesstream.position = 0
	bytesstream.charset = "utf-8"//
	bytesstream.position = 2
	stringreturn = bytesstream.readtext()
	bytesstream.close();
	bytesstream = null;
	return stringreturn;
}


function jsonToString (obj){   

        var THIS = this;    

        switch(typeof(obj)){   

            case 'string':   

                return '"' + obj.replace(/(["\\])/g, '\\$1') + '"';   

            case 'array':   

                return '[' + obj.map(THIS.jsonToString).join(',') + ']';   

            case 'object':   

                 if(obj instanceof Array){   

                    var strArr = [];   

                    var len = obj.length;   

                    for(var i=0; i<len; i++){   

                        strArr.push(THIS.jsonToString(obj[i]));   

                    }   

                    return '[' + strArr.join(',') + ']';   

                }else if(obj==null){   

                    return 'null';   

  

                }else{   

                    var string = [];   

                    for (var property in obj) string.push(THIS.jsonToString(property) + ':' + THIS.jsonToString(obj[property]));   

                    return '{' + string.join(',') + '}';   

                }   

            case 'number':   

                return obj;   

            case false:   

                return obj;   

        }   

    }

%>