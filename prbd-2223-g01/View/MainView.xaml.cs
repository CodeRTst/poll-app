using System.Collections.ObjectModel;
using System.Windows.Controls;
using Microsoft.Extensions.Logging.Console;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.View;

public partial class MainView : WindowBase {

    public MainView() {
        InitializeComponent();

        Register<Poll>(App.Messages.MSG_NEW_POLL, poll => DisplayPollDetail(poll, true));
        Register<Poll>(App.Messages.MSG_POLLNAME_CHANGED, poll => DoRenameTab(string.IsNullOrEmpty(poll.Name) ? "<new Poll>" : poll.Name));
        Register<Poll>(App.Messages.MSG_DISPLAY_POLL_DETAIL, poll => DisplayPollDetail(poll, false));
        Register<Poll>(App.Messages.MSG_DISPLAY_POLL, DisplayPoll);
        Register<Poll>(App.Messages.MSG_CLOSE_TAB, poll => DoCloseTab(poll));

    }

    private void DoRenameTab(string header) {
        if (tabControl.SelectedItem is TabItem tab) {
            MyTabControl.RenameTab(tab, header);
            tab.Tag = header;
        }
    }

    private void DisplayPollDetail(Poll poll, bool isNew) {
         OpenTab(isNew ? "<New Poll>" : poll.Name, isNew ? "new" : poll.Name, () => new PollDetailView(poll, isNew));  
    }

    private void DisplayPoll(Poll poll) {
        OpenTab(poll.Name, poll.Name, () => new PollView(poll));
    }


    private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
        var tab = tabControl.FindByTag(tag);
        if (tab == null) 
            tabControl.Add(createView(), header, tag);
         else 
            tabControl.SetFocus(tab);

    }


    private void MenuLogout_Click(object sender, System.Windows.RoutedEventArgs e) {
        NotifyColleagues(App.Messages.MSG_LOGOUT);
    }

    private void DoCloseTab(Poll poll) {
        tabControl.CloseByTag(string.IsNullOrEmpty(poll.Name) ? "new" : poll.Name);
    }
}
