using CursorKeep.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CursorKeep.Controllers
{
    public class AppController
    {
        private readonly IMoveCommand _startCommand;
        private readonly IMoveCommand _stopCommand;
        private bool _isMoving;
        public AppController(IMoveCommand startCommand, IMoveCommand stopCommand)
        {
            _startCommand = startCommand;
            _stopCommand = stopCommand;
            _isMoving = false;
        }

        public void Start()
        {
            _startCommand.Execute();
            _isMoving = true;
        }

        public void Stop()
        {
            _stopCommand.Execute();
            _isMoving = false;
        }

        public bool IsMoving => _isMoving;
    }
}
