// author: lcomplete

Object.extend = function (destination, source) {
    for (var property in source) destination[property] = source[property];
    return destination;
};

Function.prototype.bind = function (object) {
    var __method = this;
    return function () {
        return __method.apply(object, arguments);
    };
};
Object.extend(Function.prototype, {
    getArguments: function () {
        var args = [];
        for (var i = 0; i < this.arguments.length; i++) {
            args.push(this.arguments[i]);
        }
        return args;
    }
});

if (!Array.prototype.forEach) {
    Array.prototype.forEach = function (fn, bind) {
        for (var i = 0; i < this.length; i++) fn.call(bind, this[i], i);
    };
}
Array.prototype.each = Array.prototype.forEach;

if (typeof Iridescent == "undefined")
    Iridescent = {};

Iridescent.Tools = {
    isFunction: function (obj) {
        return Object.prototype.toString.call(obj) === "[object Function]";
    },
    isArray: function (obj) {
        return Object.prototype.toString.call(obj) === "[object Array]";
    },
    trim: function (str) {
        return str.replace(/(^\s*)|(\s*$)/g, "");
    }
};

Iridescent.Ajax = {
    createXHR: function () {
        if (typeof XMLHttpRequest == "undefined" && window.ActiveXObject) {
            var arrSignatures = ["MSXML2.XMLHTTP.5.0", "MSXML2.XMLHTTP.4.0",
                                 "MSXML2.XMLHTTP.3.0", "MSXML2.XMLHTTP",
                                 "Microsoft.XMLHTTP"];
            for (var i = 0; i < arrSignatures.length; i++) {
                try {
                    var oRequest = new ActiveXObject(arrSignatures[i]);
                    return oRequest;
                } catch (oError) {
                    //ignore
                }
            }
            throw new Error("MSXML is not installed on your system.");
        }
        else {
            return new XMLHttpRequest();
        }
    },
    addPostParam: function (sParams, sParamName, sParamValue) {
        if (sParams.length > 0) {
            sParams += "&";
        }
        return sParams + encodeURIComponent(sParamName) + "="
                       + encodeURIComponent(sParamValue);
    },
    post: function (sURL, method, objParams, fnCallbackArray) {
        var sParams = "";
        if (typeof objParams == "object") {
            for (var property in objParams) {
                sParams = Iridescent.Ajax.addPostParam(sParams, property, objParams[property]);
            }
        }
        var oRequest = Iridescent.Ajax.createXHR();
        var async = Iridescent.Tools.isFunction(fnCallbackArray) || (Iridescent.Tools.isArray(fnCallbackArray) && fnCallbackArray.length > 0 && Iridescent.Tools.isFunction(fnCallbackArray[0]));
        oRequest.open("post", sURL, async);
        oRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        oRequest.setRequestHeader("X-IridescentAjax-Method", method);
        if (async)
            oRequest.onreadystatechange = function () {
                if (oRequest.readyState == 4) {
                    var callback = Iridescent.Tools.isFunction(fnCallbackArray) ? fnCallbackArray : fnCallbackArray[0];
                    callback(oRequest.responseText);
                }
            };
        oRequest.send(sParams);
        if (!async)
            return oRequest.responseText;
    }
};

Iridescent.AjaxClass = function (url) {
    this.url = url;
};
Iridescent.AjaxClass.prototype = {
    invoke: function (method, objParams, callback) {
        return Iridescent.Ajax.post(this.url, method, objParams, callback);
    }
};