using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using MyPoll.ViewModel.PollVm;
using PRBD_Framework;

namespace MyPoll.ViewModel.PollDetailVm {
    public class PollDetailViewModel : ViewModelCommon {

        private Poll _poll;
        private bool _pollModified;
        private string _txtChoice;




        public PollDetailViewModel(Poll poll, bool isNew) {
            Poll = isNew ? poll : Context.Polls.FirstOrDefault(p => p.Id == poll.Id);
            IsNew = isNew;

            //Initialiser les listes-------------------------------------------------------------------------
            PollTypes = (PollType[])Enum.GetValues(typeof(PollType));
            Choices = Poll.Choices;
            Participations = Poll.Participations;
            NoParticipations = new(Context.Users.Where(p => p.Role != Role.Administrator));


            if (!isNew) { 
                NoParticipations.RemoveRange(Participations.Select(p => p.User).ToArray());
                RefreshChoicesVM();
                RefreshParticipationsVM();
            }
            
            ConfigCollectionChanged();
            ConfigRelayCommand();
        }


        

        public bool PollModified {
            get => Context.ChangeTracker.HasChanges();
            set => SetProperty(ref _pollModified, Context.ChangeTracker.HasChanges());
        }


        public Poll Poll {
            get => _poll;
            set => SetProperty(ref _poll, value);
        }



        public string Name {
            get => Poll.Name;
            set => SetProperty(Poll.Name, value, Poll, (p, v) => {
                p.Name = v;
                NotifyColleagues(App.Messages.MSG_POLLNAME_CHANGED, Poll);
            });
        }


        public PollType Type {
            get => Poll.Type;
            set => SetProperty(Poll.Type, value, Poll, (p, v) => p.Type = v);
        }


        public bool IsClosed {
            get => Poll.Closed;
            set => SetProperty(Poll.Closed, value, Poll, (p, v) => p.Closed = v);
        }

        public string TxtChoice {
            get => _txtChoice;
            set => SetProperty(ref _txtChoice, value);
        }






        public ICollection<Participation> Participations { get; private set; }
        public ICollection<Choice> Choices { get; private set; } 

        public ObservableCollectionFast<ParticipationViewModel> ParticipationVM { get; private set; } = new();
        public ObservableCollectionFast<ChoiceViewModel> ChoicesVM { get; private set; } = new();

        public ObservableCollectionFast<User> NoParticipations { get; private set; }
        public PollType[] PollTypes { get; }



        public bool IsNew { get; private set; }


        public string Creator => IsNew ? CurrentUser.FullName : Poll.Creator.FullName;
        public string Title => IsNew ? "<New Poll>" : Poll.Name;
        private bool HasOnlySingleVotes => Participations.All(p => (Choices.Where(c => c.Votes.Any(v => v.User.Id == p.User.Id)).Count() < 2));
        public bool EnablePollTypes => IsNew || HasOnlySingleVotes;
        public bool HasParticipation => Participations.Count > 0;
        public bool HasChoice => Choices.Count > 0;






        public ICommand SaveButton { get; private set; }
        public ICommand CancelButton { get; private set; }
        public ICommand AddParticipantButton { get; private set; }
        public ICommand AddMySelfButton { get; private set; }
        public ICommand AddEveryBodyButton { get; private set; }
        public ICommand AddChoiceButton { get; private set; }
        public ICommand ReopenButton { get; private set; }






        //Configuration de CollectionChanged-------------------------------------------------------------------
        private void ConfigCollectionChanged() {
            ChoicesVM.CollectionChanged += ChoicesVM_CollectionChanged;
            ParticipationVM.CollectionChanged += ParticipationsVM_CollectionChanged;
        }

        private void ChoicesVM_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            RaisePropertyChanged(nameof(HasChoice));
            RaisePropertyChanged(nameof(EnablePollTypes));
        }

        private void ParticipationsVM_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            RaisePropertyChanged(nameof(HasParticipation));
            RaisePropertyChanged(nameof(EnablePollTypes));
        }





        //Rafraichissement des listes de ViewModel ------------------------------------------------------------
        public void RefreshChoicesVM() {
            ChoicesVM.RefreshFromModel(Choices.Select(c => new ChoiceViewModel(this, c)));
        }

        private void RefreshParticipationsVM() {
            ParticipationVM.RefreshFromModel(Participations.Select(p => new ParticipationViewModel(this, p, Choices.ToList())));
        }




        //Configuration des commandes (boutons)----------------------------------------------------------------
        private void ConfigRelayCommand() {
            SaveButton = new RelayCommand(Save, CanSaveAction);
            CancelButton = new RelayCommand(Cancel);
            AddParticipantButton = new RelayCommand<User>(AddParticipant, CanAddParticipant);
            AddMySelfButton = new RelayCommand(AddMySelf, CanAddMySelf);
            AddEveryBodyButton = new RelayCommand(AddEveryBody, CanAddEveryBody);
            AddChoiceButton = new RelayCommand(AddChoice, CanAddChoice);
            ReopenButton = new RelayCommand(ReopenPoll);
        }





        private bool CanSaveAction() => IsNew ? !string.IsNullOrEmpty(Name) : PollModified;
        private bool CanAddMySelf() => IsNotAdmin && !Participations.Any(p => p.User.Id == CurrentUser.Id);
        private bool CanAddChoice() => !TxtChoice.IsNullOrEmpty();
        private bool CanAddEveryBody() => NoParticipations.Count() > 0;
        private bool CanAddParticipant(User u) => u != null;




        //Les actions des commandes (boutons)-----------------------------------------------------------------


        private void ReopenPoll() {
            IsClosed = false;
        }




            private void AddChoice() {
            Choice c = new(Poll, TxtChoice);
            Choices.Add(c);
            ChoicesVM.Add(new ChoiceViewModel(this, c));
            TxtChoice = "";
        }





        private void AddMySelf() {
            User user = NoParticipations.FirstOrDefault(u => u.Id == CurrentUser.Id);
            if (user != null) {
                Participation p = new(Poll, user);
                Participations.Add(p);
                ParticipationVM.Add(new ParticipationViewModel(this, p, Choices.ToList()));
                NoParticipations.Remove(user);
            }
        }






        private void AddEveryBody() {
            NoParticipations.ToList().ForEach(u => {
                Participation p = new(Poll, u);
                Participations.Add(p);
                ParticipationVM.Add(new ParticipationViewModel(this, p, Choices.ToList()));
                NoParticipations.Remove(u);
            });
        }






        private void AddParticipant(User u) {
            Participation p = new(Poll, u);
            Participations.Add(p);
            ParticipationVM.Add(new ParticipationViewModel(this, p, Choices.ToList()));
            NoParticipations.Remove(u);
        }







        public void DeleteParticipant(ParticipationViewModel participationVM, Participation p, int nbVote) {
            if (nbVote == 0 || App.Confirm($"This participant has already {nbVote} vote(s) for this poll.\r\nIf you proceed and save the poll, his vote(s) will be deleted.\r\nDo you confirm?")) {
                DeleteVotesOfUser(p.User);
                RefreshChoicesVM();

                Participations.Remove(p);
                ParticipationVM.Remove(participationVM);
                NoParticipations.Add(p.User);
            }
        }

        private void DeleteVotesOfUser(User user) {
            Choices.ToList().ForEach(c => { c.Votes = c.Votes.Where(v => v.UserId != user.Id).ToList(); });
        }




        public void RemoveChoice(ChoiceViewModel choiceVM, Choice choice, int nbVote) {
            if (nbVote == 0 || App.Confirm($"This choice has already {nbVote} vote(s).\r\nIf you proceed and save the poll, the corresoponding vote(s) will be deleted.\r\nDo you confirm?")) {
                Choices.Remove(choice);
                ChoicesVM.Remove(choiceVM);
                RefreshParticipationsVM();
            }
        }






        private void Cancel() {
            Context.ChangeTracker.Clear();
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
            if (!IsNew)
                NotifyColleagues(App.Messages.MSG_DISPLAY_POLL, Poll);
        }



        public override void Save() {
            if (IsNew) {
                Poll.CreatorId = CurrentUser.Id;
                Context.Add(Poll);
                IsNew = false;
            }

            //mise a jour de CurrentUser, car nous utilisons le currentUser dans le PollsListVM et dans PollVM.
            //Ces 2 VM doivent les donnees les plus recentes de la dataBase.
            //Context.Users.Update(CurrentUser);
            Context.SaveChanges();
            NotifyColleagues(App.Messages.MSG_POLL_CHANGED);
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
            NotifyColleagues(App.Messages.MSG_DISPLAY_POLL, Poll);
        }



    }
}
