using CursorKeep.Services;

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
