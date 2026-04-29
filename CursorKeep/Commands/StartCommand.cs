using CursorKeep.Commands;
using CursorKeep.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursorKeep.Commands
{
    public class StartCommand : IMoveCommand
    {
        private readonly MouseMoverService _mouseMoverService;

        public StartCommand(MouseMoverService mouseMoverService)
        {
            _mouseMoverService = mouseMoverService;
        }

        public void Execute()
        {
            _mouseMoverService.Start();
        }
    }
}
