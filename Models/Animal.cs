using System;

namespace ZooTicketSystem.Models
{
    /// <summary>
    /// Представляет животное в зоопарке
    /// </summary>
    public class Animal
    {
        public string Name { get; set; }
        public string Species { get; set; }
        public string Habitat { get; set; }
        public string Description { get; set; }
        public int Age { get; set; }

        public Animal(string name, string species, string habitat, string description, int age)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя животного не может быть пустым");
            
            if (string.IsNullOrWhiteSpace(species))
                throw new ArgumentException("Вид животного не может быть пустым");
            
            if (age < 0)
                throw new ArgumentException("Возраст животного не может быть отрицательным");

            Name = name;
            Species = species;
            Habitat = habitat ?? "Не указано";
            Description = description ?? "Описание отсутствует";
            Age = age;
        }

        public override string ToString()
        {
            return $"Животное: {Name}\n" +
                   $"  Вид: {Species}\n" +
                   $"  Возраст: {Age} лет\n" +
                   $"  Среда обитания: {Habitat}\n" +
                   $"  Описание: {Description}";
        }
    }
}

