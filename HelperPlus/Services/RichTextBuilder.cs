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
        private HelperPlus _helperPlus;

        public RichTextBuilder(HelperPlus helperPlus, HelperHandler handler, MemoryService memoryService)
        {
            _helperPlus = helperPlus;
            _handler = handler;
            _memoryService = memoryService;
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
                StringBuilder builder = new StringBuilder();
                short appendedLines = 0;
                builder.Append("<voffset=35em>");
                if (_helperPlus.Config.Environment.Enabled)
                {
                    AddEnvironmentSection(builder, player);
                }
                builder.AppendLine();
                if (_helperPlus.Config.Server.Enabled)
                {
                    AddServerSection(builder);
                }
                builder.AppendLine();
                if (_helperPlus.Config.Map.Enabled)
                {
                    AddMapSection(builder);
                }
                builder.AppendLine();
                if (_helperPlus.Config.Items.Enabled)
                {
                    AddItemsSection(builder);
                }
                builder.Append("</voffset>");
                return builder.ToString();
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error(e);
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

            if (_helperPlus.Config.Items.DisplayTotalItems)
            {
                builder.AppendLine($"Total Item Amount: {Map.Get.Items.Count}");
            }
            if (_helperPlus.Config.Items.DisplayWeaponAmount)
            {
                builder.AppendLine($"Weapon Amount: {Map.Get.Items.Count(item => item?.ItemCategory == ItemCategory.Weapon)}");
            }
            if (_helperPlus.Config.Items.DisplayGrenadeAmount)
            {
                builder.AppendLine($"Grenade Amount: {Map.Get.Items.Count(item => item?.ItemCategory == ItemCategory.Grenade)}");
            }
            if (_helperPlus.Config.Items.DisplayMedicalAmount)
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

            if (_helperPlus.Config.Map.DisplayDoorAmount)
            {
                builder.AppendLine($"Door Amount: {Map.Get.Doors.Count}");
            }
            if (_helperPlus.Config.Map.DisplayRoomAmount)
            {
                builder.AppendLine($"Room Amount: {Map.Get.Rooms.Count}");
            }
            if (_helperPlus.Config.Map.DisplayRagdollAmount)
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
            if (_helperPlus.Config.Server.DisplayServerFps)
            {
                builder.AppendLine($"FPS: {_handler.ServerFps:.##}");
            }
            if (_helperPlus.Config.Server.DisplayTotalRamUsage || _helperPlus.Config.Server.DisplaySLRamUsage)
            {
                MemoryMetrics metrics = _memoryService.CurrentTotalMetrics;
                if (_helperPlus.Config.Server.DisplayTotalRamUsage)
                {
                    builder.AppendLine($"Total Ram Usage: {metrics.Used}/{metrics.Total} MB [{((metrics.Used / metrics.Total) * 100):.##}%]");
                }
                if (_helperPlus.Config.Server.DisplaySLRamUsage)
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
            if (_helperPlus.Config.Environment.DisplayTargetPosition)
            {
                Room targetRoom = Map.Get.Rooms.OrderBy(room => Vector3.Distance(raycastHit.point, room.Position)).FirstOrDefault();
                builder.AppendLine($"Target Position: {(didRaycastHit ? raycastHit.point.ToString() : "None")}");
                builder.AppendLine($"Target MapPoint: {(didRaycastHit ? new MapPoint(targetRoom, raycastHit.point).ToString() : "None")}");
            }
            if (_helperPlus.Config.Environment.DisplayTargetName)
            {
                builder.AppendLine($"Target Name: {(didRaycastHit ? raycastHit.transform.gameObject.name : "None")}");
            }
            if (_helperPlus.Config.Environment.DisplayPosition)
            {
                builder.AppendLine($"Player Position: {player.Position}");
                builder.AppendLine($"Player MapPoint: {(didRaycastHit ? player.MapPoint.ToString() : "None")}");
            }
            if (_helperPlus.Config.Environment.DisplayCurrentRoom)
            {
                builder.AppendLine($"Current Room Name: {player.Room.RoomName}");
                builder.AppendLine($"Current Room Type: {player.Room.RoomType}");
            }
        }
    }
}
