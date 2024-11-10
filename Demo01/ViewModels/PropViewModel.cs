using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Demo01.Views;
using Demo01.XmlHelp;
using Xceed.Wpf.Toolkit;

namespace Demo01.Models
{
    partial class PropViewModel : ObservableRecipient, IRecipient<ValueChangedMessage<string>>
    {
        public PropViewModel()
        {
            IsActive = true; 
            WeakReferenceMessenger.Default.Register(this, "工具提示消息");
            LoadparaInfo();
        }

        [ObservableProperty]
        Param parameter;

        [ObservableProperty]
        string pName;
        /// <summary>
        /// 共享参数规程
        /// </summary>
        [ObservableProperty]
        string pRule;
        /// <summary>
        /// 共享参数类型
        /// </summary>
        [ObservableProperty]
        string pPType;
        /// <summary>
        /// DataCategory
        /// </summary>
        [ObservableProperty]
        string pFType;

        //[ObservableProperty]
        //int pGroup;
        /// <summary>
        /// 共享参数可见性
        /// </summary>
        [ObservableProperty]
        bool pVisible;
        /// <summary>
        /// 共享参数描述
        /// </summary>
        [ObservableProperty]
        string pDes;
        /// <summary>
        /// revit用户是否能修改
        /// </summary>
        [ObservableProperty]
        bool pModify;

        [ObservableProperty]
        ObservableCollection<Rule> ruleList = new();
        /// <summary>
        /// 选中的规程
        /// </summary>
        [ObservableProperty]
        Rule selectedRule = new();
        /// <summary>
        /// 选中的参数类型
        /// </summary>
        [ObservableProperty]
        MyType selectedType = new();
        /// <summary>
        /// 来自族类型的参数类型的名字
        /// </summary>
        string TypeFromFamily = string.Empty;
        /// <summary>
        /// 工具提示说明
        /// </summary>
        [ObservableProperty]
        string toolTipInfomation;
        [RelayCommand]
        void SetParamProp()
        {
            Parameter.Guid = Guid.NewGuid();
            Parameter.Name = "Name";
            Parameter.Rule = "Name";
            Parameter.ParamType = "saf";
            Parameter.FamType = "adf";
            //Parameter.GroupId =1;MainViewModel里设置
            Parameter.isVisible = true;
            Parameter.Describe = "adf";
            Parameter.isUserCanModify = true;
        }
        /// <summary>
        /// 得到最新的参数类型
        /// </summary>
        [RelayCommand]
        void SelecrFamilyType()
        {//切换规程时选中第一个
            if (SelectedType == null)
            {
                SelectedType = SelectedRule.TypeList[0];
            }
            //移除上一个选中的族类型
            if (
                TypeFromFamily != SelectedType.Name
                && TypeFromFamily != ""
                && SelectedRule.Name == "公共"
            )
            {
                MyType type = new MyType();

                foreach (var item in SelectedRule.TypeList)
                {
                    if (item.Name == TypeFromFamily)
                    {
                        type = item;
                    }
                }
                SelectedRule.TypeList.Remove(type);

                TypeFromFamily = string.Empty;
            }
            //选择族类型
            if (SelectedType.Name == "<族类型...>")
            {
                FamilyTypesView familyTypesView = new FamilyTypesView();
                if (familyTypesView.ShowDialog() == true)
                {
                    SelectedRule.TypeList.Add(familyTypesView.famTypeInfo);
                    SelectedType = familyTypesView.famTypeInfo;
                    TypeFromFamily = familyTypesView.famTypeInfo.Name;
                }
            }
        }
        ParamToolTipView paramToolTipView;
        /// <summary>
        /// 打开提示编辑窗口
        /// </summary>
        [RelayCommand]
        void LoadEdit()
        {
             paramToolTipView=ParamToolTipView.GetInstance;
                     paramToolTipView.ShowDialog();
            
        }
        /// <summary>
        /// 载入所有参数的信息
        /// </summary>
        void LoadparaInfo()
        {
            TypeList myTypes = XmlUtil.DeserializeFromXml<TypeList>(
                @"E:\VS2019DemoLocation\SharedParameterTxtByWpf\Demo01\XmlHelp\paraInfoFile.xml"
            );
            var groups = from t in myTypes.Types group t by t.RuleName;
            foreach (var item in groups)
            {
                Rule rule = new Rule();
                rule.Name = item.Key;
                var list = new List<MyType>();
                foreach (var i in item)
                {
                    list.Add(i);
                    //rule.TypeList.Add(i);
                }
                //(rule.TypeList ).Order((x, y) => y.Name.CompareTo(x.Name));
                list.Sort((x, y) => y.Name.CompareTo(x.Name));
                list.ForEach(t => rule.TypeList.Add(t));
                RuleList.Add(rule);
            }
            SelectedRule = RuleList[0];

            SelectedType = SelectedRule.TypeList[0];
        }
        /// <summary>
        /// 接收工具提示消息
        /// </summary>
        /// <param name="message"></param>
        public void Receive(ValueChangedMessage<string> message)
        {
            ToolTipInfomation = message.Value;
            paramToolTipView.Hide();
        }
    }
}
