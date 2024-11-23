using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel.PollsListVm {
    public class PollsListViewModel : ViewModelCommon {

        //Affiche la liste des Polls

        private ObservableCollection<PollCardViewModel> _polls;
        private string _filter;



        public PollsListViewModel() : base() {
            ApplyFilterAction();
            ConfigRelayCommand();
            Register(App.Messages.MSG_POLL_CHANGED, ApplyFilterAction);

        }




        public string Title { get; } = "prbd-2223-g01";

        public ObservableCollection<PollCardViewModel> Polls {
            get => _polls;
            set => SetProperty(ref _polls, value);
        }

        public string Filter {
            get => _filter;
            set => SetProperty(ref _filter, value, ApplyFilterAction);
        }



        public ICommand ClearFilter { get; private set; }
        public ICommand NewPollButton { get; private set; }
        public ICommand DisplayPoll { get; private set; }



        private void ConfigRelayCommand() {
            ClearFilter = new RelayCommand(() => Filter = "");
            NewPollButton = new RelayCommand(() => { NotifyColleagues(App.Messages.MSG_NEW_POLL, new Poll()); });
            DisplayPoll = new RelayCommand<PollCardViewModel>(vm => NotifyColleagues(App.Messages.MSG_DISPLAY_POLL, vm.Poll));
        }


        //Creation des PollCardViewModel et les stocké dans une liste pour les afficher dans la vue.
        private void ApplyFilterAction() {
            IEnumerable<Poll> query = IsAdmin ? Context.Polls.OrderBy(p => p.Name) : Context.Polls.Where(p => p.CreatorId == CurrentUser.Id || p.Participations.Any(par => par.UserId == CurrentUser.Id)).OrderBy(p => p.Name);
            if (!string.IsNullOrEmpty(Filter))
                query = from p in query
                        where p.Name.ToUpper().Contains(Filter.ToUpper()) || 
                            p.Choices.Any(c => c.Label.ToUpper().Contains(Filter.ToUpper()) ||
                            p.Participations.Any(p => p.User.FullName.ToUpper().Contains(Filter.ToUpper())))
                        orderby p.Name
                        select p;
            Polls = new ObservableCollection<PollCardViewModel>(query.Select(p => new PollCardViewModel(p)));
        }

    }
}
