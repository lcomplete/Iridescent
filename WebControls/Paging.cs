using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Iridescent.WebControls
{
    /// <summary>
    /// 分页控件
    /// </summary>
    [DefaultValue("RecordCount")]
    [ToolboxData("<{0}:Paging runat=server></{0}:Paging>")]
    public class Paging : WebControl, IPostBackEventHandler
    {
        private string _linkHrefWithOutPageIndex;

        #region Style

        private string _linkStyle;
        [Category("Style")]
        public string LinkStyle
        {
            get { return _linkStyle; }
            set { _linkStyle = value; }
        }

        private string _currentLinkStyle;
        [Category("Style")]
        public string CurrentLinkStyle
        {
            get { return _currentLinkStyle; }
            set { _currentLinkStyle = value; }
        }

        private string _disableLinkStyle;
        [Category("Style")]
        public string DisableLinkStyle
        {
            get { return _disableLinkStyle; }
            set { _disableLinkStyle = value; }
        }

        #endregion

        #region Class

        private string _firstLinkClass;
        private string _lastLinkClass;
        private string _prevLinkClass;
        private string _nextLinkClass;
        private string _linkClass;

        [Category("Class")]
        public string LinkClass
        {
            get { return _linkClass; }
            set { _linkClass = value; }
        }
        private string _currentLinkClass;
        [Category("Class")]
        public string CurrentLinkClass
        {
            get { return _currentLinkClass; }
            set { _currentLinkClass = value; }
        }
        private string _disableLinkClass;
        [Category("Class")]
        public string DisableLinkClass
        {
            get { return _disableLinkClass; }
            set { _disableLinkClass = value; }
        }
        [Category("Class")]
        public string FirstLinkClass
        {
            get { return _firstLinkClass; }
            set { _firstLinkClass = value; }
        }
        [Category("Class")]
        public string LastLinkClass
        {
            get { return _lastLinkClass; }
            set { _lastLinkClass = value; }
        }
        [Category("Class")]
        public string PrevLinkClass
        {
            get { return _prevLinkClass; }
            set { _prevLinkClass = value; }
        }
        [Category("Class")]
        public string NextLinkClass
        {
            get { return _nextLinkClass; }
            set { _nextLinkClass = value; }
        }

        #endregion

        #region 显示相关

        private bool _renderSpanWrapA;
        public bool RenderSpanWrapA
        {
            get { return _renderSpanWrapA; }
            set { _renderSpanWrapA = value; }
        }

        private bool _showPrevNext;
        public bool ShowPrevNext
        {
            get { return _showPrevNext; }
            set { _showPrevNext = value; }
        }

        private bool _showFirstLast;
        public bool ShowFirstLast
        {
            get { return _showFirstLast; }
            set { _showFirstLast = value; }
        }

        private string _prevPageText;
        public string PrevPageText
        {
            get { return _prevPageText; }
            set { _prevPageText = value; }
        }

        private string _nextPageText;
        public string NextPageText
        {
            get { return _nextPageText; }
            set { _nextPageText = value; }
        }

        private string _firstPageText;
        public string FirstPageText
        {
            get { return _firstPageText; }
            set { _firstPageText = value; }
        }

        private string _lastPageText;
        public string LastPageText
        {
            get { return _lastPageText; }
            set { _lastPageText = value; }
        }

        private bool _showDotted;
        public bool ShowDotted
        {
            get { return _showDotted; }
            set { _showDotted = value; }
        }

        private bool _alwaysCenter;
        public bool AlwaysCenter
        {
            get { return _alwaysCenter; }
            set { _alwaysCenter = value; }
        }

        private bool _alwaysShow;
        public bool AlwaysShow
        {
            get { return _alwaysShow; }
            set { _alwaysShow = value; }
        }

        #endregion

        #region 数量相关

        [Description("使用ViewState属性来存储数据")]
        public int RecordCount
        {
            get { return ViewState["RecordCount"] == null ? 0 : (int)ViewState["RecordCount"]; }
            set { ViewState["RecordCount"] = value; }
        }
        [Description("使用ViewState属性来存储数据")]
        public int PageSize
        {
            get { return ViewState["PageSize"] == null ? 10 : (int)ViewState["PageSize"]; }
            set { ViewState["PageSize"] = value; }
        }
        [Description("使用ViewState属性来存储数据")]
        public int MaxPagingShow
        {
            get { return ViewState["MaxPagingShow"] == null ? 10 : (int)ViewState["MaxPagingShow"]; }
            set { ViewState["MaxPagingShow"] = value; }
        }
        public int PageCount
        {
            get
            {
                return (int)(Math.Ceiling((float)RecordCount / PageSize));
            }
        }

        #endregion

        #region 使用方式相关

        private bool _enabledPostBack;
        public bool EnabledPostBack
        {
            get { return _enabledPostBack; }
            set { _enabledPostBack = value; }
        }

        private string _urlParameter;
        public string UrlParameter
        {
            get { return _urlParameter; }
            set { _urlParameter = value; }
        }

        private bool _enabledControlState;
        public bool EnabledControlState
        {
            get { return _enabledControlState; }
            set { _enabledControlState = value; }
        }

        private string _urlPatterns;
        public string UrlPatterns
        {
            get { return _urlPatterns; }
            set { _urlPatterns = value; }
        }

        private bool _enabledUrlRewrite;
        public bool EnabledUrlRewrite
        {
            get { return _enabledUrlRewrite; }
            set { _enabledUrlRewrite = value; }
        }

        #endregion


        public int CurrentPageIndex
        {
            get
            {
                if (_enabledPostBack)
                {
                    return ViewState["CurrentPageIndex"] == null ? 1 : (int)ViewState["CurrentPageIndex"];
                }
                HttpContext context = HttpContext.Current;
                int pageindex;
                if (!DesignMode)
                {
                    if (int.TryParse(context.Request.QueryString[_urlParameter], out pageindex) && pageindex > 0)
                    {
                        return pageindex;
                    }
                }
                return 1;
            }
            set
            {
                if (_enabledPostBack)
                {
                    ViewState["CurrentPageIndex"] = value;
                    OnPageChanged(EventArgs.Empty);
                }
                else
                {
                    Page.Response.Redirect(GetPagingUrl(value));
                }
            }
        }

        public Paging()
        {
            _showFirstLast = true;
            _showPrevNext = true;
            _prevPageText = "上一页";
            _nextPageText = "下一页";
            _firstPageText = "首页";
            _lastPageText = "尾页";
            _showDotted = true;
            _alwaysCenter = true;
            _alwaysShow = true;
            _urlParameter = "page";
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            if ((!AlwaysShow && PageCount == 1) || RecordCount == 0)
                return;
            int iStartIndex = 1;//起始页索引
            if (AlwaysCenter)//当前页总是显示在中间时
            {
                if (PageCount > MaxPagingShow)//页总数大于显示的页数量时 起始页索引需要更改
                {
                    if (CurrentPageIndex + MaxPagingShow / 2 > PageCount)
                    {
                        iStartIndex = PageCount - MaxPagingShow + 1;
                    }
                    else
                    {
                        iStartIndex = Math.Max(CurrentPageIndex - MaxPagingShow / 2, 1);
                    }
                }
            }
            else//当前页不显示在中间时 更改起始页索引
            {
                iStartIndex = Math.Min(((int)(Math.Ceiling((float)CurrentPageIndex / MaxPagingShow)) - 1) * MaxPagingShow, Math.Max((PageCount - MaxPagingShow), 0)) + 1;
            }
            if (CurrentPageIndex != 1)
            {
                if (ShowFirstLast)
                {
                    AddClass(output, FirstLinkClass);
                    RenderLink(output, FirstPageText, 1);
                }
                if (ShowPrevNext)
                {
                    AddClass(output, PrevLinkClass);
                    RenderLink(output, PrevPageText, CurrentPageIndex - 1);
                }
            }
            else
            {
                if (ShowFirstLast)
                {
                    AddClass(output, FirstLinkClass);
                    RenderDisableSpan(output, FirstPageText);
                }
                if (ShowPrevNext)
                {
                    AddClass(output, PrevLinkClass);
                    RenderDisableSpan(output, PrevPageText);
                }
            }

            if (ShowDotted && iStartIndex != 1)
                RenderLink(output, "...", Math.Max(CurrentPageIndex - MaxPagingShow, 1));
            int iShowCount = Math.Min(PageCount, MaxPagingShow);
            for (int i = 0; i < iShowCount; i++)
            {
                int ihrefpage = iStartIndex + i;
                if (ihrefpage != CurrentPageIndex)
                    RenderLink(output, ihrefpage.ToString(), ihrefpage);
                else
                    RenderCurrentSpan(output, ihrefpage.ToString());
            }
            if (ShowDotted && iStartIndex + MaxPagingShow < PageCount)
                RenderLink(output, "...", Math.Min(CurrentPageIndex + MaxPagingShow, PageCount));

            if (CurrentPageIndex != PageCount && PageCount != 0)
            {
                if (ShowPrevNext)
                {
                    AddClass(output, NextLinkClass);
                    RenderLink(output, NextPageText, CurrentPageIndex + 1);
                }
                if (ShowFirstLast)
                {
                    AddClass(output, LastLinkClass);
                    RenderLink(output, LastPageText, PageCount);
                }
            }
            else
            {
                if (ShowPrevNext)
                {
                    AddClass(output, NextLinkClass);
                    RenderDisableSpan(output, NextPageText);
                }
                if (ShowFirstLast)
                {
                    AddClass(output, LastLinkClass);
                    RenderDisableSpan(output, LastPageText);
                }
            }
        }

        protected void AddClass(HtmlTextWriter output, string cssclass)
        {
            if (!string.IsNullOrEmpty(cssclass))
                output.AddAttribute(HtmlTextWriterAttribute.Class, cssclass);
        }

        protected override void LoadControlState(object savedState)
        {
            if (EnabledControlState)
                base.LoadViewState(savedState);

            base.LoadControlState(savedState);
        }

        protected override object SaveControlState()
        {
            if (EnabledControlState)
                return base.SaveViewState();

            return base.SaveControlState();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (EnabledControlState)
                this.Page.RegisterRequiresControlState(this);
        }

        protected void RenderLink(HtmlTextWriter output, string slinkhtml, int ihrefpage)
        {
            if (_renderSpanWrapA)
                output.RenderBeginTag(HtmlTextWriterTag.Span);

            if (!string.IsNullOrEmpty(LinkStyle))
            {
                output.AddAttribute(HtmlTextWriterAttribute.Style, LinkStyle);
            }
            if (!string.IsNullOrEmpty(LinkClass))
            {
                output.AddAttribute(HtmlTextWriterAttribute.Class, LinkClass);
            }
            MakeHrefAttributes(output, ihrefpage);
            output.RenderBeginTag(HtmlTextWriterTag.A);
            output.Write(slinkhtml);
            output.RenderEndTag();

            if (_renderSpanWrapA)
                output.RenderEndTag();
        }

        protected virtual void MakeHrefAttributes(HtmlTextWriter output, int ihrefpage)
        {
            output.AddAttribute(HtmlTextWriterAttribute.Href, GetPagingUrl(ihrefpage));
        }

        protected virtual string GetPagingUrl(int ihrefpage)
        {
            if (_enabledPostBack)//postback方式
            {
                return "javascript:" + Page.ClientScript.GetPostBackEventReference(this, ihrefpage.ToString()) + ";";
            }
            else if (string.IsNullOrEmpty(UrlPatterns))
            {
                string url = string.Empty;
                if (EnabledUrlRewrite)//使用url重写时 按请求的原始url组合链接
                    url = CombinationUrl(Page.Request.RawUrl, _urlParameter, ihrefpage.ToString());
                else
                    url = _linkHrefWithOutPageIndex + ihrefpage.ToString();

                return url;
            }
            else
            {
                return MakeUrlFromUrlPatterns(ihrefpage);
            }
        }

        public string CombinationUrl(string srcUrl, string param, string value)
        {
            string pair = param + "=" + value;
            if (srcUrl.IndexOf("?") == -1)
                return srcUrl + "?" + pair;
            Regex regMatchParamOrEmpty = new Regex(@"(?<=^[^?]+\?|&)" + param + @"(=[^&]*)?|(?<=^[^?]+\?)$", RegexOptions.IgnoreCase);

            bool isMatch = false;
            srcUrl = regMatchParamOrEmpty.Replace(srcUrl, new MatchEvaluator(match =>
            {
                isMatch = true;
                return pair;
            }));
            if (!isMatch)
                srcUrl += "&" + pair;

            return srcUrl;
        }

        protected virtual string MakeUrlFromUrlPatterns(int ihrefpage)
        {
            return Regex.Replace(Page.Request.RawUrl, UrlPatterns, ihrefpage.ToString(), RegexOptions.IgnoreCase);
        }

        public string GetLinkHrefWithOutPageIndex()
        {
            if (!DesignMode)
            {
                string sUrl = Page.Request.Path;

                string sQueryPage = _urlParameter + "=";
                string sQuery = string.Empty;
                NameValueCollection nvRawUrl = Page.Request.QueryString;
                bool bfirstvalue = true;
                foreach (string key in nvRawUrl.AllKeys)
                {
                    if (key == _urlParameter)
                        continue;
                    string sPart = key + "=" + nvRawUrl[key];
                    if (bfirstvalue)
                    {
                        sQuery += "?" + sPart;
                        bfirstvalue = false;
                    }
                    else
                        sQuery += "&" + sPart;
                }
                if (!string.IsNullOrEmpty(sQuery))
                    sQuery += "&" + sQueryPage;
                else
                    sQuery = "?" + sQueryPage;
                return sUrl + sQuery;
            }
            return string.Empty;
        }

        protected void RenderCurrentSpan(HtmlTextWriter output, string slinkhtml)
        {
            if (!string.IsNullOrEmpty(CurrentLinkStyle))
                output.AddAttribute(HtmlTextWriterAttribute.Style, CurrentLinkStyle);
            if (!string.IsNullOrEmpty(CurrentLinkClass))
                output.AddAttribute(HtmlTextWriterAttribute.Class, CurrentLinkClass);
            output.RenderBeginTag(HtmlTextWriterTag.Span);
            output.Write(slinkhtml);
            output.RenderEndTag();
        }

        protected void RenderDisableSpan(HtmlTextWriter output, string slinkhtml)
        {
            if (!string.IsNullOrEmpty(DisableLinkStyle))
                output.AddAttribute(HtmlTextWriterAttribute.Style, DisableLinkStyle);
            if (!string.IsNullOrEmpty(DisableLinkClass))
                output.AddAttribute(HtmlTextWriterAttribute.Class, DisableLinkClass);
            output.RenderBeginTag(HtmlTextWriterTag.Span);
            output.Write(slinkhtml);
            output.RenderEndTag();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!_enabledPostBack && !this.Page.IsPostBack)
            {
                OnPageChanged(EventArgs.Empty);
            }
            if (!EnabledUrlRewrite && UrlPatterns.IsNullOrEmpty() && !EnabledPostBack)//非重写、正则替换和回传形式时 先组织url（除页数以外的部分）
                _linkHrefWithOutPageIndex = GetLinkHrefWithOutPageIndex();

            base.OnLoad(e);
        }

        #region PageChanged事件

        private static readonly object PageChangedKeyObject = new object();
        public event EventHandler PageChanged
        {
            add
            {
                base.Events.AddHandler(PageChangedKeyObject, value);
            }
            remove
            {
                base.Events.RemoveHandler(PageChangedKeyObject, value);
            }
        }

        protected virtual void OnPageChanged(EventArgs e)
        {
            EventHandler handler = base.Events[PageChangedKeyObject] as EventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region IPostBackEventHandler 成员

        public void RaisePostBackEvent(string eventArgument)
        {
            int currentPageIndex;
            if (int.TryParse(eventArgument, out currentPageIndex) && currentPageIndex > 0)
            {
                CurrentPageIndex = currentPageIndex;
            }

            OnPageChanged(EventArgs.Empty);
        }

        #endregion
    }
}
