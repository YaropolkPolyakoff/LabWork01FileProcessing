using System;
using System.Collections.Generic;

namespace ZooTicketSystem.Models
{
    public class Zoo
    {
        public string Name { get; private set; }
        private readonly List<Animal> _animals;
        public IReadOnlyList<Animal> Animals => _animals;
        public int OpeningHour { get; private set; }
        public int ClosingHour { get; private set; }
        private readonly List<System.DayOfWeek> _closedDays;
        public IReadOnlyList<System.DayOfWeek> ClosedDays => _closedDays;

        public Zoo(string name, int openingHour, int closingHour)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Название зоопарка не может быть пустым");
            }
            
            if (openingHour < 0 || openingHour > 23)
            {
                throw new ArgumentException("Час открытия должен быть от 0 до 23");
            }
            
            if (closingHour < 0 || closingHour > 23)
            {
                throw new ArgumentException("Час закрытия должен быть от 0 до 23");
            }
            
            if (closingHour <= openingHour)
            {
                throw new ArgumentException("Час закрытия должен быть позже часа открытия");
            }

            Name = name;
            OpeningHour = openingHour;
            ClosingHour = closingHour;
            _animals = new List<Animal>();
            _closedDays = new List<System.DayOfWeek> { System.DayOfWeek.Monday };
        }

        public void AddAnimal(Animal animal)
        {
            if (animal == null)
            {
                throw new ArgumentException("Животное не может быть null");
            }
            
            _animals.Add(animal);
        }

        public bool IsOpenOnDay(DateTime date)
        {
            return !_closedDays.Contains(date.DayOfWeek);
        }

        public bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == System.DayOfWeek.Saturday || 
                   date.DayOfWeek == System.DayOfWeek.Sunday;
        }
    }

