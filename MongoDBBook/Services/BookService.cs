using System;
using System.Collections.Generic;
using System.Linq;
using EFMongo.Models;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace EFMongo.Services
{
  public class BookService
  {
    private IMongoCollection<Book> _books;
    private readonly string connectionString = "mongodb://localhost:27017";
    private readonly string bookStoreDB = "BookstoreDb";
    private readonly string collection = "Books";

    public BookService()
    {
      var client = new MongoClient(connectionString);
      var database = client.GetDatabase(bookStoreDB);
      _books = database.GetCollection<Book>(collection);
    }

    public List<Book> Get()
    {
      // using (var cursor = _books.Find(book => true).ToCursor())
      // {
      //   // Do stuff with cursor
      // }

      // var cursor1 = _books.Find(book => true).ToCursor();
      //
      // // Do Stuff with cursor
      // cursor1.Dispose();
      

      return _books.Find(book => true).ToList();
    }

    public Book Get(string id)
    {
      return _books.Find<Book>(book => book.Id == id).FirstOrDefault();
    }

    public IEnumerable<string> GetFormats(string id)
    {
      var projection = Builders<Book>.Projection.Include(b => b.Formats);

      var bson = _books.Find<Book>(book => book.Id == id).Project(projection).FirstOrDefault();
      var array = bson.GetElement("Formats").Value.AsBsonArray;
      
      return array.Select(str => str.AsString);
      // return BsonSerializer.Deserialize<Book>(bson);
      // For complex objects use BsonSerializer.Deserialize<ComplexObject>(str)
    }

    public Book Create(Book book)
    {
      _books.InsertOne(book);
      return book;
    }

    public void Update(string id, Book bookIn)
    {
      _books.ReplaceOne(book => book.Id == id, bookIn);
    }

    public void Remove(Book bookIn)
    {
      _books.DeleteOne(book => book.Id == bookIn.Id);
    }

    public void Remove(string id)
    {
      _books.DeleteOne(book => book.Id == id);
    }

  }
}