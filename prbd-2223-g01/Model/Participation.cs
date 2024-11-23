using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRBD_Framework;

namespace MyPoll.Model {
    public class Participation : EntityBase<MyPollContext> {

        [Required]
        public int PollId { get; set; }
        public virtual Poll Poll { get; set; }
        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }




        public Participation() { }


        public Participation(Poll poll, User user) {
            Poll = poll;
            User = user;
        }


        public void delete() {
            Context.Votes.Where(v => v.UserId == this.UserId).ExecuteDelete();
            Context.participations.Remove(this);
        }

    }
}
