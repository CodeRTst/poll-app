using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace MyPoll.Model {

    public enum VoteType {
        Yes,
        No,
        Maybe,
        None
    }

    public class Vote : EntityBase<MyPollContext> {

        [Required]
        public int UserId { get; set; }
        [Required]
        public virtual User User { get; set; }


        [Required]
        public int ChoiceId { get; set; }
        [Required]
        public virtual Choice Choice { get; set; }


        [Required]
        public VoteType Type { get; set;}


        public Vote() { }

        public Vote(int userId, int choiceId, VoteType type) {
            UserId = userId;
            ChoiceId = choiceId;
            Type = type;
        }

        

    }
}
