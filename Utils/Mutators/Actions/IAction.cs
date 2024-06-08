using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simpletodon.Utils.Mutators.Actions
{
    public interface IAction
    {
        ActionType ActionType { get; }
        void Execute();
        void Undo();
    }
}
