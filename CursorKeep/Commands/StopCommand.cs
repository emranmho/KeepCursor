using CursorKeep.Services;

namespace CursorKeep.Commands
{
    public class StopCommand : IMoveCommand
    {
        private readonly MouseMoverService _mouseMoverService;

        public StopCommand(MouseMoverService mouseMoverService)
        {
            _mouseMoverService = mouseMoverService;
        }

        public void Execute()
        {
            _mouseMoverService.Stop();
        }
    }
}
