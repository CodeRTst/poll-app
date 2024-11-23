using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPoll.Model;
public class Administrator : User {

    public Administrator() {
        Role = Role.Administrator;
    }
    public Administrator(string fullName, string email, string password) : base(fullName, email, password) {
        Role = Role.Administrator;
    }
}

