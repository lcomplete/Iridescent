(function(){
var easy=function(selector){
    return new easy.fn.init(selector);//应把easy.fn.init看做一个类
},
_$E = window.$E;//为消除多库冲突做准备- -摆设而已
easy.fn=easy.prototype={
    init:function(selector){
        if(selector.nodeType){
            this[0]=this.E=selector;
            this.length=1;
            return this;
        }
        this[0]=this.E=document.getElementById(selector);
        this.length=1;
        return this;
    },
    version:"1.0.0",
    data:null
}
easy.fn.init.prototype = easy.fn;//为对象引用原型方法
//为对象添加方法(静态和原型)的函数 调用extend扩展静态方法 fn.extend扩展easy对象的方法
easy.extend = easy.fn.extend = function(funobj){
    if(typeof funobj !="object")
        return;
    else{
        var target=this;
	    for ( var name in funobj ) {
	       var src = target[name], copy = funobj[name];
	       if(src===copy)//不能覆盖现有方法
	           continue;
	       if ( copy !== undefined )
	           target[ name ] = copy;
	    }
	}
	return target;
}

//常用easy对象方法
easy.fn.extend({
    scrollTop: function() {
        node=this[0];
		var doc = node ? node.ownerDocument : document;
		return doc.documentElement.scrollTop || doc.body.scrollTop;
	},
	scrollLeft: function() {
	    var node=this[0];
		var doc = node ? node.ownerDocument : document;
		return doc.documentElement.scrollLeft || doc.body.scrollLeft;
	},
    rect: function(){//获取位置
        var node=this[0]
		var left = 0, top = 0, right = 0, bottom = 0;
		//ie8的getBoundingClientRect获取不准确
		if ( !node.getBoundingClientRect || easy.browser.ie8 ) {
			var n = node;
			while(n){
			    left += n.offsetLeft;
			    top += n.offsetTop; 
			    n = n.offsetParent; 
		    };
			right = left + node.offsetWidth; 
			bottom = top + node.offsetHeight;
		} else {
			var rect = node.getBoundingClientRect();
			left = right = this.scrollTop(); 
			top = bottom = this.scrollTop();
			left += rect.left;
			right += rect.right;
			top += rect.top;
			bottom += rect.bottom;
		};
		return { "left": left, "top": top, "right": right, "bottom": bottom };
	},
	clientRect: function() {//获取相对于客户端窗口的位置 非实际位置
	    var node=this[0];
		var rect = this.rect(node), sLeft = this.scrollLeft(), sTop = this.scrollTop();
		rect.left -= sLeft;
		rect.right -= sLeft;
		rect.top -= sTop;
		rect.bottom -= sTop;
		return rect;
	},
	currentStyle:document.defaultView//获取真实的style
	    ? function(){ 
	        return document.defaultView.getComputedStyle(this[0], null);
	    }
	    : function(){
	        return this[0].currentStyle;
	    }
	,
	css:function(){
	    var elem=this[0];
	    if(arguments.length==1&&typeof arguments[0]=="string"){//获取style
	        var name=arguments[0];
	        if(document.defaultView){
			    var style = document.defaultView.getComputedStyle(elem, null);
			    return name in style ? style[ name ] : style.getPropertyValue( name );
		    }
		    else{
			    var curstyle = elem.currentStyle;//IE提供currentStyle和runtimeStyle
			    //IE的透明滤镜采用filter
			    if (name == "opacity") {
				    if ( /alpha\(opacity=(.*)\)/i.test(curstyle.filter) ) {
					    var opacity = parseFloat(RegExp.$1);
					    return opacity ? opacity / 100 : 0;
				    }
				    return 1;
			    };
			    if (name == "float") { name = "styleFloat"; }
			    var ret = curstyle[ name ] || curstyle[ easy.strToCamel( name ) ];
			    //单位转换为像素
			    if ( !/^\-?\d+(px)?$/i.test( ret ) && /^\-?\d/.test( ret ) ) {
				    var style = elem.style;
				    var left = curstyle.left;
				    var rsLeft = elem.runtimeStyle.left;
				    elem.runtimeStyle.left = curstyle.left;//设置运行时样式 比style优先级高
				    style.left = ret || 0;
				    ret = style.pixelLeft + "px";//获取像素单位
				    style.left = left;//left改回原属性
				    elem.runtimeStyle.left = rsLeft;
			    }
			    return ret;
		    }
		}
		else if(arguments.length==2){//设置一个style属性
		    var name=arguments[0];
		    var value=arguments[1];
		    easy.setStyle(elem,name,value);
		}
		else if(typeof arguments[0]=="object"){//通过遍历对象设置style
		    var obj=arguments[0];
		    for(var name in obj){
		        easy.setStyle(elem,name,obj[name]);
		    }
		}
	}
});


//常用静态方法
easy.extend({
    noConflict: function() {
	    window.$E = _$E;//"$E"符号给其他库用以消除冲突
    },
    trim:function(str){
        return str.replace(/(^\s*)|(\s*$)/g,"");
    },
    strToCamel: function(s){//css属性转换为"骆驼"格式
		return s.replace(/-([a-z])/ig, function(all, letter) { return letter.toUpperCase(); });
	},
    isFunction: function( obj ) {
		return Object.prototype.toString.call(obj) === "[object Function]";
	},
	isArray: function( obj ) {
		return Object.prototype.toString.call(obj) === "[object Array]";
	},
    //将字符串转换为element
    strToElement:function(strHtml){
        var el=document.createElement("span");
        el.innerHTML=strHtml;
        return el;
    },
    setStyle:function(elem,name,value){
        if (name == "opacity" && easy.ie) {
			elem.style.filter = (elem.currentStyle.filter || "").replace( /alpha\([^)]*\)/, "" ) +
				"alpha(opacity=" + value * 100 + ")";
		} else if (name == "float") {
			elem.style[ easy.browser.ie ? "styleFloat" : "cssFloat" ] = value;
		} else {
			elem.style[ easy.strToCamel( name ) ] = value;
		}
    }
});


//浏览器验证
easy.extend({
    browser:(function(ua){
        var b = {
		    ie: /msie/.test(ua) && !/opera/.test(ua),
		    opera: /opera/.test(ua),
		    safari: /webkit/.test(ua) && !/chrome/.test(ua),
		    firefox: /firefox/.test(ua),
		    chrome: /chrome/.test(ua)
	    };
	    var vMark = "";
	    for (var i in b) {
		    if (b[i]) { vMark = "safari" == i ? "version" : i; break; }
	    }
	    b.version = vMark && RegExp("(?:" + vMark + ")[\\/: ]([\\d.]+)").test(ua) ? RegExp.$1 : "0";
	    b.ie6 = b.ie && parseInt(b.version, 10) == 6;
	    b.ie7 = b.ie && parseInt(b.version, 10) == 7;
	    b.ie8 = b.ie && parseInt(b.version, 10) == 8;
	    return b;
    })(window.navigator.userAgent.toLowerCase())
});


//统一event对象的操作
easy.extend({
    event:(function(){
        var EventUtil ={};
        EventUtil.add = function (oTarget, sEventType, fnHandler) {
            if (oTarget.addEventListener) {
                oTarget.addEventListener(sEventType, fnHandler, false);
            } else if (oTarget.attachEvent) {
                oTarget.attachEvent("on" + sEventType, fnHandler);
            } else {
                oTarget["on" + sEventType] = fnHandler;
            }
        };
        EventUtil.remove = function (oTarget, sEventType, fnHandler) {
            if (oTarget.removeEventListener) {
                oTarget.removeEventListener(sEventType, fnHandler, false);
            } else if (oTarget.detachEvent) {
                oTarget.detachEvent("on" + sEventType, fnHandler);
            } else { 
                oTarget["on" + sEventType] = null;
            }
        };
        EventUtil.formatEvent = function (oEvent) {
            if (window.event) {
                oEvent.keyCode = (oEvent.type == "keypress") ? oEvent.keyCode : 0;
                oEvent.eventPhase = 2;
                oEvent.isChar = (oEvent.charCode > 0);
                oEvent.pageX = oEvent.clientX + Math.max(document.body.scrollLeft,document.documentElement.scrollLeft);
                oEvent.pageY = oEvent.clientY + Math.max(document.body.scrollTop,document.documentElement.scrollLeft);
                oEvent.preventDefault = function () {
                    this.returnValue = false;
                };
                if (oEvent.type == "mouseout") {
                    oEvent.relatedTarget = oEvent.toElement;
                } else if (oEvent.type == "mouseover") {
                    oEvent.relatedTarget = oEvent.fromElement;
                }
                oEvent.stopPropagation = function () {
                    this.cancelBubble = true;
                };
                oEvent.target = oEvent.srcElement;
            }
            return oEvent;
        };
        EventUtil.getEvent = function() {
            if (window.event) {
                return this.formatEvent(window.event);
            } else {
                if(this.getEvent.caller.caller)
                    return this.getEvent.caller.caller.arguments[0];//html元素中绑定的事件要从caller.caller返回带event参数的事件函数
                if(this.getEvent.caller)
                    return this.getEvent.caller.arguments[0];//代码绑定的事件从caller返回
            }
        };
        return EventUtil;
    })()
});
//扩展事件绑定
easy.fn.extend({
    bind:function(type,fun){
        easy.event.add(this[0],type,fun);
    },
    unbind:function(type,fun){
        easy.event.remove(this[0],type,fun);
    }
});


//统一XMLDOM对象的属性及方法
if (!window.ActiveXObject){
    if(!easy.browser.firefox){
        XMLDocument.prototype.readyState=0;//chrome无此属性
        XMLDocument.prototype.onreadystatechange = null;
    }
    XMLDocument.prototype.__changeReadyState__ = function (iReadyState) {
        if(!easy.browser.firefox){
            this.readyState=iReadyState;
        }
        if (typeof this.onreadystatechange == "function" && iReadyState==4) {
            this.onreadystatechange();
        }
    };
    //初始化parseError对象
    XMLDocument.prototype.__initError__ = function () {
        this.parseError.errorCode = 0;
        this.parseError.filepos = -1;
        this.parseError.line = -1;
        this.parseError.linepos = -1;
        this.parseError.reason = null;
        this.parseError.srcText = null;
        this.parseError.url = null;
    };
    XMLDocument.prototype.__checkForErrors__ = function () {
        //存在parsererror元素时代表载入错误 表达式左边检测chrome 右边检测firefox ,具体的正则匹配 需要进一步修改
        if (this.documentElement.getElementsByTagName("parsererror").length!=0 || this.documentElement.tagName=="parsererror") {
            var reError = />([\s\S]*?)Location:([\s\S]*?)Line Number (\d+), Column (\d+):<sourcetext>([\s\S]*?)(?:\-*\^)/;
            reError.test(this.xml);
            this.parseError.errorCode = -999999;
            this.parseError.reason = RegExp.$1;
            this.parseError.url = RegExp.$2;
            this.parseError.line = parseInt(RegExp.$3);
            this.parseError.linepos = parseInt(RegExp.$4);
            this.parseError.srcText = RegExp.$5;
        }
    };
     //定义非IE版本的loadXML方法   
    XMLDocument.prototype.loadXML = function (sXml) {
        this.__initError__();
        this.__changeReadyState__(1);
        var oParser = new DOMParser();
        var oXmlDom = oParser.parseFromString(sXml, "text/xml");
        while (this.firstChild) {
            this.removeChild(this.firstChild);
        }
        for (var i=0; i < oXmlDom.childNodes.length; i++) {
            var oNewNode = this.importNode(oXmlDom.childNodes[i], true);
            this.appendChild(oNewNode);
        }
        //载入后检查错误
        this.__checkForErrors__();
        this.__changeReadyState__(4);
    };
    //firefox chrome 没有load方法 采用ajax+loadxml的方法代替
    XMLDocument.prototype.load = function (sURL) {
        this.__initError__();
        var pthis=this;
        easy.get(sURL,function(data){
            pthis.loadXML(data);
        });
    };
    //为非IE浏览器添加xml属性
    Node.prototype.__defineGetter__("xml", function () {
        var oSerializer = new XMLSerializer();
        return oSerializer.serializeToString(this, "text/xml");
    });
    Node.prototype.__defineGetter__("text",function(){
        if(this.documentElement)//未获取根节点时获得所有text
            return this.documentElement.textContent;
        else
            return this.textContent;
    });
}
//扩展XMLDOM的操作
easy.extend({
    //XMLDOM对象构造方法
    createXMLDocument:function(){
        if(window.ActiveXObject){
            var arrSignatures = ["MSXML2.DOMDocument.5.0", "MSXML2.DOMDocument.4.0",
                                 "MSXML2.DOMDocument.3.0", "MSXML2.DOMDocument",
                                 "Microsoft.XmlDom"];
            for (var i=0; i < arrSignatures.length; i++) {
                try{
                    var oXmlDom = new ActiveXObject(arrSignatures[i]);
                    return oXmlDom;
                }catch (oError) {
                    //ignore
                }
            }
            throw new Error("MSXML is not installed on your system.");
        }
        else if(document.implementation && document.implementation.createDocument){
            var oXmlDom = document.implementation.createDocument("","",null);
            //创建非IE版本的parseError对象
            oXmlDom.parseError = {
                valueOf: function () { return this.errorCode; },
                toString: function () { return this.errorCode.toString() }
            };
            return oXmlDom;
        }
        else{
            throw new Error("Your browser doesn't support an XML DOM object.");
        }
    },
    //从字符串中创建XMLdom对象
    createXMLDOM:function(htmlStr,fun,errorfun){
        var xmlDoc=easy.createXMLDocument();
        xmlDoc.loadXML(htmlStr);
        if(xmlDoc.parseError.errorCode!=0&&easy.isFunction(errorfun))
            errorfun();
        if(easy.isFunction(fun)){
            fun.call(xmlDoc);
        }
        return xmlDoc;
    },
    //从xml文档创建XMLDOM对象 由于采用ajax+loadxml的方式 错误验证需要在onreadystatechange中处理
    loadXMLDOM:function(url,fun,errorfun){
        var xmlDoc=easy.createXMLDocument();
        if(easy.isFunction(fun)){
            if(window.ActiveXObject){
                xmlDoc.onreadystatechange=function(){
                    if(xmlDoc.readyState==4){
                        if(xmlDoc.parseError.errorCode!=0&&easy.isFunction(errorfun))
                            errorfun.call(xmlDoc);
                        fun.call(xmlDoc);
                    }
                };
            }
            else{
                xmlDoc.onreadystatechange=function(){//除IE外的浏览器的readyState判断在prototype中经过处理
                    if(xmlDoc.parseError.errorCode!=0&&easy.isFunction(errorfun))
                            errorfun.call(xmlDoc);
                    fun.call(xmlDoc);
                }
            }
        }
        xmlDoc.load(url);
        return xmlDoc;
    }
});


//扩展DOM操作 append before 为关键方法
easy.fn.extend({
    empty:function(){
        var el=this[0];
        while (el.firstChild )
		    el.removeChild( el.firstChild );
		return this;
    },
    html:function(content){
        if(content==undefined)
            return this[0].innerHTML;
        this.empty().append(content);
    },
    text:function(content){
        if(content==undefined)
            return this[0].innerText;
        this[0].innerText=content;
    },
    prepend:function(content){
        var el=easy.strToElement(content);
        var firstNode=this[0].firstChild;
        easy(firstNode).before(content);
    },
    append:function(content){
        if(content.nodeType){//content为元素时
            this[0].appendChild(content);
        }
        else{
            var el=easy.strToElement(content);//返回一个用span包围content的元素
            while(el.firstChild){
                this[0].appendChild(el.firstChild);
            }
        }
    },
    before:function(content){
        var parent=this[0].parentNode;
        if(content.nodeType){
            parent.insertBefore(content,this[0]);
        }
        else{
            var el=easy.strToElement(content);
            while(el.firstChild){
                parent.insertBefore(el.firstChild,this[0]);
            }
        }
    },
    after:function(content){
        var nextNode=this[0].nextSibling;
        if(nextNode){
            easy(nextNode).before(content);
        }
        else{
            easy(this[0].parentNode).append(content);
        }
    }
});

//扩展ajax操作
easy.extend({
    createAjax:function(){
        if (typeof XMLHttpRequest == "undefined" && window.ActiveXObject){
            var arrSignatures = ["MSXML2.XMLHTTP.5.0", "MSXML2.XMLHTTP.4.0",
                                 "MSXML2.XMLHTTP.3.0", "MSXML2.XMLHTTP",
                                 "Microsoft.XMLHTTP"];                         
            for (var i=0; i < arrSignatures.length; i++) {
                try {      
                    var oRequest = new ActiveXObject(arrSignatures[i]);    
                    return oRequest;    
                } catch (oError) {
                    //ignore
                }
            }          
            throw new Error("MSXML is not installed on your system.");
        }
        else{
            return new XMLHttpRequest();
        }
    },
    addPostParam:function(sParams, sParamName, sParamValue){
        if (sParams.length > 0) {
            sParams += "&";
        }
        return sParams + encodeURIComponent(sParamName) + "=" 
                       + encodeURIComponent(sParamValue);
    },
    addURLParam:function(sURL, sParamName, sParamValue) {
        sURL += (sURL.indexOf("?") == -1 ? "?" : "&");
        sURL += encodeURIComponent(sParamName) + "=" + encodeURIComponent(sParamValue);
        return sURL;   
    },
    //参数为 url,para,fun 或url,fun
    get:function(){
        var sURL=arguments[0],fnCallback;
        if(arguments.length==3){
            if(typeof arguments[1]=="object"){
                var obj=arguments[1];
                for(var property in obj){
                    sURL=easy.addURLParam(sURL,property,obj[property]);
                }
            }
            fnCallback=arguments[2];
        }
        else {
            fnCallback=arguments[1];
        }
        var oRequest=easy.createAjax();
        oRequest.open("get", sURL,true);
        oRequest.onreadystatechange = function () {
        if (oRequest.readyState == 4) {
           	    fnCallback(oRequest.responseText);
            }
        }
        oRequest.send(null);
    },
    post:function (sURL, objParams, fnCallback) {
        var sParams="";
        if(typeof objParams=="object"){
            for(var property in objParams){
                sParams=easy.addPostParam(sParams,property,objParams[property]);
            }
        }
	    var oRequest=easy.createAjax();
        oRequest.open("post", sURL, true);
        oRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        oRequest.onreadystatechange = function(){
            if(oRequest.readyState == 4){
           	    fnCallback(oRequest.responseText);
            }
        }
        oRequest.send(sParams);
    }
});
window.easy=window.$E=easy;
})();

//图片预览 兼容IE-FF
function PreviewImg(imgFile,imgPreviewDiv,tr){
    if(navigator.userAgent.indexOf("Firefox")==-1 && !window.ActiveXObject){
        return;
    }
    if(!checkImage(imgFile.id)){
        document.getElementById(tr).style.display="none";
        return;
    }
    var newPreview = document.getElementById(imgPreviewDiv);
    newPreview.innerHTML="";
    var sWidth="415px",sHeight="120px";
    if(!imgFile.files){
        imgFile.select();
        var imgSrc =document.selection.createRange().text;
        document.selection.empty();
        newPreview.style.filter="progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=scale)";
        newPreview.filters.item("DXImageTransform.Microsoft.AlphaImageLoader").src=imgSrc;
        newPreview.style.width = sWidth; 
        newPreview.style.height = sHeight;
    }
    else{
        var img=new Image();
        img.src=imgFile.files[0].getAsDataURL();
        img.style.height=sHeight;
        img.style.width=sWidth;
        newPreview.appendChild(img);
    }
    document.getElementById(tr).style.display="";
}
function checkImage(fileid){
    var file=document.getElementById(fileid); 
    var i=file.value.lastIndexOf("."); 
    var ext=file.value.substring(i).toLowerCase();
    if(ext==""){
        alert("请选择图片")
        return false;
    }
    if(ext!=".gif" && ext!=".jpg" && ext!=".jpeg"&&ext!=".bmp"&&ext!=".png")
    {
        alert(ext+"为不支持的图片格式"); 
        return false; 
    }
    return true;
}

function writeCookie(name, value, hours)
{
    var expire = "";
    if(hours != null)
    {
        expire = new Date(new Date().getTime() + hours * 3600000);
        expire = "; expires=" + expire.toGMTString();   
    }
    document.cookie = name + "=" + escape(value) + expire;   
}   
  
function readCookie(name)   
{   
    var cookieValue = "";   
    var search = name + "=";   
    if(document.cookie.length > 0)   
    {
      offset = document.cookie.indexOf(search);   
      if (offset != -1)   
      {    
        offset += search.length;
        end = document.cookie.indexOf(";", offset);
        if (end == -1) end = document.cookie.length;   
        cookieValue = unescape(document.cookie.substring(offset, end))   
      }
    }
    return cookieValue;
}