using System;

namespace SpaceBattle.Lib {
    public interface IMoveCommandStartable {
        public IUObject uobject { get; }
        public ICommand command { get; }
        public IList <string> properties { get; }
    }
}