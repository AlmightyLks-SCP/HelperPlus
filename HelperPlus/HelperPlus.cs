using HarmonyLib;
using HelperPlus.Configs;
using HelperPlus.Handlers;
using HelperPlus.Services;
using Hints;
using MEC;
using Synapse.Api;
using Synapse.Api.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

namespace HelperPlus
{
    [PluginInformation(
        Author = "AlmightyLks",
        Description = "Helper",
        Name = "HelperPlus",
        SynapseMajor = 2,
        SynapseMinor = 6,
        SynapsePatch = 0,
        Version = "1.0.0"
        )]
    public class HelperPlus : AbstractPlugin
    {
        [Config(section = "HelperPlus")]
        public HelperConfig Config { get; set; }
        public Dictionary<Player, CoroutineHandle> PlayerUIHandles { get; private set; }

        private HelperHandler _handler;
        private MemoryService _memoryService;
        private RichTextBuilder _richTextBuilder;

        public override void Load()
        {
            int processId = Process.GetCurrentProcess().Id;
            _memoryService = new MemoryService(processId);
            //Only run if memory is going to be asked for
            if (Config.Server.DisplayTotalRamUsage || Config.Server.DisplaySLRamUsage)
            {
                _memoryService.BackgroundWorker.RunWorkerAsync();
            }

            PlayerUIHandles = new Dictionary<Player, CoroutineHandle>();
            _handler = new HelperHandler();
            _richTextBuilder = new RichTextBuilder(Config, _handler, _memoryService);
        }
        public override void ReloadConfigs()
        {
            if (_memoryService.BackgroundWorker.IsBusy)
            {
                _memoryService.BackgroundWorker.CancelAsync();
            }

            //Only run if memory is going to be asked for
            if (Config.Server.DisplayTotalRamUsage || Config.Server.DisplaySLRamUsage)
            {
                _memoryService.BackgroundWorker.RunWorkerAsync();
            }
        }
        public IEnumerator<float> UICoroutine(Player player)
        {
            for (; ; )
            {
                Hint hint = new TextHint(
                    _richTextBuilder.BuildRichText(player),
                    new HintParameter[] { new StringHintParameter(string.Empty) },
                    null,
                    0.75f
                    );

                player.gameObject.GetComponent<ReferenceHub>().hints.Show(hint);

                yield return Timing.WaitForSeconds(0.5f);
            }
        }
    }
}
