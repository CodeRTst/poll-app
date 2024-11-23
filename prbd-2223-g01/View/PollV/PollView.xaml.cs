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
using Microsoft.EntityFrameworkCore.Metadata;
using MyPoll.Model;
using MyPoll.ViewModel.PollVm;
using PRBD_Framework;

namespace MyPoll.View {
    /// <summary>
    /// Logique d'interaction pour PollView.xaml
    /// </summary>
    public partial class PollView : UserControlBase
    {
        private readonly PollViewModel _vm;
      
        public PollView(Poll poll)
        {
            InitializeComponent();
            DataContext = _vm = new PollViewModel(poll);
        }
    }
}
