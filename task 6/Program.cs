using System;

public class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public string ShippingType { get; set; } = "";
    public bool IsValid { get; set; }
}

public abstract class Discount
{
    public abstract decimal Apply(decimal price);
}

public class PercentageDiscount : Discount
{
    private readonly decimal _percentage;

    public PercentageDiscount(decimal percentage)
    {
        if (percentage < 0 || percentage > 1)
            throw new ArgumentException("Percentage must be between 0 and 1.");

        _percentage = percentage;
    }

    public override decimal Apply(decimal price)
    {
        return price - (price * _percentage);
    }
}

public class NoDiscount : Discount
{
    public override decimal Apply(decimal price)
    {
        return price;
    }
}

public interface IShipping
{
    decimal CalculateCost();
}

public class StandardShipping : IShipping
{
    public decimal CalculateCost() => 5.99m;
}

public class ExpressShipping : IShipping
{
    public decimal CalculateCost() => 14.99m;
}

public class FreeShipping : IShipping
{
    public decimal CalculateCost() => 0m;
}

// Order Validation
public class OrderValidator
{
    public void Validate(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (order.TotalAmount <= 0)
        {
            order.IsValid = false;
            throw new ArgumentException("Order total must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(order.CustomerEmail) ||
            !order.CustomerEmail.Contains("@"))
        {
            order.IsValid = false;
            throw new ArgumentException("Invalid customer email address.");
        }

        order.IsValid = true;
    }
}

public class OrderRepository
{
    public void Save(Order order)
    {
        Console.WriteLine($"Saving order {order.Id} to database.");
    }
}

public class EmailService
{
    public void SendConfirmationEmail(Order order)
    {
        Console.WriteLine(
            $"Sending confirmation email to {order.CustomerEmail} for order {order.Id}.");
    }
}

// Invoice Generator
public class InvoiceGenerator
{
    public string Generate(Order order)
    {
        return $"Invoice for Order {order.Id}\n" +
               $"Customer: {order.CustomerEmail}\n" +
               $"Amount: {order.TotalAmount:C}";
    }
}

// Order Processor
public class OrderProcessor
{
    private readonly OrderValidator _validator;
    private readonly OrderRepository _repository;
    private readonly EmailService _emailService;
    private readonly InvoiceGenerator _invoiceGenerator;
    private readonly IShipping _shipping;

    public OrderProcessor(
        OrderValidator validator,
        OrderRepository repository,
        EmailService emailService,
        InvoiceGenerator invoiceGenerator,
        IShipping shipping)
    {
        _validator = validator;
        _repository = repository;
        _emailService = emailService;
        _invoiceGenerator = invoiceGenerator;
        _shipping = shipping;
    }

    public void ProcessOrder(Order order, Discount discount)
    {
        if (discount == null)
            throw new ArgumentNullException(nameof(discount));

        _validator.Validate(order);

        decimal shippingCost = _shipping.CalculateCost();
        decimal discountedTotal = discount.Apply(order.TotalAmount);
        decimal finalTotal = discountedTotal + shippingCost;

        _repository.Save(order);
        _emailService.SendConfirmationEmail(order);

        string invoice = _invoiceGenerator.Generate(order);

        Console.WriteLine(invoice);
        Console.WriteLine($"Shipping cost: {shippingCost:C}");
        Console.WriteLine($"Final total: {finalTotal:C}");
        Console.WriteLine($"Order {order.Id} processed successfully.");
    }
}


public class Program
{
    public static void Main()
    {
        Order order = new Order
        {
            Id = 1,
            CustomerEmail = "customer@example.com",
            TotalAmount = 100m,
            ShippingType = "Standard"
        };

        var validator = new OrderValidator();
        var repository = new OrderRepository();
        var emailService = new EmailService();
        var invoiceGenerator = new InvoiceGenerator();

        IShipping shipping = new StandardShipping();
        Discount discount = new PercentageDiscount(0.10m);

        var processor = new OrderProcessor(
            validator,
            repository,
            emailService,
            invoiceGenerator,
            shipping);

        try
        {
            processor.ProcessOrder(order, discount);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
