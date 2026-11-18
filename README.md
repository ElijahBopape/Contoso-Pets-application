# Contoso-Pets-application
An application that helps place pets in new homes.

A C# console app for a pet shelter. Staff can keep track of the animals in their care, record vet checkups, and help customers find a pet that matches what they're looking for.

## Features
- **View all pets:** see each animal's description, personality, how long it has been at the shelter, and its vet results.
- **Add a new animal:** record a new arrival with the details known on intake (species, physical description, personality).
- **Vet checkups:** age, breed, neutered/spayed status and physical condition stay *pending* until a vet confirms them.
- **Naming waiting period:** new arrivals have no name. An animal can only be named after it has been at the shelter for 30 days.
- **Edit animals:** update an animal's personality or physical description as it settles in.
- **Customer search:** find a dog or cat by physical attributes (for example `small brown`). An animal matches only when every word appears in its description or breed.

## Design
All pet data in the `Pets` class is private (encapsulation). The rest of the app can only work with a pet through public methods such as `GiveName()`, `RecordVetCheckup()` and `MatchesSearch()`, so rules like the naming waiting period can't be bypassed.

## Getting started
Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/ElijahBopape/Contoso-Pets-application.git
cd "Contoso-Pets-application/Contoso Pets Application"
dotnet run
```

## Project structure
```
Contoso Pets Application.slnx      Solution file
Contoso Pets Application/
├── Program.cs                     Menu, input handling and starting dataset
└── Pets.cs                        The Pets class
```

## Notes
Data is kept in memory only, so animals added or edited while the app runs are reset when it closes.
