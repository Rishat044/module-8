using System;
using System.Collections.Generic;

public interface ICommand
{
    void Execute();
    void Undo();
}


public class Light
{
    public void On() => Console.WriteLine("Light is ON");
    public void Off() => Console.WriteLine("Light is OFF");
}

public class Door
{
    public void Open() => Console.WriteLine("Door is OPEN");
    public void Close() => Console.WriteLine("Door is CLOSED");
}

public class Thermostat
{
    private int temperature = 20;
    
    public void IncreaseTemperature() => Console.WriteLine($"Temperature increased to {++temperature}");
    public void DecreaseTemperature() => Console.WriteLine($"Temperature decreased to {--temperature}");
}


public class LightOnCommand : ICommand
{
    private Light _light;
    
    public LightOnCommand(Light light) => _light = light;
    
    public void Execute() => _light.On();
    public void Undo() => _light.Off();
}

public class LightOffCommand : ICommand
{
    private Light _light;
    
    public LightOffCommand(Light light) => _light = light;
    
    public void Execute() => _light.Off();
    public void Undo() => _light.On();
}

public class DoorOpenCommand : ICommand
{
    private Door _door;
    
    public DoorOpenCommand(Door door) => _door = door;
    
    public void Execute() => _door.Open();
    public void Undo() => _door.Close();
}

public class DoorCloseCommand : ICommand
{
    private Door _door;
    
    public DoorCloseCommand(Door door) => _door = door;
    
    public void Execute() => _door.Close();
    public void Undo() => _door.Open();
}

public class IncreaseTemperatureCommand : ICommand
{
    private Thermostat _thermostat;
    
    public IncreaseTemperatureCommand(Thermostat thermostat) => _thermostat = thermostat;
    
    public void Execute() => _thermostat.IncreaseTemperature();
    public void Undo() => _thermostat.DecreaseTemperature();
}

public class DecreaseTemperatureCommand : ICommand
{
    private Thermostat _thermostat;
    
    public DecreaseTemperatureCommand(Thermostat thermostat) => _thermostat = thermostat;
    
    public void Execute() => _thermostat.DecreaseTemperature();
    public void Undo() => _thermostat.IncreaseTemperature();
}

public class RemoteControl
{
    private ICommand _command;
    private Stack<ICommand> _commandHistory = new Stack<ICommand>();

    public void SetCommand(ICommand command) => _command = command;
    
    public void PressButton()
    {
        _command.Execute();
        _commandHistory.Push(_command);
    }

    public void PressUndo()
    {
        if (_commandHistory.Count > 0)
        {
            var command = _commandHistory.Pop();
            command.Undo();
        }
        else
        {
            Console.WriteLine("No commands to undo.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Light livingRoomLight = new Light();
        Door frontDoor = new Door();
        Thermostat thermostat = new Thermostat();

        ICommand lightOn = new LightOnCommand(livingRoomLight);
        ICommand lightOff = new LightOffCommand(livingRoomLight);
        ICommand doorOpen = new DoorOpenCommand(frontDoor);
        ICommand doorClose = new DoorCloseCommand(frontDoor);
        ICommand tempUp = new IncreaseTemperatureCommand(thermostat);
        ICommand tempDown = new DecreaseTemperatureCommand(thermostat);

        RemoteControl remote = new RemoteControl();

        remote.SetCommand(lightOn);
        remote.PressButton();
        remote.SetCommand(lightOff);
        remote.PressButton();
        remote.PressUndo();

        remote.SetCommand(doorOpen);
        remote.PressButton();
        remote.SetCommand(doorClose);
        remote.PressButton();
        remote.PressUndo();

    
        remote.SetCommand(tempUp);
        remote.PressButton();
        remote.PressButton();
        remote.PressUndo();
    }
}
//////////////////////////////////////////////////////////////////
///

using System;

public abstract class Beverage
{
    public void PrepareRecipe()
    {
        BoilWater();
        Brew();
        PourInCup();
        
        if (CustomerWantsCondiments())
        {
            AddCondiments();
        }
    }

    private void BoilWater() => Console.WriteLine("Boiling water");
    private void PourInCup() => Console.WriteLine("Pouring into cup");

    protected abstract void Brew();
    protected abstract void AddCondiments();

    protected virtual bool CustomerWantsCondiments()
    {
        return true;
    }
}


public class Tea : Beverage
{
    protected override void Brew() => Console.WriteLine("Steeping the tea");
    
    protected override void AddCondiments() => Console.WriteLine("Adding lemon");
}


public class Coffee : Beverage
{
    protected override void Brew() => Console.WriteLine("Dripping coffee through filter");

    protected override void AddCondiments() => Console.WriteLine("Adding sugar and milk");

    protected override bool CustomerWantsCondiments()
    {
        Console.Write("Would you like milk and sugar with your coffee (y/n)? ");
        string answer = Console.ReadLine()?.ToLower();
        return answer == "y";
    }
}


class Program
{
    static void Main(string[] args)
    {
        Beverage tea = new Tea();
        Beverage coffee = new Coffee();

        Console.WriteLine("Making tea...");
        tea.PrepareRecipe();

        Console.WriteLine("\nMaking coffee...");
        coffee.PrepareRecipe();
    }
}


/////////////////////////////////////////////////////////////
///

using System;
using System.Collections.Generic;

public interface IMediator
{
    void SendMessage(string message, User user); 
    void RegisterUser(User user); 
    void RemoveUser(User user); 
}

public class ChatRoom : IMediator
{
    private List<User> _users = new List<User>();

    public void RegisterUser(User user)
    {
        _users.Add(user);
        user.SetMediator(this);
        NotifyUsers($"{user.Name} присоединился к чату.");
    }

    public void RemoveUser(User user)
    {
        _users.Remove(user);
        NotifyUsers($"{user.Name} покинул чат.");
    }

    public void SendMessage(string message, User user)
    {
        foreach (var u in _users)
        {
            if (u != user)
            {
                u.ReceiveMessage(message, user);
            }
        }
    }

    private void NotifyUsers(string notification)
    {
        foreach (var user in _users)
        {
            user.ReceiveNotification(notification);
        }
    }
}

public abstract class User
{
    protected IMediator _mediator;
    public string Name { get; }

    public User(string name)
    {
        Name = name;
    }

    public void SetMediator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void Send(string message)
    {
        if (_mediator != null)
        {
            Console.WriteLine($"{Name} отправляет: {message}");
            _mediator.SendMessage(message, this);
        }
        else
        {
            Console.WriteLine($"{Name} не является участником чата.");
        }
    }

    public abstract void ReceiveMessage(string message, User sender);

    public abstract void ReceiveNotification(string notification);
}

public class ConcreteUser : User
{
    public ConcreteUser(string name) : base(name) {}

    public override void ReceiveMessage(string message, User sender)
    {
        Console.WriteLine($"{Name} получил сообщение от {sender.Name}: {message}");
    }

    public override void ReceiveNotification(string notification)
    {
        Console.WriteLine($"{Name} получил уведомление: {notification}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        ChatRoom chatRoom = new ChatRoom();

        User user1 = new ConcreteUser("Alice");
        User user2 = new ConcreteUser("Bob");
        User user3 = new ConcreteUser("Charlie");

        chatRoom.RegisterUser(user1);
        chatRoom.RegisterUser(user2);
        chatRoom.RegisterUser(user3);

        user1.Send("Привет всем!");
        user2.Send("Привет, Alice!");
        user3.Send("Привет, Alice и Bob!");

        chatRoom.RemoveUser(user2);

        user3.Send("Похоже, что Bob покинул чат.");
    }
}
