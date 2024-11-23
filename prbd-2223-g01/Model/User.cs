using PRBD_Framework;

namespace MyPoll.Model;


public enum Role {
    Administrator = 1,
    User = 2
}

public class User : EntityBase<MyPollContext> {

 
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Role Role { get; protected set; } = Role.User;

    public virtual ICollection<Poll> PollsCreated { get; set; } = new HashSet<Poll>();
    public virtual ICollection<Comment> CommentsCreated { get; set; } = new HashSet<Comment>();
    public virtual ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();
    public virtual ICollection<Participation> Participations { get; set; } = new HashSet<Participation>();


    public User() { }

    public User(string fullName,  string email, string password) {
        FullName = fullName;
        Email = email;
        Password = password;
    }

    public void save() {
        Context.Users.Add(this);
        Context.SaveChanges();
    }


}
