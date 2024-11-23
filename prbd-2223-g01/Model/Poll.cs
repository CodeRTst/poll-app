using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using PRBD_Framework;

namespace MyPoll.Model;

public class Poll : EntityBase<MyPollContext> {

    [Key]
    public int Id { get; set; }
    public string Name { get; set; }



    [Required, ForeignKey(nameof(Creator))]
    public int CreatorId { get; set; }
    [Required]
    public virtual User Creator { get; set; }


    public PollType Type { get; set; } = PollType.Multiple;
    public bool Closed { get; set; }



    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public virtual ICollection<Choice> Choices { get; set; } = new HashSet<Choice>();
    public virtual ICollection<Participation> Participations { get; set; } = new HashSet<Participation>();


    public Poll() { }   



    public int NbParticipants() => Participations.Count;

    public void Delete() {
        Context.Remove(this);
        Context.SaveChanges();
    }
    

}

