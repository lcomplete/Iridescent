/*!
 * write by louchen
 */

var easy=window.easy||{};
easy.layer=function(options,deep){
    this.id="easy_layer_div_"+(++easy.layer.__layerNumber);
    this.contentBgFrameId="easy_layer_bg_content_iframe"+easy.layer.__layerNumber;
    var opt={};
    $.extend(true,opt,easy.layer.defaultOptions);
    this.options=opt;
    this.setOptions(options,deep);
    this._init();
};
easy.layer.__bgIframeId="easy_layer_bg_iframe";
easy.layer.__bgDivId="easy_layer_bg_shade";
easy.layer.__layerNumber=0;

//触发类型
easy.layer.triggerType={

    hover:function(){
    
        var trigger=this.options.trigger;
        var wrapperId=this.getId();
        var src=this;
        var over=false;
        var curTrigger;
        var timeoutId;
        $("#"+wrapperId).hover(function(){
            clearTimeout(timeoutId);
            over=true;
        },function(){
            over=false;
            timeoutId= setTimeout(close,src.options.closeTimeout);
        });
        
        $(trigger).hover(function(){
            clearTimeout(timeoutId);
            over=true;
            if(curTrigger==this && src.alreadyShow){
                return;
            }
            src.options.positionType.call(src,this);
            src.show(this);
            curTrigger=this;
        },function(){
            over=false;
            timeoutId= setTimeout(close,src.options.closeTimeout);
        });
        function close(){
            if(!over)
                src.hide();
        }
        
    },
    
    click:function(){
    
        var trigger=this.options.trigger;
        var wrapperId=this.getId();
        var src=this;
        var curTrigger;
        
        $(trigger).click(function(e){
            src.options.positionType.call(src,this);
            src.show(this);
        });        
        
    }
    
};

//位置类型
easy.layer.positionType={
    center:function(){
        var wrapper=$("#"+this.getId());
        var optOffset=this.options.offset;
        var top=$(window).scrollTop()+$(window).height()/2;
        var pos={
            position:"absolute",
            left:"50%",
            top:top,
            marginLeft:-(wrapper.width()/2)+optOffset.x,
            marginTop:-(wrapper.height()/2)+optOffset.y
        };
        wrapper.css(pos);
        
        if($.ie6()){
            $("#"+this.contentBgFrameId).css(pos).css({
                width:wrapper.width(),
                height:wrapper.height()
            });
        }
    },
    margin:function(trigger){
        if(!trigger)
            return;
        trigger=trigger||this.options.trigger;
        var wrapper=$("#"+this.getId());
        var trigger=$(trigger);
        var optOffset=this.options.offset;
        var triggerOffset=trigger.offset();
        var pos={
            position:"absolute",
            left:triggerOffset.left+optOffset.x,
            top:triggerOffset.top+optOffset.y
        };
        wrapper.css(pos);
        
        if($.ie6()){
            $("#"+this.contentBgFrameId).css(pos).css({
                width:wrapper.outerWidth(),
                height:wrapper.outerHeight()
            });
        }
    }
};

//显示动作
easy.layer.showAction={
    normal:function(){
        $("#"+this.getId()).show();
    },
    fade:function(){
        $("#"+this.getId()).fadeIn(this.options.actionSpeed);
    },
    slideV:function(){
        $("#"+this.getId()).slideDown(this.options.actionSpeed);
    },
    expand:function(){
        $("#"+this.getId()).show(this.options.actionSpeed);
    }
};

//隐藏动作
easy.layer.hideAction={
    normal:function(){
        $("#"+this.getId()).hide();
    },
    fade:function(){
        $("#"+this.getId()).fadeOut(this.options.actionSpeed);
    },
    slideV:function(){
        $("#"+this.getId()).slideUp(this.options.actionSpeed);
    },
    expand:function(){
        $("#"+this.getId()).hide(this.options.actionSpeed);
    }
};

easy.layer.defaultOptions={
    trigger:"",//触发元素选择器
    content:'',
    src:"",
    loaded:false,
    cache:true,
    title:'',
    closeText:"",
    isModal:false,
    shadeColor:"#000000",
    opacity:0.3,
    
    outClass:"",
    titleClass:"",
    contentClass:"",
    closeClass:"",
    
    style:{
        position:"absolute",
        backgroundColor:"#f0f0f0",
        border:"5px solid #ddd",
        overflow:"hidden",
        width:300,
        height:120
    },
    titleStyle:{},
    closeStyle:{},
    contentStyle:{},
    
    triggerType:easy.layer.triggerType.hover,
    positionType:easy.layer.positionType.margin,
    showAction:easy.layer.showAction.normal,
    hideAction:easy.layer.hideAction.normal,
    actionSpeed:600,
    closeTimeout:1000,
    isBgClose:true,
    offset:{
        x:0,
        y:0
    },
    onshowing:function(){},//显示层之前调用 函数内可使用this引用easy.layer,可用第一个参数引用trigger
    onclosing:function(){}
};

easy.layer.prototype = {

    setOptions: function (options, deep) {
        if (typeof deep == "boolean" && !deep) {
            $.extend(this.options, options || {});
            return;
        }
        $.extend(true, this.options, options || {}); //默认为深度复制
    },
    _init: function () {

        if (!!this._hasInit) {
            return;
        }

        this._createContentBgIframe();

        var opt = this.options;
        if (opt.isModal) {
            this._createShade();
            var src = this;
            if (opt.isBgClose) {
                $("#" + easy.layer.__bgDivId).click(function () {
                    src.hide();
                    alreadyShow = false;
                });
            }
        }

        var wrapperDiv = $("<div>"); //外层div
        wrapperDiv.attr("id", this.getId());
        wrapperDiv.css(opt.style);
        wrapperDiv.css("zIndex", $.getMaxIndex());
        wrapperDiv.addClass(opt.outClass);

        if (!!opt.closeText) {//关闭div
            var src = this;
            var closeDiv = $("<div>");
            closeDiv.attr("id", this.getCloseId());
            closeDiv.css(opt.closeStyle);
            closeDiv.addClass(opt.closeClass);
            closeDiv.html(opt.closeText);
            closeDiv.click(function () {
                src.hide();
            });
            closeDiv.appendTo(wrapperDiv);
        }

        if (opt.title != "") {//标题div
            var titleDiv = $("<div>");
            titleDiv.attr("id", this.getTitleId());
            titleDiv.css(opt.titleStyle);
            titleDiv.addClass(opt.titleClass);
            titleDiv.html(opt.title);
            titleDiv.appendTo(wrapperDiv);
        }

        var contentDiv = $("<div>"); //内容div
        contentDiv.attr("id", this.getContentId());
        contentDiv.css(opt.contentStyle);
        contentDiv.addClass(opt.contentClass);

        if (this.isIframeContent()) {
            var iframe = this.createIframe();
            iframe.appendTo(contentDiv);
        }
        else
            contentDiv.html($(opt.content));

        contentDiv.appendTo(wrapperDiv);
        wrapperDiv.hide().appendTo($(document.body)); //添加到body
        this._resizeIframe();

        if (this.hasTrigger())
            opt.triggerType.apply(this); //绑定触发器

        this._hasInit = true;

    },
    getSize: function () {
        return {
            width: this.options.style.width,
            height: this.options.style.height
        };
    },
    setWidth: function (width) {
        this.options.style.width = width;
        this.getWrapper().width(width);
        this._resizeIframe();
    },
    setHeight: function (height) {
        this.options.style.height = height;
        this.getWrapper().height(height);
        this._resizeIframe();
    },
    _resizeIframe: function () {
        var iframe = this.getIframe();
        if (iframe != null) {
            var size = this.getSize();
            iframe.css({
                width: size.width,
                height: size.height + (!!this.options.titleStyle.height ? -this.options.titleStyle.height : 0)
            });
        }
    },
    _showContentBgFrame: function () {
        if ($.ie6()) {
            $("#" + this.contentBgFrameId).show();
        }
    },
    _hideContentBgFrame: function () {
        if ($.ie6()) {
            $("#" + this.contentBgFrameId).hide();
        }
    },
    _createContentBgIframe: function () {
        if ($.ie6()) {
            var iframe = $("<iframe>");
            iframe.css({
                position: "absolute",
                opacity: 0,
                display: "none"
            });
            iframe.attr({
                id: this.contentBgFrameId,
                frameborder: 0
            });
            iframe.appendTo($("body"));
        }
    },
    //创建遮罩层
    _createShade: function () {

        var __hiddenIframeId = easy.layer.__bgIframeId;
        if ($("#" + __hiddenIframeId).length == 0) {
            var opt = this.options;
            var maxIndex = $.getMaxIndex();

            var iframe = $("<iframe>");
            iframe.css({
                position: "absolute",
                top: 0,
                left: 0,
                zIndex: maxIndex++,
                opacity: 0,
                display: "none"
            });
            iframe.attr({
                id: __hiddenIframeId,
                frameborder: 0
            });
            iframe.appendTo($("body"));

            var bgDiv = '<div id="' + easy.layer.__bgDivId + '" style="background:' + opt.shadeColor + '; filter:alpha(opacity=' + (opt.opacity * 100) + '); opacity: ' + opt.opacity + '; z-index:' + maxIndex + '; position:absolute; left:0px; top:0px;display:none;"></div>';
            $("body").append(bgDiv);

            $(window).resize(function () {
                var bgIframe = $("#" + easy.layer.__bgIframeId);
                if (bgIframe.length == 0)
                    return;

                var bw = $(window).width();
                var bh = $(document.body).height() > $(window).height() ? $(document).height() : $(window).height();

                bgIframe.width(bw);
                bgIframe.height(bh);
                var elBgDiv = $("#" + easy.layer.__bgDivId);
                elBgDiv.width(bw);
                elBgDiv.height(bh);
            });

            $(window).resize();
        }

    },
    _showShade: function () {
        var bh = $(document.body).height() > $(window).height() ? $(document).height() : $(window).height();
        $("#" + easy.layer.__bgDivId).height(bh);
        $("#" + easy.layer.__bgDivId).show();
        $("#" + easy.layer.__bgIframeId).show();
    },
    _hideShade: function () {
        $("#" + easy.layer.__bgDivId).hide();
        $("#" + easy.layer.__bgIframeId).hide();
    },
    isIframeContent: function () {
        return !!this.options.src;
    },
    createIframe: function () {
        var iframe = $("<iframe>");
        var opt = this.options;
        iframe.attr({
            frameborder: 0,
            allowtransparency: true
        });
        if (opt.loaded) {
            iframe.attr("src", opt.src + (opt.cache ? "" : (/\?/.test(opt.src) ? "&_=" : "?_=") + Math.random()));
        }
        return iframe;
    },
    getIframe: function () {
        if (this.isIframeContent()) {
            return this.getWrapper().find("iframe");
        }
        return null;
    },
    setTitle: function (html) {
        this.options.title = html;
        $("#" + this.getTitleId()).html(html);
    },
    setContent: function (html) {
        this.options.content = html;
        $("#" + this.getContentId()).html(html);
        this.options.src = "";
    },
    getId: function () {
        return this.id;
    },
    getCloseId: function () {
        return this.getId() + "_close";
    },
    getTitleId: function () {
        return this.getId() + '_title';
    },
    getContentId: function () {
        return this.getId() + "_content";
    },
    getTriggers: function () {
        if (this.hasTrigger) {
            return $(this.options.trigger);
        }
    },
    getWrapper: function () {
        return $("#" + this.getId());
    },
    //是否提供了触发器
    hasTrigger: function () {
        return !!this.options.trigger;
    },
    setSrc: function (src) {
        var isIframe = this.isIframeContent();
        this.options.src = src;
        if (!isIframe) {
            $("#" + this.getContentId()).html(this.createIframe());
            if (this.options.loaded)
                return;
        }
        this.reload();
    },
    reload: function () {
        var iframe = this.getIframe();
        if (iframe != null) {
            var opt = this.options;
            iframe.attr("src", opt.src + (opt.cache ? "" : (/\?/.test(opt.src) ? "&_=" : "?_=") + Math.random()));
            this.options.loaded = true;
        }
    },
    show: function (trigger) {
        if (!!trigger)
            this.lastTrigger = trigger;
        if (typeof this.options.onshowing == "function")
            this.options.onshowing.call(this, trigger); //显示之前的处理

        if (!this.options.loaded)
            this.reload();
        if (this.alreadyShow)
            return;
        this.options.positionType.call(this, trigger); //定位层
        if (this.options.isModal) {
            this._showShade();
        }

        this._showContentBgFrame();
        this.options.showAction.apply(this); //显示层
        this.alreadyShow = true; //显示标记
    },
    hide: function () {
        if (!this.alreadyShow)
            return;
        if (this.options.isModal) {
            this._hideShade();
        }
        if (typeof this.options.onshowing == "function")
            this.options.onclosing.call(this);

        this._hideContentBgFrame();
        this.options.hideAction.apply(this); //隐藏层
        this.alreadyShow = false;
    },
    //去除事件绑定
    unbind: function (type) {
        var trigger = this.options.trigger;
        var wrapperId = this.getId();
        $(trigger).unbind(type);
        $("#" + wrapperId).unbind(type);
    },
    //清除使用资源
    abandon: function () {
        $("#" + this.getId()).remove();
    }
};

(function($){

$.extend({
    getMaxIndex:function(){ 
        var index = 0; 
        $.each($("*"),function(i,n){ 
            var tem = $(n).css("z-index"); 
            if(tem>0) 
            if(tem > index){
                index =tem + 1; 
            }
        });
        return index;
    },
    ie6:function(){
        return $.browser.msie && $.browser.version=="6.0";
    }
});

})(jQuery);
