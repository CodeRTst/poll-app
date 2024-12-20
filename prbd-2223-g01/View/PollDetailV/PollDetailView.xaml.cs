﻿using System;
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
using MyPoll.Model;
using MyPoll.ViewModel.PollDetailVm;

namespace MyPoll.View {
    /// <summary>
    /// Logique d'interaction pour PollDetailView.xaml
    /// </summary>
    public partial class PollDetailView : UserControlBase {
        private readonly PollDetailViewModel _vm;
        public PollDetailView(Poll poll, bool isNew) {
            InitializeComponent();
            DataContext = _vm = new PollDetailViewModel(poll, isNew);
        }
    }
}
