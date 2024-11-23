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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PRBD_Framework;

namespace MyPoll.View {
    /// <summary>
    /// Logique d'interaction pour PollsListView.xaml
    /// </summary>
    public partial class PollsListView : UserControlBase {
        public PollsListView() {
            InitializeComponent();
        }

        private void ListView_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) {

        }
    }
}
