<html>
<head>
    <title>功率因素和力调电费测算</title>
    <meta charset="gb2312">
    <link rel="stylesheet" href="../css/style.css">
    <script src="../js/zepto.min.js"></script>
    <script>
	function calc()
    {
        var iTimes = times.value;
        var dHigh = Math.round((HighPower.value-preHighPower.value)*iTimes);
        var dNormal = Math.round((NormalPower.value-preNormalPower.value)*iTimes);
        var dLow = Math.round((LowPower.value-preLowPower.value)*iTimes);
        var dNonePower = Math.round((NoPower.value-preNoPower.value)*iTimes);
        var allPower = dHigh+dNormal+dLow;
        var powerFactor = allPower/ Math.pow((Math.pow(allPower,2)+ Math.pow(dNonePower,2)),0.5);
	
        //document.querySelector('#powerMoney').text="kldfjldfdf";
        $('#diffHigh').text(dHigh);
        $('#diffNormal').text(dNormal);
        $('#diffLow').text(dLow);
        $('#diffNoPower').text(dNonePower);
$('#powerVol').text(allPower);
$('#noneVol').text(dNonePower);
        $('#powerFactor').text(powerFactor);
        //alert(iTimes);
    }
</script>
</head>
<body>
<div class="wxapi_container">
    <div class="lbox_close wxapi_form">
<center><H2>功率因素和力调电费测算</H2></center>
<h2>=====上月数据=====</h2>
<H2>倍率<input name="times" value="500"/></H2>
<H2>上月峰量<input name="preHighPower" value="9.12"/>
</H2>
<H2>上月平量
<input name="preNormalPower"  value="9.12"/></H2>
<H2>
上月谷量
<input name="preLowPower"  value="9.12"/>
</H2>
<h2>
上月无功量
<input name="preNoPower"  value="9.12"/></h2>
</div>
<h2>=====当前数据=====</h2>
<H2>当前峰量
<input name="HighPower" value="11.12"/>
<span id="diffHigh"/>
</H2>
<h2>
当前平量
<input name="NormalPower" value="12.33"/>
<span id="diffNormal" />
</H2>
<h2>
当前谷量
<input name="LowPower" value="13.43"/>
<span id="diffLow"/>
</H2>
<h2>
当前无功量
<input name="NoPower" value="13.12"/>
<span id="diffNoPower"/>
</H2>
<h2>
标准<input name="chkStd" value="0.90"/>
</h2>
<h2>
	有功电量:
	<span id="powerVol"/>
</H2>
<h2>
	无功电量:
	<span id="noneVol"/>
</H2>
<h2>
	功率因素:
	<span id="powerFactor"/>
</H2>

<button class="btn btn_primary" onclick="calc()" value="计算">计算</button>


</div></div>
</body>
</html>