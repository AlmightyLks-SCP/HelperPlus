using MEC;
using Synapse.Command;
using System.Collections.Generic;

namespace HelperPlus.Commands
{
    [CommandInformation(
        Name = "phelper",
        Aliases = new string[0],
        Description = "Toggle a UI for plugin debugging",
        Permission = "helperplus.phelper",
        Platforms = new[] { Platform.RemoteAdmin },
        Usage = "\"phelper\" to toggle UI"
        )]
    public class HelperCommand : ISynapseCommand
    {
        private HelperPlus _plugin;
        public HelperCommand(HelperPlus plugin)
        {
            _plugin = plugin;
        }

        public CommandResult Execute(CommandContext context)
        {
            CommandResult result = new CommandResult();
            result.State = CommandResultState.Ok;
            CoroutineHandle handle;

            if (_plugin.PlayerUIHandles.TryGetValue(context.Player, out handle))
            {
                _plugin.PlayerUIHandles.Remove(context.Player);
                Timing.KillCoroutines(handle);
            }
            else
            {
                handle = Timing.RunCoroutine(_plugin.UICoroutine(context.Player));
                _plugin.PlayerUIHandles.Add(context.Player, handle);
            }

            return result;
        }
    }
}
