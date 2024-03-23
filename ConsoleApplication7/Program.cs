using System;
using System.Collections.Generic;
using System.Linq;

public interface IHazardNotifier
{
    void NotifyDangerousEvent(string containerNumber);
}

public class OverfillException : Exception
{
    public OverfillException(string message) : base(message) { }
}

public class Container
{
    public string ContainerNumber { get; private set; }
    public double LoadCapacity { get; private set; }
    public double Weight { get; protected set; }
    public double EmptyWeight { get; private set; }

    public Container(string containerNumber, double loadCapacity, double emptyWeight)
    {
        ContainerNumber = containerNumber;
        LoadCapacity = loadCapacity;
        EmptyWeight = emptyWeight;
        Weight = emptyWeight;
    }

    public virtual void Load(double cargoWeigth)
    {
        if (cargoWeigth > LoadCapacity)
            throw new OverfillException("Cargo weight exceeds container capacity");
        Weight = cargoWeigth + EmptyWeight;
    }

    public virtual void Empty()
    {
        Weight = EmptyWeight;
    }
    public void NotifyDangerousEvent(string containerNumber)
    {
        Console.WriteLine($"Dangerous event detected in container {containerNumber}");
    }
} 
public class LiquidContainer : Container, IHazardNotifier
{
    public double Pressure { get; private set; }

    public LiquidContainer(string containerNumber, double loadCapacity, double emptyWeight, double pressure)
        : base(containerNumber, loadCapacity, emptyWeight)
    {
        Pressure = pressure;
    }

    public override void Load(double cargoWeight)
    {
        if (Pressure > 0 && cargoWeight > LoadCapacity * 0.5)
            NotifyDangerousEvent(ContainerNumber);

        base.Load(cargoWeight);
    }

    public override void Empty()
    {
        base.Empty();
        Weight *= 0.05;
    }

    public void NotifyDangerousEvent(string containerNumber)
    {
        Console.WriteLine($"Dangerous event detected in container {containerNumber}");
    }
}

public class GasContainer : Container, IHazardNotifier
{
    public double Pressure { get; private set; }

    public GasContainer(string containerNumber, double loadCapacity, double emptyWeight, double pressure)
        : base(containerNumber, loadCapacity, emptyWeight)
    {
        Pressure = pressure;
    }

    public override void Load(double cargoWeight)
    {
        if (cargoWeight > LoadCapacity * 0.5)
            NotifyDangerousEvent(ContainerNumber);

        base.Load(cargoWeight);
    }
    public override void Empty()
    {
        base.Empty();
        Weight *= 0.05;
    }

    public void NotifyDangerousEvent(string containerNumber)
    {
        Console.WriteLine($"Dangerous event detected in container {containerNumber}");
    }
}

public class RefrigeratedContainer : Container
{
    public double Temperature { get; private set; }
    public string ProductType { get; private set; }

    public RefrigeratedContainer(string containerNumber, double loadCapacity, double emptyWeight, double temperature, string productType)
        : base(containerNumber, loadCapacity, emptyWeight)
    {
        Temperature = temperature;
        ProductType = productType;
    }
}

public class ContainerShip
{
    private List<Container> containers = new List<Container>();

    public double MaxSpeed { get; private set; }
    public int MaxContainers { get; private set; }
    public double MaxWeight { get; private set; }

    public ContainerShip(double maxSpeed, int maxContainers, double maxWeight)
    {
        MaxSpeed = maxSpeed;
        MaxContainers = maxContainers;
        MaxWeight = maxWeight;
    }

    public void LoadContainer(Container container)
    {
        if (containers.Count >= MaxContainers)
            throw new InvalidOperationException("Maximum number of containers reached");

        double currentWeight = containers.Sum(c => c.Weight);
        if (currentWeight + container.Weight > MaxWeight)
            throw new InvalidOperationException("Exceeds maximum weight limit");

        containers.Add(container);
    }

    public void UnloadContainer(string containerNumber)
    {
        Container container = containers.Find(c => c.ContainerNumber == containerNumber);
        if (container != null)
            containers.Remove(container);
    }

    public void ReplaceContainer(string oldContainerNumber, Container newContainer)
    {
        int index = containers.FindIndex(c => c.ContainerNumber == oldContainerNumber);
        if (index != -1)
        {
            containers[index] = newContainer;
        }
    }

    public void PrintContainerInfo(string containerNumber)
    {
        Container container = containers.Find(c => c.ContainerNumber == containerNumber);
        if (container != null)
        {
            Console.WriteLine($"Container Number: {container.ContainerNumber}");
            Console.WriteLine($"Load Capacity: {container.LoadCapacity}");
            Console.WriteLine($"Weight: {container.Weight}");
        }
        else
        {
            Console.WriteLine("Container not found");
        }
    }

    public void PrintShipInfo()
    {
        Console.WriteLine($"Max Speed: {MaxSpeed}");
        Console.WriteLine($"Max Containers: {MaxContainers}");
        Console.WriteLine($"Max Weight: {MaxWeight}");
        Console.WriteLine($"Current Weight: {containers.Sum(c => c.Weight)}");
    }

    public void TransferContainerBetweenShips(ContainerShip targetShip, string containerNumber)
    {
        Container container = containers.Find(c => c.ContainerNumber == containerNumber);
        if (container != null)
        {
            targetShip.LoadContainer(container);
            containers.Remove(container);
            Console.WriteLine($"Container {containerNumber} transferred to the target ship.");
        }
        else
        {
            Console.WriteLine($"Container {containerNumber} not found on this ship.");
        }
    }

    public List<Container> GetContainers()
    {
        return containers;
    }
}
class Program
{
    static void Main(string[] args)
    {
        ContainerShip ship = new ContainerShip(25.5, 100, 50000);

        while (true)
        {
            Console.WriteLine("Choose an action:");
            Console.WriteLine("1. Create Container");
            Console.WriteLine("2. Load Cargo into Container");
            Console.WriteLine("3. Load Container onto Ship"); 
            Console.WriteLine("5. Unload Container from Ship");
            Console.WriteLine("7. Replace Container on Ship");
            Console.WriteLine("8. Transfer Container between Ships");
            Console.WriteLine("9. Print Container Info");
            Console.WriteLine("10. Print Ship Info");
            Console.WriteLine("11. Exit");

            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    Console.WriteLine("Enter container type (Liquid, Gas, Refrigerated):");
                    string containerType = Console.ReadLine();
                    Console.WriteLine("Enter container number:");
                    string containerNumber = Console.ReadLine();
                    Console.WriteLine("Enter load capacity:");
                    double loadCapacity = Convert.ToDouble(Console                        .ReadLine());
                    Console.WriteLine("Enter empty weight:");
                    double emptyWeight = Convert.ToDouble(Console.ReadLine());
                    if (containerType.ToLower() == "liquid")
                    {
                        Console.WriteLine("Enter pressure:");
                        double pressure = Convert.ToDouble(Console.ReadLine());
                        LiquidContainer liquidContainer = new LiquidContainer(containerNumber, loadCapacity, emptyWeight, pressure);
                        ship.LoadContainer(liquidContainer);
                    }
                    else if (containerType.ToLower() == "gas")
                    {
                        Console.WriteLine("Enter pressure:");
                        double pressure = Convert.ToDouble(Console.ReadLine());
                        GasContainer gasContainer = new GasContainer(containerNumber, loadCapacity, emptyWeight, pressure);
                        ship.LoadContainer(gasContainer);
                    }
                    else if (containerType.ToLower() == "refrigerated")
                    {
                        Console.WriteLine("Enter temperature:");
                        double temperature = Convert.ToDouble(Console.ReadLine());
                        Console.WriteLine("Enter product type:");
                        string productType = Console.ReadLine();
                        RefrigeratedContainer refrigeratedContainer = new RefrigeratedContainer(containerNumber, loadCapacity, emptyWeight, temperature, productType);
                        ship.LoadContainer(refrigeratedContainer);
                    }
                    else
                    {
                        Console.WriteLine("Invalid container type.");
                    }
                    break;
                case 2:
                    Console.WriteLine("Enter container number:");
                    string containerNum = Console.ReadLine();
                    Container selectedContainer = ship.GetContainers().Find(c => c.ContainerNumber == containerNum);
                    if (selectedContainer != null)
                    {
                        Console.WriteLine("Enter cargo weight:");
                        double cargoWeight = Convert.ToDouble(Console.ReadLine());
                        selectedContainer.Load(cargoWeight);
                    }
                    else
                    {
                        Console.WriteLine($"Container {containerNum} not found on the ship.");
                    }
                    break;
                case 3:
                    Console.WriteLine("Enter container number:");
                    string containerNumLoad = Console.ReadLine();
                    Container selectedContainerLoad = ship.GetContainers().Find(c => c.ContainerNumber == containerNumLoad);
                    if (selectedContainerLoad != null)
                    {
                        ship.LoadContainer(selectedContainerLoad);
                    }
                    else
                    {
                        Console.WriteLine($"Container {containerNumLoad} not found on the ship.");
                    }
                    break;
                case 5:
                    Console.WriteLine("Enter container number:");
                    string containerNumUnload = Console.ReadLine();
                    ship.UnloadContainer(containerNumUnload);
                    break;
                case 7:
                    Console.WriteLine("Enter container number to replace:");
                    string oldContainerNumber = Console.ReadLine();
                    Console.WriteLine("Enter new container number:");
                    string newContainerNumber = Console.ReadLine();
                    Console.WriteLine("Enter new container type (Liquid, Gas, Refrigerated):");
                    string newContainerType = Console.ReadLine();
                    Console.WriteLine("Enter new container load capacity:");
                    double newLoadCapacity = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Enter new container empty weight:");
                    double newEmptyWeight = Convert.ToDouble(Console.ReadLine());
                    if (newContainerType.ToLower() == "liquid")
                    {
                        Console.WriteLine("Enter new container pressure:");
                        double newPressure = Convert.ToDouble(Console.ReadLine());
                        LiquidContainer newLiquidContainer = new LiquidContainer(newContainerNumber, newLoadCapacity, newEmptyWeight, newPressure);
                        ship.ReplaceContainer(oldContainerNumber, newLiquidContainer);
                    }
                    else if (newContainerType.ToLower() == "gas")
                    {
                        Console.WriteLine("Enter new container pressure:");
                        double newPressure = Convert.ToDouble(Console.ReadLine());
                        GasContainer newGasContainer = new GasContainer(newContainerNumber, newLoadCapacity, newEmptyWeight, newPressure);
                        ship.ReplaceContainer(oldContainerNumber, newGasContainer);
                    }
                    else if (newContainerType.ToLower() == "refrigerated")
                    {
                        Console.WriteLine("Enter new container temperature:");
                        double newTemperature = Convert.ToDouble(Console.ReadLine());
                        Console.WriteLine("Enter new container product type:");
                        string newProductType = Console.ReadLine();
                        RefrigeratedContainer newRefrigeratedContainer = new RefrigeratedContainer(newContainerNumber, newLoadCapacity, newEmptyWeight, newTemperature, newProductType);
                        ship.ReplaceContainer(oldContainerNumber, newRefrigeratedContainer);
                    }
                    else
                    {
                        Console.WriteLine("Invalid container type.");
                    }
                    break;
                case 8:
                    Console.WriteLine("Enter container number to transfer:");
                    string containerNumTransfer = Console.ReadLine();
                    Console.WriteLine("Enter target ship's max speed:");
                    double targetMaxSpeed = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Enter target ship's max containers:");
                    int targetMaxContainers = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter target ship's max weight:");
                    double targetMaxWeight = Convert.ToDouble(Console.ReadLine());

                    ContainerShip targetShip = new ContainerShip(targetMaxSpeed, targetMaxContainers, targetMaxWeight);
                    ship.TransferContainerBetweenShips(targetShip, containerNumTransfer);
                    break;
                case 9:
                    Console.WriteLine("Enter container number:");
                    string containerNumInfo = Console.ReadLine();
                    ship.PrintContainerInfo(containerNumInfo);
                    break;
                case 10:
                    ship.PrintShipInfo();
                    break;
                case 11:
                    Console.WriteLine("Exiting program...");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please enter a number between 1 and 11.");
                    break;
            }
        }
    }
}


   