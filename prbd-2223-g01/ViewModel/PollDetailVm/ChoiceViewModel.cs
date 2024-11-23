using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel.PollDetailVm {
    public class ChoiceViewModel : ViewModelCommon {

        //Affichage, modification et suppression d'un choix dans les detailles d'un Poll.

        private readonly PollDetailViewModel _pollDetailVM;
        private bool _editMode;
        private string _txtLabel;

        public ChoiceViewModel(PollDetailViewModel pollDetailViewModel, Choice choice) {
            Choice = choice;
            _pollDetailVM = pollDetailViewModel;

            DeleteCommand = new RelayCommand(() => _pollDetailVM.RemoveChoice(this, Choice, NbVote));
            EditCommand = new RelayCommand(EditAction);
            CancelCommand = new RelayCommand(() => EditMode = false);
            SaveCommand = new RelayCommand(Save);
        }

        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }



       
        public string TxtLabel {
            get => _txtLabel;
            set => SetProperty(ref _txtLabel, value);
        }
        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref  _editMode, value);
        }




        public Choice Choice { get; }
        public int NbVote => Choice.Votes.Count();
        public string Label => $"{Choice.Label} ({NbVote})";





        public override void Save() {
            Choice.Label = TxtLabel;
            EditMode = false;
            _pollDetailVM.RefreshChoicesVM();
        }


        private void EditAction() {
            EditMode = true;
            TxtLabel = Choice.Label;
        }
    }
}
