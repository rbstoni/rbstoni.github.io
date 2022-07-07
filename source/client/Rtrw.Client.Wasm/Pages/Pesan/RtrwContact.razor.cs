using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Rtrw.Client.Wasm.Models;
using System.ComponentModel;

namespace Rtrw.Client.Wasm.Pages.Pesan
{
    public partial class RtrwContact

    {
        bool contactOptionsShow;
        bool avatarClick;
        bool settingClick;
        string direction;
        (TouchPoint ReferencePoint, DateTime StartTime) startPoint;

        [Parameter] public Contact Contact { get; set; } = new();

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
}
