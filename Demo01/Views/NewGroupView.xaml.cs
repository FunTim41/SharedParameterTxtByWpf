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

namespace Demo01.Views
{
    /// <summary>
    /// NewGroupView.xaml 的交互逻辑
    /// </summary>
    public partial class NewGroupView : Window
    {
        public NewGroupView()
        {
            InitializeComponent();
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            okBtn.IsEnabled = false;
            if (InputBox.Text != null && InputBox.Text != "")
            {
                okBtn.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
