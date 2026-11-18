using Contoso_Pets_Application;

internal class Program
{
    private static void Main(string[] args)
    {
        List<Pets> ourAnimals = new List<Pets>();

        // Dataset of the animals we already have
        LoadDataset(ourAnimals);

        // I keep showing the menu until the user chooses Exit, which sets running to false.
        bool running = true;
        while (running)
        {
            ShowMenu();
            string choice = ReadText("Enter your choice: ");
            Console.WriteLine();

            switch (choice)
            {
                case "1": ListAllPets(ourAnimals); break;
                case "2": AddNewAnimal(ourAnimals); break;
                case "3": RecordVetCheckup(ourAnimals); break;
                case "4": NameAnimal(ourAnimals); break;
                case "5": EditAnimal(ourAnimals); break;
                case "6": SearchForCustomer(ourAnimals); break;
                case "7": running = false; break;
                default: Console.WriteLine("That is not a menu option, please try again."); break;
            }

            if (running)
            {
                Console.WriteLine();
                Console.WriteLine("Press Enter to return to the menu...");
                Console.ReadLine();
            }
        }

        Console.WriteLine("Goodbye!");
    }

    private static void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("===== Contoso Pets =====");
        Console.WriteLine("1. Show info for all pets");
        Console.WriteLine("2. Add a new animal");
        Console.WriteLine("3. Record a vet checkup (age, breed, neutered/spayed, condition)");
        Console.WriteLine("4. Give an animal a name");
        Console.WriteLine("5. Edit an animal's personality or description");
        Console.WriteLine("6. Search for a pet for a customer");
        Console.WriteLine("7. Exit");
    }

    // ---------- Menu actions ----------

    private static void ListAllPets(List<Pets> pets)
    {
        Console.WriteLine($"We currently have {pets.Count} animals:");
        foreach (Pets pet in pets)
        {
            Console.WriteLine();
            pet.DisplayInfo();
        }
    }

    private static void AddNewAnimal(List<Pets> pets)
    {
        // I only ask for what we can see on the day of arrival. The vet fills in the rest later.
        string species = ReadText("Species (e.g. Dog or Cat): ");
        string description = ReadText("Physical description (size, colour, coat...): ");
        string personality = ReadText("Personality so far: ");

        // Nobody can be removed from the list, so Count + 1 is always a new, unused ID.
        int newId = pets.Count + 1;
        pets.Add(new Pets(newId, species, description, personality, DateTime.Today));

        Console.WriteLine($"Added with ID {newId}. A vet checkup is pending and it can be named after the waiting period.");
    }

    private static void RecordVetCheckup(List<Pets> pets)
    {
        Pets? pet = FindPetById(pets);
        if (pet == null)
        {
            return;
        }

        int age = ReadInt("Age in years (as confirmed by the vet): ", 0, 40);
        string breed = ReadText("Breed: ");
        bool isNeutered = ReadYesNo("Neutered/spayed? (y/n): ");
        string condition = ReadText("Physical condition (e.g. Healthy, needs dental work): ");

        pet.RecordVetCheckup(age, breed, isNeutered, condition);
        Console.WriteLine("Vet checkup recorded.");
    }

    private static void NameAnimal(List<Pets> pets)
    {
        Pets? pet = FindPetById(pets);
        if (pet == null)
        {
            return;
        }

        // I check the waiting period before asking for a name, so the user doesn't type one for nothing.
        if (!pet.CanBeNamed())
        {
            Console.WriteLine($"This animal is new and can't be named yet. Try again in {pet.DaysUntilCanBeNamed()} days.");
            return;
        }

        string name = ReadText("New name: ");
        if (pet.GiveName(name))
        {
            Console.WriteLine($"The animal is now called {name.Trim()}.");
        }
    }

    private static void EditAnimal(List<Pets> pets)
    {
        Pets? pet = FindPetById(pets);
        if (pet == null)
        {
            return;
        }

        Console.WriteLine("What do you want to edit?");
        Console.WriteLine("1. Personality");
        Console.WriteLine("2. Physical description");
        int choice = ReadInt("Choice: ", 1, 2);

        if (choice == 1)
        {
            pet.UpdatePersonality(ReadText("New personality: "));
            Console.WriteLine("Personality updated.");
        }
        else
        {
            pet.UpdateDescription(ReadText("New physical description: "));
            Console.WriteLine("Description updated.");
        }
    }

    private static void SearchForCustomer(List<Pets> pets)
    {
        string species = ReadText("Which animal does the customer want? (Dog or Cat): ");
        string attributes = ReadText("What should it look like? (e.g. small brown short-haired): ");

        // I count the matches so I can tell the user when nothing was found.
        int matches = 0;
        foreach (Pets pet in pets)
        {
            if (pet.MatchesSearch(species, attributes))
            {
                Console.WriteLine();
                pet.DisplayInfo();
                matches++;
            }
        }

        Console.WriteLine();
        Console.WriteLine(matches == 0
            ? "Sorry, no animals match that description."
            : $"Found {matches} matching animal(s).");
    }

    // ---------- Input helpers ----------
    // I put input handling here once so every menu action can reuse it.

    // Console.ReadLine() can return null, so "?? """ turns that into an empty string.
    // I keep asking until the user types something that isn't blank.
    private static string ReadText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = (Console.ReadLine() ?? "").Trim();
            if (input != "")
            {
                return input;
            }
            Console.WriteLine("Please enter a value.");
        }
    }

    // int.TryParse returns false instead of crashing when the text isn't a number.
    private static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            string input = ReadText(prompt);
            if (int.TryParse(input, out int number) && number >= min && number <= max)
            {
                return number;
            }
            Console.WriteLine($"Please enter a whole number from {min} to {max}.");
        }
    }

    private static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            string input = ReadText(prompt).ToLower();
            if (input == "y" || input == "yes") return true;
            if (input == "n" || input == "no") return false;
            Console.WriteLine("Please answer y or n.");
        }
    }

    // I return null when the ID doesn't exist. The "?" on Pets? says that null is a possible result.
    private static Pets? FindPetById(List<Pets> pets)
    {
        int id = ReadInt("Enter the pet's ID: ", 1, int.MaxValue);
        foreach (Pets pet in pets)
        {
            if (pet.HasId(id))
            {
                return pet;
            }
        }

        Console.WriteLine($"No animal with ID {id} was found.");
        return null;
    }

    // ---------- Starting data ----------

    // I create every animal the same way a real one arrives: intake first, then vet checkup, then name.
    // The arrival dates are counted back from today, so the naming waiting period always works out the same.
    private static void LoadDataset(List<Pets> pets)
    {
        Pets lola = new Pets(1, "Dog", "Medium-sized golden retriever mix with a cream coat.", "Friendly and playful, loves fetch.", DateTime.Today.AddDays(-120));
        lola.RecordVetCheckup(2, "Golden retriever mix", true, "Healthy");
        lola.GiveName("Lola");
        pets.Add(lola);

        Pets max = new Pets(2, "Dog", "Large black short-haired dog.", "Calm and loyal, great with kids.", DateTime.Today.AddDays(-200));
        max.RecordVetCheckup(5, "Labrador retriever", true, "Healthy");
        max.GiveName("Max");
        pets.Add(max);

        Pets snowball = new Pets(3, "Cat", "Small white short-haired kitten with blue eyes.", "Curious and energetic.", DateTime.Today.AddDays(-60));
        snowball.RecordVetCheckup(1, "Domestic shorthair", true, "Healthy");
        snowball.GiveName("Snowball");
        pets.Add(snowball);

        Pets tiger = new Pets(4, "Cat", "Medium orange tabby with striped markings.", "Independent but affectionate once comfortable.", DateTime.Today.AddDays(-90));
        tiger.RecordVetCheckup(4, "Domestic shorthair", true, "Healthy, slightly overweight");
        tiger.GiveName("Tiger");
        pets.Add(tiger);

        Pets coco = new Pets(5, "Rabbit", "Small brown lop-eared rabbit.", "Gentle and quiet, enjoys being petted.", DateTime.Today.AddDays(-45));
        coco.RecordVetCheckup(3, "Holland lop", true, "Healthy");
        coco.GiveName("Coco");
        pets.Add(coco);

        Pets kiwi = new Pets(6, "Parrot", "Small green parakeet with yellow head feathers.", "Chatty and social, mimics sounds.", DateTime.Today.AddDays(-300));
        kiwi.RecordVetCheckup(7, "Budgerigar", false, "Healthy");
        kiwi.GiveName("Kiwi");
        pets.Add(kiwi);

        Pets bella = new Pets(7, "Dog", "Small brown and white short-haired dog with floppy ears.", "Very sociable, loves treats.", DateTime.Today.AddDays(-35));
        bella.RecordVetCheckup(3, "Beagle", false, "Healthy, needs a dental cleaning");
        bella.GiveName("Bella");
        pets.Add(bella);

        Pets smokey = new Pets(8, "Cat", "Medium grey short-haired cat with green eyes.", "Shy at first, then very cuddly.", DateTime.Today.AddDays(-70));
        smokey.RecordVetCheckup(6, "Russian blue mix", true, "Healthy");
        smokey.GiveName("Smokey");
        pets.Add(smokey);

        // Has been with us long enough and has seen the vet, but hasn't been named yet.
        Pets husky = new Pets(9, "Dog", "Large grey and white long-haired dog with blue eyes.", "Energetic, needs lots of exercise.", DateTime.Today.AddDays(-40));
        husky.RecordVetCheckup(4, "Siberian husky", true, "Healthy");
        pets.Add(husky);

        // Seen by the vet but still inside the naming waiting period.
        Pets orangeCat = new Pets(10, "Cat", "Small white and orange short-haired cat with a short tail.", "Playful, a bit nervous around dogs.", DateTime.Today.AddDays(-20));
        orangeCat.RecordVetCheckup(2, "Domestic shorthair", false, "Recovering from an ear infection");
        pets.Add(orangeCat);

        // Brand new arrivals: no vet checkup and no name yet.
        pets.Add(new Pets(11, "Dog", "Small brown short-haired dog with white paws.", "Nervous, hides behind people.", DateTime.Today.AddDays(-10)));
        pets.Add(new Pets(12, "Cat", "Large black long-haired cat with green eyes.", "Calm, likes to sleep in the sun.", DateTime.Today.AddDays(-5)));
    }
}
