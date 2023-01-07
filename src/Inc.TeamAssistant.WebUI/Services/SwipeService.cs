using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.WebUI.Services;

internal sealed class SwipeService : ISwipeService
{
    private Point? _startSwipePoint;

    public void SwipeStarting(Point point) => _startSwipePoint = point;

    public void SwipeEnded(
        Point point,
        Action? onSwipedLeft,
        Action? onSwipedRight,
        Action? onSwipedUp = null,
        Action? onSwipedDown = null)
    {
        if (point is null)
            throw new ArgumentNullException(nameof(point));

        if (_startSwipePoint is null)
            return;

        var endSwipePoint = point;
        var diffX = _startSwipePoint.X - endSwipePoint.X;
        var diffY = _startSwipePoint.Y - endSwipePoint.Y;

        if (Math.Abs(diffX) > Math.Abs(diffY))
        {
            if (diffX > 0)
                onSwipedLeft?.Invoke();
            else
                onSwipedRight?.Invoke();
        }
        else
        {
            if (diffY > 0)
                onSwipedUp?.Invoke();
            else
                onSwipedDown?.Invoke();
        }

        _startSwipePoint = null;
    }
}