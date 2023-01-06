using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Model;

public interface ISwipeService
{
    void SwipeStarting(Point point);

    void SwipeEnded(
        Point point,
        Action? onSwipedLeft,
        Action? onSwipedRight,
        Action? onSwipedUp = null,
        Action? onSwipedDown = null);
}