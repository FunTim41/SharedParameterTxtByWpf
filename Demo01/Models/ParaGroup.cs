using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo01.Models
{
    /// <summary>
    /// 参数组
    /// </summary>
    public class ParaGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        public ObservableCollection<Param> ParamList { get; set; } = new();
    }

    /// <summary>
    /// 参数
    /// </summary>
    public class Param
    {
        public string Guid { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 共享参数规程
        /// </summary>
        public string Rule { get; set; }

        /// <summary>
        /// 共享参数参数类型
        /// </summary>
        public string ParamType { get; set; }

        /// <summary>
        /// 共享参数族类型
        /// </summary>
        public string FamType { get; set; }
        /// <summary>
        /// 组别
        /// </summary>
        public int GroupId { get; set; }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool isVisible { get; set; }

        /// <summary>
        /// 共享参数描述
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 用户是否能修改
        /// </summary>
        public bool isUserCanModify { get; set; }
    }
}
