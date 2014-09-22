using System;

namespace Iridescent.Utils.Misc
{
    /// <summary>
    /// 列成员属性
    /// </summary>
    public class CellMemberAttribute:Attribute
    {
        /// <summary>
        /// Row轴索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Column轴标题
        /// </summary>
        public string Caption { get; set; }

        public CellMemberAttribute(int index)
        {
            Index = index;
        }

        public CellMemberAttribute(string caption)
        {
            Caption = caption;
        }
    }
}