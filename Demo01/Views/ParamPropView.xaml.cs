using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
using CommunityToolkit.Mvvm.Messaging;
using Demo01.Models;

namespace Demo01.Views
{
    /// <summary>
    /// ParamPropView.xaml 的交互逻辑
    /// </summary>
    public partial class ParamPropView : Window
    {
        private static ParamPropView paramPropView;

        protected ParamPropView()
        {
            InitializeComponent();
            this.DataContext = new PropViewModel(); TitleBar.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
            WeakReferenceMessenger.Default.Register<CloseWindowMessage, string>(
                this,
                "关闭属性窗口",
                (r, m) =>
                {
                    this.Hide();
                }
            );
        }

        public static ParamPropView GetInstance
        {
            get
            {
                if (paramPropView == null)
                {
                    paramPropView = new ParamPropView();
                    return paramPropView;
                }
                return paramPropView;
            }
        }

       
    }
}
