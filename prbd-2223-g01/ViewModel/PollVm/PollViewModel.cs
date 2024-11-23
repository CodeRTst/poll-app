using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;
using static MyPoll.App;

namespace MyPoll.ViewModel.PollVm {
    public class PollViewModel : ViewModelCommon {

        private string _newCommentBody;
        private Poll _poll;
        private bool _displayCommentLink = true;
        private bool _editMode;



        public PollViewModel(Poll poll) {
            Poll = Context.Polls.FirstOrDefault(p => p.Id == poll.Id);

            Choices = Poll.Choices.ToList();
            Comments = new(Poll.Comments.OrderByDescending(c => c.Timestamp));
            var participations = Poll.Participations.OrderBy(p => p.User.FullName).ToList();
            ParticipantsVM = participations.Select(p => new ParticipantViewModel(this, p, Choices)).ToList();

            ConfigRelayCommand();
        }


        


        public ICommand CommentLinkButton { get; private set; }
        public ICommand AddNewCommentButton { get; private set; }
        public ICommand CancelNewCommentButton { get; private set; }
        public ICommand EditButton { get; private set; }
        public ICommand ReopenButton { get; private set; }
        public ICommand DeletePollButton { get; private set; }
        public ICommand DeleteCommentButton { get; private set; }



        public List<Choice> Choices { get; private set; }
        public List<ParticipantViewModel> ParticipantsVM { get; private set; }
        public ObservableCollectionFast<Comment> Comments { get; }


        public bool DisplayEditDelete => IsAdmin || Poll.CreatorId == CurrentUser.Id;
        public bool CanDeleteComment => !Poll.Closed && (IsAdmin || Poll.Creator.Id == CurrentUser.Id);
        public bool CanReopenPoll => Poll.Closed && (IsAdmin || CurrentUser.Id == Poll.Creator.Id);



        public string NewCommentBody {
            get => _newCommentBody;
            set => SetProperty(ref _newCommentBody, value);
        }



        public Poll Poll {
            get => _poll;
            set => SetProperty(ref _poll, value);
        }



        public bool DisplayCommentLink {
            get => _displayCommentLink;
            set => SetProperty(ref _displayCommentLink, value);
        }


        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref _editMode, value, () => ParticipantsVM.ForEach(p => p.UpdateVisibility()));
        }





        private void ConfigRelayCommand() {
            CommentLinkButton = new RelayCommand(() => DisplayCommentLink = false);
            AddNewCommentButton = new RelayCommand(AddNewCommentAction, CanAddNewComment);
            CancelNewCommentButton = new RelayCommand(CancelNewCommentAction);
            ReopenButton = new RelayCommand(ReopenPoll);
            EditButton = new RelayCommand(EditPoll);
            DeletePollButton = new RelayCommand(DeletePoll);
            DeleteCommentButton = new RelayCommand<Comment>(DeleteComment); 
        }

        private void ReopenPoll() {
            Poll.Closed = false;
            Context.SaveChanges();
            ParticipantsVM.ForEach(p => p.UpdateVisibility());

            RaisePropertyChanged(nameof(CanReopenPoll));
            RaisePropertyChanged(nameof(Poll));

            NotifyColleagues(App.Messages.MSG_POLL_CHANGED);
        }

        private void EditPoll() {
            NotifyColleagues(Messages.MSG_CLOSE_TAB, _poll);
            NotifyColleagues(Messages.MSG_DISPLAY_POLL_DETAIL, _poll);
        }

        private void DeletePoll() {
            if (App.Confirm("Are you sure to delete this poll?")) {
                Poll.Delete();
                NotifyColleagues(App.Messages.MSG_POLL_CHANGED);
                NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Poll);
            }
        }

        private void DeleteComment(Comment c) {
            c.Delete();
            Comments.Remove(c);
        }


        private bool CanAddNewComment() => !string.IsNullOrEmpty(NewCommentBody);
   

        private void AddNewCommentAction() {
            Comment c = new(CurrentUser.Id, _poll.Id, NewCommentBody, DateTime.Now);
            c.save();
            Comments.Insert(0,c);

            NewCommentBody = "";
            DisplayCommentLink = true;
        }

        private void CancelNewCommentAction() {
            NewCommentBody = "";
            DisplayCommentLink = true;
        }

    }
}
