using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace MyPoll.Model;
public class Choice : EntityBase<MyPollContext> {

    public int Id { get; set; }



    [Required, ForeignKey(nameof(Poll))]
    public int PollId { get; set; }
    [Required] 
    public virtual Poll Poll { get; set; }


    public string Label { get; set; }
    public virtual ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();



    public Choice() { }

    public Choice(Poll poll, string label) {
        Poll = poll;
        Label = label;
    }


    
}
