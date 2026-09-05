using System;
using System.Collections.Generic;

namespace ZooTicketSystem.Models
{
    public class Zoo
    {
        public string Name { get; set; }
        public List<Animal> Animals { get; set; }
        public int OpeningHour { get; set; }
        public int ClosingHour { get; set; }
        public List<DayOfWeek> ClosedDays { get; set; }

        public Zoo(string name, int openingHour, int closingHour)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название зоопарка не может быть пустым");
            
            if (openingHour < 0 || openingHour > 23)
                throw new ArgumentException("Час открытия должен быть от 0 до 23");
            
            if (closingHour < 0 || closingHour > 23)
                throw new ArgumentException("Час закрытия должен быть от 0 до 23");
            
            if (closingHour <= openingHour)
                throw new ArgumentException("Час закрытия должен быть позже часа открытия");

            Name = name;
            OpeningHour = openingHour;
            ClosingHour = closingHour;
            Animals = new List<Animal>();
            ClosedDays = new List<DayOfWeek> { DayOfWeek.Monday };
        }

        public void AddAnimal(Animal animal)
        {
            if (animal == null)
                throw new ArgumentException("Животное не может быть null");
            
            Animals.Add(animal);
        }

        public bool IsOpenOnDay(DateTime date)
        {
            DayOfWeek dayOfWeek = ConvertToDayOfWeek(date.DayOfWeek);
            return !ClosedDays.Contains(dayOfWeek);
        }

        private DayOfWeek ConvertToDayOfWeek(System.DayOfWeek systemDayOfWeek)
        {
            if (systemDayOfWeek == System.DayOfWeek.Monday) return DayOfWeek.Monday;
            if (systemDayOfWeek == System.DayOfWeek.Tuesday) return DayOfWeek.Tuesday;
            if (systemDayOfWeek == System.DayOfWeek.Wednesday) return DayOfWeek.Wednesday;
            if (systemDayOfWeek == System.DayOfWeek.Thursday) return DayOfWeek.Thursday;
            if (systemDayOfWeek == System.DayOfWeek.Friday) return DayOfWeek.Friday;
            if (systemDayOfWeek == System.DayOfWeek.Saturday) return DayOfWeek.Saturday;
            return DayOfWeek.Sunday;
        }

        public bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == System.DayOfWeek.Saturday || 
                   date.DayOfWeek == System.DayOfWeek.Sunday;
        }
    }
}

