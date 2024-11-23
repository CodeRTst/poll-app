using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace MyPoll.Model;
public class Comment : EntityBase<MyPollContext> {
    public int Id { get; set; }


    [Required, ForeignKey(nameof(User))]
    public int UserId { get; set; }
    [Required]
    public virtual User User { get; set; }


    [Required, ForeignKey(nameof(Poll))]
    public int PollId { get; set; }
    [Required]
    public virtual Poll Poll { get; set; }


    public string Text { get; set; }
    public DateTime Timestamp { get; set; }



    public Comment() { }


    public Comment(int userId, int pollId, string text, DateTime timestamp) {
        UserId = userId;
        PollId = pollId;
        Text = text;
        Timestamp = timestamp;
    }



    public void save() {
        Context.Comments.Add(this);
        Context.SaveChanges();
    }

    public void Delete() {
        Context.Remove(this);
        Context.SaveChanges();
    }

}
