using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using MyPoll.ViewModel.PollVm;
using PRBD_Framework;

namespace MyPoll.ViewModel.PollVm {
    public class ParticipantViewModel : ViewModelCommon {




        private readonly PollViewModel _pollViewModel;
        private readonly Poll _poll;
        private readonly List<Choice> _choices;
        private readonly Participation _participation;
        private List<ParticipantChoiceViewModel> _participantChoicesVM = new();
        private bool _editMode;



        public ParticipantViewModel(PollViewModel pollViewModel, Participation participation, List<Choice> choices) {
            _poll = participation.Poll;
            _pollViewModel = pollViewModel;
            _participation = participation;
            _choices = choices;
            RefreshData();

            EditCommand = new RelayCommand(() => EditMode = true);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            DeleteCommand = new RelayCommand(Delete);

        }






        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }


        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref _editMode, value, EditModeChanged);
        }
        public List<ParticipantChoiceViewModel> ParticipantChoicesVM {
            get => _participantChoicesVM;
            private set => SetProperty(ref _participantChoicesVM, value);
        }



        public User User => _participation.User;
        public bool ParentEditMode => _pollViewModel.EditMode;

        public bool Editable => DisplayButtons && !EditMode && !ParentEditMode;

        public bool DisplayButtons => !_poll.Closed && (IsAdmin || _participation.User.Id == CurrentUser.Id);

        public bool IsTypeSingle => _participation.Poll.Type == PollType.Single;





        public void UncheckButtons() {
            ParticipantChoicesVM.ForEach(p => p.UncheckButtons());
        }

        private void EditModeChanged() {
            _participantChoicesVM.ForEach(p => p.EditMode = EditMode);
            _pollViewModel.EditMode = EditMode;
        }

        public void UpdateVisibility() {
            RaisePropertyChanged(nameof(Editable));
            RaisePropertyChanged(nameof(DisplayButtons));
        }

        private void RefreshData() {
            ParticipantChoicesVM = _choices
                .Select(c => new ParticipantChoiceViewModel(User, c, this))
                .ToList();
        }




        public override void Save() {
            EditMode = false;
            User.Votes = new HashSet<Vote>(ParticipantChoicesVM.Where(p => p.Voted).Select(p => p.Vote).Union(User.Votes.Where(v => v.Choice.PollId != _participation.PollId)));
            Context.SaveChanges();

            // On recrée la liste ParticipantChoicesVM avec les nouvelles données
            RefreshData();
            NotifyColleagues(App.Messages.MSG_POLL_CHANGED);
        }
        private void Cancel() {
            EditMode = false;
            RefreshData();
        }

        private void Delete() {
            User.Votes = User.Votes.Where(v => v.Choice.PollId != _participation.PollId).ToList();
            Context.SaveChanges();

            RefreshData();
            NotifyColleagues(App.Messages.MSG_POLL_CHANGED);

        }

    }
}
