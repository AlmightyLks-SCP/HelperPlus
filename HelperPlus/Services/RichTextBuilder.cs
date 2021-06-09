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
            if (_helperPlus.Config.Map.DisplayDummyAmount)
            {
                builder.AppendLine($"Dummy Amount: {Map.Get.Dummies.Count}");
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
            Room targetRoom = Map.Get.Rooms.OrderBy(room => Vector3.Distance(raycastHit.point, room.Position)).FirstOrDefault();


            builder.Append($"<size={TextSize}%>");

            if (_helperPlus.Config.Environment.DisplayTargetPosition)
            {
                bool isValidPlayer = player != null && player.RoleType != RoleType.Spectator && player.RoleType != RoleType.None;

                string targetPosition = didRaycastHit ? raycastHit.point.ToString() : "None";
                string targetMapPoint = didRaycastHit && isValidPlayer && targetRoom != null ? new MapPoint(targetRoom, raycastHit.point).ToString() : "None";

                builder.AppendLine($"Target Position: {targetPosition}");
                builder.AppendLine($"Target MapPoint: {targetMapPoint}");
            }
            if (_helperPlus.Config.Environment.DisplayTargetName)
            {
                string targetName = didRaycastHit ? raycastHit.transform.gameObject.name : "None";

                builder.AppendLine($"Target Name: {targetName}");
            }
            if (_helperPlus.Config.Environment.DisplayPosition)
            {
                bool isValidPlayer = player != null && player.RoleType != RoleType.Spectator && player.RoleType != RoleType.None;
                string playerPosition = isValidPlayer ? player.Position.ToString() : "None";
                string playerMapPoint = isValidPlayer ? player.MapPoint.ToString() : "None";

                builder.AppendLine($"Player Position: {playerPosition}");
                builder.AppendLine($"Player MapPoint: {playerMapPoint}");
            }
            if (_helperPlus.Config.Environment.DisplayCurrentRoom)
            {
                bool isValidPlayer = player != null && player.RoleType != RoleType.Spectator && player.RoleType != RoleType.None;
                Room room = player?.Room;

                string currentRoomName = (isValidPlayer && room != null ? player.Room.RoomName : "None");
                string currentRoomType = (isValidPlayer && room != null ? player.Room.RoomType.ToString() : "None");

                builder.AppendLine($"Current Room Name: {currentRoomName}");
                builder.AppendLine($"Current Room Type: {currentRoomType}");
            }
        }
    }
}
