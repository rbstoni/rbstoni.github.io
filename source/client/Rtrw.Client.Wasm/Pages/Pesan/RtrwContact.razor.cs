using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel;

namespace Rtrw.Client.Wasm.Pages.Pesan
{
    public partial class RtrwContact

    {
        bool contactOptionsShow;
        bool avatarClick;
        bool settingClick;
        string message = "start touching";
        string direction;
        (TouchPoint ReferencePoint, DateTime StartTime) startPoint;

        [Parameter] public ContactResponse Contact { get; set; } = new();

        void OnTouchStartHandler(TouchEventArgs touch)
        {
            startPoint.ReferencePoint = touch.Touches[0];
            startPoint.StartTime = DateTime.Now;
        }

        void OnTouchEndHandler(TouchEventArgs touch)
        {
            var endPoint = touch.ChangedTouches[0];
            var endTime = DateTime.Now;
            var deltaX = startPoint.ReferencePoint.ClientX - endPoint.ClientX;
            var deltaTime = endTime - startPoint.StartTime;
            var velocity = Math.Abs(deltaX / deltaTime.Milliseconds);
            if (velocity > 0.5)
            {
                direction = deltaX < 0 ? "right" : "left";
                if (direction == "left")
                {
                    contactOptionsShow = true;
                }
                else
                {
                    contactOptionsShow = false;
                }
            }
        }

        void MessageOnClickHandler()
        {
        }

        void AvatarOnClickHandler()
        {
        }
    }

    public class ContactResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsPinned { get; set; }
        public string ImageUrl { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public ContactType Type { get; set; }
        public DateTime LastModified { get; set; }
    }

    public enum ContactType
    {
        [Description("Personal")]
        Personal,

        [Description("Group")]
        Group
    }
}
