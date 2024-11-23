using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel.PollDetailVm {
    public class ParticipationViewModel : ViewModelCommon {

        //Affichage et suppression d'un participant dans les detailles d'un Poll.

        private readonly PollDetailViewModel _pollDetailViewModel;


        public ParticipationViewModel(PollDetailViewModel pollDetailViewModel, Participation participation, List<Choice> choices) {
            Participation = participation;
            _pollDetailViewModel = pollDetailViewModel;
            Choices = choices;


            DeleteParticipantButton = new RelayCommand(DeleteParticipant);
        }


        public ICommand DeleteParticipantButton { get; }

        public List<Choice> Choices { get; }

        public Participation Participation { get; }



        public User User => Participation.User;

        public int NbVotes => Choices.Sum(c => c.Votes.Where(v => v.UserId == User.Id).Count());

        public string FullName => Participation.User.FullName;

        public string Label => $"{FullName} ({NbVotes})";



        private void DeleteParticipant() {
            _pollDetailViewModel.DeleteParticipant(this, Participation, NbVotes);
        }
        

    }
    
}
