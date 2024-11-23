using System.Windows;
using Microsoft.EntityFrameworkCore;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll; 



public partial class App : ApplicationBase<User, MyPollContext> {

    public enum Messages {
        MSG_LOGGED,
        MSG_LOGIN,
        MSG_LOGOUT,
        MSG_SIGNUP,
        MSG_NEW_POLL,
        MSG_POLLNAME_CHANGED,
        MSG_POLL_CHANGED,
        MSG_DISPLAY_POLL_DETAIL,
        MSG_DISPLAY_POLL,
        MSG_CLOSE_TAB
    }


    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        PrepareDatabase();


        //Navigation vers les Vues.
        Register<User>(this, Messages.MSG_LOGGED, user => {
            Login(user);
            NavigateTo<MainViewModel, User, MyPollContext>();
        });

        Register(this, Messages.MSG_LOGOUT, () => {
            Logout();
            NavigateTo<LoginViewModel, User, MyPollContext>();
        });

        Register(this, Messages.MSG_LOGIN, () => NavigateTo<LoginViewModel, User, MyPollContext>());

        Register(this, Messages.MSG_SIGNUP, () => NavigateTo<SignupViewModel, User, MyPollContext>());
    }

    private static void PrepareDatabase() {
        //Suppression ensuite creation de la base de donnees.
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }


    
}
