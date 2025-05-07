using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReadNest.Contracts.Users;
using ReadNest.Domain.Books.Entities;
using ReadNest.Domain.Users.Entities;

namespace ReadNest.DataAccessLayer
{
    public static class DbInitializer
    {
        public static async Task SeedDataAsync(IHost app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            if (!await context.Books.AnyAsync())
            {
                context.Books.AddRange(
                    new Book
                    {
                        Title = "Percy Jackson and the Lightning Thief",
                        Author = "Rick Riordan",
                        Genre = "Mythology",
                        ISBN = "978-0786838653",
                        IsAvailable = true,
                        Description = "Percy Jackson, a 12-year-old boy with ADHD and dyslexia, discovers he is the son of Poseidon and is thrust into a world of Greek gods, monsters, and prophecies. He embarks on a quest to retrieve Zeus' stolen lightning bolt and prevent a war among the gods."
                    },

                    new Book
                    {
                        Title = "The Lost Hero",
                        Author = "Rick Riordan",
                        Genre = "Mythology",
                        ISBN = "978-1423113393",
                        IsAvailable = true,
                        Description = "Three new demigods—Jason, Piper, and Leo—find themselves at Camp Half-Blood with no memory of how they got there. As they embark on a dangerous quest to save Hera and uncover a new prophecy, they realize their adventure is only the beginning of a greater war."
                    },

                    new Book
                    {
                        Title = "The Red Pyramid",
                        Author = "Rick Riordan",
                        Genre = "Mythology",
                        ISBN = "978-1423113386",
                        IsAvailable = true,
                        Description = "Siblings Carter and Sadie Kane discover they are descendants of powerful Egyptian magicians. When their father accidentally unleashes the chaos god Set, they must master their newfound abilities to prevent the world from plunging into destruction."
                    },

                    new Book
                    {
                        Title = "The Son of Neptune",
                        Author = "Rick Riordan",
                        Genre = "Mythology",
                        ISBN = "978-1423140597",
                        IsAvailable = true,
                        Description = "Percy Jackson wakes up with no memory of his past life and finds himself at Camp Jupiter, a Roman demigod camp. Teaming up with Hazel and Frank, he embarks on a mission to free Thanatos, the god of death, and stop an impending war against Gaia’s forces."
                    },

                    new Book
                    {
                        Title = "Magnus Chase and the Gods of Asgard: The Sword of Summer",
                        Author = "Rick Riordan",
                        Genre = "Mythology",
                        ISBN = "978-1423160915",
                        IsAvailable = true,
                        Description = "Magnus Chase, a homeless teen in Boston, discovers he is the son of a Norse god. After dying in battle, he is brought to Valhalla and learns of a looming apocalypse. To stop Ragnarok, he must reclaim the legendary Sword of Summer and face deadly Norse gods and giants."
                    },

                    new Book
                    {
                        Title = "Harry Potter and the Sorcerer’s Stone",
                        Author = "J.K. Rowling",
                        Genre = "Fantasy",
                        ISBN = "978-0590353427",
                        IsAvailable = true,
                        Description = "An 11-year-old orphan, Harry Potter, discovers he is a wizard and is invited to attend Hogwarts School of Witchcraft and Wizardry. As he learns about magic, friendship, and his past, he must stop the dark wizard Voldemort from returning to power."
                    },

                    new Book
                    {
                        Title = "Harry Potter and the Chamber of Secrets",
                        Author = "J.K. Rowling",
                        Genre = "Fantasy",
                        ISBN = "978-0439064873",
                        IsAvailable = true,
                        Description = "In his second year at Hogwarts, Harry faces new dangers when the Chamber of Secrets is opened, releasing a deadly monster that threatens the school. With the help of his friends, he unravels the mystery behind the attacks on Muggle-born students."
                    },

                    new Book
                    {
                        Title = "Rebecca",
                        Author = "Daphne du Maurier",
                        Genre = "Gothic Fiction",
                        ISBN = "978-0380778553",
                        IsAvailable = true,
                        Description = "A young woman marries the wealthy widower Maxim de Winter and moves to his grand estate, Manderley. However, she finds herself haunted by the presence of his late wife, Rebecca, and the sinister housekeeper Mrs. Danvers."
                    },

                    new Book
                    {
                        Title = "Jamaica Inn",
                        Author = "Daphne du Maurier",
                        Genre = "Historical Fiction",
                        ISBN = "978-0380725397",
                        IsAvailable = true,
                        Description = "After her mother's death, Mary Yellan moves to her aunt and uncle’s remote inn, only to discover that it is a den for smugglers and criminals. She becomes entangled in their dark activities while trying to uncover the truth."
                    }

                );
                await context.SaveChangesAsync();
            }

        }

    }
}
