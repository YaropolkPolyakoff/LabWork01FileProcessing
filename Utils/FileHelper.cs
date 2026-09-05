using System;
using System.Collections.Generic;
using System.IO;
using ZooTicketSystem.Models;

namespace ZooTicketSystem.Utils
{
    public class FileHelper
    {
        public static List<Animal> ReadAnimalsFromFile(string filePath)
        {
            List<Animal> animals = new List<Animal>();
            
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл с животными не найден: " + filePath);
            
            string[] lines = File.ReadAllLines(filePath);
            
            int i = 0;
            while (i < lines.Length)
            {
                string line = lines[i].Trim();
                
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    i++;
                    continue;
                }
                
                string[] parts = line.Split('|');
                
                if (parts.Length < 5)
                {
                    Console.WriteLine("Предупреждение: пропущена некорректная строка: " + line);
                    i++;
                    continue;
                }
                
                try
                {
                    string name = parts[0].Trim();
                    string species = parts[1].Trim();
                    string habitat = parts[2].Trim();
                    string description = parts[3].Trim();
                    int age = int.Parse(parts[4].Trim());
                    
                    Animal animal = new Animal(name, species, habitat, description, age);
                    animals.Add(animal);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при чтении животного: " + ex.Message);
                }
                
                i++;
            }
            
            return animals;
        }

        public static Customer ReadCustomerFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл с информацией о покупателе не найден: " + filePath);
            
            string[] lines = File.ReadAllLines(filePath);
            
            string name = "";
            int age = 0;
            string email = "";
            string phone = "";
            
            int i = 0;
            while (i < lines.Length)
            {
                string line = lines[i].Trim();
                
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    i++;
                    continue;
                }
                
                if (line.StartsWith("name=") || line.StartsWith("Name="))
                    name = line.Substring(5).Trim();
                else if (line.StartsWith("age=") || line.StartsWith("Age="))
                    age = int.Parse(line.Substring(4).Trim());
                else if (line.StartsWith("email=") || line.StartsWith("Email="))
                    email = line.Substring(6).Trim();
                else if (line.StartsWith("phone=") || line.StartsWith("Phone="))
                    phone = line.Substring(6).Trim();
                
                i++;
            }
            
            return new Customer(name, age, email, phone);
        }

        public static List<string> ReadPurchaseRequestFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл с запросом на покупку не найден: " + filePath);
            
            string[] lines = File.ReadAllLines(filePath);
            List<string> ticketTypes = new List<string>();
            
            int i = 0;
            while (i < lines.Length)
            {
                string line = lines[i].Trim();
                
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    i++;
                    continue;
                }
                
                ticketTypes.Add(line);
                i++;
            }
            
            return ticketTypes;
        }

        public static void WriteOutputToFile(string filePath, string content)
        {
            try
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                File.WriteAllText(filePath, content);
                Console.WriteLine("Результат сохранен в файл: " + filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при записи в файл: " + ex.Message);
                throw;
            }
        }

        public static void AppendToOutputFile(string filePath, string content)
        {
            try
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                File.AppendAllText(filePath, content + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при добавлении в файл: " + ex.Message);
                throw;
            }
        }
    }
}

