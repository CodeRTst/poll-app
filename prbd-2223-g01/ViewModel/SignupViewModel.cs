using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel {
    public class SignupViewModel : ViewModelCommon {



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


        private string _passwordConfirm;
        public string PasswordConfirm {
            get => _passwordConfirm;
            set => SetProperty(ref _passwordConfirm, value, () => Validate());
        }


        private string _fullName;
        public string FullName {
            get => _fullName;
            set => SetProperty(ref _fullName, value, () => Validate());
        }



        public SignupViewModel() {
            SignupButton = new RelayCommand(SignupAction, () => { return _email != null && _password != null && _passwordConfirm != null && _fullName != null && !HasErrors; });
            CancelButton = new RelayCommand(() => NotifyColleagues(App.Messages.MSG_LOGIN));
        }



        public ICommand SignupButton { get; }
        public ICommand CancelButton { get; }
        

        


        private void SignupAction() {
            User u = new(FullName,Email,SecretHasher.Hash(Password));
            u.save();
            Console.WriteLine(u.Id);
            NotifyColleagues(App.Messages.MSG_LOGGED, u);

        }

        public override bool Validate() {
            ClearErrors();

            ValidateEmail();
            ValidatePassword();
            ValidateName();

            return !HasErrors;
        }

        private void ValidatePassword() {
            if (string.IsNullOrEmpty(Password))
                AddError(nameof(Password), "required");
            else if (Password.Length < 3)
                AddError(nameof(Password), "length must be >= 3");
            else if (Password != PasswordConfirm)
                AddError(nameof(PasswordConfirm), "must have the same password");
        }

        private void ValidateName() {
            if (string.IsNullOrEmpty(FullName))
                AddError(nameof(FullName), "required");
            else if (FullName.Length < 3)
                AddError(nameof(FullName), "length must be >= 3");
            else if (FullName.Length > 20)
                AddError(nameof(FullName), "length must be <= 20");
        }


        private void ValidateEmail() {
            User user = Context.Users.FirstOrDefault(u => u.Email.Equals(Email));
            string emailPattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}(?:\.[A-Za-z]{1,4})?$";


            if (string.IsNullOrEmpty(Email))
                AddError(nameof(Email), "required");
            else if (Email.Length < 3)
                AddError(nameof(Email), "length must be >= 3");
            else if (!Regex.IsMatch(Email, emailPattern))
                AddError(nameof(Email), "wrong syntax => bob@test.be");
            else if (user != null)
                AddError(nameof(Email), "email already not exist");
      
        }
    }
}
