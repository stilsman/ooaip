using System;
using Hwdtech;

namespace SpaceBattle.Lib {
    public class StartMoveCommand : ICommand {
        private IMoveCommandStartable _commandStartable;

        public StartMoveCommand(IMoveCommandStartable commandStartable) {
            _commandStartable = commandStartable;
        }

        public void Execute() {
            var _uobject = _commandStartable.uobject;
            var _command = _commandStartable.command;
            var _properties = _commandStartable.properties;

            IoC.Resolve<Hwdtech.ICommand>("Game.Object.SetProperties", _uobject, _properties).Execute();
            var command = IoC.Resolve<Hwdtech.ICommand>("Game.Operations.Command", _command);
            IoC.Resolve<Hwdtech.ICommand>("Game.Queue.Push", command).Execute();
        }
    }
}