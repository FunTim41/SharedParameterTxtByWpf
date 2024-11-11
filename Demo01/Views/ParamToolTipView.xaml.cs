using CommunityToolkit.Mvvm.Messaging;
using Demo01.Models;
using Demo01.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Demo01.Views
{
    /// <summary>
    ///单例工具提示窗口
    /// </summary>
    public partial class ParamToolTipView : Window
    {
        static ParamToolTipView paramToolTipView;
        protected ParamToolTipView()
        {
            InitializeComponent();
            this.DataContext = new ToolTipViewModel();
            TitleBar.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
            WeakReferenceMessenger.Default.Register<CloseWindowMessage, string>(this, "关闭属性描述窗口", (r, m) =>
            {
                this.Hide();
            });
        }
        public static ParamToolTipView GetInstance
        {
            get
            {
                if (paramToolTipView == null)
                {
                    paramToolTipView = new ParamToolTipView();
                    return paramToolTipView;
                }
                return paramToolTipView;
            } 
            
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
