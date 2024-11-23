using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using MyPoll.Model;
using PRBD_Framework;



namespace MyPoll.ViewModel
{
    public class LoginViewModel : ViewModelCommon {

       


        public ICommand LoginButton { get; set; }
        public ICommand SignupButton { get; set; }
        public ICommand HarryDebugButton { get; set; }
        public ICommand JohnDebugButton { get; set; }
        public ICommand AdminDebugButton { get; set; }
    
        private string _email;
        public string Email {
            get => _email;
            set => SetProperty(ref _email, value, () => Validate());
        }


        private string _password;
        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => Validate());
        }



        public LoginViewModel() : base() {
            LoginButton = new RelayCommand(LoginAction,
                () => { return _email != null && _password != null && !HasErrors; });
            SignupButton = new RelayCommand(() => NotifyColleagues(App.Messages.MSG_SIGNUP));
            HarryDebugButton = new RelayCommand(LoginHarryAction);
            JohnDebugButton = new RelayCommand(LoginJohnAction);
            AdminDebugButton = new RelayCommand(LoginAdminAction);
        }



        private void LoginHarryAction() {
            User user = Context.Users.Find(1);
            NotifyColleagues(App.Messages.MSG_LOGGED, user);
        }

        private void LoginJohnAction() {
            User user = Context.Users.Find(3);
            NotifyColleagues(App.Messages.MSG_LOGGED, user);
        }

        private void LoginAdminAction() {
            User user = Context.Users.Find(9);
            NotifyColleagues(App.Messages.MSG_LOGGED, user);
        }

        private void LoginAction() {
            if (Validate()) {
                User user = Context.Users.Single(u => u.Email == Email);
                NotifyColleagues(App.Messages.MSG_LOGGED, user);
            }
        }


        



        public override bool Validate() {
            ClearErrors();

            User user = Context.Users.FirstOrDefault(u => u.Email.Equals(Email));
            string emailPattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}(?:\.[A-Za-z]{1,4})?$";

            if (string.IsNullOrEmpty(Email))
                AddError(nameof(Email), "required");
            else if (Email.Length < 3)
                AddError(nameof(Email), "length must be >= 3");
            else if (!Regex.IsMatch(Email, emailPattern))
                AddError(nameof(Email), "wrong syntax => bob@test.be");
            else if (user == null)
                AddError(nameof(Email), "does not exist");
            else {
                if (string.IsNullOrEmpty(Password))
                    AddError(nameof(Password), "required");
                else if (Password.Length < 3)
                    AddError(nameof(Password), "length must be >= 3");
                else if (!SecretHasher.Verify(Password, user.Password))
                    AddError(nameof(Password), "wrong password");
            }



            return !HasErrors;
        }


      


    }
}
