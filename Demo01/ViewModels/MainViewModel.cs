using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Demo01.Models;
using Demo01.Views;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit.Primitives;

namespace Demo01.ViewModels
{
    public partial class MainViewModel
        : ObservableObject,
            IRecipient<ValueChangedMessage<Param>>,
            IRecipient<ValueChangedMessage<ParaGroup>>
    {
        [ObservableProperty]
        Param newParam;

        public MainViewModel()
        {
            WeakReferenceMessenger.Default.Register<ValueChangedMessage<Param>, string>(
                this,
                "新参数"
            );
            WeakReferenceMessenger.Default.Register<ValueChangedMessage<ParaGroup>, string>(
                this,
                "指定组"
            );
        }

        /// <summary>
        /// 组相关按钮可用性
        /// </summary>
        [ObservableProperty]
        bool isGroEn = false;

        /// <summary>
        /// 参数相关按钮可用性
        /// </summary>
        [ObservableProperty]
        bool isEn = false;

        /// <summary>
        /// 新属性组名称
        /// </summary>
        [ObservableProperty]
        string newGroupName;

        /// <summary>
        /// 共享参数文件地址
        /// </summary>
        [ObservableProperty]
        string filePath;

        /// <summary>
        /// 当前选中的group
        /// </summary>
        [ObservableProperty]
        ParaGroup selectedGroup;

        [ObservableProperty]
        ObservableCollection<ParaGroup> groupList = new();

        /// <summary>
        /// 共享参数合集
        /// </summary>
        //[ObservableProperty]
        //ObservableCollection<Param> paramList = new();
        [ObservableProperty]
        Param selectedPara;

        #region 命令
        /// <summary>
        /// 创建新组
        /// </summary>
        [RelayCommand]
        void OpenNewGroupView()
        {
            NewGroupView newGroupView = new NewGroupView();
            //加入新的组名到ComboBox
            if (newGroupView.ShowDialog() == true)
            {
                bool isexist = false;
                ParaGroup group = new ParaGroup();
                group.Name = newGroupView.InputBox.Text;
                foreach (var item in GroupList)
                {
                    if (item.Name.Equals(group.Name))
                    {
                        isexist = true;
                        MessageBox.Show(
                            "已存在该组",
                            "提示",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );
                        return;
                    }
                }
                if (GroupList.Count != 0)
                {
                    group.Id = GroupList.Last().Id + 1;
                }
                else
                {
                    group.Id = 1;
                }
                if (!isexist)
                {
                    GroupList.Add(group);
                    SelectedGroup = group;
                }
            }
            isListCountIsZero();
        }

        /// <summary>
        /// 创建共享参数属性
        /// </summary>
        [RelayCommand]
        void OpenParamPropView()
        {
            ParamPropView PropView = ParamPropView.GetInstance;
            PropView.ShowDialog();
        }

        /// <summary>
        /// 载入现有共享参数文件
        /// </summary>
        [RelayCommand]
        void LoadSharedFile()
        {
            GroupList.Clear();
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                FilePath = openFile.FileName;
            }
            try
            {
                List<ParaGroup> allParaGroup = new List<ParaGroup>();
                List<Param> allParam = new List<Param>();
                foreach (var line in File.ReadLines(FilePath))
                {
                    //Console.WriteLine(line);
                    List<string> parts = line.Split('\t').ToList(); // 根据制表符分割字符串
                    if (parts.First().Equals("GROUP"))
                    {
                        ParaGroup group = new ParaGroup();
                        group.Id = int.Parse(parts[1]);
                        group.Name = parts[2];

                        allParaGroup.Add(group);
                    }
                    if (parts.First().Equals("PARAM"))
                    {
                        Param param = new Param();
                        param.Guid = parts[1];
                        param.Name = parts[2];
                        param.ParamType = parts[3];
                        param.FamType = parts[4];
                        param.GroupId = int.Parse(parts[5]);
                        param.isVisible = Convert.ToBoolean(int.Parse(parts[6]));
                        param.Describe = parts[7];
                        param.isUserCanModify = Convert.ToBoolean(int.Parse(parts[8]));
                        allParam.Add(param);
                    }
                }

                var dic = allParaGroup.ToDictionary(it => it.Id);
                var li = allParam.GroupBy(p => p.GroupId);
                foreach (var element in li)
                {
                    if (dic.TryGetValue(element.Key, out var item))
                    {
                        foreach (var p in element)
                        {
                            item.ParamList.Add(p);
                        }
                    }
                }

                foreach (var group in allParaGroup)
                {
                    GroupList.Add(group);
                }
                SelectedGroup = GroupList[0];
                SelectedPara = SelectedGroup.ParamList[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}");
            }
        }

        /// <summary>
        ///创建新共享参数文件
        /// </summary>
        [RelayCommand]
        void CreateNewSharedFile()
        {
            SaveFileDialog saveFile = new SaveFileDialog
            {
                Title = "创建共享参数文件",
                Filter = "文本文件 (*.txt)|*.txt",
                DefaultExt = ".txt",
                AddExtension = true,
            };
            if (saveFile.ShowDialog() == true)
            {
                FileStream file = File.Create(saveFile.FileName);
                file.Close();

                FilePath = saveFile.FileName;
                WriteIntoFile();
            }
        }

        [RelayCommand]
        void CloseWindow()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 重命名组
        /// </summary>
        [RelayCommand]
        void ReNameGroup()
        {
            NewGroupView newGroupView = new NewGroupView();
            newGroupView.Title = "重命名组";
            if (newGroupView.ShowDialog() == true)
            {
                ParaGroup group = new ParaGroup();
                group = SelectedGroup;
                GroupList.Remove(SelectedGroup);
                group.Name = newGroupView.InputBox.Text;
                GroupList.Add(group);

                SelectedGroup = group;
            }
        }

        /// <summary>
        /// 删除组
        /// </summary>
        [RelayCommand]
        void DeleteGroup()
        {
            GroupList.Remove(SelectedGroup);
            if (GroupList.Count != 0)
            {
                SelectedGroup = GroupList[0];
            }
            isListCountIsZero();
        }

        /// <summary>
        /// 删除共享参数
        /// </summary>
        [RelayCommand]
        void DeletePara()
        {
            SelectedGroup.ParamList.Remove(SelectedPara);
            if (SelectedGroup.ParamList.Count != 0)
            {
                SelectedPara = SelectedGroup.ParamList.Last();
            }
            isListCountIsZero();
        }

        /// <summary>
        /// 移动共享参数到新组
        /// </summary>
        [RelayCommand]
        void MovePara()
        {
            GroupChooseView groupChooseView = new GroupChooseView();
            WeakReferenceMessenger.Default.Send(
                new ValueChangedMessage<ObservableCollection<ParaGroup>>(GroupList),
                "现有组"
            );
            groupChooseView.ShowDialog();
        }

        /// <summary>
        /// 修改参数属性
        /// </summary>
        [RelayCommand]
        void ChangeParam()
        {
            ParamPropView paramPropView = ParamPropView.GetInstance;
            WeakReferenceMessenger.Default.Send(
                new ValueChangedMessage<Param>(SelectedPara),
                "修改属性"
            );
            paramPropView.ShowDialog();
        }

        /// <summary>
        /// 判断list是否为零,零则改变按钮可用性
        /// </summary>
        void isListCountIsZero()
        {
            if (GroupList.Count != 0)
            {
                IsGroEn = true;
            }
            else
            {
                IsGroEn = false;
                return;
            }
            if (SelectedGroup != null)
            {
                if (SelectedGroup.ParamList.Count != 0)
                {
                    IsEn = true;
                }
                else
                {
                    IsEn = false;
                }
            }
        }

        /// <summary>
        /// 组选择改变，刷新当前选中的参数
        /// </summary>
        [RelayCommand]
        void SelectedChanged()
        {
            if (
                SelectedGroup != null
                && SelectedGroup.ParamList != null
                && SelectedGroup.ParamList.Count != 0
            )
            {
                SelectedPara = SelectedGroup.ParamList[0];
            }

            isListCountIsZero();
        }
        #endregion
        /// <summary>
        /// 文件初始化
        /// </summary>
        private void WriteIntoFile()
        {
            // 使用 StreamWriter 写入文件
            using (StreamWriter writer = new StreamWriter(FilePath, append: false)) // append: false 表示覆盖文件
            {
                writer.WriteLine("# This is a Revit shared parameter file.");
                writer.WriteLine("# Do not edit manually.");
                writer.WriteLine("*META\tVERSION\tMINVERSION");
                writer.WriteLine("META\t2\t1");
                writer.WriteLine("*GROUP\tID\tNAME");
                writer.WriteLine(
                    "*PARAM\tGUID\tNAME\tDATATYPE\tDATACATEGORY\tGROUP\tVISIBLE\tDESCRIPTION\tUSERMODIFIABLE"
                );
            }
        }

        /// <summary>
        /// 接收的新参数
        /// </summary>
        public void Receive(ValueChangedMessage<Param> message)
        {
            NewParam = message.Value;
            NewParam.GroupId = SelectedGroup.Id;
            bool isExist = false;
            foreach (var item in GroupList)
            {
                foreach (var param in item.ParamList)
                {
                    if (NewParam.Name == param.Name)
                    {
                        isExist = true;
                        break;
                    }
                }
            }

            if (!isExist)
            {
                SelectedGroup.ParamList.Add(NewParam);
                isListCountIsZero();
                SelectedPara = NewParam;
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "是否修改",
                    "提示",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var list = SelectedGroup.ParamList.Where(x => x.Name == NewParam.Name).ToList();
                    list.ForEach(t => SelectedGroup.ParamList.Remove(t));
                    SelectedGroup.ParamList.Add(NewParam);
                    SelectedPara = NewParam;
                }
            }
        }

        /// <summary>
        /// 接收指定组
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Receive(ValueChangedMessage<ParaGroup> message)
        {
            ParaGroup paraGroup = message.Value;
            Param param = SelectedPara;
            if (param != null)
            {
                SelectedGroup.ParamList.Remove(SelectedPara);
                foreach (var item in GroupList)
                {
                    if (paraGroup.Name == item.Name)
                    {
                        param.GroupId = item.Id;
                        item.ParamList.Add(param);
                    }
                }
                if (SelectedGroup.ParamList.Count != 0)
                {
                    SelectedPara = SelectedGroup.ParamList[0];
                }
            }
            isListCountIsZero();
        }
    }
}
