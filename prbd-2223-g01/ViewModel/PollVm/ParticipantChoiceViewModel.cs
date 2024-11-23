using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FontAwesome6;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel.PollVm {
    public class ParticipantChoiceViewModel : ViewModelCommon {
        private bool _editMode;
        private bool _yesBtnChecked;
        private bool _noBtnChecked;
        private bool _maybeBtnChecked;




        public ParticipantChoiceViewModel(User user, Choice choice, ParticipantViewModel participantViewModel) {
            ParticipantVM = participantViewModel;
            Vote = user.Votes.FirstOrDefault(v => v.ChoiceId == choice.Id) ?? new Vote(user.Id, choice.Id, VoteType.None);
            ConfigRelayCommand();
        }



        public ParticipantViewModel ParticipantVM { get; }

        public Vote Vote { get; private set; }
        public bool Voted => YesBtnChecked || NoBtnChecked || MaybeBtnChecked;
        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref _editMode, value);
        }

        public bool YesBtnChecked {
            get => _yesBtnChecked;
            set => SetProperty(ref _yesBtnChecked, value);
        }

        public bool NoBtnChecked {
            get => _noBtnChecked;
            set => SetProperty(ref _noBtnChecked, value);
        }

        public bool MaybeBtnChecked {
            get => _maybeBtnChecked;
            set => SetProperty(ref _maybeBtnChecked, value);
        }





        public ICommand YesButton { get; set; }
        public ICommand NoButton { get; set; }
        public ICommand MaybeButton { get; set; }



        public EFontAwesomeIcon VoteIcon => GetVoteIcon();
        public Brush VoteColor => GetVoteColor();




        



        private void ConfigRelayCommand() {
            YesButton = new RelayCommand(() => {
                if (!YesBtnChecked) {
                    UncheckAllOrOne();
                    Vote.Type = VoteType.Yes;
                }
                YesBtnChecked = !YesBtnChecked;
            });

            NoButton = new RelayCommand(() => {
                if (!NoBtnChecked) {
                    UncheckAllOrOne();
                    Vote.Type = VoteType.No;
                }
                NoBtnChecked = !NoBtnChecked;
            });

            MaybeButton = new RelayCommand(() => {
                if (!MaybeBtnChecked) {
                    UncheckAllOrOne();
                    Vote.Type = VoteType.Maybe;
                }
                MaybeBtnChecked = !MaybeBtnChecked;
            });
        }

        private void UncheckAllOrOne() {
            if (ParticipantVM.IsTypeSingle)
                ParticipantVM.UncheckButtons();
            else
                UncheckButtons();
        }

        public void UncheckButtons() {
            YesBtnChecked = false;
            NoBtnChecked = false;
            MaybeBtnChecked = false;
        }






        private EFontAwesomeIcon GetVoteIcon() {
            if (Vote.Type == VoteType.Yes) {
                YesBtnChecked = true;
                return EFontAwesomeIcon.Solid_Check;
            }
            if (Vote.Type == VoteType.No) {
                NoBtnChecked = true;
                return EFontAwesomeIcon.Solid_Xmark;
            }
            if (Vote.Type == VoteType.Maybe) {
                MaybeBtnChecked = true;
                return EFontAwesomeIcon.Solid_CircleQuestion;
            }
            return EFontAwesomeIcon.None;
        }

        private Brush GetVoteColor() {
            if (Vote.Type == VoteType.Yes)
                return Brushes.Green;
            if (Vote.Type == VoteType.No)
                return Brushes.Red;
            if (Vote.Type == VoteType.Maybe)
                return Brushes.Yellow;

            return Brushes.White;
        }
    }
}
