namespace Tetraizor.Autoloads;

using System.Collections.Generic;
using Godot;
using Tetraizor.Data;
using Tetraizor.Managers;
using Tetraizor.Utils;

public partial class InputManager : AutoloadBase<InputManager>
{
    #region Signals
    [Signal] public delegate void PrimaryPressBeginEventHandler(Vector2 position, float pressure);
    [Signal] public delegate void PrimaryPressEndEventHandler(Vector2 position, float pressure);
    [Signal] public delegate void PrimaryPressDragEventHandler(Vector2 position, float pressure);
    [Signal] public delegate void PrimaryPressCanceledEventHandler();

    [Signal] public delegate void CursorMoveEventHandler(Vector2 position);

    [Signal] public delegate void GestureBeginEventHandler(Vector2 first, Vector2 second);
    [Signal] public delegate void GestureEndEventHandler(Vector2 first, Vector2 second);
    [Signal] public delegate void GestureDragEventHandler(Vector2 first, Vector2 second);

    [Signal] public delegate void PanEventHandler(Vector2 delta);
    [Signal] public delegate void ZoomEventHandler(float delta);
    [Signal] public delegate void RotateEventHandler(float delta);
    #endregion

    #region Android/IOS Properties
    private const float GestureTimeSeconds = .1f;

    private List<Timer> _gestureTimers = new();

    private bool _isUsingGesture = false;
    private int _primaryPress = -1;

    private float startZoom = 1f;

    private Vector2I _screenSize;

    private Vector2 _firstTouchInit;
    private Vector2 _secondTouchInit;

    private Vector2 _firstTouchPrev;
    private Vector2 _secondTouchPrev;

    private List<TouchData> _touches = new();
    public List<TouchData> Touches => _touches;
    #endregion

    #region PC/MAC/Linux Properties
    private bool _isMiddleWheelPressed = false;
    private bool _isPrimaryPressed = false;
    #endregion

    #region References
    private CameraManager _cameraManager;
    #endregion

    #region Godot Methods
    public override void _Ready()
    {
        base._Ready();

        ApplicationManager.Instance.ScreenSizeChanged += OnScreenSizeChanged;
        _screenSize = ApplicationManager.Instance.ScreenSize;

        GestureBegin += OnGestureBegin;
        GestureDrag += OnGestureDrag;
        GestureEnd += OnGestureEnd;

        CallDeferred(MethodName.DelayedReady);
    }

    private void DelayedReady()
    {
        _cameraManager = NodeManager.FindNodeOfType<CameraManager>();
    }

    public override void _Input(InputEvent @event)
    {
        // Ignore simulated mouse events.
        if (@event.Device == -1) return;

#if GODOT_ANDROID || GODOT_IOS

        if (@event is InputEventScreenTouch screenTouch)
        {
            if (screenTouch.IsPressed())
            {
                AddTouch(screenTouch.Index, screenTouch.Position, .5f);
            }
            else if (screenTouch.IsReleased())
            {
                RemoveTouch(screenTouch.Index);
            }
        }

        if (@event is InputEventScreenDrag screenDrag)
        {
            if (screenDrag.IsCanceled())
            {
                RemoveTouch(screenDrag.Index);
                return;
            }

            if (_isUsingGesture)
            {
                _touches.Find(touch => touch.Index == screenDrag.Index).Position = screenDrag.Position;

                if (_touches.Count >= 2)
                {
                    if (_touches[0].Index == screenDrag.Index)
                    {
                        EmitSignal(SignalName.GestureDrag, screenDrag.Position, _touches[1].Position);
                    }
                    else if (_touches[1].Index == screenDrag.Index)
                    {
                        EmitSignal(SignalName.GestureDrag, _touches[0].Position, screenDrag.Position);
                    }
                }
            }
            else
            {
                if (_touches[0].Index == screenDrag.Index)
                {
                    EmitSignal(SignalName.PrimaryPressDrag, screenDrag.Position, screenDrag.Pressure);
                }
            }
        }

#endif

        if (@event is InputEventMouseButton mouseEvent)
        {
            switch (mouseEvent.ButtonIndex)
            {
                case MouseButton.Left:
                    if (mouseEvent.Pressed)
                    {
                        EmitSignal(SignalName.PrimaryPressBegin, mouseEvent.Position, .5f);
                        _isPrimaryPressed = true;
                    }
                    else
                    {
                        EmitSignal(SignalName.PrimaryPressEnd, mouseEvent.Position, .5f);
                        _isPrimaryPressed = false;
                    }
                    break;

                case MouseButton.Middle:
                    if (mouseEvent.Pressed)
                    {
                        _isMiddleWheelPressed = true;
                    }
                    else
                    {
                        _isMiddleWheelPressed = false;
                    }
                    break;

                case MouseButton.WheelDown:
                    EmitSignal(SignalName.Zoom, -.05);
                    break;

                case MouseButton.WheelUp:
                    EmitSignal(SignalName.Zoom, .05);
                    break;

                default:
                    break;
            }
        }

        if (@event is InputEventMouseMotion mouseMotion)
        {
            EmitSignal(SignalName.CursorMove, mouseMotion.Position);

            if (_isPrimaryPressed)
            {
                EmitSignal(SignalName.PrimaryPressDrag, mouseMotion.Position, mouseMotion.Pressure);
            }

            if (_isMiddleWheelPressed)
            {
                EmitSignal(SignalName.Pan, mouseMotion.Relative / _cameraManager.Zoom.X * -1);
            }
        }

#if GODOT_PC

        if (@event is InputEventPanGesture panGesture)
        {
            EmitSignal(SignalName.Pan, panGesture.Delta);
        }

        if (@event is InputEventMagnifyGesture magnifyGesture)
        {
            EmitSignal(SignalName.Zoom, magnifyGesture.Factor);
        }

#endif
    }
    #endregion Godot Methods

    #region Signal Callbacks
    private void OnScreenSizeChanged(Vector2I newSize)
    {
        _screenSize = newSize;
    }
    #endregion Signal Callbacks

    #region Touch Methods
    private void RemoveTouch(int index)
    {
        var selectedTouch = _touches.Find(touch => touch.Index == index);
        int order = -1;

        if (selectedTouch != null)
        {
            order = _touches.IndexOf(selectedTouch);
            _touches.Remove(selectedTouch);
        }
        else return;

        if (_touches.Count == 0)
        {
            foreach (var timer in _gestureTimers)
            {
                timer.Stop();
                timer.QueueFree();
            }

            _gestureTimers.Clear();

            _primaryPress = -1;
        }

        if (_isUsingGesture)
        {
            if (_touches.Count == 1)
            {
                if (order == 0)
                {
                    EmitSignal(SignalName.GestureEnd, selectedTouch.Position, _touches[0].Position);
                }
                else
                {
                    EmitSignal(SignalName.GestureEnd, _touches[0].Position, selectedTouch.Position);
                }
            }
            else if (_touches.Count == 0)
            {
                _isUsingGesture = false;
            }
        }
        else
        {
            if (index == _primaryPress)
            {
                EmitSignal(SignalName.PrimaryPressEnd, selectedTouch.Position, selectedTouch.Pressure);
            }
        }
    }

    private void AddTouch(int index, Vector2 position, float pressure)
    {
        var selectedTouch = _touches.Find(touch => touch.Index == index);

        if (selectedTouch != null)
        {
            selectedTouch = new TouchData
            {
                Index = index,
                Position = position,
                Pressure = pressure
            };

            return;
        }

        _touches.Add(new TouchData
        {
            Index = index,
            Position = position,
            Pressure = pressure
        });

        if (!_isUsingGesture)
        {
            if (_touches.Count == 1)
            {
                _primaryPress = index;

                Timer timer = new Timer
                {
                    OneShot = true,
                    WaitTime = GestureTimeSeconds,
                    Autostart = true,
                };

                timer.Timeout += () =>
                {
                    _gestureTimers.Remove(timer);
                    timer.QueueFree();

                    if (_touches.Count > 1)
                    {
                        EmitSignal(SignalName.PrimaryPressCanceled);
                        EmitSignal(SignalName.GestureBegin, _touches[0].Position, _touches[1].Position);
                        _isUsingGesture = true;
                    }
                };

                AddChild(timer);

                EmitSignal(SignalName.PrimaryPressBegin, _touches[0].Position, _touches[0].Pressure);
            }
        }
        else
        {
            if (_touches.Count == 2)
            {
                EmitSignal(SignalName.GestureBegin, _touches[0].Position, _touches[1].Position);
            }
        }
    }
    #endregion Touch Methods

    #region Gesture Methods
    private void OnGestureDrag(Vector2 firstTouchCurrent, Vector2 secondTouchCurrent)
    {
        Vector2 firstTouchScaled = _cameraManager.ScreenToWorldPosition(firstTouchCurrent);
        Vector2 secondTouchScaled = _cameraManager.ScreenToWorldPosition(secondTouchCurrent);

        Vector2 firstTouchInitScaled = _cameraManager.ScreenToWorldPosition(_firstTouchInit);
        Vector2 secondTouchInitScaled = _cameraManager.ScreenToWorldPosition(_secondTouchInit);

        Vector2 firstPrevScaled = _cameraManager.ScreenToWorldPosition(_firstTouchPrev);
        Vector2 secondPrevScaled = _cameraManager.ScreenToWorldPosition(_secondTouchPrev);

        Vector2 prevAvg = firstPrevScaled + ((secondPrevScaled - firstPrevScaled) / 2);
        Vector2 currentAvg = firstTouchScaled + ((secondTouchScaled - firstTouchScaled) / 2);

        Vector2 delta = currentAvg - prevAvg;

        float currentDistance = (firstTouchScaled - secondTouchScaled).Length();
        float firstDistance = (firstTouchInitScaled - secondTouchInitScaled).Length();

        float magnification = currentDistance / firstDistance;

        float newValue = startZoom * magnification;

        _firstTouchPrev = firstTouchCurrent;
        _secondTouchPrev = secondTouchCurrent;

        EmitSignal(SignalName.Zoom, newValue - _cameraManager.Zoom.X);
        EmitSignal(SignalName.Pan, -delta);
    }

    private void OnGestureEnd(Vector2 first, Vector2 second) { }

    private void OnGestureBegin(Vector2 first, Vector2 second)
    {
        _firstTouchInit = first;
        _firstTouchPrev = first;

        _secondTouchInit = second;
        _secondTouchPrev = second;

        startZoom = _cameraManager.Zoom.X;
    }
    #endregion Gesture Methods
}
