/*

insert license info here

*/

using System;
using System.Collections;
using System.Collections.Generic;


namespace Iridescent.Entities
{
	/// <summary>
	/// Generated by MyGeneration using the NHibernate Object Mapping 1.3.1 by Grimaldi Giuseppe (giuseppe.grimaldi@infracom.it)
	/// </summary>
	[Serializable]
	public class Brand : DomainObject 
	{
		#region Private Members
		
		// Variabili di stato
		private bool _isChanged;
		private bool _isDeleted;

		// Primary Key(s) 
		private int _brandid; 
		
		// Properties 
		private string _brandname; 
		private string _description; 
		private int _goodscount; 
		private DateTime _createddate; 
		private int _orderindex; 		

		#endregion
		
		#region Default ( Empty ) Class Constructor
		
		/// <summary>
		/// default constructor
		/// </summary>
		public Brand()
		{
			_brandid = 0; 
			_brandname = null; 
			_description = null; 
			_goodscount = 0; 
			_createddate = DateTime.MinValue; 
			_orderindex = 0; 
		}
		
		#endregion // End of Default ( Empty ) Class Constructor
		
		#region Full Constructor
		
		/// <summary>
		/// full constructor
		/// </summary>
        public Brand(int brandid, string brandname, string description, int goodscount, DateTime createddate, int orderindex)
		{
			_brandid = brandid; 
			_brandname = brandname; 
			_description = description; 
			_goodscount = goodscount; 
			_createddate = createddate; 
			_orderindex = orderindex; 
		}
		
		#endregion // End Full Constructor

		#region Public Properties
			
		/// <summary>
		/// 
		/// </summary>		
		public virtual int BrandId
		{
			get { return _brandid; }
			set { _isChanged |= (_brandid != value); _brandid = value; }
		} 
	  
		/// <summary>
		/// 
		/// </summary>		
		public virtual string BrandName
		{
			get { return _brandname; }
			set	
			{
				if ( value != null )
					if( value.Length > 200)
						throw new ArgumentOutOfRangeException("Invalid value for BrandName", value, value.ToString());
				
				_isChanged |= (_brandname != value); _brandname = value;
			}
		} 
	  
		/// <summary>
		/// 
		/// </summary>		
		public virtual string Description
		{
			get { return _description; }
			set	
			{
				if ( value != null )
					if( value.Length > 2000)
						throw new ArgumentOutOfRangeException("Invalid value for Description", value, value.ToString());
				
				_isChanged |= (_description != value); _description = value;
			}
		} 
	  
		/// <summary>
		/// 
		/// </summary>		
		public virtual int GoodsCount
		{
			get { return _goodscount; }
			set { _isChanged |= (_goodscount != value); _goodscount = value; }
		} 
	  
		/// <summary>
		/// 
		/// </summary>		
        public virtual DateTime CreatedDate
		{
			get { return _createddate; }
			set { _isChanged |= (_createddate != value); _createddate = value; }
		} 
	  
		/// <summary>
		/// 
		/// </summary>		
		public virtual int OrderIndex
		{
			get { return _orderindex; }
			set { _isChanged |= (_orderindex != value); _orderindex = value; }
		} 
	  
		/// <summary>
		/// Returns whether or not the object has changed it's values.
		/// </summary>
		public virtual bool IsChanged
		{
			get { return _isChanged; }
		}
		
		/// <summary>
		/// Returns whether or not the object has changed it's values.
		/// </summary>
		public virtual bool IsDeleted
		{
			get { return _isDeleted; }
		}
		
		#endregion 
		
		#region Public Functions

		/// <summary>
		/// mark the item as deleted
		/// </summary>
		public virtual void MarkAsDeleted()
		{
			_isDeleted = true;
			_isChanged = true;
		}
		
		#endregion
		
		
	}
}