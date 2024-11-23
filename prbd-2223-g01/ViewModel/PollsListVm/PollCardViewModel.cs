using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel.PollsListVm {
    public class PollCardViewModel : ViewModelCommon {

        //Affiche le nom, createur, nombre de participants, nombre de votes, ainsi que les meilleurs choix d'un Poll.
        //Ce PollCardViewModel est stocké dans une liste dans PollsListViewModel.

        private readonly Poll _poll;
        private string _color;



        public PollCardViewModel(Poll poll) {
            Poll = poll;
        }


        public Poll Poll {
            get => _poll;
            private init => SetProperty(ref _poll, value);
        }

        public string Color {
            get => GetColor();
            set => SetProperty(ref _color, value);
        }




        public string Name => Poll.Name;
        public string Creator => Poll.Creator.FullName;
        public int NbParticipant => Poll.NbParticipants();
        public List<string> BestChoices => GetMostVotedChoicesWithSums();
        public int NbVote => Poll.Choices.Sum(c => c.Votes.Count());



        //Si l'utilisateur connecté a voté ou pas.
        private bool Voted() => Poll.Choices.Where(c => c.Votes.Any(v => v.User.Id == CurrentUser.Id)).Count() > 0;




        private string GetColor() {
            if (Poll.Closed)
                return "Pink";
            else                 if (Voted())
                    return "LightGreen";
                else
                    return "DarkGray";
        }




        //Dictionnaire contenant les Polls les plus voté avec leurs titre comme clé et la somme des votes comme valeur.
        public List<string> GetMostVotedChoicesWithSums() {
            var choicesWithVotes = Poll.Choices
                .Where(c => c.Votes.Any())
                .Select(c => new {
                    label = c.Label.Length > 40 ? c.Label.Substring(0, 24) + "...." + c.Label.Substring(c.Label.Length - 10) : c.Label,
                    sumOfVotes = c.Votes.Sum(v => {
                        if (v.Type == VoteType.Yes)
                            return 1;
                        else if (v.Type == VoteType.Maybe)
                            return 0.5;
                        else if (v.Type == VoteType.No)
                            return -1;
                        else
                            return 0;
                    })
                }).ToDictionary(c => c.label, c => c.sumOfVotes);

            if (choicesWithVotes.Any()) {
                double maxSum = choicesWithVotes.Max(c => c.Value);

                return choicesWithVotes
                    .Where(c => c.Value == maxSum).Select(c => c.Key + "  (" + c.Value + ")").ToList();
            }

            return new();
        }


    }
}
