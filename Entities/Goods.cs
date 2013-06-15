using System;
using Iridescent.OrmExpress;

namespace Iridescent.Entities
{
    [Serializable]
    public class Goods
    {
        private int _goodsid;
        private int _userid;
        private string _goodsname;
        private DateTime _createddate;
        private DateTime _lastupdatedtime;
        private int _orderindex;
        private bool _isdeleted;
        private bool _issaling;
        private int? _brandid;
        private string _brandname;
        private int _quantity;
        private decimal? _weight;
        private int _freight;
        private int _locationid;
        private decimal _saleprice;
        private bool _isrecommended;
        private int? _state;
        private int _salecount;
        private string _description;
        private int _viewcount;
        private int _commentcount;
        private decimal _score;
        private int _categoryid;
        private string _aftersaleexplain;
        private string _firstimagepath;
        private string _businessCode;
        private bool _supportCod;
        private int? _userCategoryId;

        /// <summary>
        /// 
        /// </summary>
        [PrimaryKey]
        public int GoodsId
        {
            set { _goodsid = value; }
            get { return _goodsid; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int UserId
        {
            set { _userid = value; }
            get { return _userid; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string GoodsName
        {
            set { _goodsname = value; }
            get { return _goodsname; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedDate
        {
            set { _createddate = value; }
            get { return _createddate; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastUpdatedTime
        {
            set { _lastupdatedtime = value; }
            get { return _lastupdatedtime; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int OrderIndex
        {
            set { _orderindex = value; }
            get { return _orderindex; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleted
        {
            set { _isdeleted = value; }
            get { return _isdeleted; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSaling
        {
            set { _issaling = value; }
            get { return _issaling; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? BrandId
        {
            set { _brandid = value; }
            get { return _brandid; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string BrandName
        {
            set { _brandname = value; }
            get { return _brandname; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Quantity
        {
            set { _quantity = value; }
            get { return _quantity; }
        }

        /// <summary>
        /// 
        /// </summary>
        public decimal? Weight
        {
            set { _weight = value; }
            get { return _weight; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Freight
        {
            set { _freight = value; }
            get { return _freight; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int LocationId
        {
            set { _locationid = value; }
            get { return _locationid; }
        }

        /// <summary>
        /// 
        /// </summary>
        public decimal SalePrice
        {
            set { _saleprice = value; }
            get { return _saleprice; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsRecommended
        {
            set { _isrecommended = value; }
            get { return _isrecommended; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? State
        {
            set { _state = value; }
            get { return _state; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SaleCount
        {
            set { _salecount = value; }
            get { return _salecount; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            set { _description = value; }
            get { return _description; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ViewCount
        {
            set { _viewcount = value; }
            get { return _viewcount; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CommentCount
        {
            set { _commentcount = value; }
            get { return _commentcount; }
        }

        /// <summary>
        /// 
        /// </summary>
        public decimal Score
        {
            set { _score = value; }
            get { return _score; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CategoryId
        {
            set { _categoryid = value; }
            get { return _categoryid; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AfterSaleExplain
        {
            set { _aftersaleexplain = value; }
            get { return _aftersaleexplain; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FirstImagePath
        {
            set { _firstimagepath = value; }
            get { return _firstimagepath; }
        }

        public string BusinessCode
        {
            get { return _businessCode; }
            set { _businessCode = value; }
        }


        public bool SupportCod
        {
            get { return _supportCod; }
            set { _supportCod = value; }
        }

        public int? UserCategoryId
        {
            get { return _userCategoryId; }
            set { _userCategoryId = value; }
        }
    }
}