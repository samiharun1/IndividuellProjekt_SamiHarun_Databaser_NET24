﻿using IndividuellProjekt_SamiHarun_Databaser_NET24.Data;
using IndividuellProjekt_SamiHarun_Databaser_NET24.Models;
using Microsoft.EntityFrameworkCore;

namespace IndividuellProjekt_SamiHarun_Databaser_NET24
{
    public class Program
    {
        private static SkolaContext _context = new SkolaContext();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Välj en funktion:");
                Console.WriteLine("1. Hämta alla personal");
                Console.WriteLine("2. Hämta alla studenter");
                Console.WriteLine("3. Hämta alla studenter i en viss klass");
                Console.WriteLine("4. Hämta betyg som satts senaste månaden");
                Console.WriteLine("5. Lägg till en ny student");
                Console.WriteLine("6. Lägg till ny personal");
                Console.WriteLine("7. Visa alla kurser");
                Console.WriteLine("8. Visa antal år arbetade för personal");
                Console.WriteLine("9. Antal lärare per avdelning");
                Console.WriteLine("10. Visa total lön per avdelning");

                Console.WriteLine("0. Avsluta");
                Console.Write("Välj ett alternativ: ");

                string val = Console.ReadLine();

                switch (val)
                {
                    case "1":
                        VisaPersonal();
                        break;
                    case "2":
                        VisaStudenter();
                        break;
                    case "3":
                        VisaStudenterIKlass();
                        break;
                    case "4":
                        VisaBetyg();
                        break;
                    case "5":
                        LaggTillStudent();
                        break;
                    case "6":
                        LaggTillPersonal();
                        break;
                    case "7":
                        VisaAllaKurser();
                        break;
                    case "8":
                        VisaAntalArArbetade();
                        break;
                    case "9":
                        AntalLararePerAvdelning();
                        break;
                    case "10":
                        VisaTotalLonPerAvdelning();
                        break;

                    case "0":
                        return;
                    default:
                        Console.WriteLine("Ogiltigt val, försök igen.");
                        break;
                }

                Console.WriteLine("\nTryck på Enter för att fortsätta...");
                Console.ReadLine();
            }
        }

        // Visa alla personal
        private static void VisaPersonal()
        {
            Console.WriteLine("Välj befattning att visa (Lärare, Administratör, Rektor): ");
            string befattning = Console.ReadLine();

            // Hämta personalen som en IQueryable först
            var personal = _context.Personals.AsQueryable();

            // Om befattning inte är tom, filtrera på den via AsEnumerable för att köra på klienten
            if (!string.IsNullOrEmpty(befattning))
            {
                // Hämta data till minnet och filtrera därefter på klienten
                personal = personal.AsEnumerable()
                                   .Where(p => p.Befattning.Contains(befattning, StringComparison.OrdinalIgnoreCase))
                                   .AsQueryable();
            }

            // Skriv ut resultatet
            foreach (var p in personal)
            {
                Console.WriteLine($"{p.Namn} - {p.Befattning}");
            }
        }

        // Visa alla studenter
        private static void VisaStudenter()
        {
            Console.WriteLine("Vill du sortera på (1) Förnamn eller (2) Efternamn?");
            string sortType = Console.ReadLine();

            Console.WriteLine("Vill du ha stigande eller fallande sortering? (1 = Stigande, 2 = Fallande)");
            string order = Console.ReadLine();

            var students = _context.Studenters.AsQueryable();

            if (sortType == "2") // Efternamn
            {
                // Eftersom vi inte delar upp namnet, sorterar vi på hela namnet
                if (order == "1")
                    students = students.OrderBy(s => s.Namn); // Sortera alfabetiskt efter hela namnet
                else
                    students = students.OrderByDescending(s => s.Namn); // Sortera i fallande ordning
            }
            else // Förnamn
            {
                if (order == "1")
                    students = students.OrderBy(s => s.Namn); // Sortera alfabetiskt efter hela namnet
                else
                    students = students.OrderByDescending(s => s.Namn); // Sortera i fallande ordning
            }

            foreach (var student in students)
            {
                Console.WriteLine($"{student.Namn} ({student.Klass})");
            }
        }

        // Visa studenter i en viss klass
        private static void VisaStudenterIKlass()
        {
            Console.WriteLine("Välj klass att visa (OOP24, NET24, etc.): ");
            string klass = Console.ReadLine();

            var studentsInClass = _context.Studenters
            .Where(s => s.Klass.ToLower() == klass.ToLower())
            .ToList();

            foreach (var student in studentsInClass)
            {
                Console.WriteLine($"{student.Namn} ({student.Klass})");
            }
        }

        // Visa betyg som satts senaste månaden
        private static void VisaBetyg()
        {
            var betyg = _context.Betygs
                .Include(b => b.Student) // Inkludera Student-tabellen
                .Include(b => b.Kurs)    // Inkludera Kurs-tabellen
                .Include(b => b.Larare)  // Inkludera Personal som lärare
                .Where(b => b.Datum >= DateTime.Now.AddMonths(-1))
                .Select(b => new
                {
                    StudentNamn = b.Student.Namn,
                    KursNamn = b.Kurs.Kursnamn,
                    LarareNamn = b.Larare.Namn, // Lägg till lärarens namn
                    Betyg = b.Betyg1
                })
                .ToList();

            foreach (var b in betyg)
            {
                Console.WriteLine($"{b.StudentNamn} - {b.KursNamn} - {b.LarareNamn} - {b.Betyg}");
            }
        }


        // Lägg till en ny student
        private static void LaggTillStudent()
        {

            Console.WriteLine("Ange namn på elev:");
            string namn = Console.ReadLine();

            Console.WriteLine("Ange personnummer på elev:");
            string personnummer = Console.ReadLine();

            Console.WriteLine("Ange klass på elev:");
            string klass = Console.ReadLine();


            // Skapa en ny student utan att ange ID, eftersom det sätts automatiskt av databasen
            var nyElev = new Studenter
            {
                Namn = namn,
                Personnummer = personnummer,
                Klass = klass
            };

            // Lägg till den nya studenten till databasen
            using (var context = new SkolaContext())
            {
                // Lägg till studenten och spara förändringarna
                context.Studenters.Add(nyElev);  // Id hanteras av databasen automatiskt
                context.SaveChanges();  // Spara till databasen
            }

            // Bekräftelsemeddelande
            Console.WriteLine("Eleven har lagts till. Tryck på en tangent för att återgå till menyn.");
            Console.ReadKey();
        }

        // Lägg till ny personal
        private static void LaggTillPersonal()
        {
            Console.WriteLine("Ange namn på personal: ");
            string namn = Console.ReadLine();

            Console.WriteLine("Ange befattning (Lärare, Administratör, Rektor): ");
            string befattning = Console.ReadLine();

            var newPersonal = new Personal
            {
                Namn = namn,
                Befattning = befattning
            };

            _context.Personals.Add(newPersonal);
            _context.SaveChanges();

            Console.WriteLine($"Personal {namn} har lagts till.");
        }

        private static void VisaAllaKurser()
        {
            var kurser = _context.Kursers.ToList();
            foreach (var kurs in kurser)
            {
                Console.WriteLine(kurs.Kursnamn);
            }
        }

        // Visa alla personal med år av tjänst
        private static void VisaAntalArArbetade()
        {
            Console.WriteLine("Välj befattning att visa (Lärare, Administratör, Rektor eller lämna tomt för alla): ");
            string befattning = Console.ReadLine();

            var personal = _context.Personals
                .Select(p => new
                {
                    p.Namn,
                    p.Befattning,
                    AntalAr = DateTime.Now.Year - (p.Anstallningsdatum.HasValue ? p.Anstallningsdatum.Value.Year : DateTime.Now.Year)
                });

            if (!string.IsNullOrEmpty(befattning))
            {
                personal = personal.Where(p => p.Befattning.Contains(befattning, StringComparison.OrdinalIgnoreCase));
            }

            Console.WriteLine("\nNamn - Befattning - Antal År");
            Console.WriteLine(new string('-', 30));
            foreach (var p in personal)
            {
                Console.WriteLine($"{p.Namn} - {p.Befattning} - {p.AntalAr} år");
            }
        }

        // Antal lärare per avdelning
        private static void AntalLararePerAvdelning()
        {
            var antalLarare = _context.Personals
                .Where(p => p.Befattning == "Lärare")
                .GroupBy(p => p.Avdelning)
                .Select(g => new
                {
                    Avdelning = g.Key,
                    AntalLarare = g.Count()
                })
                .ToList();

            foreach (var item in antalLarare)
            {
                Console.WriteLine($"Avdelning: {item.Avdelning}, Antal lärare: {item.AntalLarare}");
            }
        }

        // Visa total lön per avdelning
        private static void VisaTotalLonPerAvdelning()
        {
            var totalLonPerAvdelning = _context.Personals
                .Where(p => p.Befattning == "Lärare")
                .GroupBy(p => p.Avdelning)
                .Select(g => new
                {
                    Avdelning = g.Key,
                    TotalLon = g.Sum(p => p.Lon)
                })
                .ToList();

            foreach (var item in totalLonPerAvdelning)
            {
                Console.WriteLine($"Avdelning: {item.Avdelning}, Total lön: {item.TotalLon}");
            }
        }


    }
}

