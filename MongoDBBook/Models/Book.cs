using System;
using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EFMongo.Models
{
  [BsonIgnoreExtraElements]
  public class Book : ISupportInitialize
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("Name")]
    public string BookName { get; set; }

    [BsonElement("Price")]
    public decimal Price { get; set; }

    [BsonElement("Category")]
    public string Category { get; set; }

    // [BsonElement("Author")]
    // public string Author { get; set; }

    public string AuthorFirstName { get; set; }
    public string AuthorLastName { get; set; }

    public int Pages { get; set; }

    public DateTime Published { get; set; }

    public String[] Formats { get; set; }

    public override string ToString()
    {
      return "Book(" +
          "Id: " + Id + "," +
          "BookName: " + BookName + "," +
          "Price: " + Price + "," +
          "Category: " + Category + "," +
          // "Author: " + Author + "," +
          "Author: " + AuthorFirstName + " - " + AuthorLastName + ", " +
          "Published: " + Published + "," +
          "Pages: " + Pages +
          ")";
    }

    [BsonExtraElements]
    public IDictionary<string, object> ExtraElements { get; set; }
    
    public void BeginInit()
    {
      
    }
    
    public void EndInit()
    {
      object authorValue;
      if (!ExtraElements.TryGetValue("Author", out authorValue))
      {
        return;
      }
    
      var author = (string) authorValue;
      ExtraElements.Remove("Author");
      var names = author.Split(" ");
      AuthorFirstName = names[0];
      AuthorLastName = names[1];
    }
  }
}