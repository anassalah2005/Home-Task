using System;
using System.Collections.Generic;

public abstract class LibraryItem
{
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string ISBN { get; set; } = "";

    public abstract string GetDetails();
}

public class Book : LibraryItem
{
    public bool IsAvailable { get; set; }

    public override string GetDetails()
    {
        return $"Book: {Title} by {Author} — Available: {IsAvailable}";
    }
}

public class EBook : LibraryItem
{
    public double FileSizeMB { get; set; }

    public override string GetDetails()
    {
        return $"EBook: {Title} by {Author} — {FileSizeMB}MB";
    }
}

public class Member
{
    public string Name { get; set; }
    public int MemberId { get; set; }
    public List<Book> BorrowedBooks { get; private set; }

    public Member()
    {
        Name = "";
        BorrowedBooks = new List<Book>();
    }
}

public interface INotifier
{
    void Notify(string message);
}

public interface IBorrowable
{
    void Borrow(Member member, Book book);
    void Return(Member member, Book book);
}

public interface ISearchable
{
    List<Book> Search(string keyword);
}

public interface IBookRepository
{
    void Add(Book book);
    Book FindByISBN(string isbn);
    List<Book> GetAll();
}

public class BookRepository : IBookRepository, ISearchable
{
    private List<Book> books = new List<Book>();

    public void Add(Book book)
    {
        books.Add(book);
    }

    public Book FindByISBN(string isbn)
    {
        Book book = books.Find(b => b.ISBN == isbn);

        if (book == null)
        {
            throw new Exception("Book not found");
        }

        return book;
    }

    public List<Book> GetAll()
    {
        return new List<Book>(books);
    }

    public List<Book> Search(string keyword)
    {
        List<Book> result = new List<Book>();

        foreach (Book book in books)
        {
            if (book.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                book.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(book);
            }
        }

        return result;
    }
}

public class ConsoleNotifier : INotifier
{
    public void Notify(string message)
    {
        Console.WriteLine($"[Library] {message}");
    }
}

public class BorrowingService : IBorrowable
{
    private IBookRepository repository;
    private INotifier notifier;

    public BorrowingService(IBookRepository repository, INotifier notifier)
    {
        this.repository = repository;
        this.notifier = notifier;
    }

    public void Borrow(Member member, Book book)
    {
        if (member == null || book == null)
            throw new ArgumentNullException();

        if (book.IsAvailable == false)
        {
            notifier.Notify($"Book {book.Title} is not available");
            return;
        }

        book.IsAvailable = false;
        member.BorrowedBooks.Add(book);

        notifier.Notify($"{member.Name} borrowed {book.Title}");
    }

    public void Return(Member member, Book book)
    {
        if (member == null || book == null)
            throw new ArgumentNullException();

        if (member.BorrowedBooks.Contains(book) == false)
        {
            notifier.Notify("This book was not borrowed by this member");
            return;
        }

        member.BorrowedBooks.Remove(book);
        book.IsAvailable = true;

        notifier.Notify($"{member.Name} returned {book.Title}");
    }
}

public class EmailNotifier : INotifier
{
    public void Notify(string message)
    {
        Console.WriteLine($"[EMAIL] Sending: {message}");
    }
}

public class MultiNotifier : INotifier
{
    private List<INotifier> notifiers;

    public MultiNotifier(List<INotifier> notifiers)
    {
        this.notifiers = notifiers;
    }

    public void Notify(string message)
    {
        foreach (INotifier notifier in notifiers)
        {
            notifier.Notify(message);
        }
    }
}

public class Program
{
    public static void Main()
    {
        IBookRepository repo = new BookRepository();

        INotifier notifier = new MultiNotifier(new List<INotifier>
        {
            new ConsoleNotifier(),
            new EmailNotifier()
        });

        BorrowingService service = new BorrowingService(repo, notifier);

        repo.Add(new Book
        {
            Title = "Clean Code",
            Author = "Robert Martin",
            ISBN = "001",
            IsAvailable = true
        });

        repo.Add(new Book
        {
            Title = "The Pragmatic Programmer",
            Author = "Hunt & Thomas",
            ISBN = "002",
            IsAvailable = true
        });

        repo.Add(new Book
        {
            Title = "Design Patterns",
            Author = "Gang of Four",
            ISBN = "003",
            IsAvailable = true
        });

        List<LibraryItem> items = new List<LibraryItem>
        {
            repo.FindByISBN("001"),
            repo.FindByISBN("002"),
            new EBook
            {
                Title = "C# in Depth",
                Author = "Jon Skeet",
                ISBN = "E001",
                FileSizeMB = 3.7
            }
        };

        Member member = new Member
        {
            Name = "Ahmed",
            MemberId = 1
        };

        service.Borrow(member, repo.FindByISBN("001"));
        service.Borrow(member, repo.FindByISBN("002"));

        service.Return(member, repo.FindByISBN("001"));

        Console.WriteLine($"\n{member.Name}'s borrowed books:");

        foreach (Book book in member.BorrowedBooks)
        {
            Console.WriteLine(book.Title);
        }

        Console.WriteLine("\nSearch Results:");

        List<Book> results = repo.Search("programmer");

        foreach (Book book in results)
        {
            Console.WriteLine(book.Title);
        }

        Console.WriteLine("\nAll library items:");

        foreach (LibraryItem item in items)
        {
            Console.WriteLine(item.GetDetails());
        }
    }
}
