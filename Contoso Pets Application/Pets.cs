using System;
using System.Collections.Generic;
using System.Text;

namespace Contoso_Pets_Application
{
    internal class Pets
    {
        // I wait this many days before a new animal can be given a name.
        // If the shelter decides on a different waiting period, I only change this one number.
        // "const" means the value is fixed when the program is compiled and can never change while it runs.
        private const int DaysBeforeNaming = 30;

        // All the data is private. The rest of the app can only work with a pet
        // through the public methods further down. That is my encapsulation.
        private int PetID { get; set; }
        private string PetSpecies { get; set; }
        private DateTime DateArrived { get; set; }
        private string PetDescription { get; set; }   // physical attributes we can see at intake: size, colour, coat...
        private string PetPersonality { get; set; }

        // The "?" means the value is allowed to be null (empty / unknown).
        // A new animal has no name until it has been with us for DaysBeforeNaming days.
        private string? PetName { get; set; }

        // Only the vet can confirm these, so they stay null until the checkup is recorded.
        private bool VetCheckDone { get; set; }
        private int? PetAge { get; set; }
        private string? Breed { get; set; }
        private bool? IsNeutered { get; set; }
        private string? PhysicalCondition { get; set; }

        // This constructor is the only way to create a pet. I ask only for what we know
        // when the animal arrives. Name and vet details are filled in later through the methods below.
        public Pets(int id, string species, string description, string personality, DateTime dateArrived)
        {
            PetID = id;
            PetSpecies = species;
            PetDescription = description;
            PetPersonality = personality;
            DateArrived = dateArrived;
            VetCheckDone = false;
        }

        // I don't expose the ID itself. I let other code ask "is this pet number X?" instead.
        public bool HasId(int id)
        {
            return PetID == id;
        }

        // Subtracting two dates gives a TimeSpan. .Days gives the whole number of days in it.
        private int DaysWithUs()
        {
            return (DateTime.Today - DateArrived.Date).Days;
        }

        public bool CanBeNamed()
        {
            return DaysWithUs() >= DaysBeforeNaming;
        }

        // Math.Max makes sure I never return a negative number once the waiting time is over.
        public int DaysUntilCanBeNamed()
        {
            return Math.Max(0, DaysBeforeNaming - DaysWithUs());
        }

        // I return true/false so the menu knows whether the name was accepted.
        // I allow renaming so a typo can be fixed later.
        public bool GiveName(string name)
        {
            if (!CanBeNamed() || string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            PetName = name.Trim();
            return true;
        }

        // This is the vet's job. Calling it again simply overwrites the old results with the newest checkup.
        public void RecordVetCheckup(int age, string breed, bool isNeutered, string physicalCondition)
        {
            PetAge = age;
            Breed = breed;
            IsNeutered = isNeutered;
            PhysicalCondition = physicalCondition;
            VetCheckDone = true;
        }

        // Behaviour can change once the animal settles in, so personality is editable.
        public bool UpdatePersonality(string newPersonality)
        {
            if (string.IsNullOrWhiteSpace(newPersonality))
            {
                return false;
            }

            PetPersonality = newPersonality.Trim();
            return true;
        }

        public bool UpdateDescription(string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
            {
                return false;
            }

            PetDescription = newDescription.Trim();
            return true;
        }

        // The customer gives a species and some attributes, for example "dog" and "small brown".
        // I split the attributes into separate words and the pet only matches if EVERY word
        // appears in its description or breed. OrdinalIgnoreCase makes "Brown" and "brown" equal.
        public bool MatchesSearch(string species, string attributes)
        {
            if (!PetSpecies.Equals(species, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string searchableText = PetDescription + " " + Breed;
            string[] words = attributes.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                if (!searchableText.Contains(word, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        public void DisplayInfo()
        {
            // The "??" operator means: use the left value, but if it is null use the right one instead.
            string nameText = PetName ?? (CanBeNamed()
                ? "(no name yet - ready to be named)"
                : $"(no name yet - can be named in {DaysUntilCanBeNamed()} days)");

            Console.WriteLine($"ID {PetID} | {PetSpecies} | {nameText}");
            Console.WriteLine($"  Arrived:     {DateArrived:yyyy-MM-dd} ({DaysWithUs()} days with us)");
            Console.WriteLine($"  Description: {PetDescription}");
            Console.WriteLine($"  Personality: {PetPersonality}");

            if (VetCheckDone)
            {
                string neuteredText = IsNeutered == true ? "Yes" : "No";
                Console.WriteLine($"  Vet checkup: Done");
                Console.WriteLine($"    Age: {PetAge} | Breed: {Breed} | Neutered/Spayed: {neuteredText}");
                Console.WriteLine($"    Physical condition: {PhysicalCondition}");
            }
            else
            {
                Console.WriteLine("  Vet checkup: PENDING - age, breed, neutered/spayed and condition not confirmed yet");
            }
        }
    }
}
