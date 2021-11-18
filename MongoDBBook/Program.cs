using System;
using EFMongo.Services;
using EFMongo.Models;

namespace EFMongo
{
  class Program
  {
    static void Main(string[] args)
    {
      var bookService = new BookService();

      while (true)
      {
        var str = System.Console.ReadLine().ToUpper().Split(" ");

        switch (str[0])
        {
          case "1":
            bookService.Get().ForEach(Console.WriteLine);
            break;

          case "2":
            var bookRead = bookService.Get(str[1]);
            Console.WriteLine(bookRead);
            break;

          case "3":
            var bookInput = BookInput();
            bookService.Create(bookInput);
            break;

          case "4":
            var formats = bookService.GetFormats(str[1]);
            foreach (var format in formats)
            {
              Console.WriteLine(format);
            }
            break;

          case "X":
            return;
          default:
            Console.WriteLine("Command not known");
            break;
        }
      }
    }

    private static Book BookInput()
    {
      // Name, Price, Cat, Author
      Console.Write("Input book name: ");
      var name = Console.ReadLine();

      Console.Write("Input book price: ");
      var price = Convert.ToDecimal(Console.ReadLine());

      Console.Write("Input book category: ");
      var cat = Console.ReadLine();

      Console.Write("Input book Author: ");
      var author = Console.ReadLine();

      return new Book
      {
        BookName = name,
        Price = price,
        Category = cat,
        // Author = author,
        Published = DateTime.Now,
        Formats = new string[] { "paperback", "hardcover", "mp3" }
      };
    }
  }
}
