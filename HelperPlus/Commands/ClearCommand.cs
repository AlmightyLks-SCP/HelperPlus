using MEC;
using Synapse.Api;
using Synapse.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelperPlus.Commands
{
    [CommandInformation(
        Name = "clear",
        Aliases = new string[0],
        Description = "Clear several things off the server, as a clean-up utility",
        Permission = "helperplus.clear",
        Platforms = new[] { Platform.RemoteAdmin },
        Usage = "\"clear [all/ragdolls/items/dummies]\""
        )]
    public class ClearCommand : ISynapseCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            CommandResult result = new CommandResult();
            List<string> args = context.Arguments.Array.ToList();

            if (args.Count == 1)
            {
                result.Message = "Please add a specification on what to clear";
                result.State = CommandResultState.Error;
                return result;
            }

            switch (args[1].ToLower())
            {
                case "all":
                    {
                        int beforeRagdolls = Map.Get.Ragdolls.Count;
                        int beforeItems = Map.Get.Items.Count;
                        int beforeDummies = Map.Get.Dummies.Count;
                        ClearRagdolls();
                        ClearItems();
                        ClearDummies();
                        int afterRagdolls = Map.Get.Ragdolls.Count;
                        int afterItems = Map.Get.Items.Count;
                        int afterDummies = Map.Get.Dummies.Count;

                        result.Message = $"Removed {beforeRagdolls - afterRagdolls} ragdolls\n" +
                            $"Removed {beforeDummies - afterDummies} dummies\n" +
                            $"Removed {beforeItems - afterItems} items";
                        result.State = CommandResultState.Ok;
                        break;
                    }
                case "ragdolls":
                    {
                        int beforeRagdolls = Map.Get.Ragdolls.Count;
                        ClearRagdolls();
                        int afterRagdolls = Map.Get.Ragdolls.Count;

                        result.Message = $"Removed {beforeRagdolls - afterRagdolls} ragdolls";
                        result.State = CommandResultState.Ok;
                        break;
                    }
                case "items":
                    {
                        int beforeItems = Map.Get.Items.Count;
                        ClearItems();
                        int afterItems = Map.Get.Items.Count;

                        result.Message = $"Removed {beforeItems - afterItems} items";
                        result.State = CommandResultState.Ok;
                        break;
                    }
                case "dummies":
                    {
                        int beforeDummies = Map.Get.Dummies.Count;
                        ClearDummies();
                        int afterDummies = Map.Get.Dummies.Count;

                        result.Message = $"Removed {beforeDummies - afterDummies} dummies";
                        result.State = CommandResultState.Ok;
                        break;
                    }
                default:
                    {
                        result.Message = "Unknown specification";
                        result.State = CommandResultState.Error;
                        break;
                    }
            }

            return result;
        }
        private void ClearDummies()
        {
            foreach (var dummy in Map.Get.Dummies.ToArray())
            {
                try
                {
                    dummy.Destroy();
                }
                catch (Exception e)
                {
                    Synapse.Api.Logger.Get.Error(e);
                }
            }
        }
        private void ClearItems()
        {
            foreach (var item in Map.Get.Items.ToArray())
            {
                try
                {
                    item.Destroy();
                }
                catch (Exception e)
                {
                    Synapse.Api.Logger.Get.Error(e);
                }
            }
        }
        private void ClearRagdolls()
        {
            foreach (var ragdoll in Map.Get.Ragdolls.ToArray())
            {
                try
                {
                    ragdoll.Destroy();
                }
                catch (Exception e)
                {
                    Synapse.Api.Logger.Get.Error(e);
                }
            }
        }
    }
}
