using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Demo01.Models;
using Demo01.XmlHelp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo01.ViewModels
{
    partial class FamTypeViewModel : ObservableObject
    {
        [ObservableProperty]
        List<MyType> famTypes = new();
        [ObservableProperty]
        MyType selectedCate = new();
        [RelayCommand]
        void SendSelected()
        {
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<MyType>(SelectedCate), "族类型");
            WeakReferenceMessenger.Default.Send(new CloseWindowMessage(), "关闭族类型窗口");
        }

        public FamTypeViewModel()
        {
            LoadFamTypeInfo();
        }


        void LoadFamTypeInfo()
        {
            TypeList famTypeList = XmlUtil.DeserializeFromXml<TypeList>(@"E:\VS2019DemoLocation\SharedParameterTxtByWpf\Demo01\XmlHelp\cateInfo.xml");


             famTypeList.Types.Sort((x,y)=>y.Name.CompareTo(x.Name));
            FamTypes = famTypeList.Types;
             SelectedCate = FamTypes[0];

        }
    }
}
