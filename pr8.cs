using System;
using System.Collections.Generic;

public interface ICommand
{
    void Execute();
    void Undo();
}
public class Light
{
    public void On()
    {
        Console.WriteLine("Свет включен.");
    }

    public void Off()
    {
        Console.WriteLine("Свет выключен.");
    }
}
public class AirConditioner
{
    public void On()
    {
        Console.WriteLine("Кондиционер включен.");
    }

    public void Off()
    {
        Console.WriteLine("Кондиционер выключен.");
    }
}
public class TV
{
    public void On()
    {
        Console.WriteLine("Телевизор включен.");
    }

    public void Off()
    {
        Console.WriteLine("Телевизор выключен.");
    }
}
public class LightOnCommand : ICommand
{
    private Light _light;

    public LightOnCommand(Light light)
    {
        _light = light;
    }

    public void Execute()
    {
        _light.On();
    }

    public void Undo()
    {
        _light.Off();
    }
}
public class LightOffCommand : ICommand
{
    private Light _light;

    public LightOffCommand(Light light)
    {
        _light = light;
    }

    public void Execute()
    {
        _light.Off();
    }

    public void Undo()
    {
        _light.On();
    }
}
public class ACOnCommand : ICommand
{
    private AirConditioner _ac;

    public ACOnCommand(AirConditioner ac)
    {
        _ac = ac;
    }

    public void Execute()
    {
        _ac.On();
    }

    public void Undo()
    {
        _ac.Off();
    }
}
public class ACOffCommand : ICommand
{
    private AirConditioner _ac;

    public ACOffCommand(AirConditioner ac)
    {
        _ac = ac;
    }

    public void Execute()
    {
        _ac.Off();
    }

    public void Undo()
    {
        _ac.On();
    }
}
public class TVOnCommand : ICommand
{
    private TV _tv;

    public TVOnCommand(TV tv)
    {
        _tv = tv;
    }

    public void Execute()
    {
        _tv.On();
    }

    public void Undo()
    {
        _tv.Off();
    }
}
public class TVOffCommand : ICommand
{
    private TV _tv;

    public TVOffCommand(TV tv)
    {
        _tv = tv;
    }

    public void Execute()
    {
        _tv.Off();
    }

    public void Undo()
    {
        _tv.On();
    }
}
public class RemoteControl
{
    private ICommand[] _onCommands;
    private ICommand[] _offCommands;
    private Stack<ICommand> _commandHistory;

    public RemoteControl(int numberOfSlots)
    {
        _onCommands = new ICommand[numberOfSlots];
        _offCommands = new ICommand[numberOfSlots];
        _commandHistory = new Stack<ICommand>();
    }

    public void SetCommand(int slot, ICommand onCommand, ICommand offCommand)
    {
        _onCommands[slot] = onCommand;
        _offCommands[slot] = offCommand;
    }

    public void PressOnButton(int slot)
    {
        if (_onCommands[slot] != null)
        {
            _onCommands[slot].Execute();
            _commandHistory.Push(_onCommands[slot]);
        }
        else
        {
            Console.WriteLine("Команда не назначена на этот слот.");
        }
    }

    public void PressOffButton(int slot)
    {
        if (_offCommands[slot] != null)
        {
            _offCommands[slot].Execute();
            _commandHistory.Push(_offCommands[slot]);
        }
        else
        {
            Console.WriteLine("Команда не назначена на этот слот.");
        }
    }

    public void UndoLastCommand()
    {
        if (_commandHistory.Count > 0)
        {
            ICommand lastCommand = _commandHistory.Pop();
            lastCommand.Undo();
        }
        else
        {
            Console.WriteLine("Нет команд для отмены.");
        }
    }
}
public class MacroCommand : ICommand
{
    private List<ICommand> _commands;

    public MacroCommand(List<ICommand> commands)
    {
        _commands = commands;
    }

    public void Execute()
    {
        foreach (var command in _commands)
        {
            command.Execute();
        }
    }

    public void Undo()
    {
        for (int i = _commands.Count - 1; i >= 0; i--)
        {
            _commands[i].Undo();
        }
    }
}


class Program
{
    static void Main(string[] args)
    {
        RemoteControl remote = new RemoteControl(3);

        Light livingRoomLight = new Light();
        AirConditioner ac = new AirConditioner();
        TV tv = new TV();

        remote.SetCommand(0, new LightOnCommand(livingRoomLight), new LightOffCommand(livingRoomLight));
        remote.SetCommand(1, new ACOnCommand(ac), new ACOffCommand(ac));
        remote.SetCommand(2, new TVOnCommand(tv), new TVOffCommand(tv));

        remote.PressOnButton(0);
        remote.PressOffButton(0);
        remote.PressOnButton(1);
        remote.PressOffButton(1);
        remote.PressOnButton(2);
        remote.PressOffButton(2);

        remote.UndoLastCommand();
        remote.UndoLastCommand();

        var macro = new MacroCommand(new List<ICommand> {
            new LightOnCommand(livingRoomLight),
            new ACOnCommand(ac),
            new TVOnCommand(tv)
        });

        remote.SetCommand(0, macro, null);
        remote.PressOnButton(0);
        remote.UndoLastCommand();
    }
}
//////////////////////////////////////////////////
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
 
namespace ConsoleApp5
{
    public interface IMediator
    {
        void SendMessage(string message, IUser user);
        void AddUser(IUser user);
        void RemoveUser(IUser user);
    }
    public class ChatMediator : IMediator
    {
        private List<IUser> users;
        public ChatMediator()
        {
            users = new List<IUser>();
        }
        private void NotifyUsers(string message)
        {
            foreach (var user in users)
            {
                user.ReceiveSystemMessage(message);
            }
        }
        public void AddUser(IUser user)
        {
            users.Add(user);
            NotifyUsers(user.GetName() + " join to chat");
        }
        public void RemoveUser(IUser user)
        {
            users.Remove(user);
            NotifyUsers(user.GetName() + " left chat");
        }
        public void SendMessage(string message, IUser sender)
        {
            foreach (var user in users)
            {
                if(user!= sender)
                    user.ReceiveMessage(message, user);
            }
        }
    }
 
 
    public interface IUser
    {
        void SendMessage(string message);
        void ReceiveMessage(string message, IUser sender);
        void ReceiveSystemMessage(string message);
        string GetName();
    }
    public class User : IUser
    {
        private string name;
        private IMediator mediator;
        public User(string name, IMediator mediator)
        {
            this.name = name;
            this.mediator = mediator;
        }
        public string GetName()
        {
            return name;
        }
        public void SendMessage(string message)
        {
            Console.WriteLine("{0} send message: {1}", name, message);
            mediator.SendMessage(message, this);
        }
        public void ReceiveMessage(string message, IUser sender)
        {
            Console.WriteLine("{0} receive message from {1}: {2}",
                name, sender.GetName(), message);
        }
        public void ReceiveSystemMessage(string message)
        {
            Console.WriteLine("[System message for {0}]: {1}", name, message);
        }
 
    }
 
    internal class Program
    {
        static void Main(string[] args)
        {
            ChatMediator chat = new ChatMediator();
            IUser user1 = new User("User1", chat);
            IUser user2 = new User("User2", chat);
            IUser user3 = new User("User3", chat);
            IUser user4 = new User("User4", chat);
 
            chat.AddUser(user1);
            chat.AddUser(user2);
            chat.AddUser(user3);
            chat.AddUser(user4);
 
            user1.SendMessage("Hello all");
        }
    }
}

/////////////////////////////////////////////
///

using System;

public abstract class ReportGenerator
{
    public void GenerateReport()
    {
        GatherData();
        FormatData();
        CreateHeader();
        CreateContent();
        if (CustomerWantsSave())
        {
            SaveReport();
        }
        else
        {
            SendReport();
        }
    }

    protected abstract void GatherData();
    protected abstract void FormatData();
    protected abstract void CreateHeader();
    protected abstract void CreateContent();

    protected virtual bool CustomerWantsSave() => true;

    protected virtual void SaveReport()
    {
        Console.WriteLine("Отчет сохранен.");
    }

    protected virtual void SendReport()
    {
        Console.WriteLine("Отчет отправлен по электронной почте.");
    }
}

public class PdfReport : ReportGenerator
{
    protected override void GatherData()
    {
        Console.WriteLine("Сбор данных для PDF отчета.");
    }

    protected override void FormatData()
    {
        Console.WriteLine("Форматирование данных для PDF.");
    }

    protected override void CreateHeader()
    {
        Console.WriteLine("Создание заголовка для PDF отчета.");
    }

    protected override void CreateContent()
    {
        Console.WriteLine("Создание содержимого PDF отчета.");
    }
}

public class ExcelReport : ReportGenerator
{
    protected override void GatherData()
    {
        Console.WriteLine("Сбор данных для Excel отчета.");
    }

    protected override void FormatData()
    {
        Console.WriteLine("Форматирование данных для Excel.");
    }

    protected override void CreateHeader()
    {
        Console.WriteLine("Создание заголовка для Excel отчета.");
    }

    protected override void CreateContent()
    {
        Console.WriteLine("Создание содержимого Excel отчета.");
    }

    protected override void SaveReport()
    {
        Console.WriteLine("Отчет Excel сохранен.");
    }
}

public class HtmlReport : ReportGenerator
{
    protected override void GatherData()
    {
        Console.WriteLine("Сбор данных для HTML отчета.");
    }

    protected override void FormatData()
    {
        Console.WriteLine("Форматирование данных для HTML.");
    }

    protected override void CreateHeader()
    {
        Console.WriteLine("Создание заголовка для HTML отчета.");
    }

    protected override void CreateContent()
    {
        Console.WriteLine("Создание содержимого HTML отчета.");
    }

    protected override bool CustomerWantsSave()
    {
        Console.WriteLine("Вы хотите сохранить отчет? (да или нет): ");
        string response = Console.ReadLine().ToLower();
        return response == "да";
    }
}



class Program
{
    static void Main(string[] args)
    {
        ReportGenerator pdfReport = new PdfReport();
        pdfReport.GenerateReport();

        Console.WriteLine();

        ReportGenerator excelReport = new ExcelReport();
        excelReport.GenerateReport();

        Console.WriteLine();

        ReportGenerator htmlReport = new HtmlReport();
        htmlReport.GenerateReport();
    }
}
