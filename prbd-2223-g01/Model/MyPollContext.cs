using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using PRBD_Framework;
using System.Runtime;

namespace MyPoll.Model;

public class MyPollContext : DbContextBase {
    public DbSet<User> Users => Set<User>();
    public DbSet<Poll> Polls => Set<Poll>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Choice> Choices => Set<Choice>();
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<Participation> participations => Set<Participation>();


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);
        
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=prbd-2223-g01")
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseLazyLoadingProxies(true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        //ENTITY USER-------------------------------------------------------/
        modelBuilder.Entity<User>()
            .HasDiscriminator(m => m.Role)
            .HasValue<User>(Role.User)
            .HasValue<Administrator>(Role.Administrator);

        modelBuilder.Entity<User>()
            .HasMany(u => u.PollsCreated)
            .WithOne(p => p.Creator)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.CommentsCreated)
            .WithOne(c => c.User)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Votes)
            .WithOne(v => v.User)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Participations)
            .WithOne(p => p.User)
            .OnDelete(DeleteBehavior.ClientCascade);
     
          
        //ENTITY POLL-------------------------------------------------------/
        modelBuilder.Entity<Poll>()
            .Property(p => p.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Poll>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.Poll)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<Poll>()
            .HasMany(p => p.Choices)
            .WithOne(c => c.Poll)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<Poll>()
            .HasMany(p => p.Participations)
            .WithOne(p => p.Poll)
            .OnDelete(DeleteBehavior.ClientCascade);


        //ENTITY CHOICE-----------------------------------------------------/
        modelBuilder.Entity<Choice>()
            .HasMany(c => c.Votes)
            .WithOne(v => v.Choice)
            .OnDelete(DeleteBehavior.ClientCascade);


        //ENTITY VOTE-------------------------------------------------------/
        modelBuilder.Entity<Vote>()
            .HasKey(k => new {k.UserId, k.ChoiceId});       

        modelBuilder.Entity<Vote>()
            .Property(v => v.Type)
            .HasConversion<string>();


        //ENTITY PARTICIPATION----------------------------------------------/
        modelBuilder.Entity<Participation>()
            .HasKey(k => new { k.PollId, k.UserId });



        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder) {

        //USER
        modelBuilder.Entity<User>()
            .HasData(
                new User { Id = 1, FullName = "Harry Covère", Email = "harry@test.com", Password = SecretHasher.Hash("harry") },
                new User { Id = 2, FullName = "Mélusine Enfayite", Email = "melusine@test.com", Password = SecretHasher.Hash("melusine") },
                new User { Id = 3, FullName = "John Deuf", Email = "john@test.com", Password = SecretHasher.Hash("john")},
                new User { Id = 4, FullName = "Alain Terrieur", Email = "alain@test.com", Password = SecretHasher.Hash("alain")},
                new User { Id = 5, FullName = "Camille Honnête", Email = "camille@test.com", Password = SecretHasher.Hash("camille")},
                new User { Id = 6, FullName = "Jim Nastik", Email = "jim@test.com", Password = SecretHasher.Hash("jim")},
                new User { Id = 7, FullName = "Mehdi Cament", Email = "mehdi@test.com", Password = SecretHasher.Hash("mehdi")},
                new User { Id = 8, FullName = "Ali Gator", Email = "ali@test.com", Password = SecretHasher.Hash("ali")},
                new User { Id = 10, FullName = "jean neymar sanchez", Email = "has@test.com", Password = SecretHasher.Hash("has")}
            );


        //ADMINISTRATOR
        modelBuilder.Entity<Administrator>()
            .HasData(
                new Administrator() { Id = 9, FullName = "Admin", Email = "admin@test.com", Password = SecretHasher.Hash("admin")}
            );



        //POLL
        modelBuilder.Entity<Poll>()
            .HasData(
                new Poll { Id = 1, Name = "Meilleure citation ?", CreatorId = 1 },
                new Poll { Id = 2, Name = "Meilleur film de série B ?", CreatorId = 3 },
                new Poll { Id = 3, Name = "Plus belle ville du monde ?", CreatorId = 1, Type = PollType.Single },
                new Poll { Id = 4, Name = "Meilleur animé japonais ?", CreatorId = 5 },
                new Poll { Id = 5, Name = "Sport pratiqué", CreatorId = 3, Closed = true },
                new Poll { Id = 6, Name = "Langage informatique préféré", CreatorId = 7 },
                new Poll { Id = 7, Name = "Les plus beaux punchlines", CreatorId = 10, Type = PollType.Single }
            );
        


        modelBuilder.Entity<Comment>()
            .HasData(
                new Comment { Id = 1, UserId = 1, PollId = 1, Text = "M'en fout", Timestamp = DateTime.Parse("2022-12-10 16:40")},
                new Comment { Id = 2, UserId = 1, PollId = 2, Text = "Bonne question!", Timestamp = DateTime.Parse("2022-12-01 12:33")},
                new Comment { Id = 3, UserId = 2, PollId = 1, Text = "Moi aussi", Timestamp = DateTime.Parse("2022-12-11 22:07")},
                new Comment { Id = 4, UserId = 3, PollId = 1, Text = "Bla bla bla", Timestamp = DateTime.Parse("2022-12-27 08:15")},
                new Comment { Id = 5, UserId = 1, PollId = 6, Text = "I love C#", Timestamp = DateTime.Parse("2022-12-31 23:59")},
                new Comment { Id = 6, UserId = 3, PollId = 6, Text = "I hate WPF", Timestamp = DateTime.Parse("2023-01-01 00:01")},
                new Comment { Id = 7, UserId = 2, PollId = 1,
                    Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi pulvinar, dolor non commodo commodo, " +
                        "felis libero sagittis tellus, at tristique orci risus hendrerit lorem. Maecenas varius hendrerit lacinia. " +
                        "Vestibulum dapibus, libero nec accumsan pulvinar, felis velit imperdiet libero, sed venenatis massa risus " +
                        "gravida dolor. In et lobortis massa.",
                    Timestamp = DateTime.Parse("2023-01-02 08:45")
                }
            );



        modelBuilder.Entity<Choice>()
    .HasData(
        new Choice { Id = 1, PollId = 1, Label = "La science est ce que nous comprenons suffisamment bien pour l'expliquer à un ordinateur. L'art, c'est tout ce que nous faisons d'autre. (Knuth)"},
        new Choice { Id = 2, PollId = 1, Label = "La question de savoir si les machines peuvent penser... est à peu près aussi pertinente que celle de savoir si les sous-marins peuvent nager. (Dijkstra)"},
        new Choice { Id = 3, PollId = 1, Label = "Nous ne savons pas où nous allons, mais du moins il nous reste bien des choses à faire. (Turing)"},
        new Choice { Id = 4, PollId = 1, Label = "La constante d’une personne est la variable d’une autre. (Perlis)"},
        new Choice { Id = 5, PollId = 1, Label = "There are only two kinds of [programming] languages: the ones people complain about and the ones nobody uses. (Stroustrup)"},
        new Choice { Id = 6, PollId = 2, Label = "Massacre à la tronçonneuse" },
        new Choice { Id = 7, PollId = 2, Label = "Braindead" },
        new Choice { Id = 8, PollId = 2, Label = "La Nuit des morts-vivants" },
        new Choice { Id = 9, PollId = 2, Label = "Psychose" },
        new Choice { Id = 10, PollId = 2, Label = "Evil Dead" },
        new Choice { Id = 11, PollId = 3, Label = "Charleroi" },
        new Choice { Id = 12, PollId = 3, Label = "Charleville-Mézières" },
        new Choice { Id = 13, PollId = 3, Label = "Pyongyang" },
        new Choice { Id = 14, PollId = 3, Label = "Détroit" },
        new Choice { Id = 15, PollId = 4, Label = "One piece" },
        new Choice { Id = 16, PollId = 4, Label = "Hunter x Hunter" },
        new Choice { Id = 17, PollId = 4, Label = "Fullmetal Alchemist" },
        new Choice { Id = 18, PollId = 4, Label = "Death Note" },
        new Choice { Id = 19, PollId = 4, Label = "Naruto Shippûden" },
        new Choice { Id = 20, PollId = 4, Label = "Dragon Ball Z" },
        new Choice { Id = 21, PollId = 5, Label = "Curling" },
        new Choice { Id = 22, PollId = 5, Label = "Swamp Football" },
        new Choice { Id = 23, PollId = 5, Label = "Fléchettes" },
        new Choice { Id = 24, PollId = 5, Label = "Kin-ball" },
        new Choice { Id = 25, PollId = 5, Label = "Pétanque" },
        new Choice { Id = 26, PollId = 5, Label = "Lancer de tronc" },
        new Choice { Id = 27, PollId = 6, Label = "Brainfuck" },
        new Choice { Id = 28, PollId = 6, Label = "Java" },
        new Choice { Id = 29, PollId = 6, Label = "C#" },
        new Choice { Id = 30, PollId = 6, Label = "PHP" },
        new Choice { Id = 31, PollId = 6, Label = "Whitespace" },
        new Choice { Id = 32, PollId = 6, Label = "Aargh!" },
        new Choice { Id = 33, PollId = 7, Label = "Tous les jours j’me couche aux aurores,et j’pars au taf avec une gueule d’enterrement comme si j’étais croque-mort." },
        new Choice { Id = 34, PollId = 7, Label = "Tu ne peux que gagner quand t’as rien à perdre." },
        new Choice { Id = 35, PollId = 7, Label = "À trop haïr les noirs, Le Pen fait des nuits blanches." },
        new Choice { Id = 36, PollId = 7, Label = "Je fais de l’auto-stop sur le chemin du Paradis et seul le Diable s’arrête." },
        new Choice { Id = 37, PollId = 7, Label = "Ils se prennent pour Tarantino dans le Film, mais ils sont morts dans la bande annonce" },
        new Choice { Id = 38, PollId = 7, Label = "Le monde appartient aux gens qui se lèvent tôt. Nous on se lève tard pour prendre ce qui appartient aux gens qui se couchent tôt" }
    );



        modelBuilder.Entity<Vote>()
    .HasData(
        new Vote { UserId = 1, ChoiceId = 1, Type = VoteType.Yes },
        new Vote { UserId = 1, ChoiceId = 2, Type = VoteType.Maybe },
        new Vote { UserId = 1, ChoiceId = 5, Type = VoteType.No },
        new Vote { UserId = 1, ChoiceId = 11, Type = VoteType.Yes },
        new Vote { UserId = 1, ChoiceId = 16, Type = VoteType.Yes },
        new Vote { UserId = 1, ChoiceId = 17, Type = VoteType.No },
        new Vote { UserId = 1, ChoiceId = 27, Type = VoteType.Yes },
        new Vote { UserId = 2, ChoiceId = 3, Type = VoteType.Yes },
        new Vote { UserId = 2, ChoiceId = 9, Type = VoteType.Maybe },
        new Vote { UserId = 2, ChoiceId = 10, Type = VoteType.Yes },
        new Vote { UserId = 2, ChoiceId = 16, Type = VoteType.Yes },
        new Vote { UserId = 2, ChoiceId = 29, Type = VoteType.Yes },
        new Vote { UserId = 2, ChoiceId = 34, Type = VoteType.Yes },
        new Vote { UserId = 3, ChoiceId = 2, Type = VoteType.Yes },
        new Vote { UserId = 3, ChoiceId = 4, Type = VoteType.Maybe },
        new Vote { UserId = 3, ChoiceId = 16, Type = VoteType.Maybe },
        new Vote { UserId = 3, ChoiceId = 20, Type = VoteType.Yes },
        new Vote { UserId = 3, ChoiceId = 32, Type = VoteType.No },
        new Vote { UserId = 4, ChoiceId = 29, Type = VoteType.Yes },
        new Vote { UserId = 4, ChoiceId = 36, Type = VoteType.Yes },
        new Vote { UserId = 5, ChoiceId = 27, Type = VoteType.Yes },
        new Vote { UserId = 5, ChoiceId = 28, Type = VoteType.No },
        new Vote { UserId = 5, ChoiceId = 38, Type = VoteType.No },
        new Vote { UserId = 6, ChoiceId = 27, Type = VoteType.Maybe },
        new Vote { UserId = 6, ChoiceId = 28, Type = VoteType.Yes },
        new Vote { UserId = 6, ChoiceId = 29, Type = VoteType.Maybe },
        new Vote { UserId = 7, ChoiceId = 27, Type = VoteType.Maybe },
        new Vote { UserId = 7, ChoiceId = 29, Type = VoteType.Yes },
        new Vote { UserId = 7, ChoiceId = 30, Type = VoteType.Maybe },
        new Vote { UserId = 7, ChoiceId = 33, Type = VoteType.Maybe },
        new Vote { UserId = 8, ChoiceId = 27, Type = VoteType.Maybe },
        new Vote { UserId = 8, ChoiceId = 30, Type = VoteType.Yes },
        new Vote { UserId = 8, ChoiceId = 32, Type = VoteType.No },
        new Vote { UserId = 8, ChoiceId = 35, Type = VoteType.No }
    );




        modelBuilder.Entity<Participation>()
    .HasData(
        new Participation { PollId = 1, UserId = 1 },
        new Participation { PollId = 1, UserId = 2 },
        new Participation { PollId = 1, UserId = 3 },
        new Participation { PollId = 2, UserId = 2 },
        new Participation { PollId = 3, UserId = 1 },
        new Participation { PollId = 4, UserId = 1 },
        new Participation { PollId = 4, UserId = 2 },
        new Participation { PollId = 4, UserId = 3 },
        new Participation { PollId = 5, UserId = 1 },
        new Participation { PollId = 5, UserId = 2 },
        new Participation { PollId = 5, UserId = 3 },
        new Participation { PollId = 6, UserId = 1 },
        new Participation { PollId = 6, UserId = 2 },
        new Participation { PollId = 6, UserId = 3 },
        new Participation { PollId = 6, UserId = 4 },
        new Participation { PollId = 6, UserId = 5 },
        new Participation { PollId = 6, UserId = 6 },
        new Participation { PollId = 6, UserId = 7 },
        new Participation { PollId = 6, UserId = 8 },
        new Participation { PollId = 7, UserId = 2 },
        new Participation { PollId = 7, UserId = 4 },
        new Participation { PollId = 7, UserId = 5 },
        new Participation { PollId = 7, UserId = 7 },
        new Participation { PollId = 7, UserId = 8 },
        new Participation { PollId = 7, UserId = 10 }
    );


    }



}
