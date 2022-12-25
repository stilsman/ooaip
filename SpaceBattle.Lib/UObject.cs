using System;

namespace SpaceBattle.Lib {
    public interface IUObject {
        public void SetProperty(string key, Object property);
        public Object GetProperty(string key);
    }
}