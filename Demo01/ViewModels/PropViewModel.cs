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
    partial class PropViewModel : ObservableRecipient, IRecipient<ValueChangedMessage<string>>, IRecipient<ValueChangedMessage<MyType>>
    {
        /// <summary>
        /// 确认，按钮可用性
        /// </summary>
        bool allowNew = false;

        public PropViewModel()
        {
            IsActive = true;
            WeakReferenceMessenger.Default.Register<ValueChangedMessage<string> ,string>(this, "工具提示消息");
            WeakReferenceMessenger.Default.Register<ValueChangedMessage<MyType>, string>(this, "族类型");
            LoadparaInfo();
        }
        #region 参数属性
        /// <summary>
        /// 新参数
        /// </summary>
        Param parameter;

        /// <summary>
        /// 输入的共享参数名称
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
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
        #endregion
        /// <summary>
        /// 规程列表
        /// </summary>
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
        /// 得到最新的参数类型
        /// </summary>
        [RelayCommand]
        void SelecrFamilyType()
        { //切换规程时选中第一个
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
                familyTypesView.ShowDialog();
                if (typeFromFamily != null)
                {
                    SelectedRule.TypeList.Add(typeFromFamily);
                    SelectedType = typeFromFamily;
                    TypeFromFamily = typeFromFamily.Name;
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
            paramToolTipView = ParamToolTipView.GetInstance;
            paramToolTipView.ShowDialog();
        }

        /// <summary>
        /// 添加新参数到主窗口
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanAddNew))]
        void AddNewPara()
        {
            parameter = new Param()
            {
                Guid = Guid.NewGuid().ToString("D"),
                Name = PName,
                Rule = SelectedRule.Name,
                isVisible=PVisible,
                isUserCanModify=PModify,
                Describe=PDes
                
            };
            if (SelectedType.ParaType.Contains("-"))
            {
                parameter.ParamType = "FAMILYTYPE";
                parameter.FamType = SelectedType.ParaType;
            }
            else
            {
                parameter.ParamType = SelectedType.ParaType;
                parameter.FamType =string.Empty;
            }
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Param>(parameter),"新参数");
            WeakReferenceMessenger.Default.Send(new CloseWindowMessage(),"关闭属性窗口");


        }

        private bool CanAddNew()
        {
            return allowNew;
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
            PDes = message.Value;
            paramToolTipView.Hide();
        }
        MyType typeFromFamily;
        /// <summary>
        /// 接收族类型消息
        /// </summary>
        /// <param name="message"></param>
        public void Receive(ValueChangedMessage<MyType> message)
        {
            typeFromFamily= message.Value;
        }
        /// <summary>
        /// 名字输入框不为空时，改变按钮可用性
        /// </summary>
        /// <param name="value"></param>
        [RelayCommand]
         void PNameChange()
        {

            if (PName != null && PName != "")
            {
                allowNew = true;
                AddNewParaCommand.NotifyCanExecuteChanged();
            }
            else
            {
                allowNew = false;
                AddNewParaCommand.NotifyCanExecuteChanged();
            }
        }
    }
}
