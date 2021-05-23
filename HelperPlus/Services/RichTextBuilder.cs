using HelperPlus.Configs;
using HelperPlus.Handlers;
using HelperPlus.Models;
using Synapse.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

namespace HelperPlus.Services
{
    public class RichTextBuilder
    {
        private const byte TextSize = 70;

        public Dictionary<string, string> SectionColours { get; private set; }

        private HelperHandler _handler;
        private MemoryService _memoryService;
        private HelperConfig _config;

        public RichTextBuilder(HelperConfig config, HelperHandler handler, MemoryService memoryService)
        {
            _handler = handler;
            _memoryService = memoryService;
            _config = config;
            SectionColours = new Dictionary<string, string>()
            {
                { "Environment", "#51d400" },
                { "Server", "#00D4BB" },
                { "Map", "#1200D9" },
                { "Items", "#5A00E0" },
            };
        }

        public string BuildRichText(Player player)
        {
            try
            {
                //Stopwatch watch = Stopwatch.StartNew();
                StringBuilder builder = new StringBuilder();
                short appendedLines = 0;
                builder.Append("<voffset=35em>");
                if (_config.Environment.Enabled)
                {
                    AddEnvironmentSection(builder, player);
                }
                builder.AppendLine();
                if (_config.Server.Enabled)
                {
                    AddServerSection(builder);
                }
                builder.AppendLine();
                if (_config.Map.Enabled)
                {
                    AddMapSection(builder);
                }
                builder.AppendLine();
                if (_config.Items.Enabled)
                {
                    AddItemsSection(builder);
                }
                builder.Append("</voffset>");

                //watch.Stop();
                //Synapse.Api.Logger.Get.Warn($"{watch.Elapsed.TotalMilliseconds} ms");
                return builder.ToString();
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Warn(e);
            }
            return "";
        }

        private void AddItemsSection(StringBuilder builder)
        {
            builder.Append($"<size=100%>");
            builder.Append($"<color={SectionColours["Items"]}>");
            builder.Append($"<align=\"left\">");
            builder.AppendLine($"<b>Items</b>");

            builder.Append($"<size={TextSize}%>");

            if (_config.Items.DisplayTotalItems)
            {
                builder.AppendLine($"Total Item Amount: {Map.Get.Items.Count}");
            }
            if (_config.Items.DisplayWeaponAmount)
            {
                builder.AppendLine($"Weapon Amount: {Map.Get.Items.Count(item => item?.ItemCategory == ItemCategory.Weapon)}");
            }
            if (_config.Items.DisplayGrenadeAmount)
            {
                builder.AppendLine($"Grenade Amount: {Map.Get.Items.Count(item => item?.ItemCategory == ItemCategory.Grenade)}");
            }
            if (_config.Items.DisplayMedicalAmount)
            {
                builder.AppendLine($"Medical Amount: {Map.Get.Items.Count(item => item?.ItemCategory == ItemCategory.Medical)}");
            }
        }
        private void AddMapSection(StringBuilder builder)
        {
            builder.Append($"<size=100%>");
            builder.Append($"<color={SectionColours["Map"]}>");
            builder.Append($"<align=\"left\">");
            builder.AppendLine($"<b>Map</b>");

            builder.Append($"<size={TextSize}%>");

            if (_config.Map.DisplayDoorAmount)
            {
                builder.AppendLine($"Door Amount: {Map.Get.Doors.Count}");
            }
            if (_config.Map.DisplayRoomAmount)
            {
                builder.AppendLine($"Room Amount: {Map.Get.Rooms.Count}");
            }
            if (_config.Map.DisplayRagdollAmount)
            {
                builder.AppendLine($"Ragdoll Amount: {Map.Get.Ragdolls.Count}");
            }
        }
        private void AddServerSection(StringBuilder builder)
        {
            builder.Append($"<size=100%>");
            builder.Append($"<color={SectionColours["Server"]}>");
            builder.Append($"<align=\"left\">");
            builder.AppendLine($"<b>Server</b>");

            builder.Append($"<size={TextSize}%>");

            //builder.Append($"<margin-left=5em>{raycastHit.point}\n{raycastHit.point}\n<margin-left=1em>{raycastHit.point}\nTest");
            if (_config.Server.DisplayServerFps)
            {
                builder.AppendLine($"FPS: {_handler.ServerFps:.##}");
            }
            if (_config.Server.DisplayTotalRamUsage || _config.Server.DisplaySLRamUsage)
            {
                MemoryMetrics metrics = _memoryService.CurrentTotalMetrics;
                if (_config.Server.DisplayTotalRamUsage)
                {
                    builder.AppendLine($"Total Ram Usage: {metrics.Used}/{metrics.Total} MB [{((metrics.Used / metrics.Total) * 100):.##}%]");
                }
                if (_config.Server.DisplaySLRamUsage)
                {
                    double slRamUsage = _memoryService.CurrentProcessRamUsage;
                    builder.AppendLine($"SL Ram Usage: {slRamUsage}/{metrics.Total} MB [{((slRamUsage / metrics.Total) * 100):.##}%]");
                }
            }
        }
        private void AddEnvironmentSection(StringBuilder builder, Player player)
        {
            builder.Append($"<size=100%>");
            builder.Append($"<color={SectionColours["Environment"]}>");
            builder.Append($"<align=\"left\">");
            builder.AppendLine($"<b>Environment</b>");

            bool didRaycastHit = Physics.Raycast(
                player.CameraReference.transform.position,
                player.CameraReference.transform.forward,
                out RaycastHit raycastHit,
                100f
            );

            builder.Append($"<size={TextSize}%>");

            //builder.Append($"<margin-left=5em>{raycastHit.point}\n{raycastHit.point}\n<margin-left=1em>{raycastHit.point}\nTest");
            if (_config.Environment.DisplayTargetPosition)
            {
                Room targetRoom = Map.Get.Rooms.OrderBy(room => Vector3.Distance(raycastHit.point, room.Position)).FirstOrDefault();
                builder.AppendLine($"Target Position: {(didRaycastHit ? raycastHit.point.ToString() : "None")}");
                builder.AppendLine($"Target MapPoint: {(didRaycastHit ? new MapPoint(targetRoom, raycastHit.point).ToString() : "None")}");
            }
            if (_config.Environment.DisplayTargetName)
            {
                builder.AppendLine($"Target Name: {(didRaycastHit ? raycastHit.transform.gameObject.name : "None")}");
            }
            if (_config.Environment.DisplayPosition)
            {
                builder.AppendLine($"Player Position: {player.Position}");
                builder.AppendLine($"Player MapPoint: {(didRaycastHit ? player.MapPoint.ToString() : "None")}");
            }
            if (_config.Environment.DisplayCurrentRoom)
            {
                builder.AppendLine($"Current Room Name: {player.Room.RoomName}");
                builder.AppendLine($"Current Room Type: {player.Room.RoomType}");
            }
        }
    }
}
